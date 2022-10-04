using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialRoyalKnight : AbstractSubclass
{
    internal MartialRoyalKnight()
    {
        var abilityCheckAffinityRoyalKnightRoyalEnvoy = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityChampionRemarkableAthlete,
                "AbilityCheckAffinityRoyalKnightRoyalEnvoy")
            .SetAffinityGroups(new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
            {
                abilityScoreName = AttributeDefinitions.Charisma,
                affinity = CharacterAbilityCheckAffinity.HalfProficiencyWhenNotProficient
            })
            .AddToDB();

        var featureSetRoyalKnightRoyalEnvoy = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRoyalKnightRoyalEnvoy")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                abilityCheckAffinityRoyalKnightRoyalEnvoy,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfSolasta)
            .AddToDB();

        var rallyingCryPower = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerDomainLifePreserveLife, "PowerRoyalKnightRallyingCry")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.HealingWord.GuiPresentation.SpriteReference)
            .Configure(
                0,
                UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Charisma,
                ActivationTime.BonusAction,
                1,
                RechargeRate.ShortRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                FeatureDefinitionPowers.PowerDomainLifePreserveLife.EffectDescription)
            .SetOverriddenPower(FeatureDefinitionPowers.PowerFighterSecondWind)
            .AddToDB();

        // TODO: use EffectDescriptionBuilder
        rallyingCryPower.EffectDescription.EffectForms[0].HealingForm.HealingCap = HealingCap.MaximumHitPoints;
        rallyingCryPower.EffectDescription.EffectForms[0].HealingForm.DiceNumber = 4;
        rallyingCryPower.EffectDescription.targetFilteringTag = TargetFilteringTag.No;

        var powerRoyalKnightInspiringSurge = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerDomainLifePreserveLife, "PowerRoyalKnightInspiringSurge")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Heroism.GuiPresentation.SpriteReference)
            .SetActivationTime(ActivationTime.BonusAction)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetEffectDescription(
                FeatureDefinitionPowers.PowerDomainLifePreserveLife.EffectDescription
                    .Copy()
                    .SetTargetType(TargetType.Individuals)
                    .SetTargetParameter(1)
                    .SetTargetParameter2(2)
                    .SetTargetSide(Side.Ally)
                    .SetCanBePlacedOnCharacter(true)
                    .SetTargetFilteringMethod(TargetFilteringMethod.CharacterOnly)
                    .SetTargetFilteringTag(TargetFilteringTag.No)
                    .SetDurationType(DurationType.Round)
                    .SetRequiresVisibilityForPosition(true)
                    .SetRangeType(RangeType.Distance)
                    .SetEffectForms(FeatureDefinitionPowers.PowerFighterActionSurge.EffectDescription.EffectForms)
                    .SetRangeParameter(20))
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialRoyalKnight")
            .SetGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.OathOfDevotion.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                rallyingCryPower)
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
