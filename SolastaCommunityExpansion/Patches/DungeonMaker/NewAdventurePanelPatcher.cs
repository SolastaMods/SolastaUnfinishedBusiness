using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    // this patch changes the min/max requirements on campaigns
    [HarmonyPatch(typeof(NewAdventurePanel), "SelectCampaign")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class NewAdventurePanel_SelectCampaign
    {
        internal static void Prefix(UserCampaign userCampaign)
        {
            if (userCampaign != null && Main.Settings.OverrideMinMaxLevel)
            {
                userCampaign.StartLevelMin = DungeonMakerContext.DUNGEON_MIN_LEVEL;
                userCampaign.StartLevelMax = DungeonMakerContext.DUNGEON_MAX_LEVEL;
            }
        }
    }

    // this patch changes the min/max requirements on locations
    [HarmonyPatch(typeof(NewAdventurePanel), "SelectUserLocation")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class NewAdventurePanel_SelectUserLocation
    {
        internal static void Prefix(UserLocation userLocation)
        {
            if (userLocation != null && Main.Settings.OverrideMinMaxLevel)
            {
                userLocation.StartLevelMin = DungeonMakerContext.DUNGEON_MIN_LEVEL;
                userLocation.StartLevelMax = DungeonMakerContext.DUNGEON_MAX_LEVEL;
            }
        }
    }
}
