using System;
using System.Diagnostics;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// All the events captured during a game
	/// </summary>
	public class GameData : IDisposable
	{
		private GameDataset		_gameDataset;	// The dataset that holds this game's events

		/// <summary>
		/// Default Constructor
		/// </summary>
		public GameData ()
		{
			_gameDataset = new GameDataset();

			this.Reset();
		}

		/// <summary>
		/// Full Constructor
		/// </summary>
		/// <param name="gameName">The name of this game as listed in the Lobby</param>
		/// <param name="coreFile">The name of the corefile in use</param>
		/// <param name="mapName">The name of the map in play</param>
		/// <param name="squadGame">Specifies that this is a squad game</param>
		/// <param name="conquest">Specifies that this game has a Conquest win condition</param>
		/// <param name="deathmatch">Specifies that this game has a Deathmatch win condition</param>
		/// <param name="deathmatchGoal">Specifies the goal for this deathmatch game</param>
		/// <param name="friendlyFire">Specifies that players can damage their own team's players and  bases</param>
		/// <param name="revealMap">Specifies that the map was revealed at the start of the game</param>
		/// <param name="allowDevelopment">Specifies that new technology can be researched</param>
		/// <param name="allowShipyards">Specifies that Shipyards can be purchased</param>
		/// <param name="allowDefections">Specifies that players can change teams</param>
		/// <param name="InvulnerableStations">Specifies that bases can never be destroyed</param>
		/// <param name="statsCount">Specifies whether or not stats counted for this game</param>
		/// <param name="maxImbalance">The maximum team player imbalance that will be enforced</param>
		/// <param name="startingMoney">A modifier for the amount of money given to teams at the start</param>
		/// <param name="totalMoney">A modifier for the total amount of money available on the map</param>
		/// <param name="resources">The resources setting, detailing which techrocks are available and where</param>
		/// <param name="startTime">The time this game begun</param>
		/// <param name="endTime">The time this game ended</param>
		public GameData (string gameName, string coreFile, string mapName,
							bool squadGame, bool conquest, bool deathmatch,
							int deathmatchGoal, bool friendlyFire,
							bool revealMap, bool allowDevelopment, 
							bool allowShipyards, bool allowDefections,
							bool invulnerableStations, bool statsCount, int maxImbalance,
							float startingMoney, float totalMoney,
							int resources, DateTime startTime, DateTime endTime)
		{
			_gameDataset = new GameDataset();

			Initialize(gameName, coreFile, mapName, squadGame, conquest, deathmatch, deathmatchGoal, friendlyFire, revealMap,
						allowDevelopment, allowShipyards, allowDefections, invulnerableStations, statsCount, maxImbalance,
						startingMoney, totalMoney, resources, startTime, endTime);
		}

		#region IDisposable Members
		/// <summary>
		/// Disposes of this GameData
		/// </summary>
		public void Dispose ()
		{
			_gameDataset.Dispose();
		}

		#endregion

		/// <summary>
		/// Resets the current game, wiping all events and adding a default game
		/// </summary>
		public void Reset ()
		{
			// Wipe all game data
			_gameDataset.Clear();

			// Add a new "default" row
			string Unknown = "unknown";
			DateTime Now = DateTime.Now;
			Initialize("New Game", Unknown, Unknown, false, false, false, 0, false,				
						false, false, false, false, false, false, 1, 1,1, 1, Now, Now);
		}

		/// <summary>
		/// Wipes the gamedata and creates a new game with the specified settings
		/// </summary>
		/// <param name="gameName">The name of this game as listed in the Lobby</param>
		/// <param name="coreFile">The name of the corefile in use</param>
		/// <param name="mapName">The name of the map in play</param>
		/// <param name="squadGame">Specifies that this is a squad game</param>
		/// <param name="conquest">Specifies that this game has a Conquest win condition</param>
		/// <param name="deathmatch">Specifies that this game has a Deathmatch win condition</param>
		/// <param name="deathmatchGoal">Specifies the goal for this deathmatch game</param>
		/// <param name="friendlyFire">Specifies that players can damage their own team's players and  bases</param>
		/// <param name="revealMap">Specifies that the map was revealed at the start of the game</param>
		/// <param name="allowDevelopment">Specifies that new technology can be researched</param>
		/// <param name="allowShipyards">Specifies that Shipyards can be purchased</param>
		/// <param name="allowDefections">Specifies that players can change teams</param>
		/// <param name="invulnerableStations">Specifies that bases can never be destroyed</param>
		/// <param name="statsCount">Specifies whether or not stats counted for this game</param>
		/// <param name="maxImbalance">The maximum team player imbalance that will be enforced</param>
		/// <param name="startingMoney">A modifier for the amount of money given to teams at the start</param>
		/// <param name="totalMoney">A modifier for the total amount of money available on the map</param>
		/// <param name="resources">The resources setting, detailing which techrocks are available and where</param>
		/// <param name="startTime">The time this game begun</param>
		/// <param name="endTime">The time this game ended</param>
		public void Initialize (string gameName, string coreFile, string mapName,
			bool squadGame, bool conquest, bool deathmatch,
			int deathmatchGoal, bool friendlyFire,
			bool revealMap, bool allowDevelopment, 
			bool allowShipyards, bool allowDefections,
			bool invulnerableStations, bool statsCount, int maxImbalance,
			float startingMoney, float totalMoney,
			int resources, DateTime startTime, DateTime endTime)
		{
			// Create row if it doesn't exist
			if (GameRow == null)
			{
				_gameDataset.Game.AddGameRow(1, gameName, coreFile, mapName, squadGame, 
												conquest, deathmatch, deathmatchGoal,
												friendlyFire, revealMap,
												allowDevelopment, allowShipyards, 
												allowDefections, invulnerableStations, statsCount,
												maxImbalance, startingMoney, totalMoney,
												resources, startTime, endTime);
			}
			else
			{
				// Update the game with the specified parameters
				object[] NewArray = new object[] {1, gameName, coreFile, mapName, squadGame, 
												  conquest, deathmatch, deathmatchGoal, 
												  friendlyFire, revealMap,
												  allowDevelopment, allowShipyards, 
												  allowDefections, invulnerableStations, statsCount,
												  maxImbalance, startingMoney, totalMoney,
												  resources, startTime, endTime};
				GameRow.ItemArray = NewArray;
			}
			
			
		}

		/// <summary>
		/// Wipes all teams and adds the specified number of blank teams
		/// </summary>
		/// <param name="teamCount">The number of blank teams to create</param>
		public void InitializeTeamCount (int teamCount)
		{
			_gameDataset.TeamMember.Clear();
			_gameDataset.Team.Clear();

			GameDataset.GameRow GameRow = _gameDataset.Game.FindByGameID(1);

			for (int i = 0; i < teamCount; i++)
			{
				_gameDataset.Team.AddTeamRow(i, GameRow, i, "Team " + i.ToString(), 
											"New Commander", "New Faction", false,
											false, false, false, false, false);
			}
		}

		/// <summary>
		/// Sets the specified team's name, faction, and commander
		/// </summary>
		/// <param name="teamNumber">The index of the team to initialize</param>
		/// <param name="teamName">The name of the team being initialized</param>
		/// <param name="teamFaction">The team's faction</param>
		/// <param name="teamCommander">The team's commander</param>
		public void InitializeTeam (int teamNumber, string teamName, string teamFaction, string teamCommander)
		{
			GameDataset.TeamRow TeamRow = _gameDataset.Team.FindByTeamID(teamNumber);
			if (TeamRow != null)
			{
				TeamRow["TeamName"] = teamName;
				TeamRow["Commander"] = teamCommander;
				TeamRow["Faction"] = teamFaction;
			}
		}

		/// <summary>
		/// Adds the specified player to the team with default join/leave times
		/// </summary>
		/// <param name="callsign">The callsign of the player to add</param>
		/// <param name="teamIndex">The index of the team</param>
		/// <param name="startTime">The time this player joined the team</param>
		public void AddTeamMember (string callsign, int teamIndex, DateTime startTime)
		{
			GameDataset.TeamRow TeamRow = _gameDataset.Team.FindByTeamID(teamIndex);
			if (TeamRow != null)
				_gameDataset.TeamMember.AddTeamMemberRow(TeamRow, callsign, 0, startTime, DateTime.MinValue);
		}

		/// <summary>
		/// Retrieves this game's dataset
		/// </summary>
		/// <returns>The dataset for this game</returns>
		public GameDataset GetDataset ()
		{
			return _gameDataset;
		}

		/// <summary>
		/// Retrieves this game's row from the dataset
		/// </summary>
		public GameDataset.GameRow GameRow
		{
			get {return _gameDataset.Game.FindByGameID(1);}
		}

		#region Log Methods
		/// <summary>
		/// Logs an admin page with the specified parameters to this game's dataset
		/// </summary>
		/// <param name="time">The time the admin was paged</param>
		/// <param name="speakerName">The name of the player paging the admin</param>
		/// <param name="message">The message sent to the admin</param>
		public void LogAdminPage (DateTime time, string speakerName, string message)
		{
			_gameDataset.ChatLog.AddChatLogRow(GameRow, time, speakerName, "Admin", message);
		}

		/// <summary>
		/// Tags this game with the specified name
		/// </summary>
		/// <param name="time">The time this game was tagged</param>
		/// <param name="speakerName">The callsign of the player who tagged this game</param>
		/// <param name="tagName">The name of the tag</param>
		public void TagGame (DateTime time, string speakerName, string tagName)
		{	
			// If someone tagged this game, log it
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.GameTagged, time, -1, speakerName, -1, tagName, -1, string.Empty);
		}

		/// <summary>
		/// Logs a chat message with the specified parameters to this game's dataset
		/// </summary>
		/// <param name="time">The time this chat message occurred</param>
		/// <param name="speakerID">The ID of the speaker within Allsrv</param>
		/// <param name="speakerName">The name of the player or drone who sent this message</param>
		/// <param name="recipientID">The ID of the recipient within Allsrv</param>
		/// <param name="recipientName">The name of the recipient player or drone</param>
		/// <param name="commandID">The ID of the command associated with this chat message (if any)</param>
		/// <param name="voiceID">The ID of the voicechat associated with this chat message (if any)</param>
		/// <param name="text">The contents of the message</param>
		public void LogChatMessage (DateTime time, int speakerID, string speakerName,
									int recipientID, string recipientName, 
									int commandID, int voiceID, string text)
		{
			string Speaker = (speakerName.Equals("Admin")) ? "HQ" : speakerName;
			string Text = (voiceID > -1) ? "Voice Chat: " + voiceID : text;

			_gameDataset.ChatLog.AddChatLogRow(GameRow, time, Speaker, recipientName, Text);

			if (speakerID == -1)
			{	// Admin has spoken. Check for resign and draw calls...
				if (Speaker.Equals("HQ"))
				{
					string ResignString = "'s proposal to resign has ";
					int ResignIndex = -1;
					ResignIndex = Text.IndexOf(ResignString);
					if (ResignIndex > -1)
					{
						// A resign was typed!
						// Did it pass or fail?
						int ResignEventID = (int)HandledEventIDs.ResignFailed;	// Fail by default
						if (Text.IndexOf("passed", ResignIndex) == ResignIndex + ResignString.Length)
							ResignEventID = (int)HandledEventIDs.ResignPassed;	// It passed!

						string Proposer = Text.Substring(0, ResignIndex);
						string Votes = Text.Substring(ResignIndex + ResignString.Length + 9);

						_gameDataset.GameEvent.AddGameEventRow(GameRow, ResignEventID, time, GameRow.GameID, Proposer, -1, Votes, -1, string.Empty);
						TagTrace.WriteLine(TraceLevel.Verbose, "{0}'s Resign proposal logged.", Proposer);
					}

					// Check for draw
					string DrawString = "proposal to offer a draw has";
					int DrawIndex = -1;
					DrawIndex = Text.IndexOf(DrawString);
					if (DrawIndex > -1)
					{
						// A draw was typed!
						// Did it pass or fail?
						int DrawEventID = (int)HandledEventIDs.DrawFailed;	// Fail by default
						if (Text.IndexOf("passed", DrawIndex) == DrawIndex + DrawString.Length)
							DrawEventID = (int)HandledEventIDs.DrawPassed;

						string Proposer = Text.Substring(0, DrawIndex - 3);
						string Votes = Text.Substring(DrawIndex + DrawString.Length + 10);

						_gameDataset.GameEvent.AddGameEventRow(GameRow, DrawEventID, time, GameRow.GameID, Proposer, -1, Votes, -1, string.Empty);
						TagTrace.WriteLine(TraceLevel.Verbose, "{0}'s Draw proposal logged.", Proposer);
					}
				}
			}
		}

