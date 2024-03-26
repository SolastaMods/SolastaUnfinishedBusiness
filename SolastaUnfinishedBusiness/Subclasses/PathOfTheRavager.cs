using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PathOfTheRavager : AbstractSubclass
{
    private const string Name = "PathOfTheRavager";

    public PathOfTheRavager()
    {
        // Frenzy

        var additionalDamageBrutalStrike =
            FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamage{Name}Frenzy")
                .SetGuiPresentation(Category.Feature)
                .SetNotificationTag("Frenzy")
                .SetDamageDice(DieType.D6, 2)
                .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                .SetAdvancement(AdditionalDamageAdvancement.ClassLevel,
                    DiceByRankBuilder.InterpolateDiceByRankTable(1, 20, (3, 2), (9, 3), (16, 4)))
                .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
                .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                .AddCustomSubFeatures(
                    ModifyAdditionalDamageClassLevelBarbarian.Instance,
                    new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) => (
                        OperationType.Set,
                        mode is { AbilityScore: AttributeDefinitions.Strength } &&
                        character.HasConditionOfTypeOrSubType(ConditionRaging) &&
                        character.HasConditionOfCategoryAndType(
                            AttributeDefinitions.TagCombat, ConditionReckless))))
                .AddToDB();

        // Intimidating Presence

        var powerIntimidatingPresence = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}IntimidatingPresence")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("IntimidatingPresence", Resources.PowerDreadfulPresence, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Strength, 8)
                    .SetParticleEffectParameters(PowerFighterActionSurge.EffectDescription.effectParticleParameters)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        powerIntimidatingPresence.AddCustomSubFeatures(new ValidatorsValidatePowerUse(c =>
            c.HasConditionOfTypeOrSubType(ConditionRaging) &&
            c.GetRemainingPowerUses(powerIntimidatingPresence) > 0));

        var powerIntimidatingPresenceRage = FeatureDefinitionPowerBuilder
            .Create(powerIntimidatingPresence, $"Power{Name}IntimidatingPresenceRageCost")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.RagePoints)
            .AddToDB();

        powerIntimidatingPresenceRage.AddCustomSubFeatures(new ValidatorsValidatePowerUse(c =>
            c.HasConditionOfTypeOrSubType(ConditionRaging) &&
            c.GetRemainingPowerUses(powerIntimidatingPresence) == 0));

        var featureSetIntimidatingPresence = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}IntimidatingPresence")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerIntimidatingPresence, powerIntimidatingPresenceRage)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.PathBerserker)
            .AddFeaturesAtLevel(3, additionalDamageBrutalStrike)
            .AddFeaturesAtLevel(6, PowerBerserkerMindlessRage)
            .AddFeaturesAtLevel(10, ActionAffinityRetaliation)
            .AddFeaturesAtLevel(14, featureSetIntimidatingPresence)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
