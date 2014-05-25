using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Ship Killed event
	/// </summary>
	public class ShipKilledAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public ShipKilledAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The ID of the player who killed the ship
		/// </summary>
		public int KilledID
		{
			get {return (int)_args[3];}
		}

		/// <summary>
		/// The name of the player who killed the ship
		/// </summary>
		public string KilledName
		{
			get {return _args[4].ToString();}
		}

		/// <summary>
		/// The total time taken to kill this target
		/// </summary>
		public string Amount
		{
			get {return _args[6].ToString();}
		}

		/// <summary>
		/// The ID of the pilot of the killed ship
		/// </summary>
		public int KillerID
		{
			get {return (int)_args[7];}
		}

		/// <summary>
		/// The name of the pilot of the killed ship
		/// </summary>
		public string KillerName
		{
			get {return _args[8].ToString();}
		}

		/// <summary>
		/// The ID of the game where this kill occurred
		/// </summary>
		public int GameID
		{
			get {return (int)_args[9];}
		}

		/// <summary>
		/// Unknown. The x,y,z position of the killer?
		/// </summary>
		public string KilledPosition
		{
			get {return _args[10].ToString();}
		}

		/// <summary>
		/// Unknown. The x,y,z position of the killer?
		/// </summary>
		public string KillerPosition
		{
			get {return _args[11].ToString();}
		}

		/// <summary>
		/// Whether or not the killed ship was a lifepod
		/// </summary>
		public int IsLifepod
		{
			get {return (int)_args[12];}
		}
	}
}
