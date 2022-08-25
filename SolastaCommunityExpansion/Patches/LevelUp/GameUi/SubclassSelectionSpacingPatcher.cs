using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.LevelUp.GameUi;

[HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "EnterStage")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageSubclassSelectionPanel_EnterStage
{
    public static void Postfix([NotNull] CharacterStageSubclassSelectionPanel __instance)
    {
        // PATCH: adds more spacing in between subclasses badges
        var subclassesTable = __instance.subclassesTable;
        var subclassGrid = subclassesTable.GetComponent<GridLayoutGroup>();

        subclassGrid.spacing = new Vector2(subclassGrid.spacing.x, 60f);
    }
}
