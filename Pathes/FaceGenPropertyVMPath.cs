using CharacterReload.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace CharacterReload.Pathes
{
    [HarmonyPatch(typeof(FaceGenPropertyVM), "Name", MethodType.Getter)]
     class FaceGenPropertyVMNamePath
    {
        public static bool Prefix(ref string __result,  ref FaceGenPropertyVM __instance)
        {
            TextObject nameObj = ReflectUtils.ReflectField<TextObject>("_nameObj", __instance);
            float value = ReflectUtils.ReflectField<float>("_value", __instance);
            if (null != nameObj)
            {
                __result =  string.Format("{0}({1})", nameObj.ToString(), value.ToString("F2"));
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(FaceGenPropertyVM), "Value", MethodType.Setter)]
    class FaceGenPropertyVMValuePath
    {

        public static void Postfix( ref FaceGenPropertyVM __instance)
        {
            __instance.OnPropertyChanged( "Name");
        }
    }
}
