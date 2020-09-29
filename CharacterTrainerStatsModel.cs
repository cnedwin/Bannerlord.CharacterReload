using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CharacterReload
{

	public class CharacterTrainerStatsModel : DefaultCharacterStatsModel
	{
		private static CharacterTrainerStatsModel _instance;

		public const int DefaultBaseHitPoint = 100;

		public int playerBaseHitPoint;

		public Dictionary<string, int> othersBaseHitPoint;

		private CharacterTrainerStatsModel()
		{
			this.playerBaseHitPoint = 100;
			this.othersBaseHitPoint = new Dictionary<string, int>();
		}

		public static CharacterTrainerStatsModel Instance()
		{
			if (null == _instance)
			{
				_instance = new CharacterTrainerStatsModel();
			}
			return _instance;
		}



		public bool IsInitialized { get; private set; }


		private int ImportBaseHitPoint(Hero hero)
		{
			Helper.Log("ImportBaseHitPoint " + hero.Name);
			string filename = Helper.GetFilename(hero);
			if (!File.Exists(filename))
			{
				return 100;
			}
			Helper.Log("File found");
			foreach (string text in File.ReadAllLines(filename))
			{
				if (!text.Contains("#") && text.Contains("="))
				{
					string[] array2 = text.Split(new char[]
					{
						'='
					});
					string a = array2[0].Trim();
					int val = Convert.ToInt32(array2[1].Trim());
					if (a == "BaseHitPoint")
					{
						return Math.Max(val, 1);
					}
				}
			}
			return 100;
		}

		public void Initialize()
		{
			Helper.Log("ImportAllBaseHitPoints");
			Hero mainHero = Helper.MainHero;
			if (mainHero != null)
			{
				Helper.Log("Main hero found");
				IEnumerable<Hero> enumerable = null;
				string path = Helper.SavePath + mainHero.Name;
				if (Directory.Exists(path))
				{
					string[] files = Directory.GetFiles(path, "*.*.txt");
					HashSet<string> savedHeroesHash = new HashSet<string>();
					for (int i = 0; i < files.Length; i++)
					{
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(files[i]);
						savedHeroesHash.Add(fileNameWithoutExtension);
					}
					enumerable = Hero.FindAll((Hero h) => savedHeroesHash.Contains(Helper.GetHeroID(h)));
				}
				this.playerBaseHitPoint = this.ImportBaseHitPoint(mainHero);
				if (enumerable != null)
				{
					foreach (Hero hero in enumerable)
					{
						if (!hero.IsHumanPlayerCharacter)
						{
							this.othersBaseHitPoint[hero.StringId] = this.ImportBaseHitPoint(hero);
						}
					}
				}
			}
			this.IsInitialized = true;
		}

		public int GetBaseHitPoint(Hero hero)
		{
			if (hero != null)
			{
				if (hero.IsHumanPlayerCharacter)
				{
					return this.playerBaseHitPoint;
				}
				int result;
				if (this.othersBaseHitPoint.TryGetValue(hero.StringId, out result))
				{
					return result;
				}
			}
			return 100;
		}

		public override int MaxHitpoints(CharacterObject character, StatExplainer explanation = null)
		{
			int num = this.GetBaseHitPoint(character.HeroObject) - 100;
			return base.MaxHitpoints(character, explanation) + num;
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
							CharacterTrainerStatsModel.Instance().playerBaseHitPoint = Math.Max(num, 1);
						}
						else
						{
							CharacterTrainerStatsModel.Instance().othersBaseHitPoint[hero.StringId] = Math.Max(num, 1);
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
	}
}
