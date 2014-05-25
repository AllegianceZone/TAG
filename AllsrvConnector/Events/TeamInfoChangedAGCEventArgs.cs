using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the TeamInfo Changed event
	/// </summary>
	public class TeamInfoChangedAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public TeamInfoChangedAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The ID of the team that changed its info (TargetID)
		/// </summary>
		public int TeamID
		{
			get {return (int)_args[3];}
		}

		/// <summary>
		/// The name of the team that changed its info (TargetName)
		/// </summary>
		public string OldTeamName
		{
			get {return _args[4].ToString();}
		}
		
		/// <summary>
		/// The ID of the game where the team changed its info
		/// </summary>
		public int GameID
		{
			get {return (int)_args[6];}
		}
		
		/// <summary>
		/// The name of the game where the team changed its info
		/// </summary>
		public string GameName
		{
			get {return _args[7].ToString();}
		}
		
		/// <summary>
		/// The new name of the team
		/// </summary>
		public string NewTeamName
		{
			get {return _args[8].ToString();}
		}
	}
}
