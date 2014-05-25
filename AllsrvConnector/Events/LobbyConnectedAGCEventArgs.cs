using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Lobby Connected event
	/// </summary>
	public class LobbyConnectedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public LobbyConnectedAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }
		
		/// <summary>
		/// The IP address of the lost lobby
		/// </summary>
		public string LobbyIP
		{
			get {return _args[6].ToString();}
		}
	}
}
