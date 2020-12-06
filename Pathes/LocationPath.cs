using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CharacterReload.Patch
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
                //  InformationManager.DisplayMessage( new InformationMessage("DeserializeDelegate" +  text));
            }

            return true;

        }
    }

    [HarmonyPatch(typeof(Location), "CanAIExit")]
    class LocatonCanAIExitPath
    {

        public static bool Prefix(ref Location __instance, LocationCharacter character)
        {
            if (null == __instance || null == __instance.Name)
            {
                if (null != __instance)
                {
                    Traverse traverse = HarmonyLib.Traverse.Create(__instance);
                    CanUseDoor aiCanExitDelegate = traverse.Field<CanUseDoor>("_aiCanExitDelegate").Value;
                    String aiCanExit = traverse.Field<String>("_aiCanExit").Value;
                    if (null == aiCanExitDelegate && null == aiCanExit)
                    {
                        return false;
                    }
                }
            }
            return true;

        }
    }


    [HarmonyPatch(typeof(Location), "CanPlayerSee")]
    class LocatonCanPlayerSeePath
    {

        public static bool Prefix(ref Location __instance)
        {
            if (null != __instance)
            {
                Traverse traverse = HarmonyLib.Traverse.Create(__instance);
                CanUseDoor _playerCanSeeDelegate = traverse.Field<CanUseDoor>("_playerCanSeeDelegate").Value;
                String _playerCanSee = traverse.Field<String>("_playerCanSee").Value;
                if (null == _playerCanSeeDelegate && null == _playerCanSee)
                {
                    return false;
                }

            }
            return true;

        }
    }


    [HarmonyPatch(typeof(Location), "CanPlayerEnter")]
    class LocatonCanPlayerEnterPath
    {

        public static bool Prefix(ref Location __instance)
        {
            if (null != __instance)
            {
                Traverse traverse = HarmonyLib.Traverse.Create(__instance);
                CanUseDoor _playerCanEnterDelegate = traverse.Field<CanUseDoor>("_playerCanEnterDelegate").Value;
                String _playerCanEnter = traverse.Field<String>("_playerCanEnter").Value;
                if (null == _playerCanEnterDelegate && null == _playerCanEnter)
                {
                    return false;
                }

            }
            return true;

        }
    }

    [HarmonyPatch(typeof(Location), "CanAIEnter")]
    class LocatonCanAIEnterPath
    {

        public static bool Prefix(ref Location __instance, LocationCharacter character)
        {
            if (null == __instance || null == __instance.Name)
            {
                if (null != __instance)
                {
                    Traverse traverse = HarmonyLib.Traverse.Create(__instance);
                    CanUseDoor aiCanEnterDelegate = traverse.Field<CanUseDoor>("_aiCanEnterDelegate").Value;
                    String aiCanEnter = traverse.Field<String>("_aiCanEnter").Value;
                    if (null == aiCanEnterDelegate && null == aiCanEnter)
                    {
                        return false;
                    }

                }

            }
            return true;

        }
    }

    [HarmonyPatch(typeof(Location), "ContainsCharacter", new Type[] { typeof(LocationCharacter) })]
    class LocationContainsCharacterrPath
    {

        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, ref Location __instance)
        {
            Traverse traverse = HarmonyLib.Traverse.Create(__instance);
            List<LocationCharacter> _characterList = traverse.Field<List<LocationCharacter>>("_characterList").Value;
            if (null == _characterList)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(Location), "GetLocationCharacter", new Type[] { typeof(IAgentOriginBase) })]
    class LocationGetLocationCharacterPath
    {

        [HarmonyPrefix]
        public static bool Prefix(ref LocationCharacter __result, ref Location __instance)
        {
            Traverse traverse = HarmonyLib.Traverse.Create(__instance);
            List<LocationCharacter> _characterList = traverse.Field<List<LocationCharacter>>("_characterList").Value;
            if (null == _characterList)
            {
                __result = null;
                return false;
            }
            return true;
        }
    }


}
