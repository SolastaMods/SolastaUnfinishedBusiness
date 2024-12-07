using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Lunger : AbstractFightingStyle
{
    internal const string Name = "Lunger";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(Name)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(Name, Resources.Lunger, 256))
        .AddCustomSubFeatures(new IncreaseWeaponReach(1, (attackMode, rulesetItem, character) =>
            {
                // this extra code is required to properly validate before an attack
                // same code as ValidatorsWeapon.IsMelee but need this later to check on weapon tag
                var finalRulesetItem =
                    attackMode?.SourceObject as RulesetItem ?? rulesetItem ?? character?.GetMainWeapon();

                return
                    ValidatorsCharacter.HasFreeHandConsiderGrapple(character) &&
                    ValidatorsWeapon.IsMelee(finalRulesetItem?.ItemDefinition) &&
                    !ValidatorsWeapon.HasAnyWeaponTag(
                        finalRulesetItem?.ItemDefinition, TagsDefinitions.WeaponTagHeavy);
            },
            Name))
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        ClassesContext.FightingStyleChoiceBarbarian,
        ClassesContext.FightingStyleChoiceMonk,
        ClassesContext.FightingStyleChoiceRogue,
        FightingStyleChampionAdditional,
        FightingStyleFighter,
        FightingStylePaladin,
        FightingStyleRanger
    ];
}
