using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterStageFightingStyleSelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageFightingStyleSelectionPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow_Patch
    {
        private static Vector2 OriginalAnchoredPosition { get; set; } = Vector2.zero;

        public static void Prefix([NotNull] CharacterStageFightingStyleSelectionPanel __instance)
        {
            //PATCH: sorts the fighting style panel by Title
            if (Main.Settings.EnableSortingFightingStyles)
            {
                __instance.compatibleFightingStyles
                    .Sort((a, b) =>
                        String.Compare(a.FormatTitle(), b.FormatTitle(), StringComparison.CurrentCultureIgnoreCase));
            }

            //PATCH: changes the fighting style layout to allow more offerings
            var gridLayoutGroup = __instance.fightingStylesTable.GetComponent<GridLayoutGroup>();
            var count = __instance.compatibleFightingStyles.Count;

            if (OriginalAnchoredPosition == Vector2.zero)
            {
                OriginalAnchoredPosition = __instance.fightingStylesTable.anchoredPosition;
            }

            if (count > 8)
            {
                gridLayoutGroup.constraintCount = 3;
                __instance.fightingStylesTable.anchoredPosition = new Vector2(0, +15);
                //__instance.fightingStylesTable.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
            else
            {
                gridLayoutGroup.constraintCount = 2;
                __instance.fightingStylesTable.anchoredPosition = OriginalAnchoredPosition;
                //__instance.fightingStylesTable.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}
