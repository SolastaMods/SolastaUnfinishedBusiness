using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Patches.PowersAndPools
{
    internal static class ShapeOptionItemPatcher
    {
        // uses class level when offering wildshape
        [HarmonyPatch(typeof(ShapeOptionItem), "Bind")]
        internal static class ShapeOptionItemBind
        {
            internal static void Postfix(ShapeOptionItem __instance, RulesetCharacter shifter, int requiredLevel = 0)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var isShapeOptionAvailable = true;

                if (requiredLevel > 0 && shifter is RulesetCharacterHero rulesetCharacterHero)
                {
                    isShapeOptionAvailable = requiredLevel <= rulesetCharacterHero.ClassesAndLevels[Druid];
                }

                var levelLabel = __instance.GetField<ShapeOptionItem, GuiLabel>("levelLabel");
                var validLevelColor = __instance.GetField<ShapeOptionItem, Color>("validLevelColor");
                var invalidLevelColor = __instance.GetField<ShapeOptionItem, Color>("invalidLevelColor");
                var toggle = __instance.GetField<ShapeOptionItem, Toggle>("toggle");
                var canvasGroup = __instance.GetField<ShapeOptionItem, CanvasGroup>("canvasGroup");

                levelLabel.TMP_Text.color = isShapeOptionAvailable ? validLevelColor : invalidLevelColor;
                toggle.interactable = isShapeOptionAvailable;
                canvasGroup.alpha = isShapeOptionAvailable ? 1f : 0.3f;
            }
        }
    }
}
