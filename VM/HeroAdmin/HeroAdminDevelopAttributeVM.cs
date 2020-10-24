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
            for (int i = 0; i < 6; i++)
            {
                this.Attributes.Add(new HeroAdminAttributeItemVM((CharacterAttributesEnum)i, this.hero.GetAttributeValue((CharacterAttributesEnum)i), this.OnAttributeChange));
            }
        }



        public void OnAttributeChange(CharacterAttributesEnum attribute, int newValue)
        {
            this.hero.SetAttributeValue(attribute, newValue);
        }


    }
}
