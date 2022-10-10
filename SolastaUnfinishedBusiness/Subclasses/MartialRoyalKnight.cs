using SolastaUnfinishedBusiness.Api.Extensions;
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

        var powerRoyalKnightRallyingCry = FeatureDefinitionPowerBuilder
            .Create(PowerDomainLifePreserveLife, "PowerRoyalKnightRallyingCry")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.HealingWord.GuiPresentation.SpriteReference)
            .Configure(
                UsesDetermination.AbilityBonusPlusFixed,
                ActivationTime.BonusAction,
                RechargeRate.ShortRest,
                PowerDomainLifePreserveLife.EffectDescription,
                false,
                1,
                1,
                AttributeDefinitions.Charisma)
            .SetOverriddenPower(PowerFighterSecondWind)
            .AddToDB();

        // TODO: use EffectDescriptionBuilder
        powerRoyalKnightRallyingCry.EffectDescription.EffectForms[0].HealingForm.HealingCap =
            HealingCap.MaximumHitPoints;
        powerRoyalKnightRallyingCry.EffectDescription.EffectForms[0].HealingForm.DiceNumber = 4;
        powerRoyalKnightRallyingCry.EffectDescription.targetFilteringTag = TargetFilteringTag.No;

        var powerRoyalKnightInspiringSurge = FeatureDefinitionPowerBuilder
            .Create(PowerDomainLifePreserveLife, "PowerRoyalKnightInspiringSurge")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Heroism.GuiPresentation.SpriteReference)
            .SetActivationTime(ActivationTime.BonusAction)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDomainLifePreserveLife.EffectDescription)
                    .Build()
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
                    .SetEffectForms(PowerFighterActionSurge.EffectDescription.EffectForms.ToArray())
                    .SetRangeParameter(20))
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialRoyalKnight")
            .SetGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.OathOfDevotion.GuiPresentation.SpriteReference)
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
