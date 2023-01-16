using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStageSubclassSelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel),
        nameof(CharacterStageSubclassSelectionPanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] CharacterStageSubclassSelectionPanel __instance)
        {
            //PATCH: changes the subclasses layout to allow more offering
            var table = __instance.subclassesTable;
            var gridLayoutGroup = table.GetComponent<GridLayoutGroup>();
            var rectTransform = table.parent.parent.parent.GetComponent<RectTransform>();
            var count = __instance.compatibleSubclasses.Count;

            gridLayoutGroup.spacing = new Vector2(50, 100);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 30f);

            switch (count)
            {
                case > 10:
                {
                    const float TWO_THIRDS = 2 / 3f;

                    gridLayoutGroup.constraintCount = 3;
                    rectTransform.localScale = new Vector3(TWO_THIRDS, TWO_THIRDS, TWO_THIRDS);
                    break;
                }
                case > 8:
                    gridLayoutGroup.constraintCount = 3;
                    rectTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    break;
                case > 6:
                    gridLayoutGroup.constraintCount = 3;
                    rectTransform.localScale = Vector3.one;
                    break;
                default:
                    gridLayoutGroup.constraintCount = 2;
                    rectTransform.localScale = Vector3.one;
                    break;
            }

            //PATCH: sorts the sub classes panel by Title
            if (!Main.Settings.EnableSortingSubclasses)
            {
                return;
            }

            __instance.compatibleSubclasses
                .Sort((a, b) =>
                    String.Compare(a.FormatTitle(), b.FormatTitle(), StringComparison.CurrentCultureIgnoreCase));
        }
    }

    [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel),
        nameof(CharacterStageSubclassSelectionPanel.UpdateRelevance))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpdateRelevance_Patch
    {
        [UsedImplicitly]
        public static void Postfix([NotNull] CharacterStageSubclassSelectionPanel __instance)
        {
            //PATCH: updates this panel relevance (MULTICLASS)
            if (LevelUpContext.IsLevelingUp(__instance.currentHero)
                && LevelUpContext.RequiresDeity(__instance.currentHero))
            {
                __instance.isRelevant = false;
            }
        }
    }
}
