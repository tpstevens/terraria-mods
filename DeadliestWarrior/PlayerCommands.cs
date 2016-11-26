using System;

using Terraria;

namespace DeadliestWarrior
{
	class PlayerCommands
	{
		public static bool execute(String cmd, String[] args)
		{
			bool valid = true;
			String cmdLower = cmd.ToLower();

			switch (cmdLower)
			{
				case "addtime":
					cmdAddTime(args);
					break;
				case "boss":
					cmdBoss(args);
					break;
				case "dawn":
					cmdDawn(args);
					break;
				case "dusk":
					cmdDusk(args);
					break;
				case "noon":
					cmdNoon(args);
					break;
				case "item":
					cmdItem(args);
					break;
				case "settier":
				case "tier":
					cmdSetTier(args);
					break;
				case "settime":
				case "time":
					cmdSetTime(args);
					break;
				default:
					Main.NewText("ERROR: Unknown Command (" + cmd + ")");
					valid = false;
					break;
			}

			return valid;
		}

		private static void cmdAddTime(string[] args)
		{
			double h;
			if (args.Length != 1 || !double.TryParse(args[0], out h) || h <= 0)
			{
				Main.NewText("Usage: /addTime [hours (must be positive)]");
			}
			else
			{
				Utilities.setTime24(Utilities.getTime24() + h);
				Main.NewText("Time set to " + Utilities.getTime12Formatted());
			}
		}

		private static void cmdBoss(string[] args)
		{
			if (args.Length != 1 && args.Length != 2)
			{
				Main.NewText("Usage: /boss [destroyer|eye|prime|queen|saucer|skeletron] [tier (blank for random)]");
				return;
			}

			bool applySetup = false;
			ClassSetup classSetup;
			String boss = args[0].ToLower();
			Player player = Main.player[Main.selectedPlayer];

			int tier = -1;
			if (args.Length == 2)
			{
				if (!int.TryParse(args[1], out tier))
				{
					Main.NewText("Failed to recognize tier " + args[1] + " as an integer. Randomizing.");
					classSetup = new ClassSetup();
				}
				else
				{
					classSetup = new ClassSetup(tier);
				}
			}
			else
			{
				classSetup = new ClassSetup();
			}

			switch (boss)
			{
				case "destroyer":
					Utilities.setTime24(Utilities.DUSK);
					applySetup = CombatTracker._instance.registerBoss(Utilities.spawnBoss(134, player), classSetup);
					break;
				case "eye":
					Utilities.setTime24(Utilities.DUSK);
					applySetup = CombatTracker._instance.registerBoss(Utilities.spawnBoss(4, player), classSetup);
					break;
				case "prime":
					Utilities.setTime24(Utilities.DUSK);
					applySetup = CombatTracker._instance.registerBoss(Utilities.spawnBoss(127, player), classSetup);
					break;
				case "queen":
					applySetup = CombatTracker._instance.registerBoss(Utilities.spawnBoss(345, player), classSetup);
					break;
				case "saucer":
					applySetup = CombatTracker._instance.registerBoss(Utilities.spawnBoss(395, player), classSetup);
					break;
				case "skeletron":
					Utilities.setTime24(Utilities.DUSK);
					applySetup = CombatTracker._instance.registerBoss(Utilities.spawnBoss(35, player), classSetup);
					break;
				default:
					Main.NewText("Unknown boss: " + args[0]);
					break;
			}

			if (applySetup)
				Utilities.applyClassSetup(Main.player[Main.selectedPlayer], classSetup);
		}

		private static void cmdDawn(string[] args)
		{
			if (args.Length != 0)
			{
				Main.NewText("Usage: /dawn");
			}
			else
			{
				Utilities.setTime24(Utilities.DAWN);
				Main.NewText("Time set to dawn");
			}
		}

		private static void cmdDusk(string[] args)
		{
			if (args.Length != 0)
			{
				Main.NewText("Usage: /dusk");
				return;
			}

			Utilities.setTime24(Utilities.DUSK);
			Main.NewText("Time set to dusk");
		}

