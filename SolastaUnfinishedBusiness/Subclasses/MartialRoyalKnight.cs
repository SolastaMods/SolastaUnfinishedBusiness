using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialRoyalKnight : AbstractSubclass
{
    internal MartialRoyalKnight()
    {
        var abilityCheckAffinityRoyalKnightRoyalEnvoy = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityRoyalKnightRoyalEnvoy")
            .SetGuiPresentationNoContent()
            .BuildAndSetAffinityGroups(
                CharacterAbilityCheckAffinity.HalfProficiencyWhenNotProficient,
                DieType.D1,
                0,
                (AttributeDefinitions.Charisma, null))
            .AddToDB();

        var featureSetRoyalKnightRoyalEnvoy = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRoyalKnightRoyalEnvoy")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                abilityCheckAffinityRoyalKnightRoyalEnvoy,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfSolasta)
            .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create(PowerDomainLifePreserveLife.EffectDescription)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetHealingForm(
                        HealingComputation.Dice,
                        0,
                        DieType.D1,
                        4,
                        false,
                        HealingCap.MaximumHitPoints,
                        EffectForm.LevelApplianceType.MultiplyBonus)
                    .Build())
            .SetTargetFiltering(TargetFilteringMethod.CharacterOnly, TargetFilteringTag.No, 5, DieType.D8)
            .Build();

        var powerRoyalKnightRallyingCry = FeatureDefinitionPowerBuilder
            .Create("PowerRoyalKnightRallyingCry")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerRallyingCry", Resources.PowerRallyingCry, 256, 128))
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.ShortRest, AttributeDefinitions.Charisma)
            .SetEffectDescription(effectDescription)
            .SetOverriddenPower(PowerFighterSecondWind)
            .AddToDB();

        var powerRoyalKnightInspiringSurge = FeatureDefinitionPowerBuilder
            .Create(PowerDomainLifePreserveLife, "PowerRoyalKnightInspiringSurge")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Heroism)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(PowerDomainLifePreserveLife.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Distance, 20, TargetType.Individuals)
                .SetTargetFiltering(
                    TargetFilteringMethod.CharacterOnly,
                    TargetFilteringTag.No,
                    5,
                    DieType.D8)
                .SetDurationData(DurationType.Round, 1)
                .SetRequiresVisibilityForPosition()
                .SetEffectForms(PowerFighterActionSurge.EffectDescription.EffectForms.ToArray())
                .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialRoyalKnight")
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.OathOfDevotion)
            .AddFeaturesAtLevel(3,
                powerRoyalKnightRallyingCry)
            .AddFeaturesAtLevel(7,
                featureSetRoyalKnightRoyalEnvoy)
            .AddFeaturesAtLevel(10,
                powerRoyalKnightInspiringSurge)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    internal override DeityDefinition DeityDefinition { get; }
}
