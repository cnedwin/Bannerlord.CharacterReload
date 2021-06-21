
using CharacterReload.Data;
using CharacterReload.Utils;
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminDevelopSkillPerkVM: ViewModel
    {

        MBBindingList<HeroAdminSkillVM> _skills;


        HeroAdminCharacter _heroAdminCharacter;
        public const int MAX_SKILL_LEVEL = 300;

        Action<int> _onResetLevelAction;

        private HeroAdminSkillVM _currentSkillVM;
      
        public HeroAdminDevelopSkillPerkVM(HeroAdminCharacter heroAdminCharacter, Action<int> onResetLevelAction)
        {
            this._onResetLevelAction = onResetLevelAction;
            this._heroAdminCharacter = heroAdminCharacter;
            this._skills = new MBBindingList<HeroAdminSkillVM>();
            RefreshHeroSkill();
        }


        [DataSourceProperty]
        public string ResetSkillBtnText
        {
            get
            {
                return new TextObject("{=bottom_RePerksHero}Reset Perk").ToString();
            }
        }

        [DataSourceProperty]
        public string ReseFoucsBtnText
        {
            get
            {
                return new TextObject("{=bottom_RefocusHero}Reset Focus").ToString();
            }
        }

        [DataSourceProperty]
        public string ResetLevelText
        {
            get
            {
                return new TextObject("{=bottom_ReLevelHero}Reset Level", null).ToString();
            }
        }

        [DataSourceProperty]
        public MBBindingList<HeroAdminSkillVM> Skills
        {
            get
            {
                return this._skills;
            }
            set
            {
                if (value != this._skills)
                {
                    this._skills = value;
                    base.OnPropertyChangedWithValue(value, "Skills");
                }
            }
        }


        [DataSourceProperty]
        public HeroAdminSkillVM CurrentSkillView
        {
            get
            {
                return this._currentSkillVM ;
            }
            set
            {
                if (value != this._currentSkillVM)
                {
                    this._currentSkillVM = value;
                    base.OnPropertyChangedWithValue(value, "CurrentSkillView");
                }
            }
        }



        public void ExecuteResetSkill()
        {
           
            foreach (SkillObject current in TaleWorlds.CampaignSystem.Skills.All)
            {
                this._heroAdminCharacter.SetSkillValue(current, 0);
            }

           this._heroAdminCharacter.ClearPerks();
            RefreshHeroSkill();
        }

        public void ExecuteResetFouce()
        {
            /// this._hero.HeroDeveloper.ClearFocuses();
            //  ReflectUtils.ReflectMethodAndInvoke("ClearFocuses", this._heroAdminCharacter.HeroDeveloper, new object[] { });
            this._heroAdminCharacter.ClearFocuses();
            RefreshHeroSkill();
        }

        public void DoRefleshLevel()
        {
            this._heroAdminCharacter.ReLevel();
            this._onResetLevelAction(this._heroAdminCharacter.Level);
         
            //InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=tips_cr_DoRefleshLevel}After reset the hero’s Level, you need to close the clan screen and reopen it to take effect!", null).ToString()));
        }

        public void RefreshHeroSkill()
        {
            this.Skills.Clear();
            foreach (SkillObject current in TaleWorlds.CampaignSystem.Skills.All)
            {
                this.Skills.Add(new HeroAdminSkillVM(current, this._heroAdminCharacter, OnSkillSelectedChange));
            }
            this.Skills[0].IsInspected = true;
            this._currentSkillVM = this.Skills[0];
            base.OnPropertyChanged("CurrentSkillView");//通知刷新
        }

        public void OnSkillSelectedChange(HeroAdminSkillVM adminSkillVM)
        {
            if (null != this._currentSkillVM)
            {
                this._currentSkillVM.IsInspected = false;
            }
            this.CurrentSkillView = adminSkillVM;
        }

        private void ShowComfirDialog(TextObject tip, Action action)
        {
            InformationManager.ShowInquiry(new InquiryData(tip.ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), action, () => { }), false);

        }
    }
}
