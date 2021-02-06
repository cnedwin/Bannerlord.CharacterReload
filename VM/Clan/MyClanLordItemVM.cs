using CharacterReload;
using CharacterReload.Utils;
using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CharacterReload.VM
{
	[HarmonyPatch(typeof(ClanLordItemVM))]
	class ClanLordItemVMPatch
	{
		[HarmonyPatch("OnNamingHeroOver")]
		[HarmonyPostfix]
		public static void OnNamingHeroOverPostfix(ClanLordItemVM __instance, string suggestedName)
		{
			if (!CampaignUIHelper.IsStringApplicableForHeroName(suggestedName)) return;
			Hero selectedHero = __instance.GetHero();
			selectedHero.FirstName = selectedHero.Name;
            if (selectedHero.IsPartyLeader)
                ReflectUtils.ReflectMethodAndInvoke("Name", selectedHero.PartyBelongedTo, new object[] { selectedHero.CharacterObject.Name });

        }
	}

	public class MyClanLordItemVM : ClanLordItemVM
	{
		Action<ClanLordItemVM> _characterSelect;

		public MyClanLordItemVM(Hero hero, Action<ClanLordItemVM> onCharacterSelect) : base(hero, onCharacterSelect)
		{
			this._characterSelect = onCharacterSelect;
		}
		private void OnCharacterSelect()
		{

			this._characterSelect(this);
		}

		private void ExecuteLocationLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		private void ExecuteLink()
		{
			Campaign.Current.EncyclopediaManager.GoToLink(this.GetHero().EncyclopediaLink);
		}


		private void ExecuteRename()
		{
			InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=2lFwF07j}Change Name", null).ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action<string>(this.OnNamingHeroOver), null, false, new Func<string, bool>(CampaignUIHelper.IsStringApplicableForHeroName), "", ""), false);
		}

		private void OnNamingHeroOver(string suggestedName)
		{
			if (CampaignUIHelper.IsStringApplicableForHeroName(suggestedName))
			{
				var currentHero = this.GetHero();
				currentHero.CharacterObject.Name = new TextObject(suggestedName, null);
				this.Name = suggestedName;
				currentHero.FirstName = currentHero.Name;
				//if (currentHero.IsPartyLeader)
				    //ReflectUtils.ReflectMethodAndInvoke("Name", currentHero.PartyBelongedTo, new object[] { MobilePartyHelper.GeneratePartyName(currentHero.CharacterObject) });


			}

		}


		public void DoRefleshAPoint()
		{
			TextObject textObject = new TextObject("{=misc_cr_DoRefleshAPoint}The hero's attribute points have been reset", null);
			// InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));

			this.ShowComfirDialog(textObject, () => ReAttHero(GetHero()));

		}

		public void DoRefleshFPoint()
		{
			TextObject textObject = new TextObject("{=misc_cr_DoRefleshFPoint}The hero's specialization point has been reset", null);
			//   InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
			this.ShowComfirDialog(textObject, () => RefocusHero(GetHero()));

		}

		public void DoRefleshPerks()
		{
			TextObject textObject = new TextObject("{=misc_cr_DoRefleshPerk}The hero's skill tree has been reset", null);
			this.ShowComfirDialog(textObject, () => RePerksHero(GetHero()));
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=tips_cr_DoRefleshPerks}After reset the hero’s skill perk, you must save and reload the saves to take effect!", null).ToString()));


		}

		public void DoRefleshTraits()
		{
			TextObject textObject = new TextObject("{=misc_cr_DoRefleshTraits}The hero's traits have been improved", null);
			this.ShowComfirDialog(textObject, () => ReTraits(GetHero()));
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=tips_cr_DoRefleshTrait}After upgrading the hero’s traits, you need to close the clan screen and reopen it to take effect!", null).ToString()));


		}

		public void DoRefleshLevel()
		{
			TextObject textObject = new TextObject("{=misc_cr_DoRefleshLevel}The hero's level have been reset", null);
			this.ShowComfirDialog(textObject, () => ReLevel(GetHero()));
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=tips_cr_DoRefleshLevel}After reset the hero’s Level, you need to close the clan screen and reopen it to take effect!", null).ToString()));

		}

		public void ExecuteExport()
		{
			TextObject textObject = new TextObject("{=misc_cr_ExecuteExportl}Export the Hero", null);
			this.ShowComfirDialog(textObject, () => Export(GetHero()));
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=tips_cr_doexport}Character exported to " + Helper.GetFilename(GetHero()), null).ToString()));


		}

		public void ExecuteImport()
		{
			TextObject textObject = new TextObject("{=misc_cr_ExecuteImport}Import the hero", null);
			bool flag = InputKey.LeftShift.IsDown() || InputKey.RightShift.IsDown();
			//this.ShowComfirDialog(textObject, () => CharacterTrainerStatsModel.Instance().Import(GetHero(), flag));
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=tips_cr_DoExport}Character imported from " + Helper.GetFilename(GetHero()), null).ToString()));


		}


		private void ShowComfirDialog(TextObject tip, Action action)
		{
			InformationManager.ShowInquiry(new InquiryData(tip.ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), action, () => { }), false);

		}


		[DataSourceProperty]
		public string ResetAttributeText
		{
			get
			{
				return new TextObject("{=bottom_ReAttHero}ResetAttribute", null).ToString();
			}
		}

		[DataSourceProperty]
		public string ResetFocusText
		{
			get
			{
				return new TextObject("{=bottom_RefocusHero}ResetFocus", null).ToString();
			}
		}

		[DataSourceProperty]
		public string ResetPerkText
		{
			get
			{
				return new TextObject("{=bottom_RePerksHero}ResetPerk", null).ToString();
			}
		}

		[DataSourceProperty]
		public string ResetTraitsText
		{
			get
			{
				return new TextObject("{=bottom_ReTraitsHero}ImproveTraits", null).ToString();
			}
		}

		[DataSourceProperty]
		public string ResetLevelText
		{
			get
			{
				return new TextObject("{=bottom_ReLevelHero}ResetLevel", null).ToString();
			}
		}

		[DataSourceProperty]
		public string ExecuteImportText
		{
			get
			{
				return new TextObject("{=cr_execute_Import}Import", null).ToString();
			}
		}

		[DataSourceProperty]
		public string ExecuteExportText
		{
			get
			{
				return new TextObject("{=cr_execute_export}Export", null).ToString();
			}
		}

		private void RePerksHero(Hero hero)
		{
			foreach (SkillObject skill in DefaultSkills.GetAllSkills())
			{
				hero.HeroDeveloper.TakeAllPerks(skill);
			}
			hero.ClearPerks();
		}

		public void RefocusHero(Hero hero)
		{
			int num = 0;
			int num2 = 0;
			foreach (SkillObject skill in DefaultSkills.GetAllSkills())
			{
				int focus = hero.HeroDeveloper.GetFocus(skill);
				if (focus > 0)
				{
					num += focus;
					hero.HeroDeveloper.AddFocus(skill, num2 - focus, false);
				}
			}
			hero.HeroDeveloper.UnspentFocusPoints += MBMath.ClampInt(num, 0, 999);

		}


		public void ReAttHero(Hero hero)
		{
			int num = 0;
			for (CharacterAttributesEnum characterAttributesEnum = CharacterAttributesEnum.Vigor; characterAttributesEnum < CharacterAttributesEnum.NumCharacterAttributes; characterAttributesEnum++)
			{
				int attributeValue = hero.GetAttributeValue(characterAttributesEnum);
				num += attributeValue;
				hero.SetAttributeValue(characterAttributesEnum, 0);
			}
			hero.HeroDeveloper.UnspentAttributePoints += MBMath.ClampInt(num, 0, 999);
		}

		public void ReTraits(Hero hero)
		{
			int num = 2;
			hero.ClearTraits();
			hero.SetTraitLevel(DefaultTraits.Honor, num);
			hero.SetTraitLevel(DefaultTraits.Valor, num);
			hero.SetTraitLevel(DefaultTraits.Mercy, num);
			hero.SetTraitLevel(DefaultTraits.Generosity, num);
			hero.SetTraitLevel(DefaultTraits.Calculating, num);
		}

		public void ReLevel(Hero hero)
		{
			hero.HeroDeveloper.ClearDeveloper();
			hero.ClearSkills();
			this.AddSkillLevel(hero, DefaultSkills.Bow, (int)60);
			this.AddSkillLevel(hero, DefaultSkills.Riding, (int)60);
			this.AddSkillLevel(hero, DefaultSkills.Trade, (int)60);
			this.AddSkillLevel(hero, DefaultSkills.Steward, (int)60);
			hero.Initialize();
		}

		private void AddSkillLevel(Hero hero, SkillObject defaultSkills, int value)
		{
			hero.HeroDeveloper.SetInitialSkillLevel(defaultSkills, value);
			hero.HeroDeveloper.InitializeSkillXp(defaultSkills);
			bool flag = hero.HeroDeveloper.GetSkillXpProgress(defaultSkills) < 0;
			bool flag2 = flag;
			if (flag2)
			{
				hero.HeroDeveloper.AddSkillXp(defaultSkills, (float)(hero.HeroDeveloper.GetSkillXpProgress(defaultSkills) * -1 + 1), false, false);
			}
		}



		public string Export(Hero hero)
		{
			Helper.Log("Export " + hero.Name);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("# BaseHitpoint is character's max HP before perk bonus added.");
			stringBuilder.AppendLine("# IMPORTANT! If you ever change BaseHitpoint, you shouldn't delete this file or character BaseHitPoint will be reverted to 100 (Bannerlord default)");
			stringBuilder.AddItem("BaseHitPoint", 100);
			stringBuilder.AddItem("CurrentHitPoint", hero.HitPoints);
			stringBuilder.AddItem("Gold", hero.Gold);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("# Appearances");
			stringBuilder.AddItem("BodyProperties", Helper.HeroBodyPropertiesToString(hero));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("# Attributes");
			for (int i = 0; i < 6; i++)
			{
				stringBuilder.AddItem(Enum.GetName(typeof(CharacterAttributesEnum), i), hero.GetAttributeValue((CharacterAttributesEnum)i));
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("# Skills");
			MBReadOnlyList<SkillObject> skillList = Game.Current.SkillList;
			for (int j = 0; j < skillList.Count; j++)
			{
				SkillObject skillObject = skillList[j];
				stringBuilder.AddItem(skillObject.StringId, hero.GetSkillValue(skillObject));
				stringBuilder.AddItem(skillObject.StringId + ".focus", hero.HeroDeveloper.GetFocus(skillObject));
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("# Perks (set value to 1 to learn)");
			foreach (PerkObject perkObject in PerkObject.All)
			{
				stringBuilder.AddItem(perkObject.StringId, hero.GetPerkValue(perkObject) ? 1 : 0);
			}
			Helper.WriteAllText(Helper.GetFilename(hero), stringBuilder.ToString());
			return null;
		}



	}
}
