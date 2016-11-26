using Terraria;
using Terraria.ModLoader;

namespace DeadliestWarrior
{
	class CustomNPC : GlobalNPC
	{
		public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
		{
			base.OnHitByItem(npc, player, item, damage, knockback, crit);

			CombatTracker.DamageType damageType = CombatTracker.DamageType.MELEE;
			if (item.magic)
				damageType = CombatTracker.DamageType.MAGIC;
			else if (item.ranged)
				damageType = CombatTracker.DamageType.RANGED;

			CombatTracker._instance.recordHit(player, npc, damageType, damage);
		}

		public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
		{
			base.OnHitByProjectile(npc, projectile, damage, knockback, crit);

			CombatTracker.DamageType damageType = CombatTracker.DamageType.MELEE;
			if (projectile.magic)
				damageType = CombatTracker.DamageType.MAGIC;
			else if (projectile.ranged)
				damageType = CombatTracker.DamageType.RANGED;

			CombatTracker._instance.recordHit(Main.player[projectile.owner], npc, damageType, damage);
		}
	}
}