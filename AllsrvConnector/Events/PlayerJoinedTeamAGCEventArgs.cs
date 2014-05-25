using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of a Player Joined Team event
	/// </summary>
	public class PlayerJoinedTeamAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public PlayerJoinedTeamAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }
		
		/// <summary>
		/// The ID of the player who joined a team (TargetID)
		/// </summary>
		public int PlayerID
		{
			get {return (int)_args[3];}
		}
		
		/// <summary>
		/// The name of the player who joined a team (TargetName)
		/// </summary>
		public string PlayerName
		{
			get {return _args[4].ToString();}
		}
		
		/// <summary>
		/// The ID of the game where the player joined a team
		/// </summary>
		public int GameID
		{
			get {return (int)_args[6];}
		}

		/// <summary>
		/// The ID of the team that was joined
		/// </summary>
		public int TeamID
		{
			get {return (int)_args[7];}
		}

		/// <summary>
		/// The name of the team that was joined
		/// </summary>
		public string TeamName
		{
			get {return _args[8].ToString();}
		}
	}
}
