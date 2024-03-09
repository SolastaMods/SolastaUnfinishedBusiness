using System.Collections.Generic;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Models.CustomWeaponsContext;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class PolearmExpert : AbstractFightingStyle
{
    internal const string PolearmExpertName = "PolearmExpert";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(PolearmExpertName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(PolearmExpertName, Resources.PolearmExpert, 256))
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

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => [];
}
