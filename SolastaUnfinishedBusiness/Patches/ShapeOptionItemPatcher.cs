using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Subclasses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ShapeOptionItemPatcher
{
    [HarmonyPatch(typeof(ShapeOptionItem), nameof(ShapeOptionItem.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            ShapeOptionItem __instance,
            MonsterDefinition shapeDefinition,
            RulesetCharacter shifter,
            int requiredLevel = 0)
        {
            //PATCH: uses class level when offering wildshape and handle a special Circle of the Night case
            if (shifter is not RulesetCharacterHero rulesetCharacterHero ||
                !rulesetCharacterHero.ClassesAndLevels.TryGetValue(Druid, out var levels))
            {
                return;
            }

            // special Circle of the Night that requires 2 shapes available on some forms
            var isCircleOfTheNight = shifter.GetSubclassLevel(Druid, CircleOfTheNight.Name) > 0;
            var power = isCircleOfTheNight
                ? CircleOfTheNight.PowerCircleOfTheNightWildShapeCombat
                : PowerDruidWildShape;

            var rulesetUsablePower = PowerProvider.Get(power, rulesetCharacterHero);
            var hasAtLeastTwoShapes = shifter.GetRemainingUsesOfPower(rulesetUsablePower) > 1;
            var isShapeOptionAvailable =
                requiredLevel <= levels &&
                (!isCircleOfTheNight || !shapeDefinition.Name.EndsWith("Elemental") || hasAtLeastTwoShapes);

            __instance.levelLabel.TMP_Text.color = isShapeOptionAvailable
                ? __instance.validLevelColor
                : __instance.invalidLevelColor;
            __instance.toggle.interactable = isShapeOptionAvailable;
            __instance.canvasGroup.alpha = isShapeOptionAvailable ? 1f : 0.3f;
        }
    }
}
