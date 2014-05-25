using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Game Over event
	/// </summary>
	public class GameOverAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public GameOverAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The ID of the game that is over (TargetID)
		/// </summary>
		public int GameID
		{
			get {return (int)_args[3];}
		}

		/// <summary>
		/// The name of the game that is over (TargetName)
		/// </summary>
		public string GameName
		{
			get {return _args[4].ToString();}
		}
	}
}
