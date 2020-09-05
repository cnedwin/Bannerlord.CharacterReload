using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;

namespace CharacterReload.path
{
    [HarmonyPatch(typeof(ClanMembersVM), "RefreshMembersList")]
    public class PatchClanMembersVM
    {
        public static void Prefix(ref ClanMembersVM __instance)
        {

            __instance.Family = new MBBindingLordItemListInterceptor(); ;
            __instance.Companions = new MBBindingLordItemListInterceptor(); ;

        }
    }
}
