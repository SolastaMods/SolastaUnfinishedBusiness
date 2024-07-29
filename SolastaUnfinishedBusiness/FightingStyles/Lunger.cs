using System.Collections.Generic;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
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
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("FeatureLunger")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(new IncreaseWeaponReach(1, (mode, rulesetItem, _) =>
                {
                    RulesetItem attackModeRulesetItem = null;

                    if (mode?.SourceObject is RulesetItem rulesetItem1)
                    {
                        attackModeRulesetItem = rulesetItem1;
                    }

                    var item = attackModeRulesetItem ?? rulesetItem;

                    return ValidatorsWeapon.IsMelee(item) &&
                           !ValidatorsWeapon.HasAnyWeaponTag(item, TagsDefinitions.WeaponTagHeavy);
                }, Name))
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        CharacterContext.FightingStyleChoiceBarbarian,
        CharacterContext.FightingStyleChoiceMonk,
        CharacterContext.FightingStyleChoiceRogue,
        FightingStyleChampionAdditional,
        FightingStyleFighter,
        FightingStylePaladin,
        FightingStyleRanger
    ];
}
