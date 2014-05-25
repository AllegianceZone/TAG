using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// A delegate used to pass the update command to the TAG project
	/// </summary>
	public delegate void ManualUpdateDelegate (object sender, EventArgs e);

	/// <summary>
	/// Provides services to check for updates to TAG, and initiate an update cycle
	/// </summary>
	public class TagUpdate
	{
		private const string TAGUPDATEURL = "http://downloads.alleg.net/tag";
		private const string FILELIST = "filelist.txt";

		/// <summary>
		/// An event used to signal that a manual update was triggered
		/// </summary>
		public static event ManualUpdateDelegate UpdateTriggeredEvent;

		/// <summary>
		/// Checks to see if updates to TAG are available
		/// </summary>
		/// <returns>True if updates are available. False if TAG is up to date.</returns>
		public static bool UpdateAvailable ()
		{
			bool Result = false;

			try
			{
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

					if (Line.StartsWith("//"))
						continue;

					// parse filename and version
					int SpacePosition = Line.IndexOf(" ");
					string Filename = Line.Substring(0, SpacePosition);
					string Version = Line.Substring(SpacePosition + 1);

					// If we have an older one than specified...
					if (NeedsUpdating(Filename, Version))
					{
						Result = true;
						break;
					}
				}
				FilelistReader.Close();
				Client.Dispose();
			}
			catch (Exception e)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error checking for updates: {0}", e.Message);
			}

			return Result;
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

			if (version.Equals("0"))	// Don't update text files
				return false;

			Version NewestVersion = new Version(version);
			string Filename = Application.StartupPath + "\\" + filename;

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
		/// Determines if TAG can update. If any games are in progress, TAG cannot update
		/// </summary>
		/// <returns>True if no games are in progress, false if any game is running</returns>
		public static bool IsAbleToUpdate ()
		{
			bool Result = true;

			foreach (Game AGame in GameServer.Games)
			{
				if (AGame.InProgress)
				{
					Result = false;
					break;
				}
			}

			return Result;
		}

		/// <summary>
		/// Triggers launching TAGUpdate.exe, and shutting TAG down
		/// </summary>
		public static void InitiateUpdate ()
		{
			if (UpdateTriggeredEvent != null)
				UpdateTriggeredEvent(null, new EventArgs());
		}
	}
}
