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
            var rectTransform = __instance.fightingStylesTable.parent.parent.parent.GetComponent<RectTransform>();
            var gridLayoutGroup = __instance.fightingStylesTable.GetComponent<GridLayoutGroup>();

            rectTransform.anchoredPosition = new Vector2(-245.5f, 15f);
            gridLayoutGroup.spacing = new Vector2(50, 100);
            gridLayoutGroup.constraintCount = 3;
        }
    }
}
