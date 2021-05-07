using CharacterReload.Data;
using CharacterReload.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CharacterReload.VM.HeroAdmin
{
    class HeroAdminSkillVM: ViewModel
    {
		private int _currentFocusLevel;
		private string _nameText;
		private string _skillId;
		private readonly SkillObject _skillObject;
		private MBBindingList<HeroAdminPerkVM> _perks;
		private HeroAdminCharacter _hero;
		private bool _isInspected;
		private int _level;
		private float _learningRate;
		private int _maxLevel;
		private int _fullLearningRateLevel;
		private bool _canLearnSkill;
		Action<HeroAdminSkillVM> onSkillSelection;

        public HeroAdminSkillVM(SkillObject skillObject, HeroAdminCharacter hero, Action<HeroAdminSkillVM> onSkillSelection)
		{
			this._perks = new MBBindingList<HeroAdminPerkVM>();
			this._skillObject = skillObject;
			this.SkillId = skillObject.StringId;
			this.NameText = skillObject.Name.ToString();
			this.MaxLevel = 300;
			FillHeroData(hero);
			this.onSkillSelection = onSkillSelection;
        }

		public SkillObject Skill
        {
			get { return this._skillObject; }
        }

		public void FillHeroData(HeroAdminCharacter hero)
        {
			this._hero = hero;
			this.CurrentFocusLevel = hero.GetFocusValue(this.Skill);
			int boundAttributeCurrentValue = hero.GetAttributeValue(this.Skill.CharacterAttributeEnum);
			TextObject boundAttributeName = CharacterAttributes.GetCharacterAttribute(this.Skill.CharacterAttributeEnum).Name;
			float num = Campaign.Current.Models.CharacterDevelopmentModel.CalculateLearningRate(boundAttributeCurrentValue, this.CurrentFocusLevel, this.Level, this._hero.Level, boundAttributeName, false).ResultNumber;
			this.LearningRate = num;
			this.CanLearnSkill = (num > 0f);
			this.FullLearningRateLevel = MBMath.Round(Campaign.Current.Models.CharacterDevelopmentModel.CalculateLearningLimit(boundAttributeCurrentValue, this.CurrentFocusLevel, boundAttributeName, false).ResultNumber);
			this.Level = hero.GetSkillValue(this._skillObject);
			RefreshPerks();
		}

		[DataSourceProperty]
		public string FocusText
		{
			get
			{
				return new TextObject("{=cr_hero_focus}Focus").ToString();
			}
		}

		[DataSourceProperty]
		public string SkillLevelText
		{
			get
			{
				return new TextObject("{=cr_hero_skilllevel}Skill Level").ToString();
			}
		}

		[DataSourceProperty]
		public MBBindingList<HeroAdminPerkVM> Perks
		{
			get
			{
				return this._perks;
			}
			set
			{
				if (value != this._perks)
				{
					this._perks = value;
					base.OnPropertyChangedWithValue(value, "Perks");
				}
			}
		}

		[DataSourceProperty]
		public int FullLearningRateLevel
		{
			get
			{
				return this._fullLearningRateLevel;
			}
			set
			{
				if (value != this._fullLearningRateLevel)
				{
					this._fullLearningRateLevel = value;
					base.OnPropertyChangedWithValue(value, "FullLearningRateLevel");
				}
			}
		}


		[DataSourceProperty]
		public int MaxLevel
		{
			get
			{
				return this._maxLevel;
			}
			set
			{
				if (value != this._maxLevel)
				{
					this._maxLevel = value;
					base.OnPropertyChangedWithValue(value, "MaxLevel");
				}
			}
		}

		[DataSourceProperty]
		public bool CanLearnSkill
		{
			get
			{
				return this._canLearnSkill;
			}
			set
			{
				if (value != this._canLearnSkill)
				{
					this._canLearnSkill = value;
					base.OnPropertyChangedWithValue(value, "CanLearnSkill");
				}
			}
		}

		[DataSourceProperty]
		public float LearningRate
		{
			get
			{
				return this._learningRate;
			}
			set
			{
				if (value != this._learningRate)
				{
					this._learningRate = value;
					base.OnPropertyChangedWithValue(value, "LearningRate");
				}
			}
		}


		[DataSourceProperty]
		public int CurrentFocusLevel
		{
			get
			{
				return this._currentFocusLevel;
			}
			set
			{
				if (value != this._currentFocusLevel)
				{
					this._currentFocusLevel = value;
					//this._hero.HeroDeveloper.AddFocus
					SetHeroFocus(value);
					base.OnPropertyChangedWithValue(value, "CurrentFocusLevel");
				}
			}
		}

		private void SetHeroFocus( int newAmount)
		{
			this._hero.SetFocusValue(this.Skill, newAmount);
			//ReflectUtils.ReflectMethodAndInvoke("SetFocus", this._hero.HeroDeveloper, new object[] { this.Skill, newAmount });
		}


		[DataSourceProperty]
		public bool IsInspected
		{
			get
			{
				return this._isInspected;
			}
			set
			{
				if (value != this._isInspected)
				{
					this._isInspected = value;
					base.OnPropertyChangedWithValue(value, "IsInspected");
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
					SetHeroSkillLevel(value);
					base.OnPropertyChangedWithValue(value, "Level");
				}
			}
		}

		private void SetHeroSkillLevel(int newAmount)
		{
			this._hero.SetSkillValue(this.Skill, newAmount);
		}


		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue(value, "NameText");
				}
			}
		}

		[DataSourceProperty]
		public string SkillId
		{
			get
			{
				return this._skillId;
			}
			set
			{
				if (value != this._skillId)
				{
					this._skillId = value;
					base.OnPropertyChangedWithValue(value, "SkillId");
				}
			}
		}

		private void ExecuteInspect()
		{
			this.IsInspected = true;
			this.onSkillSelection(this);
		
		}

		private void RefreshPerks()
		{
			this.Perks.Clear();
			PerkObject perkObject = null;
			IEnumerable<PerkObject> listPerks = from p in PerkObject.All where p.Skill == this.Skill select p;
			foreach (PerkObject current2 in listPerks.OrderBy(p => p.RequiredSkillValue))
			{
				if (current2.AlternativePerk != null)
				{
					HeroAdminPerkVM.PerkAlternativeType perkAlternativeType = (perkObject == null) ? HeroAdminPerkVM.PerkAlternativeType.FirstAlternative : HeroAdminPerkVM.PerkAlternativeType.SecondAlternative;
					HeroAdminPerkVM item = new HeroAdminPerkVM(current2, this.IsPerkSelected(current2), perkAlternativeType, OnPerkSelectedChange);
					this.Perks.Add(item);
					if (perkAlternativeType == HeroAdminPerkVM.PerkAlternativeType.SecondAlternative)
					{
						perkObject = null;
					}
					else
					{
						perkObject = current2.AlternativePerk;
					}
				}
				else
				{
					HeroAdminPerkVM item = new HeroAdminPerkVM(current2, this.IsPerkSelected(current2), HeroAdminPerkVM.PerkAlternativeType.NoAlternative, OnPerkSelectedChange);
					this.Perks.Add(item);
					perkObject = null;
				}
			}
		}

		private void OnPerkSelectedChange(PerkObject perk, bool selected)
		{
		    this._hero.SetPerkValue(perk, selected);
			
		}

		private bool IsPerkSelected(PerkObject perk)
		{
			return this._hero.GetPerkValue(perk);
		}
	}
}
