using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using FaceDetailsCreator;
using System.IO;

namespace CharacterReload
{
    public class SubModule : MBSubModuleBase
    {
	
		protected override void OnSubModuleLoad()
		{
			try
			{
				base.OnSubModuleLoad();
				new Harmony("mod.CharacterReload.cnedwin").PatchAll();

				TaleWorlds.Core.FaceGen.ShowDebugValues = true; // Developer facegen
			}
			catch (Exception)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=misc_cr_loaderro}Error Initialising CharacterReload", null).ToString(), Color.FromUint(ModuleColors.green)));
			}
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=misc_cr_onmapload}Loaded CharacterReload succeeded", null).ToString(), Color.FromUint(ModuleColors.green)));
		}

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			base.OnGameStart(game, gameStarterObject);


			if (!(game.GameType is Campaign))
			{
				Helper.Log("GameType is not Campaign. CharacterTrainer disabled.");
				return;
			}
			if (!(gameStarterObject is CampaignGameStarter))
			{ 
				return;
		    }

			if (!Directory.Exists(Helper.SavePath))
			{
				Directory.CreateDirectory(Helper.SavePath);
			}
			Helper.ClearLog();
			Helper.Log("Set character stats model");

			gameStarterObject.AddModel(CharacterTrainerStatsModel.Instance());

			LoadXMLFiles(gameStarterObject as CampaignGameStarter);
			EncyclopediaPageChangedHandle handle = new EncyclopediaPageChangedHandle();
			game.EventManager.RegisterEvent<EncyclopediaPageChangedEvent>(handle.OnEncyclopediaPageChanged);

		}

		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			if (Campaign.Current != null && Campaign.Current.GameStarted && !CharacterTrainerStatsModel.Instance().IsInitialized)
			{
				CharacterTrainerStatsModel.Instance().Initialize();
			}
		}

		private void LoadXMLFiles(CampaignGameStarter gameInitializer)
		{
			// Load our additional strings
			gameInitializer.LoadGameTexts(BasePath.Name + "Modules/CharacterReload/ModuleData/strings.xml");

		}
	}
	

	public static class ModuleColors
	{
		public static readonly uint modMainColor = 14906114U;

		public static readonly uint green = 4282569842U;

		public static readonly uint red = 12517376U;

		public static readonly uint grey = 14606046U;

		public static readonly uint yellow = 15120402U;
	}
}