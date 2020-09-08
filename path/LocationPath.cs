using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SueMoreSpouses.patch
{
    [HarmonyPatch(typeof(Location), "DeserializeDelegate")]
    class LocatonDeserializeDelegatePath
    {

        public static bool Prefix(ref Location __instance, string text)
        {
            if (null == text)
            {
                return false;
            }
            else
            {
                // InformationManager.DisplayMessage(new InformationMessage(text));
            }

            return true;

        }
    }

    [HarmonyPatch(typeof(Location), "GetLocationCharacter", new Type[] { typeof(IAgentOriginBase) })]
    class LocatonGetLocationCharacterPath
    {

         public static bool Prefix(ref LocationCharacter __result, ref Location __instance)
        {
           // InformationManager.DisplayMessage(new InformationMessage(__instance.ToString()));
            Traverse traverse = HarmonyLib.Traverse.Create(__instance);
            List<LocationCharacter> agentOrigin = traverse.Field<List<LocationCharacter>>("_characterList").Value;
            if (null == agentOrigin)
            {
                __result = null;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Location), "GetLocationCharacter", new Type[] { typeof(IAgentOriginBase) })]
    class LocatonGetLocationCharacterPath
    {

         public static bool Prefix(ref LocationCharacter __result, ref Location __instance)
        {
           // InformationManager.DisplayMessage(new InformationMessage(__instance.ToString()));
            Traverse traverse = HarmonyLib.Traverse.Create(__instance);
            List<LocationCharacter> agentOrigin = traverse.Field<List<LocationCharacter>>("_characterList").Value;
            if (null == agentOrigin)
            {
                __result = null;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Location), "CanAIExit")]
    class LocatonCanAIExitPath
    {

        public static bool Prefix(ref Location __instance, LocationCharacter character)
        {
            if (null == character || null == __instance || null == __instance.Name)
            {
                if (null != __instance && null != __instance.Name)
                {
                    InformationManager.DisplayMessage(new InformationMessage("CanAIExit" + __instance.Name.ToString() + "  LocationCharacter= null "));

                }
                return false;
            }
            return true;

        }
    }

    [HarmonyPatch(typeof(LocationComplex), "GetLocationOfCharacter", new Type[] { typeof(LocationCharacter) })]
    class LocationComplexGetLocationOfCharacterPath
    {

        [HarmonyPrefix]
        public static bool Prefix(ref Location __result, ref LocationComplex __instance, LocationCharacter character)
        {

            FieldInfo fieldInfo = __instance.GetType().GetField("_locations", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Dictionary<string, Location> loations = null;
            IEnumerable<LocationCharacter> list = __instance.GetListOfCharacters();
            if (null != fieldInfo)
            {
                loations = (Dictionary<string, Location>)fieldInfo.GetValue(__instance);
                if (null != loations)
                {
                    foreach (KeyValuePair<string, Location> current in loations)
                    {
                        if (current.Value.ContainsCharacter(character))
                        {
                            __result = current.Value;
                        }
                    }
                }
            }
            if (null != __result)
            {
                return false;
            }
            return true;
        }
    }


}
