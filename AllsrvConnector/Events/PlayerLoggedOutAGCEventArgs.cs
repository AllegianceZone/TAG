using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Player LoggedOut event
	/// </summary>
	public class PlayerLoggedOutAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public PlayerLoggedOutAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The ID of the player who logged out (TargetID)
		/// </summary>
		public int PlayerID
		{
			get {return (int)_args[3];}
		}
		
		/// <summary>
		/// The name of the player who logged out (TargetName)
		/// </summary>
		public string PlayerName
		{
			get {return _args[4].ToString();}
		}
		
		/// <summary>
		/// The ID of the game that the player left
		/// </summary>
		public int GameID
		{
			get {return (int)_args[6];}
		}

		/// <summary>
		/// The name of the game that the player left
		/// </summary>
		public string GameName
		{
			get {return _args[7].ToString();}
		}

		/// <summary>
		/// The name of the player who ejected the logged-out player (if any)
		/// </summary>
		public string BooterName
		{
			get {return (_args.Count > 8) ? _args[8].ToString() : string.Empty;}
		}
	}
}
