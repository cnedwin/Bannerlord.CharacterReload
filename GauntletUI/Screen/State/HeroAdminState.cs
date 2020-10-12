using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CharacterReload.Screen.State
{
    class HeroAdminState : GameState
    {

		public Hero EditHero { set; get; }

		public HeroAdminState()
		{
		}

		public HeroAdminState(Hero hero)
		{
			this.EditHero = hero;

		}

		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}
	}
}
