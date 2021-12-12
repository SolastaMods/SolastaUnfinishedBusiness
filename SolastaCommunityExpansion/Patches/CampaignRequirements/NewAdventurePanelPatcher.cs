using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    // this patch changes the min/max requirements on campaigns
    [HarmonyPatch(typeof(NewAdventurePanel), "SelectCampaign")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class NewAdventurePanelSelectCampaign
    {
        internal static void Prefix(UserCampaign userCampaign)
        {
            if (userCampaign != null && Main.Settings.EnableDungeonLevelBypass)
            {
                userCampaign.StartLevelMin = Settings.DUNGEON_MIN_LEVEL;
                userCampaign.StartLevelMax = Settings.DUNGEON_MAX_LEVEL;
            }
        }
    }

    // this patch changes the min/max requirements on locations
    [HarmonyPatch(typeof(NewAdventurePanel), "SelectUserLocation")]
    internal static class NewAdventurePanelSelectUserLocation
    {
        internal static void Prefix(UserLocation userLocation)
        {
            if (userLocation != null && Main.Settings.EnableDungeonLevelBypass)
            {
                userLocation.StartLevelMin = Settings.DUNGEON_MIN_LEVEL;
                userLocation.StartLevelMax = Settings.DUNGEON_MAX_LEVEL;
            }
        }
    }
}
