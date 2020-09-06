using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace BowAndCross.patch
{
    [HarmonyPatch(typeof(SPInventoryVM), "InitializeInventory")]
    internal class InitializeInventoryCorrection
    {
        public static void Postfix(ref SPInventoryVM __instance)
        {
            bool flag = Tips.PlayerPartyHasPerk(DefaultPerks.Roguery.ConcealedBlade);
            if (flag)
            {
                for (int i = 0; i < __instance.LeftItemListVM.Count; i++)
                {
                    bool flag2 = __instance.LeftItemListVM.ElementAt(i).ItemRosterElement.EquipmentElement.Item.ItemType == 2;
                    if (flag2)
                    {
                        __instance.LeftItemListVM.ElementAt(i).IsCivilianItem = true;
                    }
                }
                for (int j = 0; j < __instance.RightItemListVM.Count; j++)
                {
                    bool flag3 = __instance.RightItemListVM.ElementAt(j).ItemRosterElement.EquipmentElement.Item.ItemType == 2;
                    if (flag3)
                    {
                        __instance.RightItemListVM.ElementAt(j).IsCivilianItem = true;
                    }
                }
            }
        }

        public InitializeInventoryCorrection()
        {
        }
    }

    [HarmonyPatch(typeof(SPInventoryVM), "AfterTransfer")]
    internal class InitializeInventoryTransferCorrection
    {
        public static void Postfix(ref SPInventoryVM __instance, InventoryLogic inventoryLogic, List<TransferCommandResult> results)
        {
            bool flag = Tips.PlayerPartyHasPerk(DefaultPerks.Roguery.ConcealedBlade);
            if (flag)
            {
                for (int i = 0; i < __instance.LeftItemListVM.Count; i++)
                {
                    bool flag2 = __instance.LeftItemListVM.ElementAt(i).ItemRosterElement.EquipmentElement.Item.ItemType == 2;
                    if (flag2)
                    {
                        __instance.LeftItemListVM.ElementAt(i).IsCivilianItem = true;
                    }
                }
                for (int j = 0; j < __instance.RightItemListVM.Count; j++)
                {
                    bool flag3 = __instance.RightItemListVM.ElementAt(j).ItemRosterElement.EquipmentElement.Item.ItemType == 2;
                    if (flag3)
                    {
                        __instance.RightItemListVM.ElementAt(j).IsCivilianItem = true;
                    }
                }
            }
        }

        public InitializeInventoryTransferCorrection()
        {
        }
    }

    [HarmonyPatch(typeof(Agent), "WeaponEquipped")]
    internal class PerkCorrections
    {
        public static bool Prefix(ref Agent __instance, EquipmentIndex equipmentSlot, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, in WeaponData ammoWeaponData, WeaponStatsData[] ammoWeaponStatsData, GameEntity weaponEntity, bool removeOldWeaponFromScene, bool isWieldedOnSpawn)
        {
            bool flag = weaponStatsData != null;
            if (flag)
            {
                for (int i = 0; i < weaponStatsData.Length; i++)
                {
                    bool flag2 = weaponStatsData[i].WeaponClass == 16 && Tips.CharacterHasPerk(__instance.Character, DefaultPerks.Crossbow.MountedCrossbowman);
                    if (flag2)
                    {
                        weaponStatsData[i].WeaponFlags = (weaponStatsData[i].WeaponFlags & 18446744073709289471UL);
                    }
                    else
                    {
                        bool flag3 = weaponStatsData[i].ItemUsageIndex == Tips.longBowIndex && Tips.CharacterHasPerk(__instance.Character, DefaultPerks.Bow.HorseMaster);
                        if (flag3)
                        {
                            weaponStatsData[i].ItemUsageIndex = Tips.bowIndex;
                        }
                    }
                }
            }
            return true;
        }
        public PerkCorrections()
        {
        }
    }

    public static class Tips
    {
        public static void Load()
        {
            Tips.bowIndex = MBItem.GetItemUsageIndex("bow");
            Tips.longBowIndex = MBItem.GetItemUsageIndex("long_bow");
        }

        public static bool HasPerk(CharacterObject character, PerkObject perk)
        {
            bool flag = character != null && perk != null;
            return flag && character.GetPerkValue(perk);
        }

        public static bool CharacterHasPerk(BasicCharacterObject character, PerkObject perk)
        {
            return Tips.HasPerk((CharacterObject)character, perk);
        }

        public static bool PlayerPartyHasPerk(PerkObject perk)
        {
            bool flag = Tips.HasPerk(Hero.MainHero.CharacterObject, perk);
            int num = Hero.MainHero.CompanionsInParty.Count<Hero>();
            int num2 = 0;
            while (!flag && num2 < num)
            {
                Hero hero = Hero.MainHero.CompanionsInParty.ElementAt(num2);
                bool flag2 = hero != null;
                if (flag2)
                {
                    bool flag3 = hero.CharacterObject != null;
                    if (flag3)
                    {
                        bool flag4 = Tips.HasPerk(hero.CharacterObject, perk);
                        if (flag4)
                        {
                            flag = true;
                        }
                    }
                }
                num2++;
            }
            return flag;
        }

        public static void LogMessage(string message)
        {
            InformationManager.DisplayMessage(new InformationMessage(message));
        }

        public static int bowIndex;

        public static int longBowIndex;
    }


}
