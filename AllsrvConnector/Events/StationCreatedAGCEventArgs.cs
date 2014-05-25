using System;
using System.Diagnostics;

using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Station Created event
	/// </summary>
	public class StationCreatedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public StationCreatedAGCEventArgs(AGCEventWrapper agcEvent)
			: base(agcEvent)
		{
			//TagTrace.WriteLine(TraceLevel.Verbose, "Args length: {0}. 6: {1}, 7: {2}, 8: {3}", _args.Count, _args[6], _args[7], _args[8]);
		}

		/// <summary>
		/// The ID of the game where this base was built
		/// </summary>
		public int GameID
		{
			get {return (int)_args[6];}
		}

		/// <summary>
		/// The ID of the team who built the base
		/// </summary>
		public int TeamID
		{
			get {return (int)_args[7];}
		}

		/// <summary>
		/// The name of the team who built the base
		/// </summary>
		public string TeamName
		{
			get {return _args[8].ToString();}
		}

		/// <summary>
		/// The name of the station that was built
		/// </summary>
		public string StationName
		{
			get {return (_args.Count > 9) ? _args[9].ToString() : string.Empty;}
		}
	}
}
