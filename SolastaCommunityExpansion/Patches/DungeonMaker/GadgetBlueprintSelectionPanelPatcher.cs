using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    // better gadget sorting
    [HarmonyPatch(typeof(GadgetBlueprintSelectionPanel), "Compare")]
    internal static class GadgetBlueprintSelectionPanelCompare
    {
        internal static bool Prefix(GadgetBlueprint left, GadgetBlueprint right, ref int __result)
        {
            if (!Main.Settings.EnableSortingDungeonMakerAssets)
            {
                return true;
            }

            __result = Models.DmProEditorContext.Compare(left, right);

            return false;
        }
    }
}
