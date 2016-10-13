using Terraria.ModLoader;

namespace DeadliestWarrior
{
    class DeadliestWarrior : Mod
    {
        public DeadliestWarrior()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true,
                AutoloadBackgrounds = true
            };
        }
    }
}
