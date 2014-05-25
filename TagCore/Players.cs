using System;
using System.Data;
using System.Diagnostics;

using AGCLib;
using ALLEGIANCESERVERLib;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// A list of players, games, and teams
	/// </summary>
	public class Players
	{
		private IAdminServer	_server = null;
		/// <summary>
		/// Allsrv's ID for the NOAT team
		/// </summary>
		private const int NOATID =	-131060;

		private DataTable	_players = null;

		/// <summary>
		/// Creates an empty player list
		/// </summary>
		/// <param name="server">A reference to the Allsrv gameserver</param>
		public Players (IAdminServer server)
		{
			_server = server;

			// Create the table
			DataTable Table = new DataTable("Players");
			Table.Columns.Add("Callsign", typeof(string));
			Table.Columns.Add("Game", typeof(int));
			Table.Columns.Add("Team", typeof(int));

			_players = Table;
		}

		/// <summary>
		/// Fills the PlayerTable with players and their teams
		/// </summary>
		/// <param name="server">A reference to the Allsrv server</param>
		public void Initialize (IAdminServer server)
		{
			_players.Clear();
			foreach (IAdminUser User in server.Users)
			{
				string Callsign = User.Name;
				IAGCGame TempGame = User.Ship.Game;
				int GameID = TempGame.GameID;
				
				// Find this player's team
				int TeamID = 0;
				for (int j = 0; j < TempGame.Teams.Count; j++)
				{
					object TeamIndex = j;
					IAGCTeam Team = TempGame.Teams.get_Item(ref TeamIndex);

					if (User.Ship.Team == Team)
						break;

					TeamID += 1;
				}

				_players.Rows.Add(new object[] {Callsign, GameID, TeamID});
			}
		}

		/// <summary>
		/// Clears this list of players
		/// </summary>
		public void Clear ()
		{
			_players.Clear();
		}

		/// <summary>
		/// Retrieves the game that the specified player is playing
		/// </summary>
		/// <param name="callsign">The callsign to search</param>
		/// <returns>This player's game, if any</returns>
		public Game GetGameByPlayer (string callsign)
		{
			Game Result = null;

			// Select the player's row
			DataRow[] Rows = _players.Select("Callsign = '" + callsign + "'");
			if (Rows.Length > 0)
			{
				int GameID = (int)Rows[0]["Game"];
				Result = GameServer.Games.GetGameByID(GameID);
			}

			return Result;
		}

		/// <summary>
		/// Retrieves the specified player's teamindex
		/// </summary>
		/// <param name="callsign">The callsign to search</param>
		/// <returns>The specified player's TeamID, or -2 if not in list</returns>
		public int GetTeamByPlayer (string callsign)
		{
			int Result = -2;

			DataRow PlayerRow = GetPlayerRow(callsign);
			if (PlayerRow != null)
				Result = (int)PlayerRow["Team"];

			return Result;
		}

		/// <summary>
		/// Adds a player to the list
		/// </summary>
		/// <param name="callsign">The callsign of the player to add</param>
		/// <param name="gameID">The ID of this player's game</param>
		/// <param name="teamIndex">The index of this player's team</param>
		public void AddPlayer (string callsign, int gameID, int teamIndex)
		{
			// Select the player's row
			DataRow PlayerRow = GetPlayerRow(callsign);

			// Not in list. Add them
			if (PlayerRow == null)
				_players.Rows.Add(new object[] {callsign, gameID, teamIndex});
		}

		/// <summary>
		/// Removes the specified player from the list
		/// </summary>
		/// <param name="callsign">The callsign of the player to remove</param>
		public void RemovePlayer (string callsign)
		{
			// Select the player's row
			DataRow PlayerRow = GetPlayerRow(callsign);
			
			// If in list, remove it
			if (PlayerRow != null)
				_players.Rows.Remove(PlayerRow);
		}

		/// <summary>
		/// Updates the specified player's team
		/// </summary>
		/// <param name="callsign">The callsign of the player whose team will be changed</param>
		/// <param name="teamID">The index of the player's new team</param>
		public void UpdateTeam (string callsign, int teamID)
		{
			// Select the player's row
			DataRow PlayerRow = GetPlayerRow(callsign);

			if (PlayerRow != null)
			{
				int GameID = (int)PlayerRow["Game"];
				int Index = GetTeamIndexFromTeamID(GameID, teamID);
				PlayerRow["Team"] = Index;
			}
		}

		/// <summary>
		/// Retrieves the team's index that has the specified ID. Returns -1 for NOAT
		/// </summary>
		/// <param name="gameID">The ID of the game</param>
		/// <param name="teamID">The ID of the team whose index should be retrieved</param>
		/// <returns>The index of the team with the specified ID</returns>
		private int GetTeamIndexFromTeamID (int gameID, int teamID)
		{
			int Result = -1;

			try
			{
				// If they're NOAT, return -1
				if (teamID != NOATID && teamID != -2)
				{
					// Grab the game
					Game CurrentGame = GameServer.Games.GetGameByID(gameID);

					// Grab the index
					Result = CurrentGame.GetTeamIndex(teamID);
				}
			}
			catch (Exception e)
			{
				TagTrace.WriteLine(TraceLevel.Error, "Error retrieving Index from GameID:TeamID({0}:{1}): {2}", gameID, teamID, e.Message);
			}
			
			return Result;
		}

		/// <summary>
		/// Retrieves the specified player's row
		/// </summary>
		/// <param name="callsign">The callsign of the row to retrieve</param>
		/// <returns>The specified player's row</returns>
		private DataRow GetPlayerRow (string callsign)
		{
			DataRow Result = null;

			DataRow[] Rows = _players.Select("Callsign = '" + callsign + "'");
			if (Rows.Length > 0)
				Result = Rows[0];
			
			return Result;
		}

		/// <summary>
		/// The number of players in this list
		/// </summary>
		public int Count
		{
			get {return _players.Rows.Count;}
		}
	}
}
