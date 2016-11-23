using Terraria;
using Terraria.ModLoader;

namespace DeadliestWarrior
{
	class CustomNPC : GlobalNPC
	{
		public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
		{
			base.OnHitByItem(npc, player, item, damage, knockback, crit);
			CombatTracker._instance.recordHit(npc, item, damage);
		}

		public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
		{
			base.OnHitByProjectile(npc, projectile, damage, knockback, crit);
			CombatTracker._instance.recordHit(npc, projectile, damage);
		}
	}
}