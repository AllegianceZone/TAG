using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Player Dropped event
	/// </summary>
	public class PlayerDroppedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public PlayerDroppedAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The name of the player that dropped (TargetName)
		/// </summary>
		public string PlayerName
		{
			get {return _args[4].ToString();}
		}
	}
}
