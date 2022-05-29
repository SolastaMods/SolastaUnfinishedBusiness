using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Pro
{
    // open DM content folded
    [HarmonyPatch(typeof(BaseBlueprintSelectionPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BaseBlueprintSelectionPanel_Bind
    {
        internal static void Postfix(BaseBlueprintSelectionPanel __instance)
        {
            if (!Main.Settings.EnableDungeonMakerPro || !Main.Settings.EnableDungeonMakerModdedContent)
            {
                return;
            }

            __instance.FoldAll();
        }
    }
}
