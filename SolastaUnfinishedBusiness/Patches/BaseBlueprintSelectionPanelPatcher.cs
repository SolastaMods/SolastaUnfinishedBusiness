#if false
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

[HarmonyPatch(typeof(BaseBlueprintSelectionPanel), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class BaseBlueprintSelectionPanel_Bind
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
#endif
