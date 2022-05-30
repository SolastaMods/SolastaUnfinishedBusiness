using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.EditorSort;

// better gadget sorting
[HarmonyPatch(typeof(GadgetBlueprintSelectionPanel), "Compare")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GadgetBlueprintSelectionPanel_Compare
{
    internal static bool Prefix(GadgetBlueprint left, GadgetBlueprint right, ref int __result)
    {
        if (!Main.Settings.EnableSortingDungeonMakerAssets)
        {
            return true;
        }

        __result = DmProEditorContext.Compare(left, right);

        return false;
    }
}
