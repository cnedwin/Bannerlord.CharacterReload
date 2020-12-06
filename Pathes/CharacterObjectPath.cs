using CharacterReload.Utils;
using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CharacterReload.Patch
{
    class CharacterObjectPath
    {
        [HarmonyPatch(typeof(CharacterObject), nameof(CharacterObject.UpdatePlayerCharacterBodyProperties))]
        static class CharacterObjectPatch
        {

            static void Postfix(CharacterObject __instance, BodyProperties properties, bool isFemale)
            {
                //if (__instance.IsPlayerCharacter && __instance.IsHero)
                //{
                //    __instance.HeroObject.SetBirthDay(HeroHelper.GetRandomBirthDayForAge((int)properties.Age));
                //}

                if (!__instance.IsPlayerCharacter && __instance.IsHero)
                {
                    Hero hero = __instance.HeroObject;
                    ReflectUtils.ReflectPropertyAndSetValue("StaticBodyProperties", properties.StaticProperties, hero);
                    hero.Weight = properties.Weight;
                    hero.Build = properties.Build;
                    //hero.SetBirthDay(HeroHelper.GetRandomBirthDayForAge((int)properties.Age));
                    hero.UpdatePlayerGender(isFemale);
                }
            }
        }
    }
}
