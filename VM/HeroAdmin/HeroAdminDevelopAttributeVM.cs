using CharacterReload.Data;
using CharacterReload.Screen.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminDevelopAttributeVM : ViewModel
    {
        private HeroAdminCharacter hero;

        private int Level { get; set; }

        MBBindingList<HeroAdminAttributeItemVM> _attributes; //成长属性

        public HeroAdminDevelopAttributeVM(HeroAdminCharacter hero)
        {
            this.hero = hero;
            this.Level = hero.Level;
            this._attributes = new MBBindingList<HeroAdminAttributeItemVM>();
            
            RefreshAttribute();
        }

      
        [DataSourceProperty]
        public MBBindingList<HeroAdminAttributeItemVM> Attributes
        {
            get
            {
                return this._attributes;
            }
            set
            {
                if (value != this._attributes)
                {
                    this._attributes = value;
                    base.OnPropertyChangedWithValue(value, "Attributes");
                }
            }
        }



        //public void DoRefleshLevel()
        //{
        //    TextObject textObject = new TextObject("{=misc_cr_DoRefleshLevel}The hero's level have been reset", null);
        //    this.ShowComfirDialog(textObject, () => {
        //        this.hero.ReLevel();
        //    });
        //    InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=tips_cr_DoRefleshLevel}After reset the hero’s Level, you need to close the clan screen and reopen it to take effect!", null).ToString()));

        //}

        public void RefreshAttribute()
        {
            this.Attributes.Clear();
            this.Attributes.Add(new HeroAdminAttributeItemVM(DefaultCharacterAttributes.Vigor, this.hero.GetAttributeValue(DefaultCharacterAttributes.Vigor), this.OnAttributeChange));
            this.Attributes.Add(new HeroAdminAttributeItemVM(DefaultCharacterAttributes.Control, this.hero.GetAttributeValue(DefaultCharacterAttributes.Control), this.OnAttributeChange));
            this.Attributes.Add(new HeroAdminAttributeItemVM(DefaultCharacterAttributes.Endurance, this.hero.GetAttributeValue(DefaultCharacterAttributes.Endurance), this.OnAttributeChange));

            this.Attributes.Add(new HeroAdminAttributeItemVM(DefaultCharacterAttributes.Cunning, this.hero.GetAttributeValue(DefaultCharacterAttributes.Cunning), this.OnAttributeChange));
            this.Attributes.Add(new HeroAdminAttributeItemVM(DefaultCharacterAttributes.Social, this.hero.GetAttributeValue(DefaultCharacterAttributes.Social), this.OnAttributeChange));
            this.Attributes.Add(new HeroAdminAttributeItemVM(DefaultCharacterAttributes.Intelligence, this.hero.GetAttributeValue(DefaultCharacterAttributes.Intelligence), this.OnAttributeChange));
        }



        public void OnAttributeChange(CharacterAttribute attribute, int newValue)
        {
            this.hero.SetAttributeValue(attribute, newValue);
        }


    }
}
