using System;
using System.Reflection;
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
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using Helpers;

namespace CharacterReload.VM
{

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


        // Jiros Add for Grow-Up code -- 
        [DataSourceProperty]
        public string HeroGrowText
        {
            get
            {
                return new TextObject("{=cr_hero_grow}Grow Up").ToString();
            }
        }
        // -- End Jiros Add

        public void ExecuteEdit()
        {
            if (selectedHero == null)
                return;
            if (selectedHero.IsChild)
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
            Action<Hero> action = this.nameCallback;

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

        /* ---- Begin Jiros Adds ---- */

        // ToDo: Right now the grown heroes get some stats as part of coming to age, but they get no specialization. /
        // 1. See how TW deteremines this, and come up with a way to give the hero some specialized skills. 
        // 2. Also see if there's a way to review a Hero's parents and come up with some 'inherited' skills and attributes.
        // 3. Grown hero is level 1. Would it make sense to give the hero a few levels along with the specialization? 

        public void ExecuteGrowUp()
        {
            if (selectedHero == null)
                return;

            if (!selectedHero.IsChild)
                return;
            GrowUp(selectedHero);
        }

        public void GrowUp(Hero hero)
        {
            float age = hero.Age;
            int infant = Campaign.Current.Models.AgeModel.BecomeInfantAge;
            //int child = Campaign.Current.Models.AgeModel.BecomeChildAge;
            int teen = Campaign.Current.Models.AgeModel.BecomeTeenagerAge;
            int adult = Campaign.Current.Models.AgeModel.HeroComesOfAge;

            if (age >= adult)
            {
                Helper.ColorRedMessage("This hero is already an adult.");
                return;
            }

            // Assure hero hits the right growth stages, depending on which stage they were grown from.--
            // Edit: Changed it to that each button press grows the hero by 1 stange.
            // To instantly grow a hero to 18, change the 'else if' functions to just 'if' 

            if (age < infant)
            {
                SendHeroGrowsOutOfInfancyEvent(hero);
                Helper.DebugMessage("Out of infrancy event sent.");
                hero.SetBirthDay(CampaignTime.YearsFromNow((float)infant * -1));
            }

            //if (age < teen)
            else if (age < teen)
            {
                SendHeroReachesTeenAgeEvent(hero);
                Helper.DebugMessage("Out of teen event sent.");
                hero.SetBirthDay(CampaignTime.YearsFromNow((float)teen * -1));
            }


            //if (age < adult)
            else
            {
                SendHeroComesOfAgeEvent(hero);
                Helper.DebugMessage("Come of age event sent.");
                hero.SetBirthDay(CampaignTime.YearsFromNow((float)adult * -1));
                var adulttextObject = new TextObject("{=tips_cr_HeroGrowAdult}Your child is now a qualified hero");
                StringHelpers.SetCharacterProperties("CR_HERO", hero.CharacterObject, null, adulttextObject);

            }


            //Helper.ColorGreenMessage("Grew "+hero.Name+" to Adulthood. Wait a day or two to assure they go through the Come-of-Age process and get stats.");
            Helper.ColorGreenMessage("Grew " + hero.Name + " Wait a day or two to assure they go through the growth process and get added stats/development.");
            Helper.DebugMessage("Hero: " + hero.Name + " | Old Age = " + age + " | New age = " + hero.Age);
        }


        // Borrowed from Pacemaker
        protected void SendHeroGrowsOutOfInfancyEvent(Hero hero) =>
            OnHeroGrowsOutOfInfancyMI.Invoke(CampaignEventDispatcher.Instance, new object[] { hero });

        protected void SendHeroReachesTeenAgeEvent(Hero hero) =>
            OnHeroReachesTeenAgeMI.Invoke(CampaignEventDispatcher.Instance, new object[] { hero });

        protected void SendHeroComesOfAgeEvent(Hero hero)
        {
            if (!hero.IsActive) // This extra check is inherited from the old vanilla code.
                OnHeroComesOfAgeMI.Invoke(CampaignEventDispatcher.Instance, new object[] { hero });
        }

        // Reflection to send these internal-access campaign events. Borrowed from Pacemaker. 
        private static readonly MethodInfo OnHeroComesOfAgeMI = AccessTools.Method(
            typeof(CampaignEventDispatcher), "OnHeroComesOfAge");

        private static readonly MethodInfo OnHeroReachesTeenAgeMI = AccessTools.Method(
            typeof(CampaignEventDispatcher), "OnHeroReachesTeenAge");

        private static readonly MethodInfo OnHeroGrowsOutOfInfancyMI = AccessTools.Method(
            typeof(CampaignEventDispatcher), "OnHeroGrowsOutOfInfancy");

        /* ---- End Jiros Adds ---- */


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
                var newName = new TextObject(heroName);
                selectedHero.Name = newName;
                selectedHero.FirstName = newName;
                if (selectedHero.IsPartyLeader)
                    selectedHero.PartyBelongedTo.Name = MobilePartyHelper.GeneratePartyName(selectedHero.CharacterObject);
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
            FaceGen.ShowDebugValues = true;
            ScreenManager.PushScreen(ViewCreator.CreateMBFaceGeneratorScreen(hero.CharacterObject, false));
        }

        private Hero selectedHero;
        private Action<Hero> editCallback;
        private Action<Hero> nameCallback;
        private EncyclopediaHeroPageVM selectedHeroPage;
    }
}