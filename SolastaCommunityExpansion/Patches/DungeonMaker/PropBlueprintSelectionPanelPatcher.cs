using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    // better props sorting
    [HarmonyPatch(typeof(PropBlueprintSelectionPanel), "Compare")]
    internal static class PropBlueprintSelectionPanelCompare
    {
        internal static bool Prefix(PropBlueprint left, PropBlueprint right, ref int __result)
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
