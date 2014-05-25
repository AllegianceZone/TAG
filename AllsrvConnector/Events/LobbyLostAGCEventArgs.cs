using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the LobbyLost event
	/// </summary>
	public class LobbyLostAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public LobbyLostAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }
	}
}
