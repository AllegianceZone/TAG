using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Lobby Disconnected event
	/// </summary>
	public class LobbyDisconnectedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public LobbyDisconnectedAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The IP address of the lost lobby
		/// </summary>
		public string LobbyIP
		{
			get {return _args[6].ToString();}
		}
	}
}
