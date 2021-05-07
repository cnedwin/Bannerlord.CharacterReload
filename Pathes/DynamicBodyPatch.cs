using HarmonyLib;
using Helpers;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CharacterReload.Patch
{
    [HarmonyPatch(typeof(DynamicBodyCampaignBehavior), "OnDailyTick")]
    public static class DynamicBodyPatch
    {
        private static readonly Type HeroBehaviorsStructType = AccessTools.Inner(typeof(DynamicBodyCampaignBehavior), "HeroBehaviors");
        private static readonly FieldInfo LastSettlementVisitTimeField = AccessTools.Field(HeroBehaviorsStructType, "LastSettlementVisitTime"); // type: CampaignTime
        private static readonly FieldInfo InASettlementField = AccessTools.Field(HeroBehaviorsStructType, "InASettlement"); // type: bool
        private static readonly FieldInfo LastEncounterTimeField = AccessTools.Field(HeroBehaviorsStructType, "LastEncounterTime"); // type: CampaignTime
        private static readonly FieldInfo IsBattleEncounteredField = AccessTools.Field(HeroBehaviorsStructType, "IsBattleEncountered"); // type: bool

        //static bool Prefix(DynamicBodyCampaignBehavior __instance, ref Dictionary<Hero, object> ____heroBehaviorsDictionary)
        static bool Prefix(DynamicBodyCampaignBehavior __instance)
        {

            IDictionary dictionary = (IDictionary)AccessTools.Field(typeof(DynamicBodyCampaignBehavior), "_heroBehaviorsDictionary").GetValue(__instance);
                foreach (var hero in dictionary.Keys.Cast<Hero>())
                {
                //This is probably invalid because the bandit soldier is not a hero object
                if (hero.IsChild  && hero.MapFaction.IsBanditFaction)
                    {
                        float banditAge = 21;
                        hero.SetBirthDay(HeroHelper.GetRandomBirthDayForAge((int)banditAge));
                        DynamicBodyProperties banditDynamicBodyProperties = new DynamicBodyProperties(hero.Age, hero.Weight, hero.Build);
                        BodyProperties banditBodyProperties = new BodyProperties(banditDynamicBodyProperties, hero.BodyProperties.StaticProperties);
                        hero.CharacterObject.UpdatePlayerCharacterBodyProperties(banditBodyProperties, hero.IsFemale);
                    }

                    if (!hero.IsChild && (hero.IsHumanPlayerCharacter || hero.IsPlayerCompanion))
                    {
                        float heroAge = hero.Age + 0.0119047f;
                        DynamicBodyProperties dynamicBodyProperties = new DynamicBodyProperties(heroAge, hero.Weight, hero.Build);
                        BodyProperties heroBodyProperties = new BodyProperties(dynamicBodyProperties, hero.BodyProperties.StaticProperties);
                        hero.CharacterObject.UpdatePlayerCharacterBodyProperties(heroBodyProperties, hero.IsFemale);

                    }
                }

            return false;
        }

    }


}