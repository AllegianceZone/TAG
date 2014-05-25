using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Admin Paged event
	/// </summary>
	public class AdminPagedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public AdminPagedAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The callsign who paged the admin
		/// </summary>
		public string Callsign
		{
			get {return _args[4].ToString();}
		}
		
		/// <summary>
		/// The ID of the game where the page was sent from
		/// </summary>
		public int GameID
		{
			get {return (int)_args[7];}
		}
		
		/// <summary>
		/// The message that was paged
		/// </summary>
		public string Message
		{
			get {return _args[8].ToString();}
		}
	}
}
