using CharacterReload.Data;
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
    class HeroAdminDevelopVM : ViewModel
    {
        private HeroAdminCharacter _heroAdminCharacter;
        private int _tableSelectedIndex = 0;

        private HeroAdminDevelopSkillPerkVM _skillVM;
        private HeroAdminDevelopTraitsVM _traitVM;
        private HeroAdminDevelopAttributeVM _attributeVM;

        public HeroAdminDevelopVM(HeroAdminCharacter heroAdminCharacter)
        {
            _heroAdminCharacter = heroAdminCharacter;
            this._skillVM = new HeroAdminDevelopSkillPerkVM(heroAdminCharacter);
            this._traitVM = new HeroAdminDevelopTraitsVM(heroAdminCharacter);
            this._attributeVM = new HeroAdminDevelopAttributeVM(heroAdminCharacter);
        }


        [DataSourceProperty]
        public string HeroAdminDevelopSkillPerkText
        {
            get
            {
                return new TextObject("{=cr_hero_skillandperk}Skill And Perk").ToString();
            }

        }

        [DataSourceProperty]
        public string HeroAdminDevelopAttributeText
        {
            get
            {
                return new TextObject("{=cr_hero_attrbute}Attrbutes").ToString();
            }
        }

        [DataSourceProperty]
        public string HeroAdminDevelopTraitsText
        {
            get
            {
                return new TextObject("{=cr_hero_trait}Traits").ToString();

            }
        }

        [DataSourceProperty]
        public HeroAdminDevelopSkillPerkVM SkillView
        {
            get
            {
                return this._skillVM;
            }

        }

        [DataSourceProperty]
        public HeroAdminDevelopAttributeVM AttributeView
        {
            get
            {
                return this._attributeVM;
            }

        }

        [DataSourceProperty]
        public HeroAdminDevelopTraitsVM TraitView
        {
            get
            {
                return this._traitVM;
            }

        }

        [DataSourceProperty]
        public bool IsSkillTableSelected
        {
            get
            {
                return this._tableSelectedIndex == 0;
            }
        }

        [DataSourceProperty]
        public bool IsAttributeTableSelected
        {
            get
            {
                return this._tableSelectedIndex == 1;
            }
        }


        [DataSourceProperty]
        public bool IsTraitTableSelected
        {
            get
            {
                return this._tableSelectedIndex == 2;
            }
        }


        public void SetSelectedCategory(int index)
        {
            if (this._tableSelectedIndex != index)
            {
                this._tableSelectedIndex = index;
                base.OnPropertyChanged("IsSkillTableSelected"); //通知更新
                base.OnPropertyChanged("IsAttributeTableSelected");
                base.OnPropertyChanged("IsTraitTableSelected");
            }
        }


    }
}
