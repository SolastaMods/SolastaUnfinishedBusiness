using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Wildshape
{
    // uses class level when offering wildshape
    [HarmonyPatch(typeof(ShapeOptionItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ShapeOptionItem_Bind
    {
        internal static void Postfix(
            ShapeOptionItem __instance,
            RulesetCharacter shifter,
            int requiredLevel)
        {
            if (shifter is RulesetCharacterHero rulesetCharacterHero
                && rulesetCharacterHero.ClassesAndLevels.TryGetValue(Druid, out var levels))
            {
                var isShapeOptionAvailable = requiredLevel <= levels;

                __instance.levelLabel.TMP_Text.color =
                    isShapeOptionAvailable ? __instance.validLevelColor : __instance.invalidLevelColor;
                __instance.toggle.interactable = isShapeOptionAvailable;
                __instance.canvasGroup.alpha = isShapeOptionAvailable ? 1f : 0.3f;
            }
        }
    }
}
