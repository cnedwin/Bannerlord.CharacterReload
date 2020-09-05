using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

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