using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UserCampaignEditorScreenPatcher
{
    [HarmonyPatch(typeof(UserCampaignEditorScreen), nameof(UserCampaignEditorScreen.Show))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Show_Patch
    {
        //PATCH: better user campaign assets sorting (DMP)
        [UsedImplicitly]
        public static void Prefix(UserCampaign campaign)
        {
            if (!Main.Settings.EnableSortingDungeonMakerAssets)
            {
                return;
            }

            campaign.userLocations.Sort((a, b) =>
                String.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
            campaign.userDialogs.Sort((a, b) =>
                String.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
            campaign.userItems.Sort((a, b) =>
                String.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
            campaign.userMonsters.Sort((a, b) =>
                String.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
            campaign.userNpcs.Sort((a, b) =>
                String.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
            campaign.userQuests.Sort((a, b) =>
                String.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
            campaign.userVariables.Sort((a, b) =>
                String.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
            campaign.userLootPacks.Sort((a, b) =>
                String.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
            campaign.userMerchantInventories.Sort((a, b) =>
                String.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
