using Terraria;
using Terraria.ModLoader;

namespace DeadliestWarrior
{
	class CustomNPC : GlobalNPC
	{
		public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
		{
			base.OnHitByItem(npc, player, item, damage, knockback, crit);
			CombatTracker._instance.recordHit(player, npc, item, damage);
		}

		public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
		{
			base.OnHitByProjectile(npc, projectile, damage, knockback, crit);
			CombatTracker._instance.recordHit(Main.player[projectile.owner], npc, projectile, damage);
		}

		public override bool CheckActive(NPC npc)
		{
			if (npc.type == 245) // ensure that Golem can be spawned anywhere
				return false;

			return base.CheckActive(npc);
		}
	}
}