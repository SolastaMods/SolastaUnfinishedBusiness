using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PathOfTheLight : AbstractSubclass
{
    private const string ConditionPathOfTheLightIlluminatedName = "ConditionPathOfTheLightIlluminated";

    private const string AdditionalDamagePathOfTheLightIlluminatingStrikeName =
        "AdditionalDamagePathOfTheLightIlluminatingStrike";

    private const string PowerPathOfTheLightIlluminatingBurstName = "PowerPathOfTheLightIlluminatingBurst";

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
            .SetDuration(DurationType.Irrelevant)
            .SetSilent(Silent.WhenAdded)
            .SetSpecialDuration(true)
            .AddFeatures(
                attackDisadvantageAgainstNonSourcePathOfTheLightIlluminated,
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
                    .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                            .SetEffectForms(
                                EffectFormBuilder
                                    .Create()
                                    .SetConditionForm(
                                        ConditionDefinitionBuilder
                                            .Create("ConditionPathOfTheLightIlluminatingStrikeInitiator")
                                            .SetGuiPresentationNoContent(true)
                                            .SetAllowMultipleInstances(false)
                                            .SetConditionType(ConditionType.Beneficial)
                                            .SetDuration(DurationType.Minute, 1)
                                            .SetTerminateWhenRemoved(true)
                                            .SetSilent(Silent.WhenAddedOrRemoved)
                                            .SetSpecialInterruptions(ConditionInterruption.RageStop)
                                            .SetFeatures(
                                                FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder
                                                    .Create(
                                                        AdditionalDamagePathOfTheLightIlluminatingStrikeName,
                                                        conditionPathOfTheLightIlluminated)
                                                    .SetGuiPresentationNoContent(
                                                        AdditionalDamageDomainLifeDivineStrike.GuiPresentation
                                                            .SpriteReference)
                                                    .AddToDB())
                                            .AddToDB(),
                                        ConditionForm.ConditionOperation.Add)
                                    .Build())
                            .Build())
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

        var powerPathOfTheLightEyesOfTruth = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheLightEyesOfTruth")
            .SetGuiPresentation(Category.Feature, SeeInvisibility.GuiPresentation.SpriteReference)
            .SetUsesFixed(ActivationTime.Permanent)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Permanent, 0, TurnOccurenceType.StartOfTurn)
                .SetTargetingData(
                    Side.Ally,
                    RangeType.Self,
                    1,
                    TargetType.Self,
                    1,
                    0)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create("ConditionPathOfTheLightEyesOfTruth")
                                .SetGuiPresentation(Category.Condition,
                                    ConditionSeeInvisibility.GuiPresentation.SpriteReference)
                                .SetAllowMultipleInstances(false)
                                .SetConditionType(ConditionType.Beneficial)
                                .SetDuration(DurationType.Permanent)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .AddFeatures(FeatureDefinitionSenses.SenseSeeInvisible16)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .SetShowCasting(false)
            .AddToDB();

        //
        // Illuminating Burst
        //

        var conditionPathOfTheLightSuppressedIlluminatingBurst = ConditionDefinitionBuilder
            .Create("ConditionPathOfTheLightSuppressedIlluminatingBurst")
            .SetGuiPresentationNoContent(true)
            .SetAllowMultipleInstances(false)
            .SetConditionType(ConditionType.Neutral)
            .SetDuration(DurationType.Permanent)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerPathOfTheLightIlluminatingBurstSuppressor = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheLightIlluminatingBurstSuppressor")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.Permanent)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self, 1, 0)
                    .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionPathOfTheLightSuppressedIlluminatingBurst,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var featureSetPathOfTheLightIlluminatingBurst = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightIlluminatingBurst")
            .SetGuiPresentation(Category.Feature)
            .SetEnumerateInDescription(false)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .SetFeatureSet(
                FeatureDefinitionPowerBuilder
                    .Create("PowerPathOfTheLightIlluminatingBurstInitiator")
                    .SetGuiPresentationNoContent(true)
                    .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetDurationData(DurationType.Round, 1)
                            .SetEffectForms(
                                EffectFormBuilder
                                    .Create()
                                    .SetConditionForm(
                                        conditionPathOfTheLightSuppressedIlluminatingBurst,
                                        ConditionForm.ConditionOperation.Remove)
                                    .Build())
                            .Build())
                    .SetShowCasting(false)
                    .AddToDB(),
                FeatureDefinitionPowerIlluminatingBurstBuilder
                    .Create(PowerPathOfTheLightIlluminatingBurstName,
                        conditionPathOfTheLightIlluminated, conditionPathOfTheLightSuppressedIlluminatingBurst)
                    .SetGuiPresentation(Category.Feature, PowerDomainSunHeraldOfTheSun.GuiPresentation.SpriteReference)
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
                powerPathOfTheLightEyesOfTruth,
                pathOfTheLightIlluminatingStrikeImprovement)
            .AddFeaturesAtLevel(14,
                featureSetPathOfTheLightIlluminatingBurst)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

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

        // includes conditions that have Illuminated as their parent (like the Illuminating Burst condition)
        if (character.HasConditionOfTypeOrSubType(ConditionPathOfTheLightIlluminatedName) ||
            (character.PersonalLightSource?.SourceName != AdditionalDamagePathOfTheLightIlluminatingStrikeName &&
             character.PersonalLightSource?.SourceName != PowerPathOfTheLightIlluminatingBurstName))
        {
            return;
        }

        var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
        var gameLocationCharacter = GameLocationCharacter.GetFromActor(removedFrom);

        visibilityService.RemoveCharacterLightSource(gameLocationCharacter, character.PersonalLightSource);
        character.PersonalLightSource = null;
    }

    //
    // Custom Builders
    //

    private sealed class ConditionDefinitionIlluminated : ConditionDefinition,
        IConditionRemovedOnSourceTurnStart, INotifyConditionRemoval
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

    private sealed class FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder : DefinitionBuilder<
        FeatureDefinitionAdditionalDamageIlluminatingStrike, FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder>
    {
        private FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder(
            string name,
            ConditionDefinition illuminatedCondition)
            : base(name, CeNamespaceGuid)
        {
            var lightSourceForm = new LightSourceForm();
            var faerieFireLightSource = FaerieFire.EffectDescription
                .GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

            lightSourceForm.Copy(faerieFireLightSource.LightSourceForm);
            lightSourceForm.brightRange = 4;
            lightSourceForm.dimAdditionalRange = 4;

            Definition.additionalDamageType = AdditionalDamageType.Specific;
            Definition.specificDamageType = DamageTypeRadiant;
            Definition.triggerCondition = AdditionalDamageTriggerCondition.AlwaysActive;
            Definition.damageValueDetermination = AdditionalDamageValueDetermination.Die;
            Definition.damageDiceNumber = 1;
            Definition.damageDieType = DieType.D6;
            Definition.damageSaveAffinity = EffectSavingThrowType.None;
            Definition.limitedUsage = FeatureLimitedUsage.OnceInMyTurn;
            Definition.notificationTag = "IlluminatingStrike";
            Definition.requiredProperty = RestrictedContextRequiredProperty.None;
            Definition.addLightSource = true;
            Definition.lightSourceForm = lightSourceForm;
            Definition.damageAdvancement = AdditionalDamageAdvancement.ClassLevel;
            Definition.DiceByRankTable.SetRange(new[]
            {
                (3, 1), (4, 1), (5, 1), (6, 1), (7, 1), (8, 1), (9, 1), (10, 2), (11, 2), (12, 2), (13, 2), (14, 2),
                (15, 2), (16, 2), (17, 2), (18, 2), (19, 2), (20, 2)
            }.Select(d => DiceByRankBuilder.BuildDiceByRank(d.Item1, d.Item2)));

            Definition.ConditionOperations.Add(
                new ConditionOperationDescription
                {
                    Operation = ConditionOperationDescription.ConditionOperation.Add,
                    ConditionDefinition = illuminatedCondition
                });

            foreach (var invisibleCondition in InvisibleConditions)
            {
                Definition.ConditionOperations.Add(
                    new ConditionOperationDescription
                    {
                        Operation = ConditionOperationDescription.ConditionOperation.Remove,
                        ConditionDefinition = invisibleCondition
                    });
            }
        }

        [NotNull]
        internal static FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder Create(
            string name,
            ConditionDefinition illuminatedCondition)
        {
            return new FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder(name, illuminatedCondition);
        }
    }

    private sealed class FeatureDefinitionPowerIlluminatingBurst : FeatureDefinitionPower, IStartOfTurnRecharge
    {
        public bool IsRechargeSilent => true;
    }

    private sealed class
        FeatureDefinitionPowerIlluminatingBurstBuilder
        : FeatureDefinitionPowerBuilder<FeatureDefinitionPowerIlluminatingBurst,
            FeatureDefinitionPowerIlluminatingBurstBuilder>
    {
        private FeatureDefinitionPowerIlluminatingBurstBuilder(
            string name,
            ConditionDefinition illuminatedCondition,
            ConditionDefinition illuminatingBurstSuppressedCondition)
            : base(name, CeNamespaceGuid)
        {
            Definition.activationTime = ActivationTime.NoCost;
            Definition.costPerUse = 1;
            Definition.disableIfConditionIsOwned = illuminatingBurstSuppressedCondition;
            Definition.effectDescription = CreateIlluminatingBurstPowerEffect(illuminatedCondition);
            Definition.fixedUsesPerRecharge = 1;
            Definition.rechargeRate = RechargeRate.OneMinute;
            Definition.showCasting = false;
            Definition.usesDetermination = UsesDetermination.Fixed;
        }

        [NotNull]
        internal static FeatureDefinitionPowerIlluminatingBurstBuilder Create(
            string name,
            ConditionDefinition illuminatedCondition,
            ConditionDefinition illuminatingBurstSuppressedCondition)
        {
            return new FeatureDefinitionPowerIlluminatingBurstBuilder(
                name, illuminatedCondition, illuminatingBurstSuppressedCondition);
        }

        private static EffectDescription CreateIlluminatingBurstPowerEffect(ConditionDefinition illuminatedCondition)
        {
            var faerieFireLightSource = FaerieFire.EffectDescription
                .GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

            return EffectDescriptionBuilder
                .Create()
                .SetSavingThrowData(
                    false,
                    AttributeDefinitions.Constitution,
                    false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Constitution)
                .SetDurationData(
                    DurationType.Minute,
                    1)
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
                .SetParticleEffectParameters(GuidingBolt)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(DamageTypeRadiant, 4, DieType.D6)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionIlluminatedByBurstBuilder
                                .Create("ConditionPathOfTheLightIlluminatedByBurst")
                                .SetGuiPresentation("ConditionPathOfTheLightIlluminated", Category.Condition,
                                    ConditionBranded.GuiPresentation.SpriteReference)
                                .SetAllowMultipleInstances(true)
                                .SetConditionType(ConditionType.Detrimental)
                                .SetDuration(DurationType.Minute, 1)
                                .SetParentCondition(illuminatedCondition)
                                .SetSilent(Silent.WhenAdded)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .CanSaveToCancel(TurnOccurenceType.EndOfTurn)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetLightSourceForm(
                            LightSourceType.Basic,
                            4,
                            4,
                            faerieFireLightSource.lightSourceForm.color,
                            faerieFireLightSource.lightSourceForm.graphicsPrefabReference)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                .Build();
        }
    }
}
