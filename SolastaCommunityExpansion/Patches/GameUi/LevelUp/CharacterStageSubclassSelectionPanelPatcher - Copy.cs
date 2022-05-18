using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(CharacterStageFightingStyleSelectionPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageFightingStyleSelectionPanel_OnBeginShow
    {
        internal static void Prefix(List<FightingStyleDefinition> ___compatibleFightingStyles, RectTransform ___fightingStylesTable)
        {
            if (Main.Settings.EnableSortingFightingStyles)
            {
                ___compatibleFightingStyles
                    .Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
            }

            var count = ___compatibleFightingStyles.Count;

            if (count > 8)
            {
                ___fightingStylesTable.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
            else
            {
                ___fightingStylesTable.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}
