using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Terminate event
	/// </summary>
	public class TerminateAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public TerminateAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }
	}
}
