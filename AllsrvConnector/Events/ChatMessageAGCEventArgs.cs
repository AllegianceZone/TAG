using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// Details of the Chat Message event
	/// </summary>
	public class ChatMessageAGCEventArgs : AGCEventArgs
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="agcEvent">The AGCEvent being thrown</param>
		public ChatMessageAGCEventArgs(AGCEventWrapper agcEvent) : base(agcEvent) { }

		/// <summary>
		/// The ID of the player sending the chat (same as TargetID)
		/// </summary>
		public int SpeakerID
		{
			get {return (int)_args[3];}
		}

		/// <summary>
		/// The callsign of the player sending the chat (Same as TargetName)
		/// </summary>
		public string SpeakerName
		{
			get {return _args[4].ToString();}
		}

		/// <summary>
		/// The ID of the command issued with the chat
		/// </summary>
		public int CommandID
		{
			get {return (int)_args[6];}
		}

		/// <summary>
		/// The MissionID of the game where the chat occurred
		/// </summary>
		public int GameID
		{
			get {return (int)_args[7];}
		}

		/// <summary>
		/// The contents of the chat message
		/// </summary>
		public string Text
		{
			get {return _args[8].ToString();}
		}

		/// <summary>
		/// The callsign or ChatTarget of the recipient
		/// </summary>
		public string RecipientName
		{
			get {return _args[9].ToString();}
		}

		/// <summary>
		/// The ID of the recipient
		/// </summary>
		public int RecipientID
		{
			get {return (int)_args[10];}
		}

		/// <summary>
		/// The type of chat being sent
		/// </summary>
		public string ChatType
		{
			get {return _args[11].ToString();}
		}

		/// <summary>
		/// The voicechat ID associated with this chat message
		/// </summary>
		public int VoiceID
		{
			get {return (int)_args[12];}
		}
	}
}
