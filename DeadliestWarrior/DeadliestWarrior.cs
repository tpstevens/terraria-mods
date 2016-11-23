using System;

using Terraria;
using Terraria.ModLoader;

namespace DeadliestWarrior
{
	class DeadliestWarrior : Mod
	{
		public DeadliestWarrior()
		{
			Properties = new ModProperties()
			{
				AutoloadGores = true,
				AutoloadSounds = true,
				AutoloadBackgrounds = true
			};
		}

		public override void Load()
		{
			base.Load();

			AddGlobalNPC("custom npc", new CustomNPC());
		}

		public override void ChatInput(string text)
		{
			string cmd;
			string[] args;

			text = text.Trim();
			if (!text.StartsWith("/"))
				return;

			int eoc = text.IndexOf(' ');
			if (eoc <= 0)
			{
				cmd = text.Substring(1).Trim();
				args = new string[0];
			}
			else
			{
				cmd = text.Substring(1, eoc).Trim();
				args = text.Substring(eoc + 1).Split(' ');
			}

			PlayerCommands.execute(cmd, args);
		}
	}
}