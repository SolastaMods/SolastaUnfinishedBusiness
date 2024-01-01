using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class ZenArcher : AbstractFightingStyle
{
    private const string ZenArcherName = "ZenArcher";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(ZenArcherName)
        .SetGuiPresentation(Category.FightingStyle, DatabaseHelper.FightingStyleDefinitions.Archery)
        .SetFeatures(
            FeatureDefinitionAttackModifierBuilder
                .Create($"Feature{ZenArcherName}")
                .SetGuiPresentation(ZenArcherName, Category.FightingStyle)
                .AddCustomSubFeatures(
                    new CanUseAttribute(
                        AttributeDefinitions.Wisdom,
                        ValidatorsWeapon.IsOfWeaponType(
                            LongbowType,
                            ShortbowType,
                            CustomWeaponsContext.HandXbowWeaponType)))
                //.SetMagicalWeapon()
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
        [CharacterContext.FightingStyleChoiceMonk];
}
