using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace CharacterReload.Pathes
{
    public static class BodyGeneratorPatch
    {
        [HarmonyPatch(typeof(BodyGenerator), nameof(BodyGenerator.SaveCurrentCharacter))]
        private static class SaveCurrentCharacter
        {
            static bool Prefix(BodyGenerator __instance)
            {

                    __instance.Character.UpdatePlayerCharacterBodyProperties(__instance.CurrentBodyProperties, __instance.IsFemale);
                    if (__instance.Character is CharacterObject characterObject)
                    {
                        float bodyAge = __instance.CurrentBodyProperties.DynamicProperties.Age;
                        if (bodyAge < 18) { bodyAge = 18; };
                        characterObject.HeroObject.SetBirthDay(HeroHelper.GetRandomBirthDayForAge((int)bodyAge));
                    }
                    return false;

            }
        }
    }
}
