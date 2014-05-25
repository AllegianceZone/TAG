using System;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// Helps clean up chat targets and wingnames
	/// </summary>
	public class ChatTargetHelper
	{
		/// <summary>
		/// Cleans the specified target/type into a readable target
		/// </summary>
		/// <param name="target">The target of the chatmessage</param>
		/// <param name="type">The type of the target</param>
		/// <returns>A "friendly" name for the chat target</returns>
		public static string CleanTargetName(string target, string type)
		{
			string Result = target;

			switch (type)
			{
				case "ALL_SECTOR":
				case "GROUP":
				case "INDIVIDUAL":
					Result = target;
					break;
				case "INDIVIDUAL_ECHO":
					Result = GetWingName(target);
					break;
				case "EVERYONE":
					Result = "All";
					break;
				case "TEAM":
					Result = "Team";
					break;
				case "LEADERS":
					Result = "LeadersOnly";
					break;
				default:
					break;
			}
			return Result;
		}

		/// <summary>
		/// Parses the specified wingtype into a friendly wing
		/// </summary>
		/// <param name="wingType">The type of wing</param>
		/// <returns>The friendly name of the specified wing</returns>
		private static string GetWingName (string wingType)
		{
			string Result = null;

			switch (wingType)
			{
				case "Wing0":
					Result = "command";
					break;
				case "Wing1":
					Result = "attack";
					break;
				case "Wing2":
					Result = "defend";
					break;
				case "Wing3":
					Result = "escort";
					break;
				case "Wing4":
					Result = "search";
					break;
				case "Wing5":
					Result = "alpha";
					break;
				case "Wing6":
					Result = "bravo";
					break;
				case "Wing7":
					Result = "charlie";
					break;
				case "Wing8":
					Result = "delta";
					break;
				case "Wing9":
					Result = "echo";
					break;
				default:
					break;
			}

			return Result;
		}
	}
}
