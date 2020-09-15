using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CharacterTrainer
{
	public class CharacterTrainerViewModel : ViewModel
	{

		public CharacterTrainerViewModel(CharacterTrainerStatsModel statsModel, Action<Hero> exportCallback, Action<Hero, bool, string> importCallback, Action<Hero> clearPointsCallback)
		{
			this.statsModel = statsModel;
			this.exportCallback = exportCallback;
			this.importCallback = importCallback;
			this.clearPointsCallback = clearPointsCallback;
		}

		public void SetHero(Hero hero)
		{
			this.selectedHero = hero;
		}

		public void ExecuteExport()
		{
			Helper.Log("ExecuteExport");
			if (this.selectedHero == null)
			{
				return;
			}
			this.Export(this.selectedHero);
			Action<Hero> action = this.exportCallback;
			if (action == null)
			{
				return;
			}
			action(this.selectedHero);
		}

		public void ExecuteImport()
		{
			Helper.Log("ExecuteImport");
			if (this.selectedHero == null)
			{
				return;
			}
			bool flag = InputKey.LeftShift.IsDown() || InputKey.RightShift.IsDown();
			string arg = this.Import(this.selectedHero, flag);
			Action<Hero, bool, string> action = this.importCallback;
			if (action == null)
			{
				return;
			}
			action(this.selectedHero, flag, arg);
		}

		public void ExecuteClearPoints()
		{
			Helper.Log("ExecuteClear");
			if (this.CanClearPoints)
			{
				this.selectedHero.HeroDeveloper.ClearUnspentPoints();
				Action<Hero> action = this.clearPointsCallback;
				if (action == null)
				{
					return;
				}
				action(this.selectedHero);
			}
		}

		public string Export(Hero hero)
		{
			Helper.Log("Export " + hero.Name);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("# BaseHitpoint is character's max HP before perk bonus added.");
			stringBuilder.AppendLine("# IMPORTANT! If you ever change BaseHitpoint, you shouldn't delete this file or character BaseHitPoint will be reverted to 100 (Bannerlord default)");
			stringBuilder.AddItem("BaseHitPoint", this.statsModel.GetBaseHitPoint(hero));
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

		public string Import(Hero hero, bool ignoreCharacterExp)
		{
			Helper.Log("Import " + hero.Name);
			string filename = Helper.GetFilename(hero);
			if (!File.Exists(filename))
			{
				return "Chararacter file does not exist";
			}
			Helper.Log("File found");
			string[] array = File.ReadAllLines(filename);
			MBReadOnlyList<SkillObject> skillList = Game.Current.SkillList;
			Dictionary<string, SkillObject> dictionary = new Dictionary<string, SkillObject>();
			for (int i = 0; i < skillList.Count; i++)
			{
				SkillObject skillObject = skillList[i];
				dictionary[skillObject.StringId] = skillObject;
			}
			Dictionary<string, PerkObject> dictionary2 = new Dictionary<string, PerkObject>();
			foreach (PerkObject perkObject in PerkObject.All)
			{
				dictionary2[perkObject.StringId] = perkObject;
			}
			foreach (string text in array)
			{
				if (!text.Contains("#") && text.Contains("="))
				{
					string[] array2 = text.Split(new char[]
					{
						'='
					});
					string text2 = array2[0].Trim();
					string text3 = array2[1].Trim();
					int num;
					int.TryParse(text3, out num);
					if (!(text2 == "Gold"))
					{
						if (!(text2 == "BaseHitPoint"))
						{
							if (!(text2 == "CurrentHitPoint"))
							{
								if (!(text2 == "BodyProperties"))
								{
									CharacterAttributesEnum charAttribute;
									if (Enum.TryParse<CharacterAttributesEnum>(text2, out charAttribute))
									{
										hero.SetAttributeValue(charAttribute, (int)MathF.Clamp((float)num, 0f, 10f));
									}
									else
									{
										bool flag = false;
										SkillObject skillObject2 = null;
										if (text2.Contains(".focus"))
										{
											skillObject2 = dictionary[text2.Split(new char[]
											{
												'.'
											})[0]];
											flag = true;
										}
										else
										{
											dictionary.TryGetValue(text2, out skillObject2);
										}
										PerkObject perk;
										if (skillObject2 != null)
										{
											if (flag)
											{
												int num2 = (int)MathF.Clamp((float)num, 0f, 5f);
												int focus = hero.HeroDeveloper.GetFocus(skillObject2);
												hero.HeroDeveloper.AddFocus(skillObject2, num2 - focus, false);
											}
											else
											{
												int num3 = num;
												if (ignoreCharacterExp)
												{
													hero.HeroDeveloper.SetInitialSkillLevel(skillObject2, num3);
													hero.HeroDeveloper.InitializeSkillXp(skillObject2);
												}
												else
												{
													int skillValue = hero.GetSkillValue(skillObject2);
													hero.HeroDeveloper.ChangeSkillLevel(skillObject2, num3 - skillValue, true);
												}
											}
										}
										else if (dictionary2.TryGetValue(text2, out perk))
										{
											bool perkValue = hero.GetPerkValue(perk);
											bool flag2 = Convert.ToBoolean(num);
											if (perkValue != flag2)
											{
												hero.SetPerkValue(perk, flag2);
											}
										}
									}
								}
								else
								{
									Helper.ApplyBodyPropertiesToHero(hero, text3);
								}
							}
							else
							{
								hero.HitPoints = Math.Max(num, 1);
							}
						}
						else if (hero.IsHumanPlayerCharacter)
						{
							this.statsModel.playerBaseHitPoint = Math.Max(num, 1);
						}
						else
						{
							this.statsModel.othersBaseHitPoint[hero.StringId] = Math.Max(num, 1);
						}
					}
					else
					{
						hero.ChangeHeroGold(num - hero.Gold);
					}
				}
			}
			return null;
		}



		private CharacterTrainerStatsModel statsModel;

		private Hero selectedHero;

		private Action<Hero> exportCallback;

		private Action<Hero, bool, string> importCallback;

		private Action<Hero> clearPointsCallback;
	}
}
