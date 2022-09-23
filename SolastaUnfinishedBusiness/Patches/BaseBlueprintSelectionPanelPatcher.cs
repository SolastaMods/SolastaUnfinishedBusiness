using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class BaseBlueprintSelectionPanelPatcher
{
    [HarmonyPatch(typeof(BaseBlueprintSelectionPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Postfix(BaseBlueprintSelectionPanel __instance)
        {
            //PATCH: open DM content folded (DMP)
            if (!Main.Settings.EnableDungeonMakerModdedContent)
            {
                return;
            }

            __instance.FoldAll();
        }
    }
}
