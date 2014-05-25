using System;
using System.Threading;
using System.Collections;

using AGCLib;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// An Allegiance Game
	/// </summary>
	public class Game : IDisposable
	{
		private IAGCGame			_game;		// A reference to the AGCGame object within Allsrv
		private int				_gameID;	// The ID of this game
		private GameData		_gameData;	// The Data of this game
		private object			_syncroot = new object();	// Synchronizes access to this GameData
		private ManualResetEvent _gameEndedMRE;

		private bool			_debugKills	= false;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="game">Allsrv's IAGCGame interface</param>
		public Game (IAGCGame game)
		{
			_game = game;
			_gameID = game.GameID;
			_gameData = new GameData(game.Name, game.GameParameters.IGCStaticFile, GameSettingsHelper.GetMapName(game),
									game.GameParameters.IsSquadGame, game.GameParameters.IsGoalConquest,
									game.GameParameters.IsGoalTeamKills, game.GameParameters.GoalTeamKills, 
									game.GameParameters.AllowFriendlyFire, game.GameParameters.ShowMap,
									game.GameParameters.AllowDevelopments, game.GameParameters.AllowShipyardPath,
									game.GameParameters.AllowDefections, game.GameParameters.InvulnerableStations, 
									game.GameParameters.ScoresCount, game.GameParameters.MaxImbalance, game.GameParameters.StartingMoney,
									game.GameParameters.He3Density, GameSettingsHelper.GetResources(game),
									DateTime.FromOADate(game.GameParameters.TimeStart), DateTime.FromOADate(game.GameParameters.TimeStart));
			_gameEndedMRE = new ManualResetEvent(false);
		}

		/// <summary>
		/// Synchronizes access to this game
		/// </summary>
		/// <returns>An object that can be used to synchronize this game</returns>
		public object GetSyncRoot ()
		{
			return _syncroot;
		}

		/// <summary>
		/// Retrieves the event used to synchronize GameEnded and GameOver events
		/// </summary>
		/// <returns></returns>
		public ManualResetEvent GetGameEndedMRE ()
		{
			return _gameEndedMRE;
		}

		#region IDisposable Members

		/// <summary>
		/// Disposes of any resources used by this game
		/// </summary>
		public void Dispose ()
		{
			_gameData.Dispose();
			_game = null;
		}

		#endregion

		/// <summary>
		/// Copies all game/team parameters into this game's GameData object
		/// </summary>
		/// <param name="startTime">The time this game started</param>
		public void Start (DateTime startTime)
		{
			// Create 
			_gameData.Initialize(_game.Name, _game.GameParameters.IGCStaticFile, GameSettingsHelper.GetMapName(_game),
									_game.GameParameters.IsSquadGame, _game.GameParameters.IsGoalConquest,
									_game.GameParameters.IsGoalTeamKills, _game.GameParameters.GoalTeamKills, 
									_game.GameParameters.AllowFriendlyFire, _game.GameParameters.ShowMap,
									_game.GameParameters.AllowDevelopments, _game.GameParameters.AllowShipyardPath,
									_game.GameParameters.AllowDefections, _game.GameParameters.InvulnerableStations, 
									_game.GameParameters.ScoresCount, _game.GameParameters.MaxImbalance, _game.GameParameters.StartingMoney,
									_game.GameParameters.He3Density, GameSettingsHelper.GetResources(_game),
									DateTime.FromOADate(_game.GameParameters.TimeStart), DateTime.FromOADate(_game.GameParameters.TimeStart));

			_gameData.InitializeTeamCount(_game.Teams.Count);
			string CommanderName = string.Empty;
			for (int i = 0; i < _game.Teams.Count; i++)
			{
				object Index = i;
				IAGCTeam Team = _game.Teams.get_Item(ref Index);

				CommanderName = GetCommander(Team);
				_gameData.InitializeTeam(i, Team.Name, Team.Civ, CommanderName);

				// Add team members to data
				for (int j = 0; j < Team.Ships.Count; j++)
				{
					object ShipIndex = j;
					IAGCShip Ship = Team.Ships.get_Item(ref ShipIndex);
					if (IsHuman(Ship))
						_gameData.AddTeamMember(Ship.Name, i, startTime);
				}
			}
		}

		/// <summary>
		/// Determines if the specified ship is human
		/// </summary>
		/// <param name="ship">The ship to determine if it's human</param>
		/// <returns>True if human, false if not</returns>
		public bool IsHuman (IAGCShip ship)
		{
			bool Result = false;

			try
			{
				if (ship != null)
				{
					if (ship.HullType != null)
					{
						short Capabilities = ship.HullType.Capabilities;
						if ((Capabilities & 16384) != 16384 &&
							(Capabilities & 32768) != 32768 &&
							(Capabilities & 1024) != 1024)
							Result = true;
					}
				}
			}
			catch (Exception)
			{
				// Checking Capabilities sometimes throws an exception on drones
			}

			return Result;
		}

		/// <summary>
		/// Analyzes all players' donating status to determine which player has
		/// the most donatees.
		/// </summary>
		public string GetCommander (IAGCTeam team)
		{
			Hashtable Commanders = new Hashtable(team.Ships.Count);

			// Build a list of all players who have someone donating to them
			for (int j = 0; j < team.Ships.Count; j++)
			{
				object ShipIndex = j;
				IAGCShip Ship = team.Ships.get_Item(ref ShipIndex);
				// Ignore non-human ships
				if (!IsHuman(Ship))
					continue;

				// Get the name of the player they're donating to, or themselves if not donating
				string DonateeName = (Ship.AutoDonate != null) ? Ship.AutoDonate.Name : Ship.Name;
			
				if (Commanders.ContainsKey(DonateeName))
				{	// If we've already got them, increment the # of players donating to them.
					int Count = (int)Commanders[DonateeName];
					Commanders[DonateeName] = Count++;
				}
				else
				{	// We don't have them, so add them with 1 player donating to them.
					Commanders.Add(DonateeName, 1);
				}
			}

			// Now we have a list of all players who have someone donating to them, and
			// the # of players that are donating to them.
			// The one with the highest # of players donating to them is the commander.
			string Commander = string.Empty;
			int NumDonaters = 0;
			foreach (string Donatee in Commanders.Keys)
			{
				int Donaters = (int)Commanders[Donatee];
				if (Donaters > NumDonaters)
				{	// This donatee has more donaters than the last guy...
					Commander = Donatee;
					NumDonaters = Donaters;
				}
			}

			return Commander;
		}

		/// <summary>
		/// Retrieves the index of the specified team
		/// </summary>
		/// <param name="teamID">The ID of the team</param>
		/// <returns>The index of the team with the specified ID</returns>
		public int GetTeamIndex (int teamID)
		{
			int Result = 0;

			for (int i = 0; i < _game.Teams.Count; i++)
			{
				object Index = (object)i;
				IAGCTeam Team = _game.Teams.get_Item(ref Index);

				if (Team.ObjectID == teamID)
					break;

				Result++;
			}

			return Result;
		}

//		/// <summary>
//		/// "Smites" the player with the specified callsign by wiping their ammo/energy/fuel/shields.
//		/// </summary>
//		/// <param name="callsign">The callsign of the player to be smited</param>
//		/// <param name="smiter">The callsign of the player who performed the #smite</param>
//		/// <returns>True if player smited, false if not</returns>
//		public bool SmitePlayer (string callsign, string smiter)
//		{
//			bool Result = false;
//			AGCShip Player = null;
//
//			for (int i = 0; i < _game.Ships.Count; i++)
//			{
//				object Index = i;
//				IAGCShip Ship = _game.Ships.get_Item(ref Index);
//				if (callsign.Equals(Ship.Name))
//				{
//					Player = Ship;
//					break;
//				}
//			}
//
//			if (Player != null)
//			{
//				Player.ShieldFraction = 0.0F;
//
//				Player.SendChat("You have been smited by " + smiter + "!!", -1);
//				Player.SendChat("Don't worry: This joke function will be removed when TAG is ready to go live ;)", -1);
//
//				Result = true;
//			}
//
//			return Result;
//		}

		/// <summary>
		/// Sends the specified message from the Admin callsign
		/// </summary>
		/// <param name="text">The message to say as Admin</param>
		public void SendChat (string text)
		{
			_game.SendChat(text, -1);
		}

		/// <summary>
		/// The ID of this game
		/// </summary>
		public int GameID
		{
			get {return _gameID;}
		}

		/// <summary>
		/// The name of this game
		/// </summary>
		public string GameName
		{
			get {return _gameData.GameRow.GameName;}
		}

		/// <summary>
		/// Returns the GameData of this game
		/// </summary>
		public GameData GameData
		{
			get {return _gameData;}
			set {_gameData = value;}
		}

		/// <summary>
		/// The number of teams in this game
		/// </summary>
		public int NumTeams
		{
			get {return _game.Teams.Count;}
		}

		/// <summary>
		/// Determines whether or not this game is started
		/// </summary>
		public bool InProgress
		{
			get {return (_game.GameStage == AGCGameStage.AGCGameStage_Started);}
		}

		/// <summary>
		/// Turns on kill debugging
		/// </summary>
		public bool DebugKills
		{
			get {return _debugKills;}
			set {_debugKills = value;}
		}
	}
}
