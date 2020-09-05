using CharacterReload.View;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Library;

namespace CharacterReload
{
    internal class MBBindingLordItemListInterceptor : MBBindingList<ClanLordItemVM>
    {
        protected override void InsertItem(int index, ClanLordItemVM item)
        {
            try
            {
                if (item != null)
                {
                    Traverse traverse = Traverse.Create(item);
                    Action<ClanLordItemVM> value = traverse.Field<Action<ClanLordItemVM>>("_onCharacterSelect").Value;
                    MyClanLordItemVM lordItem = new MyClanLordItemVM(item.GetHero(), value);
                    base.InsertItem(index, lordItem);
                }
            }
            catch (Exception e)
            {
                FileLog.Log(e.ToString());
            }
        }
    }
}
