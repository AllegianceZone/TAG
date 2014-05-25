using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Game Ended event
	/// </summary>
	public class GameEndedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public GameEndedAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The ID of the game that ended (TargetID)
		/// </summary>
		public int GameID
		{
			get {return (int)_args[3];}
		}

		/// <summary>
		/// The reason the game ended
		/// </summary>
		public string Reason
		{
			get {return _args[6].ToString();}
		}

		/// <summary>
		/// The ID of the team that won
		/// </summary>
		public int WinningTeamID
		{
			get {return (_args.Count > 7) ? (int)_args[7] : -2;}
		}

		/// <summary>
		/// The name of the team that won
		/// </summary>
		public string WinningTeamName
		{
			get {return (_args.Count > 7) ? _args[8].ToString() : string.Empty;}
		}
	}
}
