using System;
using System.Collections;

using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// The Details of an AGCEvent
	/// </summary>
	public class AGCEventArgs : EventArgs
	{
		/// <summary>
		/// The numer of arguments in a base AGCEvent
		/// </summary>
		protected const int	BASEAGCEVENTARGSCOUNT = 6;

		/// <summary>
		/// The list of arguments
		/// </summary>
		protected ArrayList	_args;
		private string		_description;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="agcEvent"></param>
		public AGCEventArgs(AGCEventWrapper agcEvent)
		{
			_args = agcEvent.AGCEventArgs._args;
			_description = agcEvent.AGCEventArgs._description;
		}

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public AGCEventArgs(IAGCEvent agcEvent)
		{
			if (agcEvent.ID == AGCEventID.AllsrvEventID_ConnectedLobby)
				_args = null;
			_args = new ArrayList(agcEvent.PropertyCount);
			_description = agcEvent.Description;

			// Grab the event stats.

			// For some reason the following loop couldn't be iterated any other way.
			// This weird obj-ref way worked though.
			for (int i = 0; i < agcEvent.PropertyCount; i++)
			{
				object Counter = i;
				object Item = agcEvent.get_Property(ref Counter);
				if (Item != null)
				{
					if (Item is DateTime)
					{
						Item = ((DateTime)Item).ToLocalTime();
					}
					_args.Add(Item);
				}
			}
		}

		/// <summary>
		/// The description of the event
		/// </summary>
		public string Description
		{
			get {return _description;}
		}

		/// <summary>
		/// The machinename of the gameserver that threw the event
		/// </summary>
		public string ServerName
		{
			get {return _args[0].ToString();}
		}

		/// <summary>
		/// The context of an event (??)
		/// </summary>
		public string Context
		{
			get {return _args[1].ToString();}
		}

		/// <summary>
		/// The type of event being thrown
		/// </summary>
		public AGCEventID EventID
		{
			get {return (AGCEventID)_args[2];}
		}

		/// <summary>
		/// The ID of the object signaling the event
		/// </summary>
		public int TargetID
		{
			get {return (int)_args[3];}
		}

		/// <summary>
		/// The name of the object signaling the event
		/// </summary>
		public string TargetName
		{
			get {return _args[4].ToString();}
		}

		/// <summary>
		/// The time the event took place
		/// </summary>
		public DateTime Time
		{
			get {return ((DateTime)_args[5]);}
		}
	}
}
