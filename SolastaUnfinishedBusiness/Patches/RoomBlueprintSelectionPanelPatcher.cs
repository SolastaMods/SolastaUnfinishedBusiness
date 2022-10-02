using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class RoomBlueprintSelectionPanelPatcher
{
    [HarmonyPatch(typeof(RoomBlueprintSelectionPanel), "Compare")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Compare_Patch
    {
        public static bool Prefix(RoomBlueprint left, RoomBlueprint right, ref int __result)
        {
            //PATCH: better rooms sorting (DMP)
            if (!Main.Settings.EnableSortingDungeonMakerAssets)
            {
                return true;
            }

            __result = DmProEditorContext.Compare(left, right);

            return false;
        }
    }
}
