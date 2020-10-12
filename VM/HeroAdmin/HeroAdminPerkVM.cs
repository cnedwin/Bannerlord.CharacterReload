using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace CharacterReload.VM.HeroAdmin
{
	class HeroAdminPerkVM : ViewModel
	{
		public enum PerkStates
		{
			None = -1,
			NotEarned,
			EarnedButNotSelected,
			InSelection,
			EarnedAndActive,
			EarnedAndNotActive,
			EarnedPreviousPerkNotSelected
		}

		public enum PerkAlternativeType
		{
			NoAlternative,
			FirstAlternative,
			SecondAlternative
		}

		public readonly PerkObject Perk;

		private readonly Concept _perkConceptObj;

		private HeroAdminPerkVM.PerkStates _currentState = HeroAdminPerkVM.PerkStates.None;

		private string _levelText;

		private string _perkId;

		private string _backgroundImage;

		private BasicTooltipViewModel _hint;

		private int _level;

		private int _alternativeType;

		private int _perkState = -1;

		private bool _isTutorialHighlightEnabled;

		Action<PerkObject, bool> _onPerkSelectedChange;

		public HeroAdminPerkVM.PerkStates CurrentState
		{
			get
			{
				return this._currentState;
			}
			private set
			{
				if (value != this._currentState)
				{
					this._currentState = value;
					this.PerkState = (int)value;
				}
			}
		}

		[DataSourceProperty]
		public bool IsTutorialHighlightEnabled
		{
			get
			{
				return this._isTutorialHighlightEnabled;
			}
			set
			{
				if (value != this._isTutorialHighlightEnabled)
				{
					this._isTutorialHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsTutorialHighlightEnabled");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue(value, "Hint");
				}
			}
		}

		[DataSourceProperty]
		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (value != this._level)
				{
					this._level = value;
					base.OnPropertyChangedWithValue(value, "Level");
				}
			}
		}

		[DataSourceProperty]
		public int PerkState
		{
			get
			{
				return this._perkState;
			}
			set
			{
				if (value != this._perkState)
				{
					this._perkState = value;
					base.OnPropertyChangedWithValue(value, "PerkState");
				}
			}
		}

		[DataSourceProperty]
		public int AlternativeType
		{
			get
			{
				return this._alternativeType;
			}
			set
			{
				if (value != this._alternativeType)
				{
					this._alternativeType = value;
					base.OnPropertyChangedWithValue(value, "AlternativeType");
				}
			}
		}

		[DataSourceProperty]
		public string LevelText
		{
			get
			{
				return this._levelText;
			}
			set
			{
				if (value != this._levelText)
				{
					this._levelText = value;
					base.OnPropertyChangedWithValue(value, "LevelText");
				}
			}
		}

		[DataSourceProperty]
		public string BackgroundImage
		{
			get
			{
				return this._backgroundImage;
			}
			set
			{
				if (value != this._backgroundImage)
				{
					this._backgroundImage = value;
					base.OnPropertyChangedWithValue(value, "BackgroundImage");
				}
			}
		}

		[DataSourceProperty]
		public string PerkId
		{
			get
			{
				return this._perkId;
			}
			set
			{
				if (value != this._perkId)
				{
					this._perkId = value;
					base.OnPropertyChangedWithValue(value, "PerkId");
				}
			}
		}

		public HeroAdminPerkVM(PerkObject perk, bool isSelected, HeroAdminPerkVM.PerkAlternativeType alternativeType, Action<PerkObject, bool> onPerkSelectedChange)
		{
			this.AlternativeType = (int)alternativeType;
			this.Perk = perk;
			this._onPerkSelectedChange = onPerkSelectedChange;
			this.PerkId = "SPPerks\\" + perk.StringId;
			this.Level = (int)perk.RequiredSkillValue;
			this.LevelText = ((int)perk.RequiredSkillValue).ToString();
			this.Hint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPerkEffectText(perk, true));
			this._perkConceptObj = Concept.All.SingleOrDefault(c => c.StringId == "str_game_objects_perks");
            if (isSelected)
            {
				this.CurrentState = HeroAdminPerkVM.PerkStates.EarnedAndActive;
			}
			else
            {
				this.CurrentState = HeroAdminPerkVM.PerkStates.EarnedButNotSelected;
			}
		}

		private void ExecuteShowPerkConcept()
		{
			if (this._perkConceptObj != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._perkConceptObj.EncyclopediaLink);
			}
		}

		private void ExecuteStartSelection()
		{
            if (this.CurrentState != HeroAdminPerkVM.PerkStates.EarnedAndActive)
            {
				this.CurrentState = HeroAdminPerkVM.PerkStates.EarnedAndActive;
				if (null != this._onPerkSelectedChange) this._onPerkSelectedChange(this.Perk, true);

			}
            else
            {
				this.CurrentState = HeroAdminPerkVM.PerkStates.EarnedButNotSelected;
				if (null != this._onPerkSelectedChange) this._onPerkSelectedChange(this.Perk, false);
			}
			
		}
	}
}