using System;
using AGCLib;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// A helper class for parsing specific game settings into user-set ones
	/// </summary>
	public class GameSettingsHelper
	{
		/// <summary>
		/// Retrieves the Resources setting for this game
		/// </summary>
		/// <param name="game">The game whose resources setting should be read</param>
		/// <returns>The integer representing the resources setting, or -1 if "custom"</returns>
		public static int GetResources (IAGCGame game)
		{
			int Result = -1;

			// Very Scarce
			if (game.GameParameters.NeutralSectorMineableAsteroids == 2 &&
				game.GameParameters.PlayerSectorMineableAsteroids == 0 &&
				game.GameParameters.NeutralSectorSpecialAsteroids == 1 &&
				game.GameParameters.PlayerSectorSpecialAsteroids == 0)
				Result = 0;

			// Scarce
			if (game.GameParameters.NeutralSectorMineableAsteroids == 2 &&
				game.GameParameters.PlayerSectorMineableAsteroids == 1 &&
				game.GameParameters.NeutralSectorSpecialAsteroids == 1 &&
				game.GameParameters.PlayerSectorSpecialAsteroids == 1)
				Result = 1;

			// Scarce+
			if (game.GameParameters.NeutralSectorMineableAsteroids == 2 &&
				game.GameParameters.PlayerSectorMineableAsteroids == 2 &&
				game.GameParameters.NeutralSectorSpecialAsteroids == 1 &&
				game.GameParameters.PlayerSectorSpecialAsteroids == 2)
				Result = 2;

			// Normal
			if (game.GameParameters.NeutralSectorMineableAsteroids == 4 &&
				game.GameParameters.PlayerSectorMineableAsteroids == 2 &&
				game.GameParameters.NeutralSectorSpecialAsteroids == 1 &&
				game.GameParameters.PlayerSectorSpecialAsteroids == 1)
				Result = 3;

			// N:NoHomeS
			if (game.GameParameters.NeutralSectorMineableAsteroids == 4 &&
				game.GameParameters.PlayerSectorMineableAsteroids == 2 &&
				game.GameParameters.NeutralSectorSpecialAsteroids == 1 &&
				game.GameParameters.PlayerSectorSpecialAsteroids == 0)
				Result = 4;

			// Equal
			if (game.GameParameters.NeutralSectorMineableAsteroids == 2 &&
				game.GameParameters.PlayerSectorMineableAsteroids == 2 &&
				game.GameParameters.NeutralSectorSpecialAsteroids == 2 &&
				game.GameParameters.PlayerSectorSpecialAsteroids == 0)
				Result = 5;

			// Plentiful
			if (game.GameParameters.NeutralSectorMineableAsteroids == 4 &&
				game.GameParameters.PlayerSectorMineableAsteroids == 2 &&
				game.GameParameters.NeutralSectorSpecialAsteroids == 2 &&
				game.GameParameters.PlayerSectorSpecialAsteroids == 1)
				Result = 6;

			// P:NoHomeS
			if (game.GameParameters.NeutralSectorMineableAsteroids == 4 &&
				game.GameParameters.PlayerSectorMineableAsteroids == 2 &&
				game.GameParameters.NeutralSectorSpecialAsteroids == 2 &&
				game.GameParameters.PlayerSectorSpecialAsteroids == 0)
				Result = 7;

			return Result;
		}

		/// <summary>
		/// Convert the map number into a map name for the specified game
		/// </summary>
		/// <param name="game">The game whose MapName should be retrieved</param>
		public static string GetMapName (IAGCGame game)
		{
			string Result = game.GameParameters.CustomMap;
			
			if (Result.Equals(string.Empty))
			{
				switch (game.GameParameters.MapType)
				{
					case 0:
						Result = "Single Ring";
						break;
					case 1:
						Result = "Double Ring";
						break;
					case 2:
						Result = "Pinwheel";
						break;
					case 3:
						Result = "Diamond Ring";
						break;
					case 4:
						Result = "Snowflake";
						break;
					case 5:
						Result = "Split Bases";
						break;
					case 6:
						Result = "Brawl";
						break;
					case 7:
						Result = "Big Ring";
						break;
					case 8:
						Result = "HiLo";
						break;
					case 9:
						Result = "HiHigher";
						break;
					case 10:
						Result = "Star";
						break;
					case 11:
						Result = "InsideOut";
						break;
					case 12:
						Result = "Grid";
						break;
					case 13:
						Result = "East West";
						break;
					case 14:
						Result = "Large Split";
						break;
					default:
						break;
				}
			}

			return Result;
		}
	}
}
