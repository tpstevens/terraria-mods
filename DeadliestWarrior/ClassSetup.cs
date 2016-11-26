using System;

using Terraria;

namespace DeadliestWarrior
{
	public class ClassSetup
	{
		public readonly int[] armor;
		public readonly int[] weapons;
		public readonly int baseLife = 200, baseMana = 20;
		public readonly int tier = -1;

		private static int[,] armorTiers = {
			{727, 728, 729},	// wooden
			{90, 81, 77},		// iron
			{696, 697, 698},    // platinum
			{1216, 1218, 1219},	// ranged titanium
			{1002, 1004, 1005},	// ranged chlorophyte
			{2189, 1504, 1505},	// dps spectre (magic)
			{2757, 2758, 2759}	// vortex (ranged)
		};

		private static int[,] weaponTiers = {
			{0, 24, 39},		// [magic], wooden sword, wooden bow
			{0, 4, 99},			// [magic], iron broadsword, iron bow
			{744, 3484, 3480},  // diamond staff, platinum broadsword, platinum bow
			{3209, 3054, 3788}, // Crystal Serpent, Shadowflame Knife, Onyx Blaster
			{2188, 3013, 1229},	// Venom Staff, Fetid Baghnakhs, Chlorophyte Shotbow
			{1260, 1259, 1254},	// Rainbow Gun, Flower Pow, Sniper Rifle
			{2622, 1569, 3540}	// Razorblade Typhoon, Vampire Knives, Phantasm
		};

		public ClassSetup() : this(new Random(DateTime.Now.Millisecond).Next() % armorTiers.GetLength(0))
		{
			// Intentionally empty
		}

		public ClassSetup(int tier)
		{
			this.tier = tier;
			armor = new int[3];
			weapons = new int[3];

			if (tier < 0 || tier >= armorTiers.GetLength(0))
			{
				Main.NewText("Tier " + tier + " must be between 0 and " + (armorTiers.GetLength(0) - 1) + ", inclusive.");
			}
			else
			{
				for (int i = 0; i < 3; ++i)
				{
					armor[i] = armorTiers[tier, i];
					weapons[i] = weaponTiers[tier, i];
				}

				switch (tier)
				{
					case 0:
						baseLife = 200;
						baseMana = 20;
						break;
					case 1:
						baseLife = 250;
						baseMana = 80;
						break;
					case 2:
						baseLife = 300;
						baseMana = 160;
						break;
					case 3:
					case 4:
						baseLife = 400;
						baseMana = 200;
						break;
					default:
						baseLife = 500;
						baseMana = 200;
						break;
				}
			}
		}
	}
}
