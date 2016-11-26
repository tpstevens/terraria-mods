using System;
using System.Collections.Generic;
using System.IO;

using Terraria;

namespace DeadliestWarrior
{
	public sealed class CombatTracker
	{
		public static readonly CombatTracker _instance = new CombatTracker();
		private static readonly string VERSION_CODE = "v4";
		
		private CombatLog currentLog = null;

		public enum DamageType {MELEE = 0, MAGIC = 1, RANGED = 2, SIZE = 3};

		private class CombatLog
		{
			private Player player;
			public NPC boss;

			private int armorHead, armorChest, armorLegs;
			private Dictionary<int, int> requiredBossParts;
			public int totalDamageToBoss = 0, totalBossHealth = 0;
			public int maxLife, maxMana;
			public int[] damageByType;
			public int[] weaponIdsByType;

			public CombatLog(Player player, NPC boss, ClassSetup setup)
			{
				this.player = player;
				this.boss = boss;

				armorHead = player.armor[0].type;
				armorChest = player.armor[1].type;
				armorLegs = player.armor[2].type;
				maxLife = player.statLifeMax2;
				maxMana = player.statManaMax2;

				requiredBossParts = new Dictionary<int, int>();

				damageByType = new int[(int)DamageType.SIZE];
				weaponIdsByType = new int[(int)DamageType.SIZE];
				weaponIdsByType[(int)DamageType.MAGIC] = setup.weapons[0];
				weaponIdsByType[(int)DamageType.MELEE] = setup.weapons[1];
				weaponIdsByType[(int)DamageType.RANGED] = setup.weapons[2];
			}

			public void checkForRequiredParts()
			{
				for (int i = 0; i < Main.npc.Length; ++i)
				{
					NPC n = Main.npc[i];
					if (n.type > 0 && n.active && Utilities.countsTowardsRequiredBossHealth(n.type) && !requiredBossParts.ContainsKey(n.whoAmI))
					{
						requiredBossParts.Add(n.whoAmI, n.lifeMax);
						totalBossHealth += n.lifeMax;
						Main.NewText("Added " + n.name + " (id = " + n.type + ") with " + n.lifeMax + " health.");
					}
				}
			}

			public NPC getCurrentBoss()
			{
				return boss;
			}

			public void recordDamage(NPC npc, DamageType type, int damage)
			{
				damageByType[(int)type] += damage;
				totalDamageToBoss += damage;

				checkForRequiredParts();
			}

			public void writeToFile()
			{
				try
				{
					DateTime currentTime = DateTime.Now;
					string fileName = currentTime.ToString("yyyy-MM-dd_hh-mm-ss") + ".txt";
					string folderName = "CombatLogs/" + VERSION_CODE;

					Directory.CreateDirectory(folderName);
					StreamWriter s = File.CreateText(folderName + "/" + fileName);

					s.Write("<data version=\"" + VERSION_CODE + "\">\n");
					s.Write("\t<player>\n");
					s.Write("\t\t<name>" + player.name + "</name>\n");
					s.Write("\t\t<armor>\n");
					s.Write("\t\t\t<head>\n");
					s.Write("\t\t\t\t<id>" + armorHead + "</id>\n");
					s.Write("\t\t\t\t<name>" + Utilities.getItemName(armorHead) + "</name>\n");
					s.Write("\t\t\t</head>\n");
					s.Write("\t\t\t<chest>\n");
					s.Write("\t\t\t\t<id>" + armorChest + "</id>\n");
					s.Write("\t\t\t\t<name>" + Utilities.getItemName(armorChest) + "</name>\n");
					s.Write("\t\t\t</chest>\n");
					s.Write("\t\t\t<legs>\n");
					s.Write("\t\t\t\t<id>" + armorLegs + "</id>\n");
					s.Write("\t\t\t\t<name>" + Utilities.getItemName(armorLegs) + "</name>\n");
					s.Write("\t\t\t</legs>\n");
					s.Write("\t\t</armor>\n");
					s.Write("\t\t<stats>\n");
					s.Write("\t\t\t<maxLife>" + maxLife + "</maxLife>\n");
					s.Write("\t\t\t<maxMana>" + maxMana + "</maxMana>\n");
					s.Write("\t\t</stats>\n");
					s.Write("\t\t<weapons>\n");

					for (int i = 0; i < (int)DamageType.SIZE; ++i)
					{
						if (weaponIdsByType[i] != 0)
						{
							s.Write("\t\t\t<weapon>\n");
							s.Write("\t\t\t\t<id>" + weaponIdsByType[i] +"</id>\n");
							s.Write("\t\t\t\t<name>" + Utilities.getItemName(weaponIdsByType[i]) + "</name>\n");
							s.Write("\t\t\t\t<damage>" + damageByType[i] + "</damage>\n");
							s.Write("\t\t\t</weapon>\n");
						}
					}

					s.Write("\t\t</weapons>\n");
					s.Write("\t</player>\n");
					s.Write("\t<boss>\n");
					s.Write("\t\t<id>" + boss.type + "</id>\n");
					s.Write("\t\t<name>" + boss.name + "</name>\n");
					s.Write("\t\t<expert>" + Main.expertMode.ToString() + "</expert>\n");
					s.Write("\t\t<maxLife>" + totalBossHealth + "</maxLife>\n");
					s.Write("\t\t<damageTaken>" + totalDamageToBoss + "</damageTaken>\n");
					s.Write("\t\t<killed>" + (boss.life <= 0).ToString() + "</killed>\n");
					s.Write("\t</boss>\n");
					s.Write("</data>");
					
					s.Close();

					Main.NewText("Logged to file.");
				}
				catch (Exception e)
				{
					Main.NewText("(" + e.StackTrace + ") " + e.Message);
				}
			}
		}

		public bool bossActive()
		{
			bool bossActive = false;

			if (currentLog != null)
			{
				for (int i = 0; i < Main.npc.Length; ++i)
				{
					if (Main.npc[i].active && Main.npc[i].boss)
					{
						bossActive = true;
						break;
					}
				}
			}

			return bossActive;
		}

		public bool registerBoss(NPC boss, ClassSetup setup)
		{
			if (boss != null)
			{
				if (currentLog != null)
				{
					if (bossActive())
					{
						Main.NewText("Cannot track multiple bosses at once (failed to register " + boss.displayName + ")");
						return false;
					}
					else
					{
						Main.NewText("Clearing inactive boss.");
						currentLog.writeToFile();
						currentLog = null;
					}
				}

				currentLog = new CombatLog(Main.player[Main.selectedPlayer], boss, setup);
				Main.NewText("Registered " + boss.displayName + " successfully");

				return true;
			}

			return false;
		}

		private void recordBossDeath()
		{
			if (currentLog != null)
			{
				Main.NewText(currentLog.boss.displayName + " is dead. Total damage dealt: " + currentLog.totalDamageToBoss + "/" + currentLog.totalBossHealth);
				currentLog.writeToFile();
				currentLog = null;
			}
		}

		public void recordPlayerDeath()
		{
			if (currentLog != null)
			{
				currentLog.checkForRequiredParts();
				currentLog.writeToFile();
				currentLog = null;
			}
		}

		public void recordHit(Player player, NPC npc, DamageType type, int damage)
		{
			if (player.whoAmI == Main.selectedPlayer && currentLog != null && Utilities.isSupportedBoss(npc.type))
			{
				currentLog.recordDamage(npc, type, damage);

				if (currentLog.boss.life <= 0)
					recordBossDeath();
			}
		}
	}
}
