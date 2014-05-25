using System;
using System.Diagnostics;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Provides access to the ASGS web services
	/// </summary>
	public class AsgsConnector
	{

		private static Asgs.Services _services;
		private static bool		_isInitialized = false;
		private static string	_asgsUrl = TagConfig.DEFAULTASGSURL;

		/// <summary>
		/// Initializes the ASGSConnector
		/// </summary>
		/// <param name="asgsUrl">The URL of ASGS's services, or null if the default should be used</param>
		/// <param name="timeout">The number of milliseconds to wait for a response from the ASGS server upon posting</param>
		public static void Initialize (string asgsUrl, int timeout)
		{
			// If a URL was specified that's not null or empty, save it.
			if (asgsUrl != null)
			{
				if (!asgsUrl.Equals(string.Empty))
				{
					_asgsUrl = asgsUrl;
				}
			}

			// Load services
			_services = new Asgs.Services();
			_services.Timeout = timeout;
			_services.Url = _asgsUrl;

			_isInitialized = true;
		}

		/// <summary>
		/// Posts the specified game to the ASGS database
		/// </summary>
		/// <param name="gamedata">The compressed xml of a TagDataset</param>
		/// <param name="message">A reply message from the ASGS server</param>
		/// <returns>Positive if successful (GameID)
		///			-1 if error occurred</returns>
		public static int PostGame (string gamedata, out string message)
		{
			int Result = -1;
			message = "An error occurred while posting stats to ASGS.";

			try
			{
				Result = _services.PostGameStatistics(gamedata, out message);
			}
			catch (Exception e)
			{
				message = "Error posting game: " + e.Message;
				TagTrace.WriteLine(TraceLevel.Error, message);
			}

			return Result;
		}
	}
}
