using System;
using System.Collections;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Provides help with interpreting callsigns
	/// </summary>
	public class CallsignHelper
	{
		private static ArrayList _serverAdmins = null;

		/// <summary>
		/// Provides the list of server admins for future verification
		/// </summary>
		/// <param name="serverAdmins">A list of server admins</param>
		public static void SetServerAdmins(ArrayList serverAdmins)
		{
			_serverAdmins = serverAdmins;
		}

		/// <summary>
		/// Retrieves the authentication level of the specified callsign
		/// </summary>
		/// <param name="callsign">The callsign whose authentication should be parsed</param>
		/// <returns>An AuthLevel value equivelant to the credentials shown with their tokens/tags</returns>
		public static AuthLevel GetAuthLevel (string callsign)
		{
			AuthLevel Result = AuthLevel.User;
			string Callsign = callsign.ToLower();

			if (Callsign.StartsWith("?") || Callsign.StartsWith("$") || Callsign.EndsWith("@alleg"))
				Result = AuthLevel.Alleg;

			if (Callsign.StartsWith("+") || Callsign.EndsWith("@hq") || _serverAdmins.Contains(Callsign))
				Result = AuthLevel.Admin;

			return Result;
		}
	}

	public enum AuthLevel
	{
		User = 0,
		Alleg = 1,
		Admin = 2
	}
}
