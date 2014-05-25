using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Game Started event
	/// </summary>
	public class GameStartedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public GameStartedAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The ID of the game that started (TargetID)
		/// </summary>
		public int GameID
		{
			get {return (int)_args[3];}
		}

		/// <summary>
		/// The name of the game that started (TargetName)
		/// </summary>
		public string GameName
		{
			get {return _args[4].ToString();}
		}
	}
}
