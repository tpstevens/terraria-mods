using System;

using Terraria;

namespace DeadliestWarrior
{
	class Utilities
	{
		public const double DAWN = 4.5;
		public const double DUSK = 19.5;

		public static bool isSupportedBoss(int npcType)
		{
			switch (npcType)
			{
				case 4:		// Eye of Cthulu
				case 35:	// Skeletron
				case 127:	// Skeletron Prime
				case 134:	// Destroyer
				case 345:	// Ice Queen
				case 395:	// Martian Saucer
					return true;
				default:
					return false;
			}
		}

		public static void setTime24(double hours)
		{
			while (hours >= 24)
				hours -= 24;

			if (hours >= DUSK)
			{
				Main.time = (hours - DUSK) * 3600;
				Main.dayTime = false;
			}
			else if (hours >= DAWN)
			{
				Main.time = (hours - DAWN) * 3600;
				Main.dayTime = true;
			}
			else
			{
				Main.time = (hours + (24.0 - DUSK)) * 3600;
				Main.dayTime = false;
			}
		}

		public static double getTime24()
		{
			double time = (Main.time / 3600 + (Main.dayTime ? 4.5 : 19.5));
			while (time >= 24)
				time -= 24;
			return time;
		}

		public static String getTime12Formatted()
		{
			double time = getTime24();
			String ampm = (time < 12) ? " AM" : " PM";

			if (time < 1)
				time += 12;
			else if (time >= 13)
				time -= 12;

			int hours = (int)time;
			int minutes = (int)((time - hours) * 60.0);

			return hours + ":" + (minutes < 10 ? "0" : "") + minutes + ampm;
		}

		public static String getTime24Formatted()
		{
			double time = getTime24();
			int hours = (int)time;
			int minutes = (int)((time - hours) * 60.0);
			return hours + ":" + (minutes < 10 ? "0" : "") + minutes;
		}

		public static NPC spawnNPC(int type, Player player)
		{
			Random r = new Random();
			int quadrant = r.Next() % 4;
			int degrees = r.Next() % 90;
			int h = Main.offScreenRange * 32;

			int x = (int) (h * Math.Cos(degrees));
			int y = (int)(h * Math.Sin(degrees));

			if (quadrant == 1 || quadrant == 2)
				x *= -1;

			if (quadrant == 2 || quadrant == 3)
				y *= -1;

			int npcId = NPC.NewNPC((int)player.Center.X + x, (int)player.Center.Y + y, type);
			if (npcId < Main.npc.Length)
				return Main.npc[npcId];

			return null;
		}
	}
}
