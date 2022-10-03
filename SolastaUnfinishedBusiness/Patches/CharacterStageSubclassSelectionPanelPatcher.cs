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
        private static Vector2 OriginalAnchoredPosition { get; set; } = Vector2.zero;

        public static void Prefix([NotNull] CharacterStageSubclassSelectionPanel __instance)
        {
            //PATCH: sorts the sub classes panel by Title
            if (Main.Settings.EnableSortingSubclasses)
            {
                __instance.compatibleSubclasses
                    .Sort((a, b) =>
                        String.Compare(a.FormatTitle(), b.FormatTitle(), StringComparison.CurrentCultureIgnoreCase));
            }

            //PATCH: changes the subclasses layout to allow more offerings
            var gridLayoutGroup = __instance.subclassesTable.GetComponent<GridLayoutGroup>();
            var count = __instance.compatibleSubclasses.Count;

            if (OriginalAnchoredPosition == Vector2.zero)
            {
                OriginalAnchoredPosition = __instance.subclassesTable.anchoredPosition;
            }

            if (count > 8)
            {
                gridLayoutGroup.constraintCount = 3;
                //__instance.subclassesTable.anchoredPosition = new Vector2(0, +15);
                __instance.subclassesTable.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
            else
            {
                gridLayoutGroup.constraintCount = 2;
                //__instance.subclassesTable.anchoredPosition = OriginalAnchoredPosition;
                __instance.subclassesTable.localScale = new Vector3(1f, 1f, 1f);
            }
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
