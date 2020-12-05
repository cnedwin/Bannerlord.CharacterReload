using HarmonyLib;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;


namespace CharacterCreation.Patches
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
                    if (hero.IsChild  && hero.MapFaction.IsBanditFaction)
                    {
                        float banditAge = 21;
                        DynamicBodyProperties bandit = new DynamicBodyProperties(banditAge, hero.Weight, hero.Build);
                        hero.BodyProperties.DynamicProperties.Equals(bandit);
                        BodyProperties heroBodyProperties = new BodyProperties(bandit, hero.BodyProperties.StaticProperties);
                        hero.CharacterObject.UpdatePlayerCharacterBodyProperties(heroBodyProperties, hero.IsFemale);
                    }

                    if (!hero.IsChild  && (hero.IsHumanPlayerCharacter || hero.IsPlayerCompanion))
                    {

                    CampaignTime deltaTime = CharacterReload.SubModule.GetDeltaTime(true);
                    double yearsElapsed = deltaTime.ToYears;

                    DynamicBodyProperties dynamicBodyProperties = new DynamicBodyProperties(hero.Age, hero.Weight, hero.Build);
                    BodyProperties heroBodyProperties = new BodyProperties(hero.BodyProperties.DynamicProperties, hero.BodyProperties.StaticProperties);
                    hero.CharacterObject.UpdatePlayerCharacterBodyProperties(heroBodyProperties, hero.IsFemale);
                    }
                }

            return false;
        }
    }
}