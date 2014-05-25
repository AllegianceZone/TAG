using System;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

using FreeAllegiance.Tag.Events;

namespace FreeAllegiance.Tag
{

	/// <summary>
	/// Handles events raised by Allsrv
	/// </summary>
	public class AGCEventHandler
	{
		#region Initialization
		private static AllsrvConnector	_connector = null;

		/// <summary>
		/// Connects the AllsrvConnector's events to the core's handlers
		/// </summary>
		public static void Initialize (AllsrvConnector connector)
		{
			_connector = connector;
			if (connector != null)
			{
				connector.AdminPagedEvent += new AdminPagedAGCEventDelegate(AdminPagedAGCEventHandler);
				connector.ChatMessageEvent += new ChatMessageAGCEventDelegate(ChatMessageAGCEventHandler);
				connector.GameCreatedEvent += new GameCreatedAGCEventDelegate(GameCreatedAGCEventHandler);
				connector.GameDestroyedEvent += new GameDestroyedAGCEventDelegate(GameDestroyedAGCEventHandler);
				connector.GameEndedEvent += new GameEndedAGCEventDelegate(GameEndedAGCEventHandler);
				connector.GameOverEvent += new GameOverAGCEventDelegate(GameOverAGCEventHandler);
				connector.GameStartedEvent += new GameStartedAGCEventDelegate(GameStartedAGCEventHandler);
				connector.LobbyConnectedEvent += new LobbyConnectedAGCEventDelegate(LobbyConnectedAGCEventHandler);
				connector.LobbyDisconnectedEvent += new LobbyDisconnectedAGCEventDelegate(LobbyDisconnectedAGCEventHandler);
				connector.LobbyDisconnectingEvent += new LobbyDisconnectingAGCEventDelegate(LobbyDisconnectingAGCEventHandler);
				connector.LobbyLostEvent += new LobbyLostAGCEventDelegate(LobbyLostAGCEventHandler);
				connector.PlayerDroppedEvent += new PlayerDroppedAGCEventDelegate(PlayerDroppedAGCEventHandler);
				connector.PlayerJoinedTeamEvent += new PlayerJoinedTeamAGCEventDelegate(PlayerJoinedTeamAGCEventHandler);
				connector.PlayerLeftTeamEvent += new PlayerLeftTeamAGCEventDelegate(PlayerLeftTeamAGCEventHandler);
				connector.PlayerLoggedInEvent += new PlayerLoggedInAGCEventDelegate(PlayerLoggedInAGCEventHandler);
				connector.PlayerLoggedOutEvent += new PlayerLoggedOutAGCEventDelegate(PlayerLoggedOutAGCEventHandler);
				connector.ShipKilledEvent += new ShipKilledAGCEventDelegate(ShipKilledAGCEventHandler);
				connector.StationCapturedEvent += new StationCapturedAGCEventDelegate(StationCapturedAGCEventHandler);
				connector.StationCreatedEvent += new StationCreatedAGCEventDelegate(StationCreatedAGCEventHandler);
				connector.StationDestroyedEvent += new StationDestroyedAGCEventDelegate(StationDestroyedAGCEventHandler);
				connector.TeamInfoChangedEvent += new TeamInfoChangedAGCEventDelegate(TeamInfoChangedAGCEventHandler);
				connector.TerminateEvent += new TerminateAGCEventDelegate(TerminateAGCEventHandler);
			}
		}

