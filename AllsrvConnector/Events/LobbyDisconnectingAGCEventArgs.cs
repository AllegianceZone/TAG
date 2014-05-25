using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Lobby Disconnecting event
	/// </summary>
	public class LobbyDisconnectingAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public LobbyDisconnectingAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The IP address of the lost lobby
		/// </summary>
		public string LobbyIP
		{
			get {return _args[6].ToString();}
		}
	}
}
