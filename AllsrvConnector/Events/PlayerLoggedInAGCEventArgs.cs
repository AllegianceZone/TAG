using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Player LoggedIn event
	/// </summary>
	public class PlayerLoggedInAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public PlayerLoggedInAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }
		
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
		/// The ID of the game that the player logged into
		/// </summary>
		public int GameID
		{
			get {return (int)_args[6];}
		}

		/// <summary>
		/// The name of the game that the player logged into
		/// </summary>
		public string GameName
		{
			get {return _args[7].ToString();}
		}
	}
}
