using CharacterReload.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private HeroAdminCharacter _hero;
        MBBindingList<HeroAdminAttributeItemVM> _attributes; //成长属性

        public HeroAdminDevelopAttributeVM(HeroAdminCharacter hero)
        {
            this._hero = hero;
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

        [DataSourceProperty]
        public string ResetLevelText
        {
            get
            {
                return new TextObject("{=bottom_ReLevelHero}ResetLevel", null).ToString();
            }
        }

        public void RefreshAttribute()
        {
            this.Attributes.Clear();
            for (int i = 0; i < 6; i++)
            {
                this.Attributes.Add(new HeroAdminAttributeItemVM((CharacterAttributesEnum)i, this._hero.GetAttributeValue((CharacterAttributesEnum)i), this.OnAttributeChange));
            }
        }



        public void ReLevel(Hero hero)
        {
            hero.HeroDeveloper.ClearDeveloper();
            hero.ClearSkills();
            hero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Bow, (int)60);
            hero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Riding, (int)60);
            hero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Trade, (int)60);
            hero.HeroDeveloper.ChangeSkillLevel(DefaultSkills.Steward, (int)60);
            hero.Initialize();
        }

        public void DoRefleshLevel()
        {

            TextObject textObject = new TextObject("{=misc_cr_DoRefleshLevel}The hero's level have been reset", null);
            //this.ShowComfirDialog(textObject, () => ReLevel(HeroAdminCharacter _hero));
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=tips_cr_DoRefleshLevel}After reset the hero’s Level, you need to close the clan screen and reopen it to take effect!", null).ToString()));
        }

        private void ShowComfirDialog(TextObject tip, Action action)
        {
            InformationManager.ShowInquiry(new InquiryData(tip.ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), action, () => { }), false);

        }

        public void OnAttributeChange(CharacterAttributesEnum attribute, int newValue)
        {
            this._hero.SetAttributeValue(attribute, newValue);
        }

    }
}
