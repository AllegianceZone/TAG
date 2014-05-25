using System;
using System.Collections.Generic;
using System.Text;
using AGCLib;
using FreeAllegiance.Tag.Events;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// BT - 4/16/2012 - ACSS / Mach
	/// Passing an unwrapped __ComObject through a thread pool causes InvalidCastExcpetion under VS2010.
	/// </summary>
	public class AGCEventWrapper
	{
		public string ComputerName { get; set; }

		public string Context { get; set; }

		public string Description { get; set; }

		public AGCEventID ID { get; set; }

		public int PropertyCount { get; set; }

		public int SubjectID { get; set; }

		public string SubjectName { get; set; }

		public DateTime Time { get; set; }

		public readonly AGCEventArgs AGCEventArgs;

		public AGCEventWrapper(IAGCEvent agcEvent)
		{
			this.ComputerName = agcEvent.ComputerName;
			this.Context = agcEvent.Context;
			this.Description = agcEvent.Description;
			this.ID = agcEvent.ID;
			this.PropertyCount = agcEvent.PropertyCount;
			this.SubjectID = agcEvent.SubjectID;
			this.SubjectName = agcEvent.SubjectName;
			this.Time = agcEvent.Time;
			this.AGCEventArgs = new AGCEventArgs(agcEvent);
		}
	}
}
