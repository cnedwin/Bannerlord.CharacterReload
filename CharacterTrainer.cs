using System;
using System.IO;
using System.Reflection;
using SandBox.GauntletUI;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CharacterTrainer
{
	public class SubModule : MBSubModuleBase
	{
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			base.OnGameStart(game, gameStarterObject);
			if (!Directory.Exists(Helper.SavePath))
			{
				Directory.CreateDirectory(Helper.SavePath);
			}
			Helper.ClearLog();
			Helper.Log("Initialize CharacterTrainer v1.0.12");
			if (!(game.GameType is Campaign))
			{
				Helper.Log("GameType is not Campaign. CharacterTrainer disabled.");
				return;
			}
			Helper.Log("Set character stats model");
			gameStarterObject.AddModel(this.statsModel = new CharacterTrainerStatsModel());
			Helper.Log("Register EncyclopediaPageChangedEvent");
			game.EventManager.RegisterEvent<EncyclopediaPageChangedEvent>(delegate (EncyclopediaPageChangedEvent e)
			{
				EncyclopediaData.EncyclopediaPages newPage = e.NewPage;
				if (newPage != EncyclopediaData.EncyclopediaPages.Hero)
				{
					this.selectedHeroPage = null;
					this.selectedHero = null;
					if (this.gauntletLayerTopScreen != null && this.gauntletLayer != null)
					{
						this.gauntletLayerTopScreen.RemoveLayer(this.gauntletLayer);
						if (this.gauntletMovie != null)
						{
							this.gauntletLayer.ReleaseMovie(this.gauntletMovie);
						}
						this.gauntletLayerTopScreen = null;
						this.gauntletMovie = null;
					}
					return;
				}
				GauntletEncyclopediaScreenManager gauntletEncyclopediaScreenManager = MapScreen.Instance.EncyclopediaScreenManager as GauntletEncyclopediaScreenManager;
				if (gauntletEncyclopediaScreenManager == null)
				{
					Helper.Log("gauntletScreenManager is null");
					Helper.Log(MapScreen.Instance.EncyclopediaScreenManager.GetType().ToString());
					return;
				}
				FieldInfo field = typeof(GauntletEncyclopediaScreenManager).GetField("_encyclopediaData", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field2 = typeof(EncyclopediaData).GetField("_activeDatasource", BindingFlags.Instance | BindingFlags.NonPublic);
				EncyclopediaData encyclopediaData = (EncyclopediaData)field.GetValue(gauntletEncyclopediaScreenManager);
				Helper.Log("encyclopediaData " + encyclopediaData.GetType().ToString());
				EncyclopediaPageVM encyclopediaPageVM = (EncyclopediaPageVM)field2.GetValue(encyclopediaData);
				Helper.Log("activeDataSource " + encyclopediaPageVM.GetType().ToString());
				this.selectedHeroPage = (encyclopediaPageVM as EncyclopediaHeroPageVM);
				if (this.selectedHeroPage == null)
				{
					Helper.Log("selectedHeroPage is null");
					return;
				}
				this.selectedHero = (this.selectedHeroPage.Obj as Hero);
				if (this.selectedHero == null)
				{
					Helper.Log("selectedHeroPage.Obj is null");
				}
				if (this.gauntletLayer == null)
				{
					this.gauntletLayer = new GauntletLayer(211, "GauntletLayer");
				}
				try
				{
					if (this.viewModel == null)
					{
						this.viewModel = new CharacterTrainerViewModel(this.statsModel, delegate (Hero exportedHero)
						{
							InformationManager.DisplayMessage(new InformationMessage("Character exported to " + Helper.GetFilename(exportedHero)));
						}, delegate (Hero importedHero, bool ignoreExp, string error)
						{
							if (!string.IsNullOrEmpty(error))
							{
								InformationManager.DisplayMessage(new InformationMessage(error));
								return;
							}
							this.selectedHeroPage.RefreshValues();
							InformationManager.DisplayMessage(new InformationMessage("Character imported" + (ignoreExp ? " (Exp ignored)" : string.Empty) + " from " + Helper.GetFilename(this.selectedHero)));
						}, delegate (Hero clearedHero)
						{
							InformationManager.DisplayMessage(new InformationMessage("Character unspent points cleared"));
						});
					}
					this.viewModel.SetHero(this.selectedHero);
					Helper.Log("Load EncyclopediaHeroPageCharacterTrainer");
					this.gauntletMovie = this.gauntletLayer.LoadMovie("EncyclopediaHeroPageCharacterTrainer", this.viewModel);
					Helper.Log("Adding to top screen");
					this.gauntletLayerTopScreen = ScreenManager.TopScreen;
					this.gauntletLayerTopScreen.AddLayer(this.gauntletLayer);
					this.gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.MouseButtons);
				}
				catch (Exception ex)
				{
					Helper.Log("Error! " + ex.Message);
					Helper.Log(ex.StackTrace);
				}
			});
			Helper.Log("Initialized successfully");
		}

		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			if (Campaign.Current == null)
			{
				return;
			}
			if (!Campaign.Current.GameStarted)
			{
				return;
			}
			if (!this.statsModel.IsInitialized)
			{
				this.statsModel.Initialize();
			}
		}

		private const string Version = "v1.0.12";

		private CharacterTrainerStatsModel statsModel;

		private CharacterTrainerViewModel viewModel;

		private EncyclopediaHeroPageVM selectedHeroPage;

		private Hero selectedHero;

		private ScreenBase gauntletLayerTopScreen;

		private GauntletLayer gauntletLayer;

		private GauntletMovie gauntletMovie;
	}
}