		/// <summary>
		/// Disconnects the core's handlers from AllsrvConnector's events
		/// </summary>
		public static void Uninitialize ()
		{
			if (_connector != null)
			{
				_connector.AdminPagedEvent -= new AdminPagedAGCEventDelegate(AdminPagedAGCEventHandler);
				_connector.ChatMessageEvent -= new ChatMessageAGCEventDelegate(ChatMessageAGCEventHandler);
				_connector.GameCreatedEvent -= new GameCreatedAGCEventDelegate(GameCreatedAGCEventHandler);
				_connector.GameDestroyedEvent -= new GameDestroyedAGCEventDelegate(GameDestroyedAGCEventHandler);
				_connector.GameEndedEvent -= new GameEndedAGCEventDelegate(GameEndedAGCEventHandler);
				_connector.GameOverEvent -= new GameOverAGCEventDelegate(GameOverAGCEventHandler);
				_connector.GameStartedEvent -= new GameStartedAGCEventDelegate(GameStartedAGCEventHandler);
				_connector.LobbyConnectedEvent -= new LobbyConnectedAGCEventDelegate(LobbyConnectedAGCEventHandler);
				_connector.LobbyDisconnectedEvent -= new LobbyDisconnectedAGCEventDelegate(LobbyDisconnectedAGCEventHandler);
				_connector.LobbyDisconnectingEvent -= new LobbyDisconnectingAGCEventDelegate(LobbyDisconnectingAGCEventHandler);
				_connector.LobbyLostEvent -= new LobbyLostAGCEventDelegate(LobbyLostAGCEventHandler);
				_connector.PlayerDroppedEvent -= new PlayerDroppedAGCEventDelegate(PlayerDroppedAGCEventHandler);
				_connector.PlayerJoinedTeamEvent -= new PlayerJoinedTeamAGCEventDelegate(PlayerJoinedTeamAGCEventHandler);
				_connector.PlayerLeftTeamEvent -= new PlayerLeftTeamAGCEventDelegate(PlayerLeftTeamAGCEventHandler);
				_connector.PlayerLoggedInEvent -= new PlayerLoggedInAGCEventDelegate(PlayerLoggedInAGCEventHandler);
				_connector.PlayerLoggedOutEvent -= new PlayerLoggedOutAGCEventDelegate(PlayerLoggedOutAGCEventHandler);
				_connector.ShipKilledEvent -= new ShipKilledAGCEventDelegate(ShipKilledAGCEventHandler);
				_connector.StationCapturedEvent -= new StationCapturedAGCEventDelegate(StationCapturedAGCEventHandler);
				_connector.StationCreatedEvent -= new StationCreatedAGCEventDelegate(StationCreatedAGCEventHandler);
				_connector.StationDestroyedEvent -= new StationDestroyedAGCEventDelegate(StationDestroyedAGCEventHandler);
				_connector.TeamInfoChangedEvent -= new TeamInfoChangedAGCEventDelegate(TeamInfoChangedAGCEventHandler);
				_connector.TerminateEvent -= new TerminateAGCEventDelegate(TerminateAGCEventHandler);
			}
		}
		#endregion

