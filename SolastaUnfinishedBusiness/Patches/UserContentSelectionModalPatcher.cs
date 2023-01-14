using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class UserContentSelectionModalPatcher
{
    [HarmonyPatch(typeof(UserContentSelectionModal), "EnumerateContents")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class PostLoadJson_Patch
    {
        //BUGFIX: Allows DM to export loot packs to other campaigns (DMP)
        public static bool Prefix(UserContentSelectionModal __instance)
        {
            __instance.contentsByType.Clear();
            __instance.groupsByType.Clear();

            foreach (var userLocation in __instance.SourceCampaign.UserLocations)
            {
                __instance.AddUserContent(userLocation);
            }

            foreach (var userQuest in __instance.SourceCampaign.UserQuests)
            {
                __instance.AddUserContent(userQuest);
            }

            foreach (var userDialog in __instance.SourceCampaign.UserDialogs)
            {
                __instance.AddUserContent(userDialog);
            }

            foreach (var userVariable in __instance.SourceCampaign.UserVariables)
            {
                __instance.AddUserContent(userVariable);
            }

            foreach (var userItem in __instance.SourceCampaign.UserItems)
            {
                __instance.AddUserContent(userItem);
            }

            foreach (var userMonster in __instance.SourceCampaign.UserMonsters)
            {
                __instance.AddUserContent(userMonster);
            }

            foreach (var userNpc in __instance.SourceCampaign.UserNpcs)
            {
                __instance.AddUserContent(userNpc);
            }

            foreach (var userMerchantInventory in __instance.SourceCampaign.UserMerchantInventories)
            {
                __instance.AddUserContent(userMerchantInventory);
            }

            foreach (var userLootPack in __instance.SourceCampaign.UserLootPacks)
            {
                __instance.AddUserContent(userLootPack);
            }

            while (__instance.contentGroupsTable.childCount < __instance.contentsByType.Count)
            {
                Gui.GetPrefabFromPool(__instance.contentGroupPrefab, __instance.contentGroupsTable);
            }

            var num = 0;

            foreach (var keyValuePair in __instance.contentsByType)
            {
                var component = __instance.contentGroupsTable.GetChild(num++).GetComponent<UserContentGroup>();

                component.Bind(keyValuePair.Key, keyValuePair.Value, __instance.ContentGroupSelected,
                    __instance.ContentGroupDeselected, __instance.ContentGroupContentSelectionChanged);
                __instance.groupsByType.Add(keyValuePair.Key, component);
            }

            __instance.RefreshNow();
            return false;
        }
    }
}
