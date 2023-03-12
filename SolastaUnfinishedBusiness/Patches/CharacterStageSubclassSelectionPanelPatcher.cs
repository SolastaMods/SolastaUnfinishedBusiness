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
            var tableParent = table.parent;
            var gridLayoutGroup = table.GetComponent<GridLayoutGroup>();
            var rectTransform = tableParent.parent.parent.GetComponent<RectTransform>();
            var mask = tableParent.GetComponent<Mask>();

            gridLayoutGroup.spacing = new Vector2(50, 100);
            gridLayoutGroup.constraintCount = 3;
            rectTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 30f);
            mask.rectTransform.sizeDelta = new Vector2(0, 250);

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
