using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;

namespace CharacterReload.Patch
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

    [HarmonyPatch(typeof(ClanLordItemVM), "OnNamingHeroOver")]
    class ClanLordItemVMPatch
    {
        [HarmonyPostfix]
        public static void OnNamingHeroOverPostfix(ClanLordItemVM __instance, string suggestedName)
        {
            if (!CampaignUIHelper.IsStringApplicableForHeroName(suggestedName)) return;
            Hero selectedHero = __instance.GetHero();
            selectedHero.FirstName = selectedHero.Name;
        }
    }
}
