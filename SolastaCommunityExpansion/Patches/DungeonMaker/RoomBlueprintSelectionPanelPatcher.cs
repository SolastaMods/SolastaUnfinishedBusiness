using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    // better rooms sorting
    [HarmonyPatch(typeof(RoomBlueprintSelectionPanel), "Compare")]
    internal static class RoomBlueprintSelectionPanelCompare
    {
        internal static bool Prefix(RoomBlueprint left, RoomBlueprint right, ref int __result)
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
