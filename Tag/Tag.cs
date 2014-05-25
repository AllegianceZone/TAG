using System;
using System.Reflection;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows.Forms;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Tigereye's Allegiance Gizmo
	/// </summary>
	public class Tag
	{
		private const string UPDATERNAME = "TAGUpdater.exe";

		#region ConsoleInputMode Hack
		[DllImport("kernel32", SetLastError=true)]
		private static extern IntPtr GetStdHandle(IntPtr whichHandle);
    
		[DllImport("kernel32", SetLastError=true)]
		private static extern bool GetConsoleMode(IntPtr handle, out uint mode);
    
		[DllImport("kernel32", SetLastError=true)]
		private static extern bool SetConsoleMode(IntPtr handle, uint mode);
    
		private static readonly IntPtr STD_INPUT_HANDLE = new IntPtr(-10);

		private const uint ENABLE_LINE_INPUT = 2;
		private const uint ENABLE_ECHO_INPUT = 4;    
    
		/// <summary>
		/// Disables the "EnableLineInput" flag for the Console.Read() method
		/// </summary>
		private static void DisableConsoleLineInputMode () 
		{
			IntPtr console = GetStdHandle(STD_INPUT_HANDLE);
			uint oldMode;
			if (GetConsoleMode(console, out oldMode)) 
			{
				uint newMode = oldMode & ~(ENABLE_LINE_INPUT | ENABLE_ECHO_INPUT);
				if (SetConsoleMode(console, newMode)) 
				{
					return;
				}
			}
			Console.WriteLine("Failed to get/set console input mode: " + Marshal.GetLastWin32Error().ToString());
		}
		#endregion

		private static TagConfig	_config = null;			// The configuration file
		private static Mutex		_mutex = null;			// A mutex used to ensure there's only 1 Tag process running at once
		private static Thread		_mainThread = null;		// The main thread used to set up TAG's processing
		private static System.Threading.Timer		_updateTimer = null;	// The Timer control used to trigger update checks

		private static ManualResetEvent _closeAppResetEvent = null;		// Prevents the main thread from closing; triggers the end of application
		private static ManualResetEvent _tagStartedResetEvent = null;	// Stalls the console 'Q' loop until TAG has finished starting

		/// <summary>
		/// Main entry point of the application
		/// </summary>
		/// <param name="args">Commandline arguments</param>
		
		[STAThread()]
		public static void Main (string[] args)
		{
			// Ensure we're the only Tag running. Quit if already running
			bool firstInstance;
			_mutex = new Mutex(false, "Local\\Tag", out firstInstance);
			if (firstInstance == false)
			{
				if (Environment.UserInteractive)
					MessageBox.Show("Only one instance of TAG can run at a time!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);

				return;
			}

			// They typed a command line parameter
			if (args.Length > 0)
			{
				string Param = args[0].ToLower();
				// If they want to install service...
				switch (Param)
				{
					case "-service":
						// If service not already installed...
						if (TagServiceController.TagService == null)
						{
							// Retrieve allsrv service name
							string AllsrvServiceName = TagServiceController.AllsrvService.ServiceName;

							// Add it.
							string ServicePath = string.Concat(Application.ExecutablePath);
							ServiceInstaller.InstallService(ServicePath, TagServiceController.TAGSERVICENAME, "TAG Service", AllsrvServiceName);
							Console.WriteLine("Service Installed");
						}
						else
						{
							Console.WriteLine("Service is already installed");
						}
						break;
					case "-noservice":
						// Remove it
						ServiceInstaller.UninstallService(TagServiceController.TAGSERVICENAME);
						Console.WriteLine("Service Uninstalled");
						break;
					default:
						Console.WriteLine("Invalid argument. Valid arguments are:");
						Console.WriteLine("-service		Installs TAG as a service");
						Console.WriteLine("-noservice	Uninstalls TAG service");
						break;
				}

				return;
			}

			// If run as a service, run service ;)
			if (Environment.UserInteractive == false)
			{
				TagService.StartService();
			}
			else
			{
				Tag.DisableConsoleLineInputMode();
				_tagStartedResetEvent = new ManualResetEvent(false);

				// Run Tag in console mode
				Tag.Start();
				_tagStartedResetEvent.WaitOne();

				if (_closeAppResetEvent != null)
				{
					Console.WriteLine("---------- Press 'Q' to Exit ----------");
					// Prevent from ending until Q is pressed
					int CharID;
					while (true)
					{
						CharID = Console.Read();
						if (CharID != -1)
						{
							char Char = (char)CharID;
							if (Char == 'Q' || Char == 'q')
								break;
						}
					}
					// Clean up before exiting
					Tag.Stop();
				}
			}
		}

		/// <summary>
		/// Starts TAG up: Pings ASGS, connects to Allsrv, and starts logging games.
		/// </summary>
		public static void Start ()
		{
			if (_mainThread == null)
			{
				// Create the thread pointing to DoStart()
				_mainThread = new Thread(new ThreadStart(DoStart));

				// Set the thread's properties
				_mainThread.Name = "TAG Main Thread";
				_mainThread.IsBackground = false;

				// Start the thread
				_mainThread.Start();
			}
		}

		/// <summary>
		/// Reads the config file and initializes tracing and logging
		/// </summary>
		public static void LoadConfiguration ()
		{
			if (_config == null)
			{
				// Try to read the configuration
				try
				{
					_config = (TagConfig)ConfigurationSettings.GetConfig("TagConfig");

					if (_config == null)
						throw new ConfigurationException("Error parsing TagConfig section of config file");
				}
				catch (Exception e)
				{
					Console.WriteLine("Error loading Config file: {0}", e.Message);
					Console.WriteLine("Exiting...");
					return;
				}

				// Make the configuration available to the CallsignHelper for ServerAdmin tags
				CallsignHelper.SetServerAdmins(_config.ServerAdmins);
				
				// Initialize tracing
				TagTrace.Initialize(_config.TraceLevel, _config.TracePath, _config.TraceArchiveDir, _config.TraceConsole, null);

				TagTrace.WriteLine(TraceLevel.Info, "TAG Build {0} is starting...", Build.ToString());

				// Initialize reconnect timer
				ReconnectTimer.Initialize(_config.ReconnectInterval, _config.MaxRetries);
				ReconnectTimer.ShutdownTagEvent += new AsyncCallback(ReconnectShutdown);
				
				// Configure ASGS as necessary
				if (_config.AsgsUrl != null)
					AsgsConnector.Initialize(_config.AsgsUrl, _config.PostTimeout);

				if (_config.CssUrl != null)
					CssConnector.Initialize(_config.CssUrl, _config.PostTimeout, _config.UseCss);
				
				TagTrace.WriteLine(TraceLevel.Verbose, "Initializing logging...");
				GameLogger.Initialize(_config.XmlPath);
				
				TagTrace.WriteLine(TraceLevel.Info, "Configuration Loaded.");
			}
		}

		/// <summary>
		/// Performs the startup operations necessary to make TAG log Allsrv events
		/// </summary>
		public static void DoStart ()
		{
			// Load configuration if necessary
			if (_config == null)
				LoadConfiguration();

			if (_config.SkipUpdates == true)
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "TAG Automatic updates are disabled in the configuration file with the skipUpdates setting.");
			}
			else
			{

#if !DEBUG
			TagTrace.WriteLine(TraceLevel.Verbose, "Checking for updates...");
			// Check for updates to TAG before starting
			if (TagUpdate.UpdateAvailable())
			{
				TagTrace.WriteLine(TraceLevel.Info, "An update to TAG is available. Exiting to perform update...");
				PerformUpdate(null, new EventArgs());
				
				// Allow main thread to continue
				_tagStartedResetEvent.Set();

				return;
			}
			else
				TagTrace.WriteLine(TraceLevel.Verbose, "No updates are available.");
#endif

				// Prepare the TAG Update timer
				TimeSpan TimeUntilNextUpdate = _config.UpdateTime.Subtract(DateTime.Now);
				_updateTimer = new System.Threading.Timer(new TimerCallback(CheckForUpdates), null, TimeUntilNextUpdate, new TimeSpan(1, 0, 0, 0));

				// Hook up the Update event for manual update requests
				TagUpdate.UpdateTriggeredEvent += new ManualUpdateDelegate(PerformUpdate);
			}

			try
			{
				// Initialize GameServer
				TagTrace.WriteLine(TraceLevel.Info, "Connecting to GameServer....");
				GameServer.Initialize();
			}
			catch (Exception e)
			{
				TagTrace.WriteLine(TraceLevel.Error, "TAG could not connect to Allsrv-: {0}", e.ToString());

				// Allow main thread to continue
				_tagStartedResetEvent.Set();

				if (TagServiceController.IsTagServiceStarted())
					TagServiceController.StopTagService();
				else
					new Thread(new ThreadStart(Stop)).Start();

				return;
			}
			
			// Create the ResetEvent used to prevent from returning
			_closeAppResetEvent = new ManualResetEvent(false);

			// Allow main thrad to continue into 'Q' loop
			if (_tagStartedResetEvent != null)
				_tagStartedResetEvent.Set();

			TagTrace.WriteLine(TraceLevel.Info, "TAG is ready.");

			// Block the thread from exiting until we're shutting down
			_closeAppResetEvent.WaitOne();
		}

		/// <summary>
		/// Shuts down TAG after receiving the reconnect "Shutdown" event
		/// </summary>
		/// <param name="ar"></param>
		public static void ReconnectShutdown (IAsyncResult ar)
		{
			Stop();
		}

		/// <summary>
		/// Disconnects from allsrv, logs games that were in progress and shuts down TAG.
		/// </summary>
		public static void Stop ()
		{
			TagTrace.WriteLine(TraceLevel.Info, "Shutting down...");
			// De-initialize core (unhook events)
			AGCEventHandler.Uninitialize();
			TagTrace.WriteLine(TraceLevel.Verbose, "Events unhooked.");

			TagTrace.WriteLine(TraceLevel.Info, "Shutting the logger down...");
			// Shutdown the logger
			GameLogger.Uninitialize();
			TagTrace.WriteLine(TraceLevel.Verbose, "Logger shut down.");

			TagTrace.WriteLine(TraceLevel.Info, "Disconnecting from Allsrv...");
			// Disconnect from allsrv
			GameServer.Disconnect();
			TagTrace.WriteLine(TraceLevel.Verbose, "Disconnected.");

			TagTrace.WriteLine(TraceLevel.Info, "Exiting...");
			// Signal the "Main" thread to end processing
			if (_closeAppResetEvent != null)
				_closeAppResetEvent.Set();

			// Allow the main thread to end, or force-close after 5s
			if (_mainThread != null)
				_mainThread.Join(5000);

			// Shutdown Tracing and Debugging
			TagTrace.Uninitialize();
			TagTrace.Uninitialize();
		}

		/// <summary>
		/// Checks for updates to TAG
		/// </summary>
		/// <param name="info">Additional info used by the Update timer (null)</param>
		public static void CheckForUpdates (object info)
		{
			TagTrace.WriteLine(TraceLevel.Verbose, "Checking for updates...");
			if (TagUpdate.IsAbleToUpdate())
			{
				if (TagUpdate.UpdateAvailable())
				{
					TagTrace.WriteLine(TraceLevel.Info, "An update to TAG is available. Performing update...");
					TagUpdate.InitiateUpdate();
				}
			}
			else
				TagTrace.WriteLine(TraceLevel.Verbose, "TAG can't update now. Games in progress.");
		}

		/// <summary>
		/// Performs the TAG update
		/// </summary>
		/// <param name="sender">The object that raised this event</param>
		/// <param name="e">The event arguments</param>
		public static void PerformUpdate (object sender, EventArgs e)
		{
			string UpdaterPath = Application.StartupPath + "\\" + UPDATERNAME;

			// Launch the updater
			Process.Start(new ProcessStartInfo(UpdaterPath));

			TagTrace.WriteLine(TraceLevel.Verbose, "Updater application launched");

			if (TagServiceController.IsTagServiceStarted())
			{
				// TAG is a service. Stop it!
				TagServiceController.StopTagService();
				TagTrace.WriteLine(TraceLevel.Verbose, "TAG Service stopped");
			}
			else
			{
				// TAG is console. Stop it!
				Tag.Stop();
			}
		}

		/// <summary>
		/// Retrieves the current configuration file
		/// </summary>
		public static TagConfig Config
		{
			get {return _config;}
		}

		/// <summary>
		/// The build number of TAG
		/// </summary>
		public static int Build
		{
			get {return Assembly.GetExecutingAssembly().GetName().Version.Build;}
		}
	}
}
