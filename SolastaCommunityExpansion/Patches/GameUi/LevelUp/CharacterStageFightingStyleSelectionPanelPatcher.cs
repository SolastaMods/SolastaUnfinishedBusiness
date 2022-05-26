using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(CharacterStageFightingStyleSelectionPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageFightingStyleSelectionPanel_OnBeginShow
    {
        private static Vector2 OriginalAnchoredPosition { get; set; } = Vector2.zero;

        internal static void Prefix(List<FightingStyleDefinition> ___compatibleFightingStyles, RectTransform ___fightingStylesTable)
        {
            if (Main.Settings.EnableSortingFightingStyles)
            {
                ___compatibleFightingStyles
                    .Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
            }

            var gridLayoutGroup = ___fightingStylesTable.GetComponent<GridLayoutGroup>();
            var count = ___compatibleFightingStyles.Count;

            if (OriginalAnchoredPosition == Vector2.zero)
            {
                OriginalAnchoredPosition = ___fightingStylesTable.anchoredPosition;
            }

            if (count > 8)
            {
                gridLayoutGroup.constraintCount = 3;
                ___fightingStylesTable.anchoredPosition = new Vector2(0, +15);
                //___fightingStylesTable.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
            else
            {
                gridLayoutGroup.constraintCount = 2;
                ___fightingStylesTable.anchoredPosition = OriginalAnchoredPosition;
                //___fightingStylesTable.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}
