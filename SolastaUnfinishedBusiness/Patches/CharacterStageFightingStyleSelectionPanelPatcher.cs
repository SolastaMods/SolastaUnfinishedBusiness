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
            //PATCH: changes the fighting style layout to allow more offerings
            var gridLayoutGroup = __instance.fightingStylesTable.GetComponent<GridLayoutGroup>();
            var rectTransform = __instance.fightingStylesTable.GetComponent<RectTransform>();

            // line counts on display
            gridLayoutGroup.constraintCount = 3;

            if (__instance.compatibleFightingStyles.Count > 12)
            {
                rectTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
            else
            {
                rectTransform.localScale = Vector3.one;
            }

            //PATCH: sorts the fighting style panel by Title
            if (!Main.Settings.EnableSortingFightingStyles)
            {
                return;
            }

            __instance.compatibleFightingStyles
                .Sort((a, b) =>
                    String.Compare(a.FormatTitle(), b.FormatTitle(), StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
