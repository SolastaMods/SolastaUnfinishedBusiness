using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PathOfTheLight : AbstractSubclass
{
    private const string ConditionPathOfTheLightIlluminatedName = "ConditionPathOfTheLightIlluminated";

    private const string AdditionalDamageIlluminatingStrikePathOfTheLightName =
        "AdditionalDamageIlluminatingStrikePathOfTheLight";

    private const string PowerIlluminatingBurstPathOfTheLightName = "PowerIlluminatingBurstPathOfTheLight";

    private static readonly List<ConditionDefinition> InvisibleConditions =
        new() { ConditionInvisibleBase, ConditionDefinitions.ConditionInvisible, ConditionInvisibleGreater };

    internal PathOfTheLight()
    {
        var attackDisadvantageAgainstNonSourcePathOfTheLightIlluminated =
            FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder
                .Create("AttackDisadvantageAgainstNonSourcePathOfTheLightIlluminated")
                .SetGuiPresentation(Category.Feature)
                .SetConditionName(ConditionPathOfTheLightIlluminatedName)
                .AddToDB();

        var featureSetPathOfTheLightIlluminatedPreventInvisibility = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightIlluminatedPreventInvisibility")
            .SetGuiPresentation(Category.Feature)
            .SetEnumerateInDescription(false)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .AddFeatureSet(
                InvisibleConditions
                    .Select(x => FeatureDefinitionConditionAffinityBuilder
                        .Create("ConditionAffinityPathOfTheLightIlluminatedPrevent" +
                                x.Name.Replace("Condition", string.Empty))
                        .SetGuiPresentationNoContent(true)
                        .SetConditionAffinityType(ConditionAffinityType.Immunity)
                        .SetConditionType(x)
                        .AddToDB())
                    .OfType<FeatureDefinition>()
                    .ToArray())
            .AddToDB();

        var conditionPathOfTheLightIlluminated = ConditionDefinitionIlluminatedBuilder
            .Create(ConditionPathOfTheLightIlluminatedName)
            .SetGuiPresentation(Category.Condition, ConditionBranded.GuiPresentation.SpriteReference)
            .SetAllowMultipleInstances(true)
            .SetConditionType(ConditionType.Detrimental)
            .SetDuration(DurationType.Irrelevant, 1, false)
            .SetSilent(Silent.WhenAdded)
            .SetSpecialDuration(true)
            .AddFeatures(attackDisadvantageAgainstNonSourcePathOfTheLightIlluminated,
                featureSetPathOfTheLightIlluminatedPreventInvisibility)
            .AddToDB();

        var featureSetPathOfTheLightIlluminatingStrike = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightIlluminatingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetEnumerateInDescription(false)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .AddFeatureSet(
                FeatureDefinitionPowerBuilder
                    .Create("PowerPathOfTheLightIlluminatingStrike")
                    .SetGuiPresentationNoContent(true)
                    .SetActivationTime(ActivationTime.OnRageStartAutomatic)
                    .SetEffectDescription(
                        CreateIlluminatingStrikeInitiatorPowerEffect(conditionPathOfTheLightIlluminated))
                    .SetRechargeRate(RechargeRate.AtWill)
                    .AddToDB())
            .AddToDB();

        var featureSetPathOfTheLightPierceTheDarkness = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightPierceTheDarkness")
            .SetGuiPresentation(Category.Feature)
            .SetEnumerateInDescription(false)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .AddFeatureSet(FeatureDefinitionSenses.SenseSuperiorDarkvision)
            .AddToDB();

        var featureSetPathOfTheLightLightsProtection = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightLightsProtection")
            .SetGuiPresentation(Category.Feature)
            .SetEnumerateInDescription(false)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .AddFeatureSet(
                FeatureDefinitionOpportunityAttackImmunityIfAttackerHasConditionBuilder
                    .Create("OpportunityAttackImmunityIfAttackerHasConditionPathOfTheLightLightsProtection")
                    .SetGuiPresentationNoContent()
                    .SetConditionName(ConditionPathOfTheLightIlluminatedName)
                    .AddToDB())
            .AddToDB();

        var pathOfTheLightIlluminatingStrikeImprovement = FeatureDefinitionBuilder
            .Create("PathOfTheLightIlluminatingStrikeImprovement")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        //
        // Illuminating Burst
        //

        var conditionPathOfTheLightSuppressedIlluminatingBurst = ConditionDefinitionBuilder
            .Create("ConditionPathOfTheLightSuppressedIlluminatingBurst")
            .SetGuiPresentationNoContent(true)
            .SetAllowMultipleInstances(false)
            .SetConditionType(ConditionType.Neutral)
            .SetDuration(DurationType.Permanent, 1, false) // don't validate inconsistent data
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();
        
        var powerPathOfTheLightIlluminatingBurstSuppressor = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheLightIlluminatingBurstSuppressor")
            .SetGuiPresentationNoContent(true)
            .SetActivationTime(ActivationTime.Permanent)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self, 1, 0)
                    .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart)
                    .AddEffectForm(new EffectForm
                    {
                        FormType = EffectForm.EffectFormType.Condition,
                        ConditionForm = new ConditionForm
                        {
                            Operation = ConditionForm.ConditionOperation.Add,
                            ConditionDefinition = conditionPathOfTheLightSuppressedIlluminatingBurst
                        }
                    })
                    .Build())
            .AddToDB();

        var powerPathOfTheLightIlluminatingBurstInitiator = FeatureDefinitionPowerBuilder
                .Create("PowerPathOfTheLightIlluminatingBurstInitiator")
                .Configure(
                    1,
                    UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    ActivationTime.OnRageStartAutomatic,
                    1,
                    RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    new EffectDescriptionBuilder()
                        .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfTurn)
                        .AddEffectForm(new EffectForm
                        {
                            FormType = EffectForm.EffectFormType.Condition,
                            ConditionForm = new ConditionForm
                            {
                                Operation = ConditionForm.ConditionOperation.Remove,
                                ConditionDefinition = conditionPathOfTheLightSuppressedIlluminatingBurst
                            }
                        })
                        .Build())
                .SetShowCasting(false)
                .AddToDB();
            
        var featureSetPathOfTheLightIlluminatingBurst = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightIlluminatingBurst")
            .SetGuiPresentation(Category.Feature)
            .SetEnumerateInDescription(false)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .SetFeatureSet(
                powerPathOfTheLightIlluminatingBurstInitiator,
                FeatureDefinitionPowerIlluminatingBurstBuilder
                    .Create(PowerIlluminatingBurstPathOfTheLightName, conditionPathOfTheLightIlluminated,
                        conditionPathOfTheLightSuppressedIlluminatingBurst)
                    .SetGuiPresentation(Category.Feature,
                        PowerDomainSunHeraldOfTheSun.GuiPresentation.SpriteReference)
                    .AddToDB(),
                powerPathOfTheLightIlluminatingBurstSuppressor)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PathOfTheLight")
            .SetGuiPresentation(Category.Subclass, DomainSun.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                featureSetPathOfTheLightIlluminatingStrike,
                featureSetPathOfTheLightPierceTheDarkness)
            .AddFeaturesAtLevel(6,
                featureSetPathOfTheLightLightsProtection)
            .AddFeaturesAtLevel(10,
                BuildEyesOfTruth(),
                pathOfTheLightIlluminatingStrikeImprovement)
            .AddFeaturesAtLevel(14,
                featureSetPathOfTheLightIlluminatingBurst)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    private static FeatureDefinition BuildEyesOfTruth()
    {
        var conditionPathOfTheLightEyesOfTruth = ConditionDefinitionBuilder
            .Create("ConditionPathOfTheLightEyesOfTruth")
            .SetGuiPresentation(Category.Condition, ConditionSeeInvisibility.GuiPresentation.SpriteReference)
            .SetAllowMultipleInstances(false)
            .SetConditionType(ConditionType.Beneficial)
            .SetDuration(DurationType.Permanent, 1, false) // don't validate inconsistent data
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(FeatureDefinitionSenses.SenseSeeInvisible16)
            .AddToDB();

        return FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheLightEyesOfTruth")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.SeeInvisibility.GuiPresentation.SpriteReference)
            .SetShowCasting(false)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.StartOfTurn)
                .SetTargetingData(
                    Side.Ally,
                    RangeType.Self,
                    1,
                    TargetType.Self,
                    1,
                    0)
                .AddEffectForm(new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Condition,
                    ConditionForm = new ConditionForm
                    {
                        Operation = ConditionForm.ConditionOperation.Add,
                        ConditionDefinition = conditionPathOfTheLightEyesOfTruth
                    }
                })
                .Build())
            .SetRechargeRate(RechargeRate.AtWill)
            .SetActivationTime(ActivationTime.Permanent)
            .AddToDB();
    }

    private static FeatureDefinitionAdditionalDamage BuildAdditionalDamageIlluminatingStrike(ConditionDefinition illuminatedCondition)
    {
        var additionalDamageIlluminatingStrikePathOfTheLight = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageIlluminatingStrikePathOfTheLightName)
            .Configure(
                "IlluminatingStrike",
                FeatureLimitedUsage.OnceInMyTurn,
                AdditionalDamageValueDetermination.Die,
                AdditionalDamageTriggerCondition.AlwaysActive,
                RestrictedContextRequiredProperty.None,
                false,
                DieType.D6,
                1,
                AdditionalDamageType.Specific,
                DamageTypeRadiant,
                AdditionalDamageAdvancement.ClassLevel,
                (new[]
                {
                    (3, 1),
                    (4, 1),
                    (5, 1),
                    (6, 1),
                    (7, 1),
                    (8, 1),
                    (9, 1),
                    (10, 2),
                    (11, 2),
                    (12, 2),
                    (13, 2),
                    (14, 2),
                    (15, 2),
                    (16, 2),
                    (17, 2),
                    (18, 2),
                    (19, 2),
                    (20, 2)
                }).Select(d => DiceByRankBuilder.BuildDiceByRank(d.Item1, d.Item2))
            )
            .SetAddLightSource(true)
            .SetLightSourceForm(CreateIlluminatedLightSource())
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    Operation = ConditionOperationDescription.ConditionOperation.Add,
                    ConditionDefinition = illuminatedCondition
                })
            .AddToDB();
        // Definition.damageSaveAffinity = EffectSavingThrowType.None;

        foreach (var invisibleCondition in InvisibleConditions)
        {
            additionalDamageIlluminatingStrikePathOfTheLight.ConditionOperations.Add(
                new ConditionOperationDescription
                {
                    Operation = ConditionOperationDescription.ConditionOperation.Remove,
                    ConditionDefinition = invisibleCondition
                });
        }

        return additionalDamageIlluminatingStrikePathOfTheLight;
    }

    [NotNull]
    private static LightSourceForm CreateIlluminatedLightSource()
    {
        var faerieFireLightSource =
            SpellDefinitions.FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType
                .LightSource);

        var lightSourceForm = new LightSourceForm();

        lightSourceForm.Copy(faerieFireLightSource.LightSourceForm);
        lightSourceForm.brightRange = 4;
        lightSourceForm.dimAdditionalRange = 4;

        return lightSourceForm;
    }
        
    private static EffectDescription CreateIlluminatingStrikeInitiatorPowerEffect(
        ConditionDefinition illuminatedCondition)
    {
        var conditionPathOfTheLightIlluminatingStrikeInitiator = ConditionDefinitionBuilder
            .Create("ConditionPathOfTheLightIlluminatingStrikeInitiator")
            .SetGuiPresentationNoContent(true)
            .SetAllowMultipleInstances(false)
            .SetConditionType(ConditionType.Beneficial)
            .SetDuration(DurationType.Minute, 1)
            .SetTerminateWhenRemoved(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .SetFeatures(BuildAdditionalDamageIlluminatingStrike(illuminatedCondition))
            .AddToDB();

        return new EffectDescriptionBuilder()
            .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
            .AddEffectForm(new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm = new ConditionForm
                {
                    Operation = ConditionForm.ConditionOperation.Add,
                    ConditionDefinition = conditionPathOfTheLightIlluminatingStrikeInitiator
                }
            })
            .Build();
    }

    private static void ApplyLightsProtectionHealing(ulong sourceGuid)
    {
        var lightsProtectionAmountHealedByClassLevel = new Dictionary<int, int>
        {
            { 6, 3 },
            { 7, 3 },
            { 8, 4 },
            { 9, 4 },
            { 10, 5 },
            { 11, 5 },
            { 12, 6 },
            { 13, 6 },
            { 14, 7 },
            { 15, 7 },
            { 16, 8 },
            { 17, 8 },
            { 18, 9 },
            { 19, 9 },
            { 20, 10 }
        };

        if (RulesetEntity.GetEntity<RulesetCharacter>(sourceGuid) is not RulesetCharacterHero conditionSource ||
            conditionSource.IsDead)
        {
            return;
        }

        if (!conditionSource.ClassesAndLevels.TryGetValue(CharacterClassDefinitions.Barbarian, out var levelsInClass))
        {
            // Character doesn't have levels in class
            return;
        }

        if (!lightsProtectionAmountHealedByClassLevel.TryGetValue(levelsInClass, out var amountHealed))
        {
            // Character doesn't heal at the current level
            return;
        }

        if (amountHealed > 0)
        {
            conditionSource.ReceiveHealing(amountHealed, true, sourceGuid);
        }
    }

    private static void HandleAfterIlluminatedConditionRemoved(RulesetActor removedFrom)
    {
        if (removedFrom is not RulesetCharacter character)
        {
            return;
        }

        // Intentionally *includes* conditions that have Illuminated as their parent (like the Illuminating Burst condition)
        if (character.HasConditionOfTypeOrSubType(ConditionPathOfTheLightIlluminatedName) ||
            (character.PersonalLightSource?.SourceName != AdditionalDamageIlluminatingStrikePathOfTheLightName &&
             character.PersonalLightSource?.SourceName != PowerIlluminatingBurstPathOfTheLightName))
        {
            return;
        }

        var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();

        visibilityService.RemoveCharacterLightSource(
            GameLocationCharacter.GetFromActor(removedFrom),
            character.PersonalLightSource);
        character.PersonalLightSource = null;
    }

    //
    // Helper Classes
    //

    private sealed class ConditionDefinitionIlluminated : ConditionDefinition, IConditionRemovedOnSourceTurnStart,
        INotifyConditionRemoval
    {
        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            HandleAfterIlluminatedConditionRemoved(removedFrom);
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, [NotNull] RulesetCondition rulesetCondition)
        {
            ApplyLightsProtectionHealing(rulesetCondition.SourceGuid);
        }
    }

    [UsedImplicitly]
    private sealed class ConditionDefinitionIlluminatedBuilder
        : ConditionDefinitionBuilder<ConditionDefinitionIlluminated, ConditionDefinitionIlluminatedBuilder>
    {
        internal ConditionDefinitionIlluminatedBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
        }
    }

    private sealed class ConditionDefinitionIlluminatedByBurst : ConditionDefinition, INotifyConditionRemoval
    {
        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            HandleAfterIlluminatedConditionRemoved(removedFrom);
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, [NotNull] RulesetCondition rulesetCondition)
        {
            ApplyLightsProtectionHealing(rulesetCondition.SourceGuid);
        }
    }


    [UsedImplicitly]
    private sealed class ConditionDefinitionIlluminatedByBurstBuilder
        : ConditionDefinitionBuilder<ConditionDefinitionIlluminatedByBurst,
            ConditionDefinitionIlluminatedByBurstBuilder>
    {
        internal ConditionDefinitionIlluminatedByBurstBuilder(string name, Guid guidNamespace)
            : base(name, guidNamespace)
        {
        }
    }

    private sealed class FeatureDefinitionAdditionalDamageIlluminatingStrike : FeatureDefinitionAdditionalDamage,
        IClassHoldingFeature
    {
        // allows Illuminating Strike damage to scale with barbarian level
        public CharacterClassDefinition Class => CharacterClassDefinitions.Barbarian;
    }

    private sealed class FeatureDefinitionPowerIlluminatingBurst : FeatureDefinitionPower, IStartOfTurnRecharge
    {
        public bool IsRechargeSilent => true;
    }

    private sealed class
        FeatureDefinitionPowerIlluminatingBurstBuilder : FeatureDefinitionPowerBuilder<
            FeatureDefinitionPowerIlluminatingBurst,
            FeatureDefinitionPowerIlluminatingBurstBuilder>
    {
        private FeatureDefinitionPowerIlluminatingBurstBuilder(
            string name,
            ConditionDefinition illuminatedCondition,
            ConditionDefinition illuminatingBurstSuppressedCondition)
            : base(name, CeNamespaceGuid)
        {
            Definition.activationTime = ActivationTime.NoCost;
            Definition.effectDescription = CreateIlluminatingBurstPowerEffect(illuminatedCondition);
            Definition.rechargeRate = RechargeRate.OneMinute;
            Definition.fixedUsesPerRecharge = 1;
            Definition.usesDetermination = UsesDetermination.Fixed;
            Definition.costPerUse = 1;
            Definition.showCasting = false;
            Definition.disableIfConditionIsOwned = illuminatingBurstSuppressedCondition;
        }

        [NotNull]
        internal static FeatureDefinitionPowerIlluminatingBurstBuilder Create(
            string name,
            ConditionDefinition illuminatedCondition,
            ConditionDefinition illuminatingBurstSuppressedCondition)
        {
            return new FeatureDefinitionPowerIlluminatingBurstBuilder(name, illuminatedCondition,
                illuminatingBurstSuppressedCondition);
        }

        private static EffectDescription CreateIlluminatingBurstPowerEffect(ConditionDefinition illuminatedCondition)
        {
            var conditionPathOfTheLightIlluminatedByBurst = ConditionDefinitionIlluminatedByBurstBuilder
                .Create("ConditionPathOfTheLightIlluminatedByBurst")
                .SetGuiPresentation("ConditionPathOfTheLightIlluminated", Category.Condition,
                    ConditionBranded.GuiPresentation.SpriteReference)
                .SetAllowMultipleInstances(true)
                .SetConditionType(ConditionType.Detrimental)
                .SetDuration(DurationType.Minute, 1)
                .SetParentCondition(illuminatedCondition)
                .SetSilent(Silent.WhenAdded)
                .AddToDB();

            var faerieFireLightSource =
                SpellDefinitions.FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType
                    .LightSource);

            var lightSourceForm = new LightSourceForm();

            lightSourceForm.Copy(faerieFireLightSource.LightSourceForm);
            lightSourceForm.brightRange = 4;
            lightSourceForm.dimAdditionalRange = 4;

            return new EffectDescriptionBuilder()
                .SetSavingThrowData(
                    true,
                    false,
                    AttributeDefinitions.Constitution,
                    false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Constitution)
                .SetDurationData(
                    DurationType.Minute,
                    1,
                    TurnOccurenceType.EndOfTurn)
                .SetTargetingData(
                    Side.Enemy,
                    RangeType.Distance,
                    6,
                    TargetType.IndividualsUnique,
                    3,
                    0)
                .SetSpeed(
                    SpeedType.CellsPerSeconds,
                    9.5f)
                .SetParticleEffectParameters(
                    SpellDefinitions.GuidingBolt.EffectDescription.EffectParticleParameters)
                .AddEffectForm(
                    new EffectForm
                    {
                        FormType = EffectForm.EffectFormType.Damage,
                        DamageForm =
                            new DamageForm { DamageType = DamageTypeRadiant, DiceNumber = 4, DieType = DieType.D6 },
                        SavingThrowAffinity = EffectSavingThrowType.Negates
                    })
                .AddEffectForm(
                    new EffectForm
                    {
                        FormType = EffectForm.EffectFormType.Condition,
                        ConditionForm =
                            new ConditionForm
                            {
                                Operation = ConditionForm.ConditionOperation.Add,
                                ConditionDefinition = conditionPathOfTheLightIlluminatedByBurst
                            },
                        CanSaveToCancel = true,
                        SaveOccurence = TurnOccurenceType.EndOfTurn,
                        SavingThrowAffinity = EffectSavingThrowType.Negates
                    })
                .AddEffectForm(new EffectForm
                {
                    FormType = EffectForm.EffectFormType.LightSource,
                    SavingThrowAffinity = EffectSavingThrowType.Negates,
                    lightSourceForm = lightSourceForm
                })
                .Build();
        }
    }
}
