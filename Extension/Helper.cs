using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CharacterTrainer
{
	public static class Helper
	{
		public static Hero MainHero
		{
			get
			{
				if (Game.Current == null)
				{
					return null;
				}
				CharacterObject characterObject = Game.Current.PlayerTroop as CharacterObject;
				if (characterObject == null)
				{
					return null;
				}
				return characterObject.HeroObject;
			}
		}

		public static string SavePath
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Mount and Blade II Bannerlord\\CharacterReload\\";
			}
		}

		public static string LogPath
		{
			get
			{
				return Helper.SavePath + "CharacterReload_log.txt";
			}
		}

		public static string GetHeroID(Hero hero)
		{
			if (hero.IsHumanPlayerCharacter)
			{
				return hero.Name.ToString();
			}
			return hero.Name + "." + hero.StringId.Replace("TaleWorlds.CampaignSystem.CharacterObject", string.Empty);
		}

		public static string GetFilename(Hero hero)
		{
			return string.Concat(new object[]
			{
				Helper.SavePath,
				Helper.MainHero.Name,
				"\\",
				Helper.GetHeroID(hero),
				".txt"
			});
		}

		public static void ClearLog()
		{
			string logPath = Helper.LogPath;
			if (File.Exists(logPath))
			{
				File.Delete(logPath);
			}
		}

		public static void Log(string text)
		{
			File.AppendAllText(Helper.LogPath, text + "\n");
		}

		public static string HeroBodyPropertiesToString(Hero hero)
		{
			DynamicBodyProperties dynamicProperties = hero.BodyProperties.DynamicProperties;
			StaticBodyProperties staticProperties = hero.BodyProperties.StaticProperties;
			return string.Join(";", new object[]
			{
				hero.IsFemale.ToString(),
				dynamicProperties.Age,
				dynamicProperties.Build,
				dynamicProperties.Weight,
				staticProperties.KeyPart1,
				staticProperties.KeyPart2,
				staticProperties.KeyPart3,
				staticProperties.KeyPart4,
				staticProperties.KeyPart5,
				staticProperties.KeyPart6,
				staticProperties.KeyPart7,
				staticProperties.KeyPart8
			});
		}

		public static void ApplyBodyPropertiesToHero(Hero hero, string str)
		{
			if (hero.CharacterObject == null)
			{
				return;
			}
			string[] array = str.Split(new char[]
			{
				';'
			});
			int num = 0;
			bool isFemale = Convert.ToBoolean(array[num++]);
			hero.CharacterObject.UpdatePlayerCharacterBodyProperties(new BodyProperties(new DynamicBodyProperties
			{
				Age = Convert.ToSingle(array[num++]),
				Build = Convert.ToSingle(array[num++]),
				Weight = Convert.ToSingle(array[num++])
			}, new StaticBodyProperties(Convert.ToUInt64(array[num++]), Convert.ToUInt64(array[num++]), Convert.ToUInt64(array[num++]), Convert.ToUInt64(array[num++]), Convert.ToUInt64(array[num++]), Convert.ToUInt64(array[num++]), Convert.ToUInt64(array[num++]), Convert.ToUInt64(array[num++]))), isFemale);
		}

		public static void WriteAllText(string filename, string contents)
		{
			string directoryName = Path.GetDirectoryName(filename);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(filename, contents);
		}
	}
}
