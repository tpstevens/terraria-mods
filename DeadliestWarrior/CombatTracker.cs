using System;
using System.Collections.Generic;
using System.IO;

using Terraria;

namespace DeadliestWarrior
{
	public sealed class CombatTracker
	{
		public static readonly CombatTracker _instance = new CombatTracker();
		private static readonly string VERSION_CODE = "v1";
		
		private CombatLog currentLog;

		private class CombatLog
		{
			private Player player;
			public NPC boss;

			private int armorHead, armorChest, armorLegs;
			private Dictionary<int, int> requiredBossParts;
			private Dictionary<Tuple<int, string>, WeaponLog> weaponLogs;
			public int totalDamageToBoss, totalBossHealth;
			public int maxLife, maxMana;

			public CombatLog(Player player, NPC boss)
			{
				this.player = player;
				this.boss = boss;

				armorHead = player.armor[0].type;
				armorChest = player.armor[1].type;
				armorLegs = player.armor[2].type;
				maxLife = player.statLifeMax2;
				maxMana = player.statManaMax2;

				totalDamageToBoss = 0;

				Main.NewText(armorHead + " " + armorChest + " " + armorLegs);

				requiredBossParts = new Dictionary<int, int>();
				weaponLogs = new Dictionary<Tuple<int,string>, WeaponLog>();
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
						// Main.NewText("Added " + n.name + " (id = " + n.type + ") with " + n.lifeMax + " health.");
					}
				}
			}

			public NPC getCurrentBoss()
			{
				return boss;
			}

			public void recordDamage(NPC npc, string type, int id, int damage)
			{
				try
				{
					Tuple<int, string> key = new Tuple<int, string>(id, type);
					WeaponLog w = null;
					weaponLogs.TryGetValue(key, out w);

					if (w == null)
					{
						w = new WeaponLog(id, type);
						weaponLogs.Add(key, w);
					}

					w.damageDealt += damage;
					totalDamageToBoss += damage;

					checkForRequiredParts();
				}
				catch (Exception e)
				{
					Main.NewText(e.StackTrace);
				}
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
					s.Write("\t\t\t<head>" + armorHead + "</head>\n");
					s.Write("\t\t\t<chest>" + armorChest + "</chest>\n");
					s.Write("\t\t\t<legs>" + armorLegs + "</legs>\n");
					s.Write("\t\t</armor>\n");
					s.Write("\t\t<stats>\n");
					s.Write("\t\t\t<maxLife>" + maxLife + "</maxLife>\n");
					s.Write("\t\t\t<maxMana>" + maxMana + "</maxMana>\n");
					s.Write("\t\t</stats>\n");
					s.Write("\t\t<weapons>\n");

					foreach (KeyValuePair<Tuple<int, string>, WeaponLog> p in weaponLogs)
					{
						s.Write("\t\t\t<weapon>\n");
						s.Write("\t\t\t\t<id>" + p.Value.weaponId + "</id>\n");
						s.Write("\t\t\t\t<damage>" + p.Value.damageDealt + "</damage>\n");
						s.Write("\t\t\t\t<type>" + p.Value.type + "</type>\n");
						s.Write("\t\t\t</weapon>\n");
					}

					s.Write("\t\t</weapons>\n");
					s.Write("\t</player>\n");
					s.Write("\t<boss>\n");
					s.Write("\t\t<id>" + boss.type + "</id>\n");
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

		private class WeaponLog
		{
			public int weaponId;
			public int damageDealt = 0;
			public string type;
			public bool valid = false;

			public WeaponLog(int id, string type)
			{
				weaponId = id;
				this.type = type;
				valid = true;
			}
		}

		public CombatTracker()
		{
			// Intentionally empty
		}

		public bool bossActive()
		{
			return currentLog != null;
		}

		public bool registerBoss(NPC boss)
		{
			if (boss != null)
			{
				if (currentLog != null)
				{
					Main.NewText("Cannot track multiple bosses at once (failed to register " + boss.displayName + ")");
					return false;
				}

				currentLog = new CombatLog(Main.player[Main.selectedPlayer], boss);
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

		public void recordHit(Player player, NPC npc, Item item, int damage)
		{
			if (player.whoAmI == Main.selectedPlayer)
				recordHit(npc, "item", item.type, damage);
		}

		public void recordHit(Player player, NPC npc, Projectile projectile, int damage)
		{
			if (player.whoAmI == Main.selectedPlayer)
				recordHit(npc, "projectile", projectile.type, damage);
		}

		private void recordHit(NPC npc, string type, int id, int damage)
		{
			if (currentLog != null && Utilities.isSupportedBoss(npc.type))
			{
				currentLog.recordDamage(npc, type, id, damage);

				if (currentLog.boss.life <= 0)
					recordBossDeath();
			}
		}
	}
}
