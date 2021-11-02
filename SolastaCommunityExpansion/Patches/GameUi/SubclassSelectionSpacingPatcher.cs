using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUIUpdates.Patches
{
    // spacing
    class SubclassSelectionSpacingPatcher
    {
        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "EnterStage")]
        internal static class CharacterStageSubclassSelectionPanel_EnterStage
        {
            public static void Postfix(CharacterStageSubclassSelectionPanel __instance)
            {
                Transform subclassesTable = Traverse.Create(__instance).Field<Transform>("subclassesTable").Value;
                GridLayoutGroup subclassGrid = subclassesTable.GetComponent<GridLayoutGroup>();
                subclassGrid.spacing = new Vector2(subclassGrid.spacing.x, 60f);
            }
        }
    }
}
