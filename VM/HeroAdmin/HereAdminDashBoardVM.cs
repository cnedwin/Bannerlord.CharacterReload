
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
            this._heroAdminCharacterVM = new HeroAdminCharacterVM(hero.Name.ToString());
            this._heroAdminRecordVM = new HeroAdminRecordVM(this._heroAdminCharacter, this.OnToLoadHeroCharacter);
            this._heroAdminDevelopVM = new HeroAdminDevelopVM(this._heroAdminCharacter);

            this._heroAdminCharacterVM.FillFrom(hero.BodyProperties, hero.CharacterObject.FirstCivilianEquipment, hero.Culture, hero.IsFemale);
        }


        private void OnHeroSelected(Hero hero)
        {
            this._heroAdminCharacter = HeroAdminCharacter.FromHero(hero);
            this._heroAdminCharacterVM.DisplayerHeroName = hero.Name.ToString();
            this._heroAdminCharacterVM.FillFrom(hero.BodyProperties, hero.CharacterObject.FirstCivilianEquipment, hero.Culture, hero.IsFemale);
            ResetData();
        }

        private void OnToLoadHeroCharacter(HeroAdminCharacter data)
        {
            this._heroAdminCharacter = data;
            //this._heroAdminCharacterVM.RefreshHeroData();
            BodyProperties bodyProperties = BodyProperties.Default;
            BodyProperties.FromString(data.BodyPropertiesString, out bodyProperties);
            this._heroAdminCharacterVM.FillFrom(bodyProperties, this._hero.CharacterObject.FirstCivilianEquipment, this._hero.Culture, this._hero.IsFemale);

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
                return new HintViewModel(GameTexts.FindText("str_reset", null).ToString(), null); ;
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
            this._heroAdminDevelopVM = new HeroAdminDevelopVM(_heroAdminCharacter);
            base.OnPropertyChanged("HeronDevelop");
        }


    }
}
