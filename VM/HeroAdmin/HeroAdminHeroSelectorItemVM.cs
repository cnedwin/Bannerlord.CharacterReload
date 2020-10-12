using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminHeroSelectorItemVM : ViewModel
    {
        private Hero _hero;
        private ImageIdentifierVM _visual;
        private Action<HeroAdminHeroSelectorItemVM> _onSelectedHeroAction;

        public HeroAdminHeroSelectorItemVM(Hero hero, Action<HeroAdminHeroSelectorItemVM> action)
        {
            this._hero = hero;
            this._onSelectedHeroAction = action;
            CharacterCode characterCode = CharacterCode.CreateFrom(hero.CharacterObject);
            this.Visual = new ImageIdentifierVM(characterCode);
        }

        public Hero Hero
        {
            get {
                return this._hero;
            }
         }

        private void ExecuteSelect()
        {
            this._onSelectedHeroAction(this);
        }

        [DataSourceProperty]
        public string DisplayName
        {
            get
            {
                return this._hero.Name.ToString();
            }

        }

        [DataSourceProperty]
        public ImageIdentifierVM Visual
        {
            get
            {
                return this._visual;
            }
            set
            {
                if (value != this._visual)
                {
                    this._visual = value;
                    base.OnPropertyChanged("Visual");
                }
            }
        }
    }
}
