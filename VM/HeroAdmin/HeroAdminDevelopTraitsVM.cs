using CharacterReload.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminDevelopTraitsVM: ViewModel
    {
        private HeroAdminCharacter _hero;
        MBBindingList<HeroAdminDevelopTraitItemVM> _traits;

        public HeroAdminDevelopTraitsVM(HeroAdminCharacter hero)
        {
            this._hero = hero;
            this._traits = new MBBindingList<HeroAdminDevelopTraitItemVM>();
            RefreshTraits();
        }

        private void RefreshTraits()
        {
            this._traits.Clear();
            int k = 0;
            foreach (TraitObject trait in TraitObject.All)
            {
                k++;
                int level = this._hero.GetTraitLevel(trait);
                this._traits.Add(new HeroAdminDevelopTraitItemVM(trait, level, this.OnTraitChange));
                if (k == 12)
                {
                    break;
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<HeroAdminDevelopTraitItemVM> Traits
        {
            get
            {
                return this._traits;
            }
            set
            {
                if (value != this._traits)
                {
                    this._traits = value;
                    base.OnPropertyChangedWithValue(value, "Traits");
                }
            }
        }

        
        private void OnTraitChange(TraitObject trait, int newLevel)
        {
            this._hero.SetTraitLevel(trait, newLevel);
        }
    }
}
