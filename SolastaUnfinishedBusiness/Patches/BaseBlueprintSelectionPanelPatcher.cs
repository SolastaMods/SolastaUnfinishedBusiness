using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class BaseBlueprintSelectionPanelPatcher
{
    [HarmonyPatch(typeof(BaseBlueprintSelectionPanel), nameof(BaseBlueprintSelectionPanel.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
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
