using System;
using System.IO;
using System.Data;
using System.Diagnostics;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Persists a completed game to Xml
	/// </summary>
	public class XmlLogger
	{
		private static bool		_isInitialized = false;
		private static string	_path = string.Empty;

		/// <summary>
		/// Initializes the XmlLogging to the specified folder
		/// </summary>
		/// <param name="path">The path to the folder where GameLogs should be stored</param>
		public static void Initialize (string path)
		{
			if (path != null)
			{
				// Expand path to fully-qualified
				_path = Path.GetFullPath(path);
				_isInitialized = true;
			}

		}

		/// <summary>
		/// Logs the specified game to an Xml file in the configured folder
		/// </summary>
		/// <param name="game">The Xml text of the game to log</param>
		/// <param name="gameID">The ID of the game if known</param>
		public static void LogGame (GameData game, int gameID)
		{
			if (_isInitialized == false)
				return;

			// Prepare filename
			DateTime Now = DateTime.Now;

			// Prepare name of file
			string FileName = string.Format("{0}.{1:00}.{2:00}_{3:00}.{4:00}-Game {5}.xml", Now.Year, Now.Month, Now.Day, Now.Hour, Now.Minute, (gameID > 0) ? gameID.ToString() : "unknown");
			string FilePath = string.Concat(_path, "\\", FileName);

			// Create directory if it does not already exist
			if (!Directory.Exists(_path))
				Directory.CreateDirectory(_path);

			// Write game to file
			game.GetDataset().WriteXml(FilePath, XmlWriteMode.WriteSchema);
			TagTrace.WriteLine(TraceLevel.Info, "Game written to disk: {0}", FileName);
		}
	}
}
