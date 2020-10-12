using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;
using SandBox.GauntletUI;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using HarmonyLib;
using TaleWorlds.MountAndBlade.LegacyGUI.Missions;
using CharacterReload.Screen.State;

namespace CharacterReload.VM
{

    // Fixed 固定宽高， CoverChildren 自适应（以子控件） StretchToParent（以父控件做参考）
    public partial class HeroBuilderVM : ViewModel
    {

        public void SetHero(Hero hero)
        {
            selectedHero = hero;
        }

        public HeroBuilderVM(Action<Hero> editCallback)
        {
            this.editCallback = editCallback;
        }

        [DataSourceProperty]
        public string EditAppearanceText
        {
            get
            {
                return new TextObject("{=cr_edit_appearance}Edit Appearance").ToString();
            }
        }

        [DataSourceProperty]
        public string ChangeNameText
        {
            get
            {
                return new TextObject("{=cr_change_name}Change Name").ToString();
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



        public void ExecuteEdit()
        {
            if (selectedHero == null)
                return;

            Edit(selectedHero);
            Action<Hero> action = this.editCallback;
            if (action == null)
                return;

            action(selectedHero);
        }

        public void ExecuteName()
        {
            if (selectedHero == null)
                return;

            Name(selectedHero);
            Action<Hero> action = nameCallback;

            if (action == null)
                return;

            action(selectedHero);
        }

        /**
         * 跳转英雄管理界面
         */
        public void ExecuteHeroAdmin()
        {
            if (selectedHero == null)
                return;

            //界面跳转，state 定位某个screen 进行跳转
            HeroAdminState state = Game.Current.GameStateManager.CreateState<HeroAdminState>(new object[]
                 {
                    this.selectedHero,
                 });
            Game.Current.GameStateManager.PushState(state, 0);
        }

        public void Name(Hero hero)
        {
            if (hero.CharacterObject == null)
                return;

            InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=CharacterCreation_CharacterRenamerText}Character Renamer").ToString(), new TextObject("{=CharacterCreation_EnterNewNameText}Enter a new name").ToString(),
               true, true, new TextObject("{=CharacterCreation_RenameText}Rename").ToString(), new TextObject("{=CharacterCreation_CancelText}Cancel").ToString(), new Action<string>(RenameHero), InformationManager.HideInquiry, false));
        }

        private void RenameHero(string heroName)
        {
            if (selectedHero.CharacterObject == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(heroName))
            {
                selectedHero.Name = new TextObject(heroName);
                ClosePage();
            }
            else
            {
                return;
            }
        }

        public void RefreshPage()
        {
            GauntletEncyclopediaScreenManager gauntletEncyclopediaScreenManager = MapScreen.Instance.EncyclopediaScreenManager as GauntletEncyclopediaScreenManager;
            if (gauntletEncyclopediaScreenManager == null)
                return;

            EncyclopediaData encyclopediaData = AccessTools.Field(typeof(GauntletEncyclopediaScreenManager), "_encyclopediaData").GetValue(gauntletEncyclopediaScreenManager) as EncyclopediaData;
            EncyclopediaPageVM encyclopediaPageVM = AccessTools.Field(typeof(EncyclopediaData), "_activeDatasource").GetValue(encyclopediaData) as EncyclopediaPageVM;

            this.selectedHeroPage = (encyclopediaPageVM as EncyclopediaHeroPageVM);

            if (this.selectedHeroPage == null)
                return;

            this.selectedHeroPage.Refresh();
        }

        public void ClosePage()
        {
            GauntletEncyclopediaScreenManager gauntletEncyclopediaScreenManager = MapScreen.Instance.EncyclopediaScreenManager as GauntletEncyclopediaScreenManager;
            if (gauntletEncyclopediaScreenManager == null)
                return;

            EncyclopediaData encyclopediaData = AccessTools.Field(typeof(GauntletEncyclopediaScreenManager), "_encyclopediaData").GetValue(gauntletEncyclopediaScreenManager) as EncyclopediaData;
            EncyclopediaPageVM encyclopediaPageVM = AccessTools.Field(typeof(EncyclopediaData), "_activeDatasource").GetValue(encyclopediaData) as EncyclopediaPageVM;

            this.selectedHeroPage = (encyclopediaPageVM as EncyclopediaHeroPageVM);

            if (this.selectedHeroPage == null)
                return;

            gauntletEncyclopediaScreenManager.CloseEncyclopedia();
        }

        public void Edit(Hero hero)
        {
            if (hero.CharacterObject == null)
                return;

            ClosePage();
            TaleWorlds.Core.FaceGen.ShowDebugValues = true;
            ScreenManager.PushScreen(ViewCreator.CreateMBFaceGeneratorScreen(hero.CharacterObject, false));
        }

        private Hero selectedHero;
        private Action<Hero> editCallback;
        private Action<Hero> nameCallback;
        private EncyclopediaHeroPageVM selectedHeroPage;
    }
}
