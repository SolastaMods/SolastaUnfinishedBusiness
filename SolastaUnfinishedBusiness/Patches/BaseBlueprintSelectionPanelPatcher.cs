using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class BaseBlueprintSelectionPanelPatcher
{
    [HarmonyPatch(typeof(BaseBlueprintSelectionPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix(BaseBlueprintSelectionPanel __instance)
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
