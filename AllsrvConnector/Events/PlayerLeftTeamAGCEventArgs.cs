using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Player LeftTeam event
	/// </summary>
	public class PlayerLeftTeamAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public PlayerLeftTeamAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }
		
		/// <summary>
		/// The ID of the player who logged in (TargetID)
		/// </summary>
		public int PlayerID
		{
			get {return (int)_args[3];}
		}
		
		/// <summary>
		/// The name of the player who logged in (TargetName)
		/// </summary>
		public string PlayerName
		{
			get {return _args[4].ToString();}
		}
		
		/// <summary>
		/// The ID of the game where this player left a team
		/// </summary>
		public int GameID
		{
			get {return (int)_args[6];}
		}

		/// <summary>
		/// The ID of the team that was left
		/// </summary>
		public int TeamID
		{
			get {return (int)_args[7];}
		}

		/// <summary>
		/// The name of the team that was left
		/// </summary>
		public string TeamName
		{
			get {return _args[8].ToString();}
		}

		/// <summary>
		/// The name of the player who ejected the logged-out player (if any)
		/// </summary>
		public string BooterName
		{
			get {return (_args.Count > 9) ? _args[9].ToString() : string.Empty;}
		}
	}
}
