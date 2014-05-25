using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Win32;

using AGCLib;
using ALLEGIANCESERVERLib;
using FreeAllegiance.Tag.Events;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Manages the connection to the Allegiance Server
	/// </summary>
	public class AllsrvConnector : IAdminSessionEvents
	{

		private const string SESSIONGUID = "DE3ED156-76A0-4A8E-8CFE-9ED26C3B0A5E";
		private static string ALLEGIANCESERVERREGKEY = "SOFTWARE\\Microsoft\\Microsoft Games\\Allegiance\\1.2\\Server";

		[DllImportAttribute("AGMLib.dll")]
		private static extern int GetAdminSession(out IAdminSession pout);

		private static Guid					_sessionGuid = new Guid(SESSIONGUID);
		private static int					_cookie = 0;
		private static IAdminSession		_session = null;
		private static UCOMIConnectionPoint	_icp = null;

		private static AllsrvConnector		_connector = null;

		/// <summary>
		/// Default Constructor
		/// </summary>
		private AllsrvConnector ()
		{
			IAdminSession IAS = null;

			// Call into AGCLib to get the AdminSession
			int ErrorCode = GetAdminSession(out IAS);
			if (ErrorCode != 0)
				throw new ApplicationException("Attempt to retrieve Admin Session failed. Error " + ErrorCode.ToString());

			_session = IAS;
			TagTrace.WriteLine(TraceLevel.Verbose, "Admin Session retrieved from Allsrv.");

			// Hook into events
			UCOMIConnectionPointContainer uCOMIConnectionPointContainer = (UCOMIConnectionPointContainer)_session;
			uCOMIConnectionPointContainer.FindConnectionPoint(ref _sessionGuid, out _icp);
			_cookie = 0;
			_icp.Advise(this, out _cookie);
			_session.ActivateAllEvents();
		}

		/// <summary>
		/// Connects to Allsrv and hooks into events
		/// </summary>
		/// <returns>The connector to Allsrv</returns>
		public static AllsrvConnector Connect ()
		{
			if (_connector == null)
				_connector = new AllsrvConnector();

			return _connector;
		}

		/// <summary>
		/// Disconnects from Allsrv
		/// </summary>
		public void Disconnect ()
		{
			if (_cookie != 0)
			{
				_connector = null;

				try
				{
					_session.DeactivateAllEvents();
				}
				catch (Exception)
				{	
					// An exception will be thrown if the session has already been detached
					// Ignore it!
					TagTrace.WriteLine(TraceLevel.Verbose, "Error while attempting to detach session. Session is already destroyed.");
				}
				finally
				{
					_session = null;
				}

				try
				{
					_icp.Unadvise(_cookie);
				}
				catch (Exception e)
				{
					// Ignore exceptions if we're already detached
					TagTrace.WriteLine(TraceLevel.Verbose, "Error while disposing session. Session is already disposed.", e.Message);
				}
				finally
				{
					_cookie = 0;
				}
			}
		}

		/// <summary>
		/// Starts Allsrv in desktop or service mode as installed
		/// </summary>
		public static void StartAllsrv ()
		{
			try
			{
				// If Allsrv is a stopped service, start it!
				if (TagServiceController.AllsrvService != null)
				{
					if (TagServiceController.IsAllsrvServiceStarted() == false)
					{
						TagServiceController.StartAllsrvService();
						TagTrace.WriteLine(TraceLevel.Info, "Allsrv Service started.");
					}
				}
				else
				{
					// Start Allsrv in console mode
					string ServerEXEPath = null;

					// Get registry key
					RegistryKey ServerKey = Registry.LocalMachine.OpenSubKey(ALLEGIANCESERVERREGKEY);
					if (ServerKey != null)
					{
						ServerEXEPath = ServerKey.GetValue("EXE Path").ToString();
						ServerKey.Close();
					}
					else
					{
						throw new ApplicationException("Could not find Allsrv.exe");
					}
				
					// start executable
					if (ServerEXEPath != null)
					{
						Process.Start(ServerEXEPath);
						TagTrace.WriteLine(TraceLevel.Info, "Allsrv started in console mode.");
					}
				}
				Thread.Sleep(1500);	// Wait a second for Allsrv to start
			}
			catch (Exception e)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error starting Allsrv: {0}", e.Message);
			}
		}

		/// <summary>
		/// Stops Allsrv, either via AGClib, stopping the service, or by manually killing the process as appropriate
		/// </summary>
		public static void StopAllsrv ()
		{
			// If Allsrv service isn't stopped, stop it!
			if (TagServiceController.AllsrvService != null)
			{
				if (TagServiceController.IsAllsrvServiceStarted())
					TagServiceController.StopAllsrvService();
			}
			else
			{
				// TODO: Should I kill allsrv.exe process? Kinda ugly...
			}
		}

		/// <summary>
		/// Checks to see if Allsrv is running
		/// </summary>
		/// <returns>True if Allsrv is running, False if it is not</returns>
		public static bool IsRunning ()
		{
			bool Result = false;

			string[] AllsrvNames = new string[2] {TagServiceController.ALLSRVNAME, TagServiceController.ALLSRVNAME32};
			
			foreach (string Name in AllsrvNames)
			{
				Process[] RunningProcesses = Process.GetProcessesByName(Name);
				if (RunningProcesses.Length > 0)
				{
					Result = true;
					break;
				}
			}

			return Result;
		}

		/// <summary>
		/// Callback that receives the AGCEvents from Allsrv
		/// </summary>
		public void OnEvent(IAGCEvent pEvent)
		{
			TagTrace.WriteLine(TraceLevel.Verbose, "AllsrvConnector::OnEvent(): Event received: {0}", pEvent.ID);
			ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessEvent), new AGCEventWrapper(pEvent));
		}

		/// <summary>
		/// Asynchronously processes the received event from Allsrv
		/// </summary>
		/// <param name="state">The event object received from Allsrv</param>
		public void ProcessEvent (object state)
		{
			// Quit if no event was received
			if (state == null)
				return;

			string EventName = string.Empty;
			try
			{
				/// BT - 4/16/2012 - ACSS / Mach
				/// Passing an unwrapped __ComObject through a thread pool causes InvalidCastExcpetion under VS2010.
				if (state is AGCEventWrapper == false)
					TagTrace.WriteLine(TraceLevel.Verbose, "AllsrvConnector::ProcessEvent(): state is not an AGCEventWrapper, skipping.");

				// Cast param to an event
				AGCEventWrapper pEvent = (AGCEventWrapper)state;
				EventName = pEvent.ID.ToString();

				// Parse the event args, throwing the necessary AllsrvConnector Event
				switch (pEvent.ID)
				{
					case AGCEventID.EventID_AdminPage:
						AdminPagedEvent(this, new AdminPagedAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_ChatMessage:
						ChatMessageEvent(this, new ChatMessageAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_GameCreated:
						GameCreatedEvent(this, new GameCreatedAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_GameDestroyed:
						GameDestroyedEvent(this, new GameDestroyedAGCEventArgs(pEvent));
						break;
					case AGCEventID.AllsrvEventID_GameEnded:
						GameEndedEvent(this, new GameEndedAGCEventArgs(pEvent));
						break;
					case AGCEventID.AllsrvEventID_GameOver:
						GameOverEvent(this, new GameOverAGCEventArgs(pEvent));
						break;
					case AGCEventID.AllsrvEventID_GameStarted:
						GameStartedEvent(this, new GameStartedAGCEventArgs(pEvent));
						break;
					case AGCEventID.AllsrvEventID_ConnectedLobby:
						LobbyConnectedEvent(this, new LobbyConnectedAGCEventArgs(pEvent));
						break;
					case AGCEventID.AllsrvEventID_DisconnectedLobby:
						LobbyDisconnectedEvent(this, new LobbyDisconnectedAGCEventArgs(pEvent));
						break;
					case AGCEventID.AllsrvEventID_DisconnectingLobby:
						LobbyDisconnectingEvent(this, new LobbyDisconnectingAGCEventArgs(pEvent));
						break;
					case AGCEventID.AllsrvEventID_LostLobby:
						LobbyLostEvent(this, new LobbyLostAGCEventArgs(pEvent));
						break;
					case AGCEventID.AllsrvEventID_PlayerDropped:
						PlayerDroppedEvent(this, new PlayerDroppedAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_JoinTeam:
						PlayerJoinedTeamEvent(this, new PlayerJoinedTeamAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_LeaveTeam:
						PlayerLeftTeamEvent(this, new PlayerLeftTeamAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_LoginGame:
						PlayerLoggedInEvent(this, new PlayerLoggedInAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_LogoutGame:
						PlayerLoggedOutEvent(this, new PlayerLoggedOutAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_ShipKilled:
						ShipKilledEvent(this, new ShipKilledAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_StationChangesSides:
						StationCapturedEvent(this, new StationCapturedAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_StationCreated:
						StationCreatedEvent(this, new StationCreatedAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_StationDeleted:
						StationDestroyedEvent(this, new StationDestroyedAGCEventArgs(pEvent));
						break;
					case AGCEventID.EventID_TeamInfoChange:
						TeamInfoChangedEvent(this, new TeamInfoChangedAGCEventArgs(pEvent));
						break;
					case AGCEventID.AllsrvEventID_Terminate:
						TerminateEvent(this, new TerminateAGCEventArgs(pEvent));
						break;
					default:
						TagTrace.WriteLine(TraceLevel.Verbose, "Ignoring unhandled event: {0}", pEvent.ID.ToString());
						break;
				}
			}
			catch (Exception e)
			{
				string Message = e.Message;
				if (e.Message.Equals(string.Empty))
				{
					if (e.InnerException != null)
						Message = e.InnerException.Message;
				}

				TagTrace.WriteLine(TraceLevel.Error, "Error processing {0}: {1}", EventName, Message);
				TagTrace.WriteLine(TraceLevel.Error, "   Exception: {0}", e.ToString());
			}
		}

		/// <summary>
		/// Retrieves the state of the connection to Allsrv
		/// </summary>
		public bool State
		{
			get {return _cookie != 0;}
		}

		/// <summary>
		/// Retrieves the session to Allsrv
		/// </summary>
		public IAdminSession Session
		{
			get {return _session;}
		}

		/// <summary>
		/// Event fired when an AdminPaged event is received from Allsrv
		/// </summary>
		public event AdminPagedAGCEventDelegate AdminPagedEvent;

		/// <summary>
		/// Event fired when a ChatMessage event is received from Allsrv
		/// </summary>
		public event ChatMessageAGCEventDelegate ChatMessageEvent;

		/// <summary>
		/// Event fired when a GameCreated event is received from Allsrv
		/// </summary>
		public event GameCreatedAGCEventDelegate GameCreatedEvent;

		/// <summary>
		/// Event fired when a GameDestroyed event is received from Allsrv
		/// </summary>
		public event GameDestroyedAGCEventDelegate GameDestroyedEvent;

		/// <summary>
		/// Event fired when a GameEnded event is received from Allsrv
		/// </summary>
		public event GameEndedAGCEventDelegate GameEndedEvent;

		/// <summary>
		/// Event fired when a GameOver event is received from Allsrv
		/// </summary>
		public event GameOverAGCEventDelegate GameOverEvent;

		/// <summary>
		/// Event fired when a GameStarted event is received from Allsrv
		/// </summary>
		public event GameStartedAGCEventDelegate GameStartedEvent;

		/// <summary>
		/// Event fired when a LobbyConnected event is received from Allsrv
		/// </summary>
		public event LobbyConnectedAGCEventDelegate LobbyConnectedEvent;

		/// <summary>
		/// Event fired when a LobbyDisconnected event is received from Allsrv
		/// </summary>
		public event LobbyDisconnectedAGCEventDelegate LobbyDisconnectedEvent;

		/// <summary>
		/// Event fired when a LobbyDisconnecting event is received from Allsrv
		/// </summary>
		public event LobbyDisconnectingAGCEventDelegate LobbyDisconnectingEvent;

		/// <summary>
		/// Event fired when a LobbyLost event is received from Allsrv
		/// </summary>
		public event LobbyLostAGCEventDelegate LobbyLostEvent;

		/// <summary>
		/// Event fired when a PlayerDropped event is received from Allsrv
		/// </summary>
		public event PlayerDroppedAGCEventDelegate PlayerDroppedEvent;

		/// <summary>
		/// Event fired when a PlayerJoinedTeam event is received from Allsrv
		/// </summary>
		public event PlayerJoinedTeamAGCEventDelegate PlayerJoinedTeamEvent;

		/// <summary>
		/// Event fired when a PlayerLeftTeam event is received from Allsrv
		/// </summary>
		public event PlayerLeftTeamAGCEventDelegate PlayerLeftTeamEvent;

		/// <summary>
		/// Event fired when a PlayerLoggedIn event is received from Allsrv
		/// </summary>
		public event PlayerLoggedInAGCEventDelegate PlayerLoggedInEvent;

		/// <summary>
		/// Event fired when a PlayerLoggedOut event is received from Allsrv
		/// </summary>
		public event PlayerLoggedOutAGCEventDelegate PlayerLoggedOutEvent;

		/// <summary>
		/// Event fired when a ShipKilled event is received from Allsrv
		/// </summary>
		public event ShipKilledAGCEventDelegate ShipKilledEvent;

		/// <summary>
		/// Event fired when a StationCaptured event is received from Allsrv
		/// </summary>
		public event StationCapturedAGCEventDelegate StationCapturedEvent;

		/// <summary>
		/// Event fired when a StationCreated event is received from Allsrv
		/// </summary>
		public event StationCreatedAGCEventDelegate StationCreatedEvent;

		/// <summary>
		/// Event fired when a StationDestroyed event is received from Allsrv
		/// </summary>
		public event StationDestroyedAGCEventDelegate StationDestroyedEvent;

		/// <summary>
		/// Event fired when a TeamInfoChanged event is received from Allsrv
		/// </summary>
		public event TeamInfoChangedAGCEventDelegate TeamInfoChangedEvent;

		/// <summary>
		/// Event fired when a TeamInfoChanged event is received from Allsrv
		/// </summary>
		public event TerminateAGCEventDelegate TerminateEvent;

		/// <summary>
		/// Retrieves the name of the Allsrv service, or null if it is not installed
		/// </summary>
		public static string AllsrvServiceName
		{
			get
			{
				if (TagServiceController.AllsrvService != null)
					return TagServiceController.AllsrvService.ServiceName;
				else
					return null;
			}
		}
	}
}
