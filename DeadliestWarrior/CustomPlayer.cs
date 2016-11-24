using Terraria.ModLoader;

namespace DeadliestWarrior
{
	class CustomPlayer : ModPlayer
	{
		public override void Kill(double damage, int hitDirection, bool pvp, string deathText)
		{
			base.Kill(damage, hitDirection, pvp, deathText);

			CombatTracker._instance.recordPlayerDeath();
		}
	}
}
