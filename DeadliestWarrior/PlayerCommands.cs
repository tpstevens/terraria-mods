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
				case "debug":
					Main.NewText("Current time: " + Utilities.getTime24Formatted());
					break;
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
				case "item":
					cmdItem(args);
					break;
				case "randomize":
					cmdRandomize(args);
					break;
				case "settime":
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
			if (args.Length != 1)
			{
				Main.NewText("Usage: /boss [destroyer|eye|golem|prime|queen|saucer|skeletron]");
				return;
			}

			String boss = args[0].ToLower();
			Player player = Main.player[Main.selectedPlayer];

			switch (boss)
			{
				case "destroyer":
					Utilities.setTime24(Utilities.DUSK);
					CombatTracker._instance.registerBoss(Utilities.spawnBoss(134, player));
					break;
				case "eye":
					Utilities.setTime24(Utilities.DUSK);
					CombatTracker._instance.registerBoss(Utilities.spawnBoss(4, player));
					break;
				case "golem":
					CombatTracker._instance.registerBoss(Utilities.spawnBoss(245, player));
					break;
				case "prime":
					Utilities.setTime24(Utilities.DUSK);
					CombatTracker._instance.registerBoss(Utilities.spawnBoss(127, player));
					break;
				case "queen":
					CombatTracker._instance.registerBoss(Utilities.spawnBoss(345, player));
					break;
				case "saucer":
					CombatTracker._instance.registerBoss(Utilities.spawnBoss(395, player));
					break;
				case "skeletron":
					Utilities.setTime24(Utilities.DUSK);
					CombatTracker._instance.registerBoss(Utilities.spawnBoss(35, player));
					break;
				default:
					Main.NewText("Unknown boss: " + args[0]);
					break;
			}
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

		private static int[] cmdRandomize(string[] args)
		{
			if (args.Length != 0)
			{
				Main.NewText("Usage: /randomize");
				return null;
			}

			Player player = Main.player[Main.selectedPlayer];
			Random r = new Random();

			int[,] armor = {
				{727, 728, 729},	// wooden - mixed
				{90, 81, 77},		// iron - mixed
				{696, 697, 698}		// platinum - mixed
			};

			int[,] weapons = {
				{ 39, 24, 0 },		// wooden bow, wooden sword
				{ 99, 4, 0 },		// iron bow, iron broadsword
				{ 3480, 3484, 744 }	// platinum bow, platinum broadsword, diamond staff
			};

			// Clear player inventory
			for (int i = 0; i < player.inventory.Length; ++i)
			{
				player.inventory[i].SetDefaults(0);
			}

			// Choose an item tier
			int index = r.Next() % armor.GetLength(0);

			// Randomize armor set
			for (int i = 0; i < 3; ++i)
			{
				player.armor[i].SetDefaults(armor[index, i]);
			}

			// Randomize weapon set
			for (int i = 0; i < weapons.GetLength(1); ++i)
			{
				player.QuickSpawnItem(weapons[index, i]);
			}

			// Give player unlimited stuff
			player.QuickSpawnItem(3103);	// endless quiver
			player.QuickSpawnItem(3104);    // endless musket pouch
			player.QuickSpawnItem(189, 30); // lots of mana potions

			return weapons[index];
		}

		private static void cmdSetTime(string[] args)
		{
			if (args.Length != 1 && args.Length != 2)
			{
				Main.NewText("Usage: /setTime [hours] [am/pm (optional)]");
				Main.NewText("Usage: /setTime [hours]:[minutes] [am/pm (optional)]");
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
