using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "EnterStage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageSubclassSelectionPanel_EnterStage
    {
        public static void Postfix(CharacterStageSubclassSelectionPanel __instance)
        {
            var subclassesTable = __instance.GetField<Transform>("subclassesTable");
            var subclassGrid = subclassesTable.GetComponent<GridLayoutGroup>();

            subclassGrid.spacing = new Vector2(subclassGrid.spacing.x, 60f);
        }
    }
}
