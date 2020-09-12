using CharacterCreation.Models;
using FaceDetailsCreator.utils;
using SandBox.GauntletUI;
using SandBox.View.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FaceDetailsCreator
{
    class EncyclopediaPageChangedHandle
    {

        private HeroBuilderVM viewModel;
        private EncyclopediaHeroPageVM selectedHeroPage;
        private Hero selectedHero;
        private ScreenBase gauntletLayerTopScreen;
        private GauntletLayer gauntletLayer;
        private GauntletMovie gauntletMovie;

        public void OnEncyclopediaPageChanged(EncyclopediaPageChangedEvent e)
        {
            EncyclopediaData.EncyclopediaPages newPage = e.NewPage;
            if ((int)newPage != 12)
            {
                selectedHeroPage = null;
                selectedHero = null;
                if (gauntletLayerTopScreen != null && gauntletLayer != null)
                {
                    gauntletLayerTopScreen.RemoveLayer(gauntletLayer);
                    if (gauntletMovie != null)
                    {
                        gauntletLayer.ReleaseMovie(gauntletMovie);
                    }
                    gauntletLayerTopScreen = null;
                    gauntletMovie = null;
                }
                return;
            }
            GauntletEncyclopediaScreenManager gauntletEncyclopediaScreenManager = MapScreen.Instance.EncyclopediaScreenManager as GauntletEncyclopediaScreenManager;
            if (gauntletEncyclopediaScreenManager == null)
            {
                return;
            }

            EncyclopediaData encyclopediaData = (EncyclopediaData)ReflectUtils.ReflectField("_encyclopediaData", gauntletEncyclopediaScreenManager);
              //  AccessTools.Field(typeof(GauntletEncyclopediaScreenManager), ).GetValue(gauntletEncyclopediaScreenManager) as EncyclopediaData;
            EncyclopediaPageVM encyclopediaPageVM = (EncyclopediaPageVM)ReflectUtils.ReflectField("_activeDatasource", encyclopediaData);
            //AccessTools.Field(typeof(EncyclopediaData), "_activeDatasource").GetValue(encyclopediaData) as EncyclopediaPageVM;
            selectedHeroPage = (encyclopediaPageVM as EncyclopediaHeroPageVM);

            if (selectedHeroPage == null)
            {
                return;
            }
            selectedHero = (selectedHeroPage.Obj as Hero);
            if (selectedHero == null)
            {
                return;
            }
            if (gauntletLayer == null)
            {
                gauntletLayer = new GauntletLayer(211, "GauntletLayer");
            }

            try
            {
                if (viewModel == null)
                {
                    viewModel = new HeroBuilderVM(delegate (Hero editHero)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=CharacterCreation_EditAppearanceForHeroMessage}Entering edit appearance for: ").ToString() + editHero));
                    });
                }
                viewModel.SetHero(selectedHero);
                gauntletMovie = gauntletLayer.LoadMovie("HeroEditor", viewModel);
                gauntletLayerTopScreen = ScreenManager.TopScreen;
                gauntletLayerTopScreen.AddLayer(gauntletLayer);
                gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.MouseButtons);

                // Refresh
                selectedHeroPage.Refresh();
            }
            catch (Exception ex)
            {
               // MessageBox.Show($"Error :\n{ex.Message} \n\n{ex.InnerException?.Message}");
            }
        }
    }
}
