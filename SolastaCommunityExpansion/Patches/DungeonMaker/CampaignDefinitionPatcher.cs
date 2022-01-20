using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    // this patch changes the min requirements on official campaigns
    [HarmonyPatch(typeof(CampaignDefinition), "MinStartLevel", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CampaignDefinition_MinStartLevel_Getter
    {
        internal static void Postfix(ref int __result)
        {
            if (Main.Settings.OverrideMinMaxLevel)
            {
                __result = Models.DungeonMakerContext.DUNGEON_MIN_LEVEL;
            }
        }
    }

    // this patch changes the max requirements on official campaigns
    [HarmonyPatch(typeof(CampaignDefinition), "MaxStartLevel", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CampaignDefinition_MaxStartLevel_Getter
    {
        internal static void Postfix(ref int __result)
        {
            if (Main.Settings.OverrideMinMaxLevel)
            {
                __result = Models.DungeonMakerContext.DUNGEON_MAX_LEVEL;
            }
        }
    }
}
