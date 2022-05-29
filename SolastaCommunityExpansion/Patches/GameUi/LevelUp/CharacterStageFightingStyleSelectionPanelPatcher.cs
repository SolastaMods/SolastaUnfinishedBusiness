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

        internal static void Prefix(CharacterStageFightingStyleSelectionPanel __instance)
        {
            if (Main.Settings.EnableSortingFightingStyles)
            {
                __instance.compatibleFightingStyles
                    .Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
            }

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
