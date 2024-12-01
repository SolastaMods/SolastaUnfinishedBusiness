using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;

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

            campaign.userLocations.Sort(Sorting.CompareTitle);
            campaign.userDialogs.Sort(Sorting.CompareTitle);
            campaign.userItems.Sort(Sorting.CompareTitle);
            campaign.userMonsters.Sort(Sorting.CompareTitle);
            campaign.userNpcs.Sort(Sorting.CompareTitle);
            campaign.userQuests.Sort(Sorting.CompareTitle);
            campaign.userVariables.Sort(Sorting.CompareTitle);
            campaign.userLootPacks.Sort(Sorting.CompareTitle);
            campaign.userMerchantInventories.Sort(Sorting.CompareTitle);
        }
    }
}
