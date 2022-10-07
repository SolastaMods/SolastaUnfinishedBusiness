using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterStageSubclassSelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow_Patch
    {
        public static void Prefix([NotNull] CharacterStageSubclassSelectionPanel __instance)
        {
            //PATCH: sorts the sub classes panel by Title
            if (Main.Settings.EnableSortingSubclasses)
            {
                __instance.compatibleSubclasses
                    .Sort((a, b) =>
                        String.Compare(a.FormatTitle(), b.FormatTitle(), StringComparison.CurrentCultureIgnoreCase));
            }

            //PATCH: changes the subclasses layout to allow more offering
            var rectTransform = __instance.subclassesTable.parent.parent.parent.GetComponent<RectTransform>();
            var gridLayoutGroup = __instance.subclassesTable.GetComponent<GridLayoutGroup>();

            rectTransform.anchoredPosition = new Vector2(-245.5f, 15f);
            gridLayoutGroup.spacing = new Vector2(50, 100);
            gridLayoutGroup.constraintCount = 3;
        }
    }

    [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "UpdateRelevance")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class UpdateRelevance_Patch
    {
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
