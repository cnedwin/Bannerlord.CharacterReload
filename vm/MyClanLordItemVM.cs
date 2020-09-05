using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CharacterReload.View
{
    public class MyClanLordItemVM : ClanLordItemVM
    {
        Action<ClanLordItemVM> _characterSelect;
        public MyClanLordItemVM(Hero hero, Action<ClanLordItemVM> onCharacterSelect) : base(hero, onCharacterSelect)
        {
            this._characterSelect = onCharacterSelect;
        }
        private void OnCharacterSelect()
        {

            this._characterSelect(this);
        }

        private void ExecuteLocationLink(string link)
        {
            Campaign.Current.EncyclopediaManager.GoToLink(link);
        }

        private void ExecuteLink()
        {
            Campaign.Current.EncyclopediaManager.GoToLink(this.GetHero().EncyclopediaLink);
        }


        private void ExecuteRename()
        {
            InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=2lFwF07j}Change Name", null).ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action<string>(this.OnNamingHeroOver), null, false, new Func<string, bool>(CampaignUIHelper.IsStringApplicableForHeroName), ""), false);
        }

        private void OnNamingHeroOver(string suggestedName)
        {
            if (CampaignUIHelper.IsStringApplicableForHeroName(suggestedName))
            {
                this.GetHero().CharacterObject.Name = new TextObject(suggestedName, null);
                this.Name = suggestedName;
            }
        }

        public void DoRefleshAPoint()
        {
            TextObject textObject = new TextObject("{=misc_cr_DoRefleshAPoint}The hero's attribute points have been reset", null);
           // InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
            
            this.ShowComfirDialog(textObject, () => ReAttHero(GetHero()));

        }

        public void DoRefleshFPoint()
        {
            TextObject textObject = new TextObject("{=misc_cr_DoRefleshFPoint}The hero's specialization point has been reset", null);
            //   InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
            this.ShowComfirDialog(textObject, () => RefocusHero(GetHero()));
         
        }

        public void DoRefleshPerks()
        {
            TextObject textObject = new TextObject("{=misc_cr_DoRefleshPerk}The hero's skill tree has been reset", null);
            //  InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
            this.ShowComfirDialog(textObject, () => RePerksHero(GetHero()));
           
        }

        private void ShowComfirDialog(TextObject tip, Action action)
        {
            InformationManager.ShowInquiry(new InquiryData(tip.ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), action, () => { }), false);

        }

        [DataSourceProperty]
        public string ResetAttributeText
        {
            get
            {
                return new TextObject("{=bottom_ReAttHero}ResetAttribute", null).ToString();
            }
        }

        [DataSourceProperty]
        public string ResetFocusText
        {
            get
            {
                return new TextObject("{=bottom_RefocusHero}ResetFocus", null).ToString();
            }
        }

        [DataSourceProperty]
        public string ResetPerkText
        {
            get
            {
                return new TextObject("{=bottom_RePerksHero}ResetPerk", null).ToString();
            }
        }


        private  void RePerksHero(Hero hero)
        {
            foreach (SkillObject skill in DefaultSkills.GetAllSkills())
            {
                hero.HeroDeveloper.TakeAllPerks(skill);
            }
            hero.ClearPerks();
        }

        public  void RefocusHero(Hero hero)
        {
            int num = 0;
            int num2 = 0;
            foreach (SkillObject skill in DefaultSkills.GetAllSkills())
            {
                int focus = hero.HeroDeveloper.GetFocus(skill);
                if (focus > 0)
                {
                    num += focus;
                    hero.HeroDeveloper.AddFocus(skill, num2 - focus, false);
                }
            }
            hero.HeroDeveloper.UnspentFocusPoints += MBMath.ClampInt(num, 0, 999);

        }


        public  void ReAttHero(Hero hero)
        {
            int num = 0;
            for (CharacterAttributesEnum characterAttributesEnum = CharacterAttributesEnum.Vigor; characterAttributesEnum < CharacterAttributesEnum.NumCharacterAttributes; characterAttributesEnum++)
            {
                int attributeValue = hero.GetAttributeValue(characterAttributesEnum);
                num += attributeValue;
                hero.SetAttributeValue(characterAttributesEnum, 0);
            }
            hero.HeroDeveloper.UnspentAttributePoints += MBMath.ClampInt(num, 0, 999);
        }

    }

 
}

