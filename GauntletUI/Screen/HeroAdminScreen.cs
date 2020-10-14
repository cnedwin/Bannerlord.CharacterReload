using CharacterReload.Screen.State;
using CharacterReload.VM;
using CharacterReload.VM.HeroAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View.Screen;
using TaleWorlds.TwoDimension;

namespace CharacterReload.Screen
{
	[GameStateScreen(typeof(HeroAdminState))]
	class HeroAdminScreen : ScreenBase, IGameStateListener
	{
		private GauntletLayer _gauntletLayer;
		///private SpriteCategory _clanCategory;
		private SpriteCategory _clanCategory;

		private SpriteCategory _characterdeveloper;

		private HereAdminDashBoardVM _dataSource;

		HeroAdminState _heroAdminState;

		public HeroAdminScreen(HeroAdminState heroAdminState)
		{
			LoadingWindow.EnableGlobalLoadingWindow(false);
			this._heroAdminState = heroAdminState;
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			LoadingWindow.DisableGlobalLoadingWindow();//关闭加载界面
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit") || this._gauntletLayer.Input.IsGameKeyReleased(36))
			{
				this.CloseScreen();
			}
		}

		public void OnExit()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			LoadingWindow.EnableGlobalLoadingWindow(true);

		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			if (LoadingWindow.GetGlobalLoadingWindowState())
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}

			this._dataSource = null;
			this._gauntletLayer = null;

		}

		protected override void OnActivate()
		{
			base.OnActivate();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uIResourceDepot = UIResourceManager.UIResourceDepot;
			this._characterdeveloper = spriteData.SpriteCategories["ui_characterdeveloper"];
			this._characterdeveloper.Load(resourceContext, uIResourceDepot);

			//this._clanCategory = spriteData.SpriteCategories["ui_encyclopedia"];
			//this._clanCategory.Load(resourceContext, uIResourceDepot);

			this._clanCategory = spriteData.SpriteCategories["ui_clan"];
			this._clanCategory.Load(resourceContext, uIResourceDepot);

			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer");
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			base.AddLayer(this._gauntletLayer);

			this._dataSource = new HereAdminDashBoardVM(this._heroAdminState.EditHero, OnCloseHereAdminDashBoard);

			this._gauntletLayer.LoadMovie("HeroAdminDashBoard", this._dataSource);

		}

		private void OnCloseHereAdminDashBoard()
		{
			OnExit();

		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			LoadingWindow.EnableGlobalLoadingWindow(false);
			InformationManager.HideInformations();
		}
		private void CloseScreen()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		void IGameStateListener.OnActivate()
		{
		}

		void IGameStateListener.OnDeactivate()
		{
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{

		}
	}
}
