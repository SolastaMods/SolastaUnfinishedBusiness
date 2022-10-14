using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
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

        var powerRoyalKnightRallyingCry = FeatureDefinitionPowerBuilder
            .Create("PowerRoyalKnightRallyingCry")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.HealingWord)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.ShortRest, AttributeDefinitions.Charisma)
            .SetEffectDescription(PowerDomainLifePreserveLife.EffectDescription, true)
            .SetOverriddenPower(PowerFighterSecondWind)
            .AddToDB();

        // TODO: use EffectDescriptionBuilder
        powerRoyalKnightRallyingCry.EffectDescription.EffectForms[0].HealingForm.HealingCap =
            HealingCap.MaximumHitPoints;
        powerRoyalKnightRallyingCry.EffectDescription.EffectForms[0].HealingForm.DiceNumber = 4;
        powerRoyalKnightRallyingCry.EffectDescription.targetFilteringTag = TargetFilteringTag.No;

        var powerRoyalKnightInspiringSurge = FeatureDefinitionPowerBuilder
            .Create(PowerDomainLifePreserveLife, "PowerRoyalKnightInspiringSurge")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Heroism)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDomainLifePreserveLife.EffectDescription)
                    .SetCanBePlacedOnCharacter()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 20, TargetType.Individuals)
                    .SetTargetFiltering(
                        TargetFilteringMethod.CharacterOnly,
                        TargetFilteringTag.No,
                        5,
                        DieType.D8)
                    .SetDurationData(DurationType.Round, 1)
                    .SetRequiresVisibilityForPosition(true)
                    .SetEffectForms(PowerFighterActionSurge.EffectDescription.EffectForms.ToArray())
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialRoyalKnight")
            .SetGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.OathOfDevotion)
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
}