		private static void cmdItem(string[] args)
		{
			if (args.Length != 1)
			{
				Main.NewText("Usage: /item [id]");
				return;
			}

			int itemId = -1;
			if (!int.TryParse(args[0], out itemId))
			{
				Main.NewText("Could not parse " + args[0] + " as an item ID");
				return;
			}

			Main.player[Main.selectedPlayer].QuickSpawnItem(itemId);
		}

		private static void cmdNoon(string[] args)
		{
			if (args.Length != 0)
			{
				Main.NewText("Usage: /noon");
				return;
			}

			Utilities.setTime24(12);
		}

		private static void cmdSetTier(string[] args)
		{
			int tier = -1;

			if (args.Length != 0 && args.Length != 1)
			{
				Main.NewText("Usage: /setTier [int tier]");
				return;
			}
			else if (args.Length == 1 && !int.TryParse(args[0], out tier))
			{
				Main.NewText("Usage: /setTier [int tier]");
				return;
			}

			Player player = Main.player[Main.selectedPlayer];
			ClassSetup setup;

			setup = (args.Length == 0) ? new ClassSetup() : new ClassSetup(tier);
			Utilities.applyClassSetup(Main.player[Main.selectedPlayer], setup);
		}

		private static void cmdSetTime(string[] args)
		{
			if (args.Length != 1 && args.Length != 2)
			{
				Main.NewText("Usage: /[setTime|time] [hours] [am/pm (optional)]");
				Main.NewText("Usage: /[setTime|time] [hours]:[minutes] [am/pm (optional)]");
				return;
			}

			bool isAM = (Main.dayTime && ((Main.time / 3600) < 7.5)) || // 12:00pm == 7.5 hours after 4:30am
						(!Main.dayTime && ((Main.time / 3600) >= 4.5)); // 12:00am == 4.5 hours after 7:30pm
			bool validTime = false;
			int hours = 0;
			int minutes = 0;
			double time;
			String sTime = args[0].Trim().ToLower();
			String sMeridian = (args.Length > 1) ? args[1].Trim().ToLower() : "";
			String[] aSplitTime;

			// Check number of arguments and the value of sMeridian, if it exists
			if (args.Length == 1)
			{
				if (sTime.Length >= 3 && (sTime.EndsWith("am") || sTime.EndsWith("pm")))
				{
					sMeridian = sTime.Substring(sTime.Length - 2, 2);
					sTime = sTime.Substring(0, sTime.Length - 2);
				}
			}
			else if (args.Length != 2 || (!sMeridian.Equals("am") && !sMeridian.EndsWith("pm")))
			{
				Main.NewText("Usage: /setTime [hours][am/pm (optional)]");
				return;
			}

			// Update isAM if a valid sMeridian was found
			isAM = (sMeridian != null) ? sMeridian.Equals("am") : isAM;

			// Get the time
			aSplitTime = sTime.Split(':');
			switch (aSplitTime.Length)
			{
				case 1:
					validTime = int.TryParse(aSplitTime[0], out hours) &&
								hours >= 1 && hours <= 12;
					break;
				case 2:
					validTime = int.TryParse(aSplitTime[0], out hours) &&
								int.TryParse(aSplitTime[1], out minutes) &&
								hours >= 1 && hours <= 12 &&
								minutes >= 0 && minutes <= 59;
					break;
				default:
					break;
			}

			// Return if time is invalid
			if (!validTime)
			{
				Main.NewText("Usage: /setTime [hours][am/pm (optional)]");
				return;
			}

			// Convert to 24-hour time
			time = hours + minutes / 60.0;
			if (isAM && time >= 12)
			{
				time -= 12;
			}
			else if (!isAM && time < 12)
			{
				time += 12;
			}

			// Set the time and print the result
			Utilities.setTime24(time);
			Main.NewText("Time set to " + Utilities.getTime12Formatted());
		}
	}
}
