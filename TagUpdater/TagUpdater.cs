using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows.Forms;

namespace FreeAllegiance.Tag.Updater
{
	/// <summary>
	/// Updates TAG by downloading the latest version from a central server
	/// </summary>
	class TagUpdater
	{
		private const string TAGUPDATEURL = "http://downloads.alleg.net/tag";
		private const string FILELIST = "filelist.txt";
		private const int	 EXITTIMER = 10000;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				// Wait for TAG to shut down...
				Console.WriteLine("Waiting for TAG to shut down...");
				Process[] Processes = Process.GetProcessesByName("TAG");
				if (Processes.Length > 0)
				{
					Process TagProcess = Processes[0];
					TagProcess.WaitForExit(EXITTIMER);
					if (!TagProcess.HasExited)
					{
						Console.WriteLine("TAG failed to shut down. Aborting update.");
						return;
					}
				}

				Console.WriteLine("TAG is not running. Updating...");

				// Get tagfilelist.txt from server
				string FileListUrl = string.Concat(TAGUPDATEURL, "/", FILELIST);
				WebClient Client = new WebClient();
				byte[] Filelist = Client.DownloadData(FileListUrl);

				// open tagfilelist.txt
				StreamReader FilelistReader = new StreamReader(new MemoryStream(Filelist));

				while (true)
				{
					string Line = FilelistReader.ReadLine();

					// Quit if we're at the end of the file
					if (Line == null)
						break;
					if (Line.Equals(string.Empty))
						continue;

					// Ignore comments
					if (Line.StartsWith("//"))
						continue;

					// parse filename and version
					int SpacePosition = Line.IndexOf(" ");
					string Filename = Line.Substring(0, SpacePosition);
					string Version = Line.Substring(SpacePosition + 1);

					// If we have an older one than specified...
					if (NeedsUpdating(Filename, Version))
					{
						Console.WriteLine("Updating {0} to v{1}", Filename, Version);
						// Prepare the request string for the updated file
						string FilenameUrl = string.Concat(TAGUPDATEURL, "/", Filename);

						// Prepare the local path to write
						string NewFilename = Application.StartupPath + "\\" + Filename;

						// Delete the old file
						if (File.Exists(NewFilename))
							File.Delete(NewFilename);
					
						// Download the new file
						Client.DownloadFile(FilenameUrl, NewFilename);
					}
				}
				// Close the filelist reader
				FilelistReader.Close();
				Client.Dispose();

				// launch TAG service or console as necessary
				ServiceController TagServiceController = GetTagServiceController();
				if (TagServiceController != null)
				{
					TagServiceController.Start();
				}
				else
				{
					string TagPath = Application.StartupPath + "\\" + "Tag.exe";
					Process.Start(new ProcessStartInfo(TagPath));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("An error occurred while updating: {0}", e.Message);
			}
		}

		/// <summary>
		/// Determines if the specified file is out of date or not
		/// </summary>
		/// <param name="filename">The filename to check</param>
		/// <param name="version">The newest version of the file</param>
		/// <returns>True if the local file is older than version
		///			False if the local file is the same or newer than version</returns>
		private static bool NeedsUpdating (string filename, string version)
		{
			bool Result = true;	// Update by default

			// All txt files have version 0, so we don't need to update them
			if (version.Equals("0"))
				return false;

			Version NewestVersion = new Version(version);
			string Filename = Application.StartupPath + "\\" + filename;
			
			// If file doesn't exist, download it
			if (!File.Exists(Filename))
				return true;

			// If it's a dll or exe...
			if (Filename.EndsWith("dll") || Filename.EndsWith("exe"))
			{
				// Get the current version
				AssemblyName Name = AssemblyName.GetAssemblyName(Filename);
				Version AssemblyVersion = Name.Version;
				
				// If we have the current or better version, don't download
				if (AssemblyVersion >= NewestVersion)
					Result = false;
				else
					Result = true;
			}

			return Result;
		}

		/// <summary>
		/// Retrieves the TAG service
		/// </summary>
		/// <returns>A reference to the TAG Service controller, or null if in console mode</returns>
		private static ServiceController GetTagServiceController ()
		{
			ServiceController Result = null;

			// Get a list of all windows services, and loop through
			ServiceController[] Controllers = ServiceController.GetServices();
			foreach (ServiceController Controller in Controllers)
			{
				if (Controller.ServiceName.Equals("TAGService"))
				{	// If this service is named Allsrv, then we found it
					Result = Controller;
					break;
				}
			}

			return Result;
		}
	}
}
