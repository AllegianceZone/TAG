using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Station Destroyed event
	/// </summary>
	public class StationDestroyedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public StationDestroyedAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The ID of the player who killed this station (TargetID)
		/// </summary>
		public int KillerID
		{
			get {return (int)_args[3];}
		}

		/// <summary>
		/// The name of the player who killed this station (TargetName)
		/// </summary>
		public string KillerName
		{
			get {return _args[4].ToString();}
		}

		/// <summary>
		/// The ID of the game where this basekill occurred (-1 if FAZ R1)
		/// </summary>
		public int GameID
		{
			get {return (_args.Count > 9) ? (int)_args[6] : -1;}
		}

		/// <summary>
		/// The ID of the team who lost their base
		/// </summary>
		public int TeamID
		{
			get {return (_args.Count > 9) ? (int)_args[7] : (int)_args[6];}
		}

		/// <summary>
		/// The name of the team who lost their base
		/// </summary>
		public string TeamName
		{
			get {return (_args.Count > 9) ? _args[8].ToString() : _args[7].ToString();}
		}

		/// <summary>
		/// The name of the station that was destroyed
		/// </summary>
		public string StationName
		{
			get {return (_args.Count > 9) ? _args[9].ToString() : string.Empty;}
		}
	}
}