//		/// <summary>
//		/// Logs a game created message to this game's dataset
//		/// </summary>
//		/// <param name="time">The time this game was created</param>
//		/// <param name="gameName">The name of the created game</param>
//		public void LogGameCreated (DateTime time, string gameName)
//		{
//			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.GameCreated, time, -1, gameName, -1, string.Empty, -1, string.Empty);
//		}
//
//		/// <summary>
//		/// Logs a game destroyed message to this game's dataset
//		/// </summary>
//		/// <param name="time">The time this game was destroyed</param>
//		/// <param name="gameName">The name of the destroyed game</param>
//		public void LogGameDestroyed (DateTime time, string gameName)
//		{
//			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.GameDestroyed, time, -1, gameName, -1, string.Empty, -1, string.Empty);
//		}

		/// <summary>
		/// Logs a game ended message to this game's dataset
		/// </summary>
		/// <param name="time">The time this game ended</param>
		/// <param name="winningTeamID">The ID of the winning team</param>
		/// <param name="winningTeamName">The name of the winning team</param>
		/// <param name="gameEndedReason">The reason that the game has ended</param>
		public void LogGameEnded (DateTime time, int winningTeamID, string winningTeamName, string gameEndedReason)
		{
			try
			{
				// Set GameEnded time
				GameRow.EndTime = time;

				// Assign winning team by ID if we're using FAZ R2 or later
				if (winningTeamID >= 0)
				{	// A specific team won. Record it
					GameDataset.TeamRow Team = _gameDataset.Team.FindByTeamID(winningTeamID);
					if (Team != null)
						Team.Won = true;
				}
				else
				{
					// We're using FAZ R1 or earlier. Assign win by teamname...

					// Clean teamname for lookup - teamnames can't have '
					string WinningTeamName = winningTeamName.Replace("'", "''");
				
					// Look up team
					string TeamSelect = string.Concat("TeamName = '", WinningTeamName, "'");
					GameDataset.TeamRow[] Teams = (GameDataset.TeamRow[])_gameDataset.Team.Select(TeamSelect);
					if (Teams.Length > 0)
					{
						GameDataset.TeamRow Team = Teams[0];
						Team.Won = true;
					}
				}
			
				if (gameEndedReason.Equals("The game was declared a draw"))
				{
					// It's a draw. Everybody loses!
					foreach (GameDataset.TeamRow Team in _gameDataset.Team.Rows)
						Team.Won = false;
				}

				TagTrace.WriteLine(TraceLevel.Verbose, "Tabulating durations for {0} TeamMembers...", _gameDataset.TeamMember.Rows.Count);
				// Tabulate teammember durations
				foreach (GameDataset.TeamMemberRow TeamMember in _gameDataset.TeamMember.Rows)
				{
					// Record leavetime for players in game
					if (TeamMember.LeaveTime == DateTime.MinValue)
						TeamMember.LeaveTime = time;
				
					// Calculate time on team
					TimeSpan Difference = TeamMember.LeaveTime.Subtract(TeamMember.JoinTime);
					int SecondsDifference = (int)Difference.TotalSeconds;

					// Record duration
					TeamMember.Duration = SecondsDifference;
				}

				// Log event
				_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.GameEnded, time, GameRow.GameID, GameRow.GameName, _gameDataset.TeamMember.Rows.Count, winningTeamName, -1, gameEndedReason);
			}
			catch (Exception e)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error logging GameEnded event: {0}", e.Message);
			}
		}

