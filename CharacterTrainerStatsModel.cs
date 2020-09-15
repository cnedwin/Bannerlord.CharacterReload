using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace CharacterTrainer
{
	public class CharacterTrainerStatsModel : DefaultCharacterStatsModel
	{
		public bool IsInitialized { get; private set; }

		public CharacterTrainerStatsModel()
		{
			this.playerBaseHitPoint = 100;
			this.othersBaseHitPoint = new Dictionary<string, int>();
		}

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

		public const int DefaultBaseHitPoint = 100;

		public int playerBaseHitPoint;

		public Dictionary<string, int> othersBaseHitPoint;
	}
}
