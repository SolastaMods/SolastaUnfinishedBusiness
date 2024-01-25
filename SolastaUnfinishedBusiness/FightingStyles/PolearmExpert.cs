using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomGenericBehaviors;
using SolastaUnfinishedBusiness.CustomSpecificBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Models.CustomWeaponsContext;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class PolearmExpert : AbstractFightingStyle
{
    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create("PolearmExpert")
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite("PolearmExpert", Resources.PolearmExpert, 256))
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("FeaturePolearm")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(
                    new CanMakeAoOOnReachEntered
                    {
                        WeaponValidator = (mode, _, _) => ValidatorsWeapon.IsPolearmType(mode)
                    },
                    new AddPolearmFollowUpAttack(QuarterstaffType),
                    new AddPolearmFollowUpAttack(SpearType),
                    new AddPolearmFollowUpAttack(HalberdWeaponType),
                    new AddPolearmFollowUpAttack(PikeWeaponType),
                    new AddPolearmFollowUpAttack(LongMaceWeaponType))
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
    [
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    ];
}
