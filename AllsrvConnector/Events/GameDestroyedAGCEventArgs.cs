using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Game Destroyed event
	/// </summary>
	public class GameDestroyedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public GameDestroyedAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }
		
		/// <summary>
		/// The ID of the game that was destroyed (TargetID)
		/// </summary>
		public int GameID
		{
			get {return (int)_args[3];}
		}

		/// <summary>
		/// The name of the game that was destroyed (TargetName)
		/// </summary>
		public string GameName
		{
			get {return _args[4].ToString();}
		}
	}
}
