using System;
using System.Data;
using System.Diagnostics;

using AGCLib;
using ALLEGIANCESERVERLib;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Provides access to the Allegiance Server's information
	/// </summary>
	public class GameServer
	{
		private static Players			_players = null;
		private static Games			_games = null;
		private static AllsrvConnector	_connector = null;

		private static IAdminServer		_server = null;

		/// <summary>
		/// Initializes the GameServer and reads the games from the server
		/// </summary>
		public static void Initialize ()
		{
			_players = new Players(_server);

			// Start Allsrv if not already started
			if (!AllsrvConnector.IsRunning())
				AllsrvConnector.StartAllsrv();

			// Connect to Allsrv and retrieve the server object
			_connector = AllsrvConnector.Connect();
			_server = _connector.Session.Server;
			TagTrace.WriteLine(TraceLevel.Info, "Connection to Allsrv established.");
			
			// Load Player list
			TagTrace.WriteLine(TraceLevel.Verbose, "Retrieving players...");
			_players.Initialize(_server);
			TagTrace.WriteLine(TraceLevel.Info, "Playerlist loaded. {0} players online.", _players.Count);
			
			// Read the games
			TagTrace.WriteLine(TraceLevel.Verbose, "Reading Games...");
			_games = new Games();
			for (int i = 0; i < _server.Games.Count; i++)
			{
				object Index = (object)i;
				IAGCGame TempGame = _server.Games.get_Item(ref Index);
				Game NewGame = new Game(TempGame);
				_games.Add(NewGame);

				// Initialize game if in progress
				if (NewGame.InProgress)
				{
					// Get StartTime
					DateTime StartTime = DateTime.FromOADate(TempGame.GameParameters.TimeStart);

					// Initialize game
					NewGame.Start(StartTime);

					// Log event
					NewGame.GameData.LogGameStarted(StartTime);
				}
			}
			TagTrace.WriteLine(TraceLevel.Info, "Games read. {0} games online.", _games.Count);

			// Connect event handlers
			TagTrace.WriteLine(TraceLevel.Verbose, "Hooking up events...");
			AGCEventHandler.Initialize(_connector);
			TagTrace.WriteLine(TraceLevel.Verbose, "Events connected.");
		}

		/// <summary>
		/// Disconnects from Allsrv and releases any resources in use.
		/// </summary>
		public static void Disconnect ()
		{
			if (_connector != null)
			{
				_connector.Disconnect();
				_connector = null;
			}

			_server = null;
			if (_games != null)
			{
				_games.Clear();
				_games = null;
			}

			if (_players != null)
			{
				_players.Clear();
				_players = null;
			}
		}

		/// <summary>
		/// Attempts to load the game with the specified ID from Allsrv
		/// </summary>
		/// <param name="gameID">The ID of the game to load</param>
		/// <returns>A reference to the newly-added game</returns>
		public static Game LoadGame (int gameID)
		{
			Game Result = null;

			if (gameID > 0)
			{
				for (int i = 0; i < _server.Games.Count; i++)
				{
					object Index = i;
					IAGCGame NewGame = _server.Games.get_Item(ref Index);
					if (NewGame.GameID == gameID)
					{
						Result = new Game(NewGame);
						_games.Add(Result);
						break;
					}
				}
			}

			return Result;
		}

		/// <summary>
		/// Sends a private message to the specified user
		/// </summary>
		/// <param name="callsign">The callsign of the user</param>
		/// <param name="message">The message to send</param>
		public static void SendChat (string callsign, string message)
		{
			foreach (IAdminUser User in _server.Users)
			{
				if (User.Name.Equals(callsign))
				{
					User.SendMsg(message);
					break;
				}
			}
		}

		/// <summary>
		/// Sends the specified message from the Admin callsign
		/// </summary>
		/// <param name="message">The message to say as Admin</param>
		public static void SendChat (string message)
		{
			_server.SendMsg(message);
		}

		/// <summary>
		/// Boots the specified player off of the server
		/// </summary>
		/// <param name="callsign">The callsign of the user to boot</param>
		public static void BootUser (string callsign)
		{
			IAdminUser User = _server.get_FindUser(callsign);
			if (User != null)
				User.Boot();
		}

		/// <summary>
		/// A list of games currently running on this server
		/// </summary>
		public static Games Games
		{
			get {return _games;}
		}

		/// <summary>
		/// A list of players on this server
		/// </summary>
		public static Players Players
		{
			get {return _players;}
		}
	}
}