//		/// <summary>
//		/// Logs a game over message to this game's dataset
//		/// </summary>
//		/// <param name="time">The time this game was finished</param>
//		public void LogGameOver (DateTime time)
//		{
//			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.GameOver, time, GameRow.GameID, GameRow.GameName, -1, string.Empty, -1, string.Empty);
//		}

		/// <summary>
		/// Logs a game start to this game's dataset
		/// </summary>
		/// <param name="time">The time this game started</param>
		public void LogGameStarted (DateTime time)
		{
			// Set GameStarted time
			GameRow.StartTime = time;

			// Log event
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.GameStarted, time, GameRow.GameID, GameRow.GameName, _gameDataset.TeamMember.Rows.Count, string.Empty, -1, string.Empty);
		}

		/// <summary>
		/// Logs a lobby connected message to this game's dataset
		/// </summary>
		/// <param name="time">The time the lobby was connected</param>
		/// <param name="lobbyIP">The IP address of the connected lobby</param>
		public void LogLobbyConnected (DateTime time, string lobbyIP)
		{
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.LobbyConnected, time, -1, string.Empty, -1, string.Empty, -1, string.Empty);
		}

		/// <summary>
		/// Logs an attempt at disconnecting from the lobby to this game's dataset
		/// </summary>
		/// <param name="time">The time the server started to disconnect from the lobby</param>
		/// <param name="lobbyIP">The IP address of the lobby being disconnected</param>
		public void LogLobbyDisconnecting (DateTime time, string lobbyIP)
		{
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.LobbyDisconnecting, time, -1, string.Empty, -1, string.Empty, -1, string.Empty);
		}

		/// <summary>
		/// Logs a lobby disconnect to this game's dataset
		/// </summary>
		/// <param name="time">The time the lobby was disconnected</param>
		/// <param name="lobbyIP">The IP address of the disconnected lobby</param>
		public void LogLobbyDisconnected (DateTime time, string lobbyIP)
		{
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.LobbyDisconnected, time, -1, string.Empty, -1, string.Empty, -1, string.Empty);
		}

		/// <summary>
		/// Logs a dropped lobby event to this game's dataset
		/// </summary>
		/// <param name="time">The time the lobby connection was detected as lost</param>
		public void LogLobbyLost (DateTime time)
		{
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.LobbyLost, time, -1, string.Empty, -1, string.Empty, -1, string.Empty);
		}

		/// <summary>
		/// Logs a dropped player event to this game's dataset
		/// </summary>
		/// <param name="time">The time the player was detected as dropped from the server</param>
		/// <param name="playerName">The name of the player that was dropped</param>
		public void LogPlayerDropped (DateTime time, string playerName)
		{
//	Log
//			// Remove their gametime
//			GameDataset.TeamMemberRow[] TeamMembers = (GameDataset.TeamMemberRow[])_gameDataset.TeamMember.Select("Callsign = '" + playerName + "'");
//			if (TeamMembers.Length > 0)
//				TeamMembers[0].LeaveTime = time;

			// Log event
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.PlayerDropped, time, GameRow.GameID, playerName, -1, string.Empty, -1, string.Empty);
		}

		/// <summary>
		/// Logs a player joining a team to this game's dataset
		/// </summary>
		/// <param name="time">The time this player joined the team</param>
		/// <param name="playerID">The ID of the player joining the team</param>
		/// <param name="playerName">The name of the player joining the team</param>
		/// <param name="teamID">The ID of the team being joined</param>
		/// <param name="teamName">The name of the team being joined</param>
		public void LogPlayerJoinedTeam (DateTime time, int playerID, string playerName,
										int teamID, string teamName)
		{
			// Were they already on the team?
			string TeamMemberSelect = string.Concat("Callsign = '", playerName, "' AND TeamID = ", teamID.ToString());
			GameDataset.TeamMemberRow[] TeamMembers = (GameDataset.TeamMemberRow[])_gameDataset.TeamMember.Select(TeamMemberSelect);
			if (TeamMembers.Length > 0)
			{	// They were on a team. Adjust existing teammember time
				GameDataset.TeamMemberRow TeamMember = TeamMembers[0];

				// If the player is rejoining...
				if (TeamMember.LeaveTime != DateTime.MinValue)
				{
					// Get the difference between when they last left and now
					TimeSpan Difference = time.Subtract(TeamMember.LeaveTime);
					// Add difference to StartTime
					TeamMember.JoinTime = TeamMember.JoinTime.Add(Difference);

					// Reset LeaveTime because they haven't left yet
					TeamMember.LeaveTime = DateTime.MinValue;
				}
			}
			else // They weren't on this team before. Add them.
			{
				// Retrieve the TeamRow.
				GameDataset.TeamRow TeamRow = _gameDataset.Team.FindByTeamID(teamID);
				if (TeamRow != null)
				{
					// Add teamrow with "Minimum" leavetime
					_gameDataset.TeamMember.AddTeamMemberRow(TeamRow, playerName, 0, time, DateTime.MinValue);
				}
			}

			// Log event
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.JoinedTeam, time, playerID, playerName, teamID, teamName, -1, string.Empty);
		}

		/// <summary>
		/// Logs a player leaving a team to this game's dataset
		/// </summary>
		/// <param name="time">The time this player left the team</param>
		/// <param name="playerID">The ID of the player leaving the team</param>
		/// <param name="playerName">The name of the player leaving the team</param>
		/// <param name="teamID">The ID of the team that was left</param>
		/// <param name="teamName">The name of the team that was left</param>
		/// <param name="booterName">The name of the player who ejected this player from his team, or NULL if the player left on their own</param>
		public void LogPlayerLeftTeam (DateTime time, int playerID, string playerName,
										int teamID, string teamName, string booterName)
		{
			// Remove their gametime
			GameDataset.TeamMemberRow[] TeamMembers = (GameDataset.TeamMemberRow[])_gameDataset.TeamMember.Select("Callsign = '" + playerName + "'");
			if (TeamMembers.Length > 0)
				TeamMembers[0].LeaveTime = time;
			else
				TagTrace.WriteLine(TraceLevel.Error, "Error logging PlayerLeftTeam: Couldn't find TeamMember {0}", playerName);

			// Log event
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.LeftTeam, time, playerID, playerName, teamID, teamName, -1, booterName);
		}

		/// <summary>
		/// Logs a player joining a game
		/// </summary>
		/// <param name="time">The time this player joined the game</param>
		/// <param name="playerID">The ID of the player joining the game</param>
		/// <param name="playerName">The name of the player joining the game</param>
		public void LogPlayerLoggedIn (DateTime time, int playerID, string playerName)
		{
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.LoginGame, time, playerID, playerName, GameRow.GameID, GameRow.GameName, -1, string.Empty);
		}

		/// <summary>
		/// Logs a player leaving a game
		/// </summary>
		/// <param name="time">The time this player left the game</param>
		/// <param name="playerID">The ID of the player who left the game</param>
		/// <param name="playerName">The name of the player who left the game</param>
		/// <param name="booterName">The name of the player who #banned this player from the game, or NULL if they left on their own</param>
		public void LogPlayerLoggedOut (DateTime time, int playerID, string playerName, string booterName)
		{
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.LogoutGame, time, playerID, playerName, GameRow.GameID, GameRow.GameName, -1, booterName);
		}

		/// <summary>
		/// Logs a ship destruction to this game's dataset
		/// </summary>
		/// <param name="time">The time this ship was killed</param>
		/// <param name="killedID">The ID of the killed ship's pilot</param>
		/// <param name="killedName">The name of the killed ship's pilot</param>
		/// <param name="killedPosition">A vector including the X,Y,Z coordinates of the ship when killed</param>
		/// <param name="killerID">The ID of the ship who destroyed this one</param>
		/// <param name="killerName">The name of the pilot of the ship that destroyed this ship</param>
		/// <param name="killerPosition">A vector including the X,Y,Z coordinates of the ship that killed this one</param>
		/// <param name="amount">The time taken to kill this ship</param>
		/// <param name="isLifepod">1 if this ship was a lifepod, 0 if it was not</param>
		public void LogShipKilled (DateTime time, int killedID, string killedName, string killedPosition,
									int killerID, string killerName, string killerPosition, string amount,
									int isLifepod)
		{
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.ShipKilled, time, killedID, killedName, killerID, killerName, isLifepod, amount);
		}

		/// <summary>
		/// Logs a station capture to this game's dataset
		/// </summary>
		/// <param name="time">The time this station was captured</param>
		/// <param name="capturerID">The ID of the ship that did the capturing</param>
		/// <param name="capturerName">The name of the pilot who performed the base capture</param>
		/// <param name="teamID">The ID of the team who lost their station</param>
		/// <param name="teamName">The name of the team that lost their station</param>
		/// <param name="stationName">The name of the captured station</param>
		public void LogStationCaptured (DateTime time, int capturerID, string capturerName,
										int teamID, string teamName, string stationName)
		{
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.StationCaptured, time, capturerID, capturerName, teamID, teamName, -1, stationName);
		}

		/// <summary>
		/// Logs a station creation to this game's dataset
		/// </summary>
		/// <param name="time">The time this station was created</param>
		/// <param name="teamID">The ID of the team that created this station</param>
		/// <param name="teamName">The name of the team that created this station</param>
		/// <param name="stationName">The name of the created station</param>
		public void LogStationCreated (DateTime time, int teamID, string teamName,
										string stationName)
		{
			// Store the techpath of the created station
			GameDataset.TeamRow Team = _gameDataset.Team.FindByTeamID(teamID);
			if (Team != null)
			{
				if (stationName.IndexOf("Tactical") > -1)
					Team.ResearchedTactical = true;

				if (stationName.IndexOf("Expansion") > -1)
					Team.ResearchedExpansion = true;

				if (stationName.IndexOf("Supremacy") > -1)
					Team.ResearchedSupremacy = true;

				if (stationName.IndexOf("Shipyard") > -1 || stationName.IndexOf("Drydock") > -1)
					Team.ResearchedShipyard = true;

				if (stationName.IndexOf("Starbase") > -1)
					Team.ResearchedStarbase = true;
			}

			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.StationCreated, time, GameRow.GameID, GameRow.GameName, teamID, teamName, -1, stationName);
		}

		/// <summary>
		/// Logs a station destruction to this game's dataset
		/// </summary>
		/// <param name="time">The time this station was destroyed</param>
		/// <param name="destroyerID">The ID of the player who destroyed this station</param>
		/// <param name="destroyerName">The name of the player who destroyed this station</param>
		/// <param name="teamID">The ID of the team that lost their station</param>
		/// <param name="teamName">The name of the team that lost their station</param>
		/// <param name="stationName">The name of the destroyed station</param>
		public void LogStationDestroyed (DateTime time, int destroyerID, string destroyerName,
										int teamID, string teamName, string stationName)
		{
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.StationDestroyed, time, destroyerID, destroyerName, teamID, teamName, -1, stationName);
		}

		/// <summary>
		/// Logs a team info change to this game's dataset
		/// </summary>
		/// <param name="time">The time this team changed their info</param>
		/// <param name="teamID">The ID of the team that changed its info</param>
		/// <param name="oldTeamName">The Team's old name</param>
		/// <param name="newTeamName">The new name for the team</param>
		public void LogTeamInfoChanged (DateTime time, int teamID, string oldTeamName, string newTeamName)
		{
			_gameDataset.GameEvent.AddGameEventRow(GameRow, (int)HandledEventIDs.TeamInfoChanged, time, GameRow.GameID, GameRow.GameName, teamID, oldTeamName, teamID, newTeamName);
		}
		#endregion
	}
}