		/// <summary>
		/// Handles the AdminPaged AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void AdminPagedAGCEventHandler (object sender, AdminPagedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "AdminPage Event received from {0}: {1}", e.Callsign, e.Message);
				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);
				if (CurrentGame != null)
				{
					TagTrace.WriteLine(TraceLevel.Verbose, "AdminPage Event waiting to lock GameData...");
					lock (CurrentGame.GetSyncRoot())
					{
						CurrentGame.GameData.LogAdminPage(e.Time, e.Callsign, e.Message);
						TagTrace.WriteLine(TraceLevel.Verbose, "AdminPage logged.");
					}

					// If it's a #command, parse it
					if (e.Message.StartsWith("#"))
					{
						int SpacePosition = e.Message.IndexOf(" ");
						string CommandName = (SpacePosition < 0) ? e.Message : e.Message.Substring(0, SpacePosition);
						string Params = (SpacePosition < 0) ? string.Empty : e.Message.Substring(SpacePosition + 1);

						switch (CommandName)
						{
							case "#debugkills":		// Allows debugging of kill logging
								CurrentGame.DebugKills = !CurrentGame.DebugKills;
								CurrentGame.SendChat("Kill debugging is now " + ((CurrentGame.DebugKills) ? "on" : "off"));
								break;
							case "#hq":
								if (CallsignHelper.GetAuthLevel(e.Callsign) > AuthLevel.User)
									GameServer.SendChat(Params);
								else
									GameServer.SendChat(e.Callsign, "You must be logged in with administrative tokens or tags in order to broadcast across the server");
								break;
							case "#hqgame":
								if (CallsignHelper.GetAuthLevel(e.Callsign) > AuthLevel.User)
									CurrentGame.SendChat(Params);
								else
									GameServer.SendChat(e.Callsign, "You must be logged in with administrative tokens or tags in order to broadcast to this game");
								break;
//							case "#smite":
//								if (CurrentGame.SmitePlayer(Params, e.Callsign))
//									GameServer.SendChat(e.Callsign, "Player smited");
//								else
//									GameServer.SendChat(e.Callsign, "Specified player not found");
//
//								break;
							case "#tag":
								if (CallsignHelper.GetAuthLevel(e.Callsign) > AuthLevel.User)
								{
									CurrentGame.GameData.TagGame(e.Time, e.Callsign, Params);
									GameServer.SendChat(e.Callsign, "This game has been tagged " + Params + ".");
								}
								else
								{
									GameServer.SendChat(e.Callsign, "You must be logged in with administrative tokens or tags in order to #tag a game");
								}
								break;
							case "#update":	// Forces an update-check for TAG
								if (TagUpdate.IsAbleToUpdate() || CallsignHelper.GetAuthLevel(e.Callsign) == AuthLevel.Admin)
								{
									if (TagUpdate.UpdateAvailable())
									{
										GameServer.SendChat(e.Callsign, "TAG update available. Installing...");
										TagUpdate.InitiateUpdate();
									}
									else
									{
										GameServer.SendChat(e.Callsign, "TAG is already up to date.");
									}
								}
								else
								{
									GameServer.SendChat(e.Callsign, "TAG can't update now. There are games in progress.");
								}
								break;
							case "#version":		// A simple version check to see what version is running
								Version TagVersion = Assembly.GetEntryAssembly().GetName().Version;
								string TagVersionString = string.Format("TAG v{0}.{1}.{2}.{3} online", TagVersion.Major, TagVersion.Minor, TagVersion.Build, TagVersion.Revision);
								GameServer.SendChat(e.Callsign, TagVersionString);
								break;
							default:
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling AdminPaged event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the ChatMessage AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void ChatMessageAGCEventHandler (object sender, ChatMessageAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "Chat Event received");
				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);
				if (CurrentGame != null)
				{
					// Ignore all chats made by cons/drones
					if (!e.SpeakerName.StartsWith("."))
					{
						string TargetName = ChatTargetHelper.CleanTargetName(e.RecipientName, e.ChatType);

						TagTrace.WriteLine(TraceLevel.Verbose, "Chat Event waiting to lock GameData...");
						lock (CurrentGame.GetSyncRoot())
						{
							CurrentGame.GameData.LogChatMessage(e.Time, e.SpeakerID, e.SpeakerName, e.RecipientID, TargetName,
								e.CommandID, e.VoiceID, e.Text);
							TagTrace.WriteLine(TraceLevel.Verbose, "Chat logged.");
						}
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling ChatMessage event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the GameCreated AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void GameCreatedAGCEventHandler (object sender, GameCreatedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "GameCreated event received.");
				// Create the new game
				Game CurrentGame = GameServer.LoadGame(e.GameID);
				if (CurrentGame == null)
					throw new ApplicationException("Unknown error while creating game!");

			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling GameCreated event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the GameDestroyed AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void GameDestroyedAGCEventHandler (object sender, GameDestroyedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "GameDestroyed event received");
				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);
				if (CurrentGame != null)
				{
					TagTrace.WriteLine(TraceLevel.Verbose, "GameDestroyed Event waiting to lock GameData...");
					lock (CurrentGame.GetSyncRoot())
					{
						//CurrentGame.GameData.LogGameDestroyed(e.Time);
						GameServer.Games.Remove(CurrentGame);
						TagTrace.WriteLine(TraceLevel.Verbose, "Game removed from server.");
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling GameDestroyed event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the GameEnded AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void GameEndedAGCEventHandler (object sender, GameEndedAGCEventArgs e)
		{
			Game CurrentGame = null;

			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "GameEnded Event received from game {0}", e.GameID);

				CurrentGame = GameServer.Games.GetGameByID(e.GameID);
				if (CurrentGame != null)
				{
					// Prevent GameOverEvent from continuing
					CurrentGame.GetGameEndedMRE().Reset();

					int WinningTeamID = e.WinningTeamID;
					string WinningTeamName = e.WinningTeamName;

					// Check if we're using FAZ R1 or earlier
					if (e.WinningTeamID == -2)
					{
						WinningTeamID = -1;

						string ReasonString = "Reason = '";
						int ReasonIndex = e.Description.IndexOf(ReasonString);
						int TeamStartIndex = ReasonIndex + ReasonString.Length;

						// Parse teamname from description (whether they won by destroying or capturing)
						int WonIndex = e.Description.IndexOf(" won ");
						if (WonIndex > 0)
						{
							WinningTeamName = e.Description.Substring(TeamStartIndex, WonIndex - TeamStartIndex);
						}
						else
						{
							// When a game ends by capture, the text is "has" instead of "won"
							int HasIndex = e.Description.IndexOf(" has ", TeamStartIndex);
							if (HasIndex > 0)
							{
								WinningTeamName = e.Description.Substring(TeamStartIndex, HasIndex - TeamStartIndex);
							}
						}
					}
					else
					{
						// Convert TeamID into TeamIndex
						WinningTeamID = CurrentGame.GetTeamIndex(e.WinningTeamID);
					}

					
					TagTrace.WriteLine(TraceLevel.Verbose, "GameEnded Event waiting to lock GameData...");
					lock (CurrentGame.GetSyncRoot())
					{
						CurrentGame.GameData.LogGameEnded(e.Time, WinningTeamID, WinningTeamName, e.Reason);
						TagTrace.WriteLine(TraceLevel.Info, "Game {0} Ended.", e.GameID);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling GameEnded event: {0}", ex.Message);
			}
			finally
			{
				// Allow GameOverEvent to begin
				if (CurrentGame != null)
					CurrentGame.GetGameEndedMRE().Set();
			}
		}

		/// <summary>
		/// Handles the GameOver AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void GameOverAGCEventHandler (object sender, GameOverAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "GameOver Event received from game {0}", e.GameID);
				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);
				if (CurrentGame != null)
				{
					// Prevent GameOver from continuing until GameEnded has completed
					CurrentGame.GetGameEndedMRE().WaitOne();

					TagTrace.WriteLine(TraceLevel.Verbose, "GameOver Event waiting to lock GameData...");
					lock (CurrentGame.GetSyncRoot())
					{
						// Switch out the existing GameData for a new one
						GameData EndedGameData = CurrentGame.GameData;
						CurrentGame.GameData = new GameData();

						// Queue posting
						ThreadPool.QueueUserWorkItem(new WaitCallback(PostGame), EndedGameData);
						TagTrace.WriteLine(TraceLevel.Verbose, "Game {0} is reset, and its data queued for posting", e.GameID);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling GameOver event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the GameStarted AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void GameStartedAGCEventHandler (object sender, GameStartedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "GameStarted event received from game {0}", e.GameID);
				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);
				if (CurrentGame != null)
				{
					TagTrace.WriteLine(TraceLevel.Verbose, "GameStarted Event waiting to lock GameData...");
					lock (CurrentGame.GetSyncRoot())
					{
						// Initialize game
						CurrentGame.Start(e.Time);

						// Log event
						CurrentGame.GameData.LogGameStarted(e.Time);
						TagTrace.WriteLine(TraceLevel.Verbose, "Game {0} Started.", e.GameID);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling GameStarted event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the LobbyConnected AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void LobbyConnectedAGCEventHandler (object sender, LobbyConnectedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "LobbyConnected event received.");
				// Inform users that the lobby is (back) up
				GameServer.SendChat("This server has connected to the Lobby. New players can now join this game.");

				// Log lobby connects to all current games
				foreach (Game CurrentGame in GameServer.Games)
				{
					TagTrace.WriteLine(TraceLevel.Verbose, "LobbyConnected Event waiting to lock Game {0}'s GameData...", CurrentGame.GameID);
					lock (CurrentGame.GetSyncRoot())
					{
						CurrentGame.GameData.LogLobbyConnected(e.Time, e.LobbyIP);
						TagTrace.WriteLine(TraceLevel.Info, "LobbyConnected event logged to game {0}.", CurrentGame.GameID);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling LobbyConnected event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the LobbyDisconnected AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void LobbyDisconnectedAGCEventHandler (object sender, LobbyDisconnectedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "LobbyDisconnected event received.");
				// Log lobby Disconnects to all current games
				foreach (Game CurrentGame in GameServer.Games)
				{
					TagTrace.WriteLine(TraceLevel.Verbose, "LobbyDisconnected Event waiting to lock Game {0}'s GameData...", CurrentGame.GameID);
					lock (CurrentGame.GetSyncRoot())
					{
						CurrentGame.GameData.LogLobbyDisconnected(e.Time, e.LobbyIP);
						TagTrace.WriteLine(TraceLevel.Info, "LobbyDisconnected event logged to game {0}.", CurrentGame.GameID);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling LobbyDisconnected event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the LobbyDisconnecting AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void LobbyDisconnectingAGCEventHandler (object sender, LobbyDisconnectingAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "LobbyDisconnecting event received.");
				// Log lobby disconnectings to all current games
				foreach (Game CurrentGame in GameServer.Games)
				{
					TagTrace.WriteLine(TraceLevel.Verbose, "LobbyDisconnecting Event waiting to lock Game {0}'s GameData...", CurrentGame.GameID);
					lock (CurrentGame.GetSyncRoot())
					{
						CurrentGame.GameData.LogLobbyDisconnecting(e.Time, e.LobbyIP);
						TagTrace.WriteLine(TraceLevel.Info, "LobbyDisconnecting event logged to game {0}.", CurrentGame.GameID);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling LobbyDisconnecting event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the LobbyLost AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void LobbyLostAGCEventHandler (object sender, LobbyLostAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "LobbyLost event received.");

				// Send a chat to all games informing that lobby was lost
				GameServer.SendChat("This server has disconnected from the lobby.  New players will be unable to find this game until the server reconnects.");
				
				// Log lobby losts to all current games
				foreach (Game CurrentGame in GameServer.Games)
				{
					TagTrace.WriteLine(TraceLevel.Verbose, "LobbyLost Event waiting to lock Game {0}'s GameData...", CurrentGame.GameID);
					lock (CurrentGame.GetSyncRoot())
					{
						CurrentGame.GameData.LogLobbyLost(e.Time);
						TagTrace.WriteLine(TraceLevel.Info, "LobbyLost event logged to game {0}.", CurrentGame.GameID);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling LobbyLost event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the PlayerDropped AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void PlayerDroppedAGCEventHandler (object sender, PlayerDroppedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "PlayerDropped event received.");

				// Grab the game
				Game CurrentGame = GameServer.Players.GetGameByPlayer(e.PlayerName);

				if (CurrentGame != null)
				{
					lock (CurrentGame.GetSyncRoot())
					{
						// Get their current team
						int TeamNumber = GameServer.Players.GetTeamByPlayer(e.PlayerName);

						// Remove the player
						GameServer.Players.RemovePlayer(e.PlayerName);

						// Only log playerdrops if they're in a game in progress
						if (CurrentGame.InProgress && TeamNumber > -1)
						{
							CurrentGame.GameData.LogPlayerDropped(e.Time, e.PlayerName);
							TagTrace.WriteLine(TraceLevel.Verbose, "{0} Dropped", e.PlayerName);
						}
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling PlayerDropped event: {0}", ex.Message);
				TagTrace.WriteLine(TraceLevel.Error, "PlayerName: {1}", (e.PlayerName != null) ? e.PlayerName : "NULL");
			}
		}

		/// <summary>
		/// Handles the PlayerJoinedTeam AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void PlayerJoinedTeamAGCEventHandler (object sender, PlayerJoinedTeamAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "PlayerJoinedTeam event received.");

				// Get game
				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);

				if (CurrentGame != null)
				{
					lock (CurrentGame.GetSyncRoot())
					{
						// Update team
						GameServer.Players.UpdateTeam(e.PlayerName, e.TeamID);
						
						// Ignore joining NOAT
						if (e.TeamID != -2 && e.TeamID != -131060)
						{
							// Only log joiners when game is in progress
							if (CurrentGame.InProgress)
							{
								CurrentGame.GameData.LogPlayerJoinedTeam(e.Time, e.PlayerID, e.PlayerName, e.TeamID, e.TeamName);
								TagTrace.WriteLine(TraceLevel.Verbose, "{0} joined {1}", e.PlayerName, e.TeamName);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling PlayerJoined event: {0}", ex.Message);
				TagTrace.WriteLine(TraceLevel.Error, "GameID: {0}, PlayerID: {1}, PlayerName: {2}, TeamID: {3}, TeamName: {4}",
					e.GameID, e.PlayerID, (e.PlayerName != null) ? e.PlayerName : "NULL",
					e.TeamID, (e.TeamName != null) ? e.TeamName : "NULL");
			}
		}

		/// <summary>
		/// Handles the PlayerLeftTeam AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void PlayerLeftTeamAGCEventHandler (object sender, PlayerLeftTeamAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "PlayerLeftTeam event received.");

				// Get game
				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);

				if (CurrentGame != null)
				{
					lock (CurrentGame.GetSyncRoot())
					{
						// Update team
						GameServer.Players.UpdateTeam(e.PlayerName, -1);
						
						// Ignore leaving NOAT (unless booted)
						if ((e.TeamID != -2 && e.TeamID != -131060) || !e.BooterName.Equals(string.Empty))
						{
							// Only log LeaveTeams if in progress (or boots)
							if (CurrentGame.InProgress || !e.BooterName.Equals(string.Empty))
							{
								CurrentGame.GameData.LogPlayerLeftTeam(e.Time, e.PlayerID, e.PlayerName, e.TeamID, e.TeamName, e.BooterName);
								TagTrace.WriteLine(TraceLevel.Verbose, "{0} left {1}", e.PlayerName, e.TeamName);

								// Inform everyone of team ban
								if (!e.BooterName.Equals(string.Empty))
									CurrentGame.SendChat(string.Concat(e.BooterName, " has booted ", e.PlayerName, " from team ", e.TeamName, "."));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling PlayerLeft event: {0}", ex.Message);
				TagTrace.WriteLine(TraceLevel.Error, "GameID: {0}, PlayerID: {1}, PlayerName: {2}, TeamID: {3}, TeamName: {4}",
									e.GameID, e.PlayerID, (e.PlayerName != null) ? e.PlayerName : "NULL",
									e.TeamID, (e.TeamName != null) ? e.TeamName : "NULL");
			}
		}

		/// <summary>
		/// Handles the PlayerLoggedIn AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void PlayerLoggedInAGCEventHandler (object sender, PlayerLoggedInAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "PlayerLoggedIn event received.");

				// Get game
				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);

				if (CurrentGame != null)
				{
					lock (CurrentGame.GetSyncRoot())
					{
						// Add player to list
						GameServer.Players.AddPlayer(e.PlayerName, e.GameID, -1);

						CurrentGame.GameData.LogPlayerLoggedIn(e.Time, e.PlayerID, e.PlayerName);
						TagTrace.WriteLine(TraceLevel.Verbose, "{0} logged into {1}", e.PlayerName, CurrentGame.GameName);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling PlayerLoggedIn event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the PlayerLoggedOut AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void PlayerLoggedOutAGCEventHandler (object sender, PlayerLoggedOutAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "PlayerLoggedOut event received.");

				// Get current game
				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);

				if (CurrentGame != null)
				{
					lock (CurrentGame.GetSyncRoot())
					{
						// Get their current team
						int TeamNumber = GameServer.Players.GetTeamByPlayer(e.PlayerName);
				
						// Remove player from player list
						GameServer.Players.RemovePlayer(e.PlayerName);

						// Only log LoggedOut messages for players in a game in progress. Always log boots
						if ((CurrentGame.InProgress && TeamNumber > -1) || !e.BooterName.Equals(string.Empty))
						{
							CurrentGame.GameData.LogPlayerLoggedOut(e.Time, e.PlayerID, e.PlayerName, e.BooterName);
							TagTrace.WriteLine(TraceLevel.Verbose, "{0} logged out of {1}", e.PlayerName, e.GameName);

							// Inform everyone of lobby ban
							if (!e.BooterName.Equals(string.Empty))
								CurrentGame.SendChat(string.Concat(e.BooterName, " has lobby-banned ", e.PlayerName, "."));
						}
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling PlayerLoggedOut event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the ShipKilled AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void ShipKilledAGCEventHandler (object sender, ShipKilledAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "ShipKilled event received.");

				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);
				if (CurrentGame != null)
				{
					string KillerName = e.KillerName;

					// If something unknown killed this ship
					if (e.KillerID == -1 && e.KillerName.Equals(string.Empty))
					{
						KillerName = "Caltrop/Alephres/Con-bomb";
						// Ignore cons building
						if (e.KilledName.StartsWith("."))
						{
							TagTrace.WriteLine(TraceLevel.Verbose, "Ignoring ShipKilled because {0} built", e.KilledName);
							return;
						}
					}

					// If a lifepod wasn't killed...
					if (e.IsLifepod == 0)
					{
						lock (CurrentGame.GetSyncRoot())
						{
							CurrentGame.GameData.LogShipKilled(e.Time, e.KilledID, e.KilledName, e.KilledPosition, e.KillerID, KillerName, e.KillerPosition, e.Amount, e.IsLifepod);
					
							// Spit the kill out if we're debugging them
							if (CurrentGame.DebugKills)
								CurrentGame.SendChat(KillerName + " killed " + e.KilledName);
					
							TagTrace.WriteLine(TraceLevel.Verbose, "{0} killed {1} in {2}", KillerName, e.KilledName, CurrentGame.GameName);
						}
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling ShipKilled event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the StationCaptured AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void StationCapturedAGCEventHandler (object sender, StationCapturedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "StationCaptured event received.");

				// Get current game
				Game CurrentGame = null;
				string StationName = "Unknown Station";

				if (e.GameID == -1)
				{
					CurrentGame = GameServer.Players.GetGameByPlayer(e.CapturerName);
				}
				else
				{
					CurrentGame = GameServer.Games.GetGameByID(e.GameID);
					StationName = e.StationName;
				}

				// Log the event if we have the game
				if (CurrentGame != null)
				{
					lock (CurrentGame.GetSyncRoot())
					{
						CurrentGame.GameData.LogStationCaptured(e.Time, e.CapturerID, e.CapturerName, e.TeamID, e.TeamName, StationName);

						TagTrace.WriteLine(TraceLevel.Verbose, "{0} captured {1}'s {2}", e.CapturerName, e.TeamName, StationName);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling StationCaptured event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the StationCreated AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void StationCreatedAGCEventHandler (object sender, StationCreatedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "StationCreated event received.");

				// Get current game
				Game CurrentGame = null;
				string StationName = (e.StationName.Equals(string.Empty)) ? "Unknown Station" : e.StationName;

				CurrentGame = GameServer.Games.GetGameByID(e.GameID);

				if (CurrentGame != null)
				{
					lock (CurrentGame.GetSyncRoot())
					{
						CurrentGame.GameData.LogStationCreated(e.Time, e.TeamID, e.TeamName, StationName);

						TagTrace.WriteLine(TraceLevel.Verbose, "{0} built a(n) {1}", e.TeamName, StationName);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling StationBuilt event: {0}", ex.Message);
				TagTrace.WriteLine(TraceLevel.Error, "GameID: {0}", e.GameID);
				TagTrace.WriteLine(TraceLevel.Error, "TeamID: {0}", e.TeamID);
				TagTrace.WriteLine(TraceLevel.Error, "TeamName: {0}", (e.TeamName != null) ? e.TeamName : "NULL");
				TagTrace.WriteLine(TraceLevel.Error, "StationName: {0}", (e.StationName != null) ? e.StationName : "NULL");
			}
		}

		/// <summary>
		/// Handles the StationDestroyed AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void StationDestroyedAGCEventHandler (object sender, StationDestroyedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "StationDestroyed event received.");

				// Get current game
				Game CurrentGame = null;
				string StationName = "Unknown Station";

				if (e.GameID == -1)
				{
					CurrentGame = GameServer.Players.GetGameByPlayer(e.KillerName);
				}
				else
				{
					CurrentGame = GameServer.Games.GetGameByID(e.GameID);
					StationName = e.StationName;
				}

				// Log the event if we know the game
				if (CurrentGame != null)
				{
					lock (CurrentGame.GetSyncRoot())
					{
						CurrentGame.GameData.LogStationDestroyed(e.Time, e.KillerID, e.KillerName, e.TeamID, e.TeamName, StationName);

						TagTrace.WriteLine(TraceLevel.Verbose, "{0} destroyed {1}'s {2}", e.KillerName, e.TeamName, StationName);
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling StationDestroyed event: {0}", ex.Message);
				TagTrace.WriteLine(TraceLevel.Error, "GameID: {0}, TeamID: {1}, TeamName: {2}, StationName: {3}",
					e.GameID, e.TeamID, 
					(e.TeamName != null) ? e.TeamName : "NULL",
					(e.StationName != null) ? e.StationName : "NULL");
			}
		}

		/// <summary>
		/// Handles the TeamInfoChanged AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void TeamInfoChangedAGCEventHandler (object sender, TeamInfoChangedAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "TeamInfoChanged event received.");

				Game CurrentGame = GameServer.Games.GetGameByID(e.GameID);
				if (CurrentGame != null)
				{
					if (CurrentGame.InProgress)		// Only log this during a game
					{
						lock (CurrentGame.GetSyncRoot())
						{
							CurrentGame.GameData.LogTeamInfoChanged(e.Time, e.TeamID, e.OldTeamName, e.NewTeamName);
							TagTrace.WriteLine(TraceLevel.Verbose, "{0} changed their TeamInfo", e.OldTeamName);
						}
					}
				}
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling TeamInfoChanged event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Handles the Terminate AGCEvent
		/// </summary>
		/// <param name="sender">The object firing the event</param>
		/// <param name="e">The arguments of the event</param>
		public static void TerminateAGCEventHandler (object sender, TerminateAGCEventArgs e)
		{
			try
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "Terminate event received.");
				
				// De-initialize core (unhook events)
				AGCEventHandler.Uninitialize();

				// Kill all game info
				GameServer.Disconnect();

				// Start reconnect timer
				ReconnectTimer.Start();
			}
			catch (Exception ex)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error handling Terminate event: {0}", ex.Message);
			}
		}

		/// <summary>
		/// Posts the specified game to ASGS, and logs it
		/// </summary>
		/// <param name="data">The data to post</param>
		public static void PostGame(object data)
		{
			// Unpackage and prepare data
			GameData Data = (GameData)data;
			GameDataset Set = Data.GetDataset();
			string Xml = Set.GetXml();
			string CompressedXml = Compression.Compress(Xml);
			TagTrace.WriteLine(TraceLevel.Verbose, "Game data ready to post. Sending to stats server...");
			TagTrace.WriteLine(TraceLevel.Verbose, "Use CSS: " + CssConnector.UseCss + ", CSS Url: " + CssConnector.CssUrl);

			// Post game to ASGS
			string PostMessage;
			int GameID;

			if (CssConnector.UseCss == true)
				GameID = CssConnector.PostGame(CompressedXml, out PostMessage);
			else
				GameID = AsgsConnector.PostGame(CompressedXml, out PostMessage);

			TagTrace.WriteLine(TraceLevel.Info, "Game #{0} Posted: {1}", GameID, PostMessage);

			// Post message to all servers
			GameServer.SendChat(PostMessage);

			// Log games as configured
			XmlLogger.LogGame(Data, GameID);

			// Get rid of this GameData since we no longer need it
			Data.Dispose();
			TagTrace.WriteLine(TraceLevel.Verbose, "Game data disposed");

			// If no games are in progress...
			if (TagUpdate.IsAbleToUpdate())
			{
				TagTrace.WriteLine(TraceLevel.Verbose, "TAG is able to update. No games are in progress");
				// And an update is available...
				if (TagUpdate.UpdateAvailable())
				{
					TagUpdate.InitiateUpdate();	// Update!
				}
			}
		}
	}
}
