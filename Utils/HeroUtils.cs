using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CharacterReload.Utils
{
    class HeroUtils
    {

      public static void UpdateHeroCharacterBodyProperties(CharacterObject character, BodyProperties properties, bool isFemale)
      {
            if (character.IsHero)
            {
                Hero hero = character.HeroObject;
                ReflectUtils.ReflectPropertyAndSetValue("StaticBodyProperties", properties.StaticProperties, hero);
                hero.Weight = properties.Weight;
                hero.Build = properties.Build;
                hero.SetBirthDay(HeroHelper.GetRandomBirthDayForAge((int)properties.Age));
                hero.UpdatePlayerGender(isFemale);
            }
        }
    }
}
