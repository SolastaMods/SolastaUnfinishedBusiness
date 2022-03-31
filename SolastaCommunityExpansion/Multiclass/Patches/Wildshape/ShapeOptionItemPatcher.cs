using HarmonyLib;
using SolastaCommunityExpansion;
using UnityEngine;
using UnityEngine.UI;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaMulticlass.Patches.Wildshape
{
    internal static class ShapeOptionItemPatcher
    {
        // uses class level when offering wildshape
        [HarmonyPatch(typeof(ShapeOptionItem), "Bind")]
        internal static class ShapeOptionItemBind
        {
            internal static void Postfix(
                GuiLabel ___levelLabel,
                Color ___validLevelColor,
                Color ___invalidLevelColor,
                Toggle ___toggle,
                CanvasGroup ___canvasGroup,
                RulesetCharacter shifter,
                int requiredLevel = 0)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var isShapeOptionAvailable = shifter is RulesetCharacterHero rulesetCharacterHero && requiredLevel <= rulesetCharacterHero.ClassesAndLevels[Druid];
                
                ___levelLabel.TMP_Text.color = isShapeOptionAvailable ? ___validLevelColor : ___invalidLevelColor;
                ___toggle.interactable = isShapeOptionAvailable;
                ___canvasGroup.alpha = isShapeOptionAvailable ? 1f : 0.3f;
            }
        }
    }
}
