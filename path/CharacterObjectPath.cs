using FaceDetailsCreator.utils;
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

namespace FaceDetailsCreator.path
{
    class CharacterObjectPath
    {
        [HarmonyPatch(typeof(CharacterObject), nameof(CharacterObject.UpdatePlayerCharacterBodyProperties))]
        static class CharacterObjectPatch
        {
            private static readonly TextObject HeroUpdatedMsg = new TextObject("{=CharacterCreation_HeroUpdatedMsg}Hero updated: ");

            static void Postfix(CharacterObject __instance, BodyProperties properties, bool isFemale)
            {
                if (!__instance.IsPlayerCharacter && __instance.IsHero)
                {
                    Hero hero = __instance.HeroObject;
                    ReflectUtils.ReflectPropertyAndSetValue("StaticBodyProperties", properties.StaticProperties, hero);
                    hero.Weight = properties.Weight;
                    hero.Build = properties.Build;
                    hero.BirthDay = HeroHelper.GetRandomBirthDayForAge(properties.Age); 
                    hero.UpdatePlayerGender(isFemale);
                }
            }
        }
    }
}
