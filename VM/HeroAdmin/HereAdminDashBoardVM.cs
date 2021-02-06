using CharacterReload.Data;
using CharacterReload.VM.HeroAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CharacterReload.VM.HeroAdmin
{
    class HereAdminDashBoardVM : ViewModel
    {
        Hero _hero;
        HeroAdminCharacter _heroAdminCharacter;

        HeroAdminCharacterVM _heroAdminCharacterVM;
        HeroAdminDevelopVM _heroAdminDevelopVM;
        HeroAdminRecordVM _heroAdminRecordVM;
        HeroAdminHeroSelectorVM _HeroAdminHeroSelectorVM;
        private readonly Action _closeHereAdminDashBoard;

        public HereAdminDashBoardVM(Hero hero, Action closeHereAdminDashBoard)
        {
            this._HeroAdminHeroSelectorVM = new HeroAdminHeroSelectorVM(hero, OnHeroSelected);
            this._closeHereAdminDashBoard = closeHereAdminDashBoard;
            this._hero = hero;
            this._heroAdminCharacter = HeroAdminCharacter.FromHero(hero);
            this._heroAdminCharacterVM = new HeroAdminCharacterVM(hero.Name.ToString(), hero.Level);
            this._heroAdminRecordVM = new HeroAdminRecordVM(this._heroAdminCharacter, this.OnToLoadHeroCharacter);
            this._heroAdminDevelopVM = new HeroAdminDevelopVM(this._heroAdminCharacter, OnResetLevelAction);

            this._heroAdminCharacterVM.FillFrom(hero.BodyProperties, hero.CharacterObject.FirstBattleEquipment, hero.Culture, hero.IsFemale);
        }


        private void OnResetLevelAction(int level)
        {
            this._heroAdminCharacterVM.RefreshHeroLevel(level);
        }

        private void OnHeroSelected(Hero hero)
        {
            this._hero = hero;
            this._heroAdminCharacter = HeroAdminCharacter.FromHero(hero);
            this._heroAdminCharacterVM.DisplayerHeroName = hero.Name.ToString();
            this._heroAdminCharacterVM.RefreshHeroLevel(this._heroAdminCharacter.Level);
            this._heroAdminCharacterVM.FillFrom(hero.BodyProperties, hero.CharacterObject.FirstBattleEquipment, hero.Culture, hero.IsFemale);
            this._heroAdminRecordVM.UpdateHeroAdminCharacter(this._heroAdminCharacter);
            ResetData();

        }

        private void OnToLoadHeroCharacter(HeroAdminCharacter data, bool include)
        {
            if (include)
            {
                this._heroAdminCharacter = data;
            }
            else
            {
                string tmp = this._heroAdminCharacter.BodyPropertiesString;
                bool isFemale = this._heroAdminCharacter.IsFemale;
                this._heroAdminCharacter = data;
                this._heroAdminCharacter.BodyPropertiesString = tmp;
                this._heroAdminCharacter.IsFemale = isFemale;
            }

            //this._heroAdminCharacterVM.RefreshHeroData();
            BodyProperties bodyProperties = BodyProperties.Default;
            BodyProperties.FromString(this._heroAdminCharacter.BodyPropertiesString, out bodyProperties);
            this._heroAdminCharacterVM.FillFrom(bodyProperties, this._hero.CharacterObject.FirstBattleEquipment, this._hero.Culture, this._heroAdminCharacter.IsFemale);
            ResetData();
        }


        [DataSourceProperty]
        public string DisplayerHeroName
        {
            get
            {
                return this._hero.Name.ToString();
            }
        }

        [DataSourceProperty]
        public HeroAdminHeroSelectorVM HeroSelector
        {
            get
            {
                return this._HeroAdminHeroSelectorVM;
            }
        }

        [DataSourceProperty]
        public HeroAdminCharacterVM HeroCharacter
        {
            get
            {
                return this._heroAdminCharacterVM;
            }
        }

        [DataSourceProperty]
        public HeroAdminDevelopVM HeronDevelop
        {
            get
            {
                return this._heroAdminDevelopVM;
            }
        }

        [DataSourceProperty]
        public HeroAdminRecordVM HeroAdminRecord
        {
            get
            {
                return this._heroAdminRecordVM;
            }
        }

        [DataSourceProperty]
        public string DoneLbl
        {
            get
            {
                return GameTexts.FindText("str_done", null).ToString();
            }

        }

        [DataSourceProperty]
        public string CancelLbl
        {
            get
            {
                return GameTexts.FindText("str_cancel", null).ToString();
            }

        }

        [DataSourceProperty]
        public HintViewModel ResetHint
        {
            get
            {
                return new HintViewModel(GameTexts.FindText("str_reset", null), null);
            }

        }

        [DataSourceProperty]
        public string HeroAdminText
        {
            get
            {
                return new TextObject("{=cr_hero_admin}Edit Hero").ToString();

            }

        }

        private void ExecuteCancel()
        {

            if (null != this._closeHereAdminDashBoard) this._closeHereAdminDashBoard();
        }

        private void ExecuteDone()
        {
            this._heroAdminCharacter.ToHero(this._hero);
            if (null != this._closeHereAdminDashBoard) this._closeHereAdminDashBoard();
        }


        public void ExecuteReset()
        {
            this._heroAdminCharacter = HeroAdminCharacter.FromHero(this._hero);

            ///this._heroAdminCharacterVM = new HeroAdminCharacterVM(this._hero);

            ResetData();
        }




        private void ResetData()
        {
            this._heroAdminDevelopVM = new HeroAdminDevelopVM(_heroAdminCharacter, OnResetLevelAction);
            this._heroAdminCharacterVM.RefreshHeroLevel(_heroAdminCharacter.Level);
            base.OnPropertyChanged("HeronDevelop");
        }

        private void ShowComfirDialog(TextObject tip, Action action)
        {
            InformationManager.ShowInquiry(new InquiryData(tip.ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), action, () => { }), false);

        }

    }
}