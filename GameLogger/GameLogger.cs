using System;
using System.IO;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Stores completed game logs according to the user's settings
	/// </summary>
	public class GameLogger
	{
		/// <summary>
		/// Initializes game logging with the specified settings
		/// </summary>
		/// <param name="xmlPath">The path to a folder where game XML files will be written, or null if no logging</param>
		public static void Initialize (string xmlPath)
		{
			
			// Configure necessary loggers
			if (xmlPath != null)
				XmlLogger.Initialize(xmlPath);
		}

		/// <summary>
		/// Uninitializes all loggers
		/// </summary>
		public static void Uninitialize ()
		{
		}

		/// <summary>
		/// Logs the specified game to the destinations specified in the config file
		/// </summary>
		/// <param name="game">The gamedata to log</param>
		/// <param name="gameID">ASGS's ID of this game</param>
		public static void LogGame (GameData game, int gameID)
		{
			GameDataset Data = game.GetDataset();
			string DataXml = Data.GetXml();

			XmlLogger.LogGame(game, gameID);
		}
	}
}
