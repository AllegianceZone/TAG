using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Game Created event
	/// </summary>
	public class GameCreatedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public GameCreatedAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The ID of the created game (TargetID)
		/// </summary>
		public int GameID
		{
			get {return (int)_args[3];}
		}

		/// <summary>
		/// The name of the created game (TargetName)
		/// </summary>
		public string GameName
		{
			get {return _args[4].ToString();}
		}
	}
}
