using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminHeroSelectorVM: ViewModel
    {
        MBBindingList<HeroAdminHeroSelectorItemVM> _heros;

        private Clan currentclan;
        private bool _isVisible;
        Action<Hero> _onSelectedHeroAction;

        public HeroAdminHeroSelectorVM(Hero currentHero, Action<Hero> onSelectedHeroAction)
        {
            this._onSelectedHeroAction = onSelectedHeroAction;
            this._heros = new MBBindingList<HeroAdminHeroSelectorItemVM>();
            if(null == currentHero.Clan)
            {
                this.currentclan = Clan.PlayerClan;
            }
            else
            {
                this.currentclan = currentHero.Clan;
            }

            RefreshHeros();
        }

        [DataSourceProperty]
        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }
            set
            {
                if (value != this._isVisible)
                {
                    this._isVisible = value;
                    base.OnPropertyChangedWithValue(value, "IsVisible");
                }
            }
        }

        [DataSourceProperty]
        public string SelectHeroText
        {
            get
            {
                return new TextObject("{=cr_Select_Hero}Select Hero", null).ToString();
            }
        }

        [DataSourceProperty]
        public MBBindingList<HeroAdminHeroSelectorItemVM> Heros
        {
            get
            {
                return this._heros;
            }
            set
            {
                if (value != this._heros)
                {
                    this._heros = value;
                    base.OnPropertyChangedWithValue(value, "Heros");
                }
            }
        }

        private void RefreshHeros()
        {
            List<Hero> list = this.currentclan.Heroes.ToList();
            this.Heros.Clear();
            foreach (Hero hero in list)
            { 
                if (hero.Age >= Campaign.Current.Models.AgeModel.HeroComesOfAge )
                {
                    this.Heros.Add(new HeroAdminHeroSelectorItemVM(hero, OnSelectedHero));
                }
                
            }
            
        }

        private void OnSelectedHero(HeroAdminHeroSelectorItemVM heroSelectorItemVM)
        {
            this._isVisible = false;
            this._onSelectedHeroAction(heroSelectorItemVM.Hero);
        }


    }
}
