using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Pro
{
    // open DM content folded
    [HarmonyPatch(typeof(BaseBlueprintSelectionPanel), "Bind")]
    internal static class BaseBlueprintSelectionPanelBind
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
