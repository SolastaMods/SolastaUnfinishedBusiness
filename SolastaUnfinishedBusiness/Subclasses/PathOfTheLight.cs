using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
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

    private static readonly Dictionary<int, int> LightsProtectionAmountHealedByClassLevel = new()
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

    internal PathOfTheLight()
    {
        var disadvantageAgainstNonSource = FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder
            .Create("AttackDisadvantageAgainstNonSourcePathOfTheLightIlluminated")
            .SetGuiPresentation(Category.Feature)
            .SetConditionName(ConditionPathOfTheLightIlluminatedName)
            .AddToDB();

        var preventInvisibility = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightIlluminatedPreventInvisibility")
            .SetGuiPresentation(Category.Feature)
            .SetEnumerateInDescription(false)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .AddFeatureSet(InvisibleConditions
                .Select(x => FeatureDefinitionConditionAffinityBuilder
                    .Create("ConditionAffinityPathOfTheLightIlluminatedPrevent" +
                            x.Name.Replace("Condition", string.Empty))
                    .SetGuiPresentationNoContent(true)
                    .SetConditionAffinityType(ConditionAffinityType.Immunity)
                    .SetConditionType(x)
                    .AddToDB()))
            .AddToDB();

        var conditionPathOfTheLightIlluminated = ConditionDefinitionIlluminatedBuilder
            .Create(ConditionPathOfTheLightIlluminatedName)
            .SetGuiPresentation(Category.Condition, ConditionBranded.GuiPresentation.SpriteReference)
            .SetAllowMultipleInstances(true)
            .SetConditionType(ConditionType.Detrimental)
            .SetDuration(DurationType.Irrelevant, 1, false)
            .SetSilent(Silent.WhenAdded)
            .SetSpecialDuration(true)
            .AddFeatures(disadvantageAgainstNonSource, preventInvisibility)
            .AddToDB();

        var illuminatingStrike = FeatureDefinitionFeatureSetBuilder
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

        var pierceOfTheDarkness = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightPierceTheDarkness")
            .SetGuiPresentation(Category.Feature)
            .SetEnumerateInDescription(false)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .AddFeatureSet(FeatureDefinitionSenses.SenseSuperiorDarkvision)
            .AddToDB();

        var lightsProtection = FeatureDefinitionFeatureSetBuilder
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

        var illuminatingStrikeImprovement = FeatureDefinitionBuilder
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

        var suppressIlluminatingBurst = new EffectForm
        {
            FormType = EffectForm.EffectFormType.Condition,
            ConditionForm = new ConditionForm
            {
                Operation = ConditionForm.ConditionOperation.Add,
                ConditionDefinition = conditionPathOfTheLightSuppressedIlluminatingBurst
            }
        };

        var suppressIlluminatingBurstEffect = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.StartOfTurn)
            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self, 1, 0)
            .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart)
            .AddEffectForm(suppressIlluminatingBurst)
            .Build();

        var powerPathOfTheLightIlluminatingBurstSuppressor = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheLightIlluminatingBurstSuppressor")
            .SetGuiPresentationNoContent(true)
            .SetActivationTime(ActivationTime.Permanent)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetEffectDescription(suppressIlluminatingBurstEffect)
            .AddToDB();

        var illuminatingBurst = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightIlluminatingBurst")
            .SetGuiPresentation(Category.Feature)
            .SetEnumerateInDescription(false)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .SetFeatureSet(
                FeatureDefinitionPowerIlluminatingBurstInitiatorBuilder
                    .Create("PowerPathOfTheLightIlluminatingBurstInitiator",
                        conditionPathOfTheLightSuppressedIlluminatingBurst)
                    .SetGuiPresentationNoContent(true)
                    .AddToDB(),
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
            .AddFeatureAtLevel(illuminatingStrike, 3)
            .AddFeatureAtLevel(pierceOfTheDarkness, 3)
            .AddFeatureAtLevel(lightsProtection, 6)
            .AddFeatureAtLevel(BuildEyesOfTruth(), 10)
            .AddFeatureAtLevel(illuminatingStrikeImprovement, 10)
            .AddFeatureAtLevel(illuminatingBurst, 14)
            .AddToDB();
    }

    private CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    private static FeatureDefinition BuildEyesOfTruth()
    {
        var seeingInvisibleCondition = ConditionDefinitionBuilder
            .Create("ConditionPathOfTheLightEyesOfTruth")
            .SetGuiPresentation(Category.Condition, ConditionSeeInvisibility.GuiPresentation.SpriteReference)
            .SetAllowMultipleInstances(false)
            .SetConditionType(ConditionType.Beneficial)
            .SetDuration(DurationType.Permanent, 1, false) // don't validate inconsistent data
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(FeatureDefinitionSenses.SenseSeeInvisible16)
            .AddToDB();

        var seeInvisibleEffectBuilder = new EffectDescriptionBuilder();
        var seeInvisibleConditionForm = new EffectForm
        {
            FormType = EffectForm.EffectFormType.Condition,
            ConditionForm = new ConditionForm
            {
                Operation = ConditionForm.ConditionOperation.Add, ConditionDefinition = seeingInvisibleCondition
            }
        };

        seeInvisibleEffectBuilder
            .SetDurationData(DurationType.Permanent, 1, TurnOccurenceType.StartOfTurn)
            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self, 1, 0)
            .AddEffectForm(seeInvisibleConditionForm);

        return FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheLightEyesOfTruth")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.SeeInvisibility.GuiPresentation.SpriteReference)
            .SetShowCasting(false)
            .SetEffectDescription(seeInvisibleEffectBuilder.Build())
            .SetRechargeRate(RechargeRate.AtWill)
            .SetActivationTime(ActivationTime.Permanent)
            .AddToDB();
    }

    private static EffectDescription CreateIlluminatingStrikeInitiatorPowerEffect(
        ConditionDefinition illuminatedCondition)
    {
        var initiatorCondition = ConditionDefinitionBuilder
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
                    .Create(AdditionalDamageIlluminatingStrikePathOfTheLightName, illuminatedCondition)
                    .SetGuiPresentationNoContent(AdditionalDamageDomainLifeDivineStrike.GuiPresentation
                        .SpriteReference)
                    .AddToDB())
            .AddToDB();

        var enableIlluminatingStrike = new EffectForm
        {
            FormType = EffectForm.EffectFormType.Condition,
            ConditionForm = new ConditionForm
            {
                Operation = ConditionForm.ConditionOperation.Add, ConditionDefinition = initiatorCondition
            }
        };

        var effectDescriptionBuilder = new EffectDescriptionBuilder();

        effectDescriptionBuilder
            .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
            .AddEffectForm(enableIlluminatingStrike);

        return effectDescriptionBuilder.Build();
    }

    private static void ApplyLightsProtectionHealing(ulong sourceGuid)
    {
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

        if (!LightsProtectionAmountHealedByClassLevel.TryGetValue(levelsInClass, out var amountHealed))
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

        visibilityService.RemoveCharacterLightSource(GameLocationCharacter.GetFromActor(removedFrom),
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

    // ReSharper disable once ClassNeverInstantiated.Local
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

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class ConditionDefinitionIlluminatedByBurstBuilder
        : ConditionDefinitionBuilder<ConditionDefinitionIlluminatedByBurst,
            ConditionDefinitionIlluminatedByBurstBuilder>
    {
        internal ConditionDefinitionIlluminatedByBurstBuilder(string name, Guid guidNamespace) : base(name,
            guidNamespace)
        {
        }
    }

    private sealed class FeatureDefinitionAdditionalDamageIlluminatingStrike : FeatureDefinitionAdditionalDamage,
        IClassHoldingFeature
    {
        // allows Illuminating Strike damage to scale with barbarian level
        public CharacterClassDefinition Class => CharacterClassDefinitions.Barbarian;
    }

    private sealed class FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder :
        FeatureDefinitionAdditionalDamageBuilder<
            FeatureDefinitionAdditionalDamageIlluminatingStrike,
            FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder>
    {
        private FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder(string name,
            ConditionDefinition illuminatedCondition)
            : base(name, GuidHelper.Create(CENamespaceGuid, name).ToString())
        {
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
            Definition.lightSourceForm = CreateIlluminatedLightSource();

            SetAdvancement(
                AdditionalDamageAdvancement.ClassLevel,
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
            );

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
        public static FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder Create(string name,
            ConditionDefinition illuminatedCondition)
        {
            return new FeatureDefinitionAdditionalDamageIlluminatingStrikeBuilder(name, illuminatedCondition);
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
            : base(name, GuidHelper.Create(CENamespaceGuid, name).ToString())
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
        public static FeatureDefinitionPowerIlluminatingBurstBuilder Create(
            string name,
            ConditionDefinition illuminatedCondition,
            ConditionDefinition illuminatingBurstSuppressedCondition)
        {
            return new FeatureDefinitionPowerIlluminatingBurstBuilder(name, illuminatedCondition,
                illuminatingBurstSuppressedCondition);
        }

        private static EffectDescription CreateIlluminatingBurstPowerEffect(ConditionDefinition illuminatedCondition)
        {
            var effectDescriptionBuilder = new EffectDescriptionBuilder();

            var dealDamage = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Damage,
                DamageForm =
                    new DamageForm { DamageType = DamageTypeRadiant, DiceNumber = 4, DieType = DieType.D6 },
                SavingThrowAffinity = EffectSavingThrowType.Negates
            };

            var illuminatedByBurstCondition = ConditionDefinitionIlluminatedByBurstBuilder
                .Create("ConditionPathOfTheLightIlluminatedByBurst")
                .SetGuiPresentation("ConditionPathOfTheLightIlluminated", Category.Condition,
                    ConditionBranded.GuiPresentation.SpriteReference)
                .SetAllowMultipleInstances(true)
                .SetConditionType(ConditionType.Detrimental)
                .SetDuration(DurationType.Minute, 1)
                .SetParentCondition(illuminatedCondition)
                .SetSilent(Silent.WhenAdded)
                .AddToDB();

            var addIlluminatedCondition = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm =
                    new ConditionForm
                    {
                        Operation = ConditionForm.ConditionOperation.Add,
                        ConditionDefinition = illuminatedByBurstCondition
                    },
                CanSaveToCancel = true,
                SaveOccurence = TurnOccurenceType.EndOfTurn,
                SavingThrowAffinity = EffectSavingThrowType.Negates
            };

            var faerieFireLightSource =
                SpellDefinitions.FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType
                    .LightSource);

            var lightSourceForm = new LightSourceForm();

            lightSourceForm.Copy(faerieFireLightSource.LightSourceForm);
            lightSourceForm.brightRange = 4;
            lightSourceForm.dimAdditionalRange = 4;

            var addLightSource = new EffectForm
            {
                FormType = EffectForm.EffectFormType.LightSource,
                SavingThrowAffinity = EffectSavingThrowType.Negates,
                lightSourceForm = lightSourceForm
            };

            effectDescriptionBuilder
                .SetSavingThrowData(
                    true,
                    false,
                    AttributeDefinitions.Constitution,
                    false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Constitution,
                    10,
                    false,
                    new List<SaveAffinityBySenseDescription>())
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
                .AddEffectForm(dealDamage)
                .AddEffectForm(addIlluminatedCondition)
                .AddEffectForm(addLightSource);

            return effectDescriptionBuilder.Build();
        }
    }

    // Builds the power that enables Illuminating Burst on the turn you enter a rage (by removing the condition disabling it)
    private sealed class FeatureDefinitionPowerIlluminatingBurstInitiatorBuilder : FeatureDefinitionPowerBuilder
    {
        private FeatureDefinitionPowerIlluminatingBurstInitiatorBuilder(string name,
            ConditionDefinition illuminatingBurstSuppressedCondition)
            : base(name, GuidHelper.Create(CENamespaceGuid, name).ToString())
        {
            Definition.activationTime = ActivationTime.OnRageStartAutomatic;
            Definition.effectDescription =
                CreateIlluminatingBurstSuppressedPowerEffect(illuminatingBurstSuppressedCondition);
            Definition.rechargeRate = RechargeRate.AtWill;
            Definition.showCasting = false;
        }

        [NotNull]
        public static FeatureDefinitionPowerIlluminatingBurstInitiatorBuilder Create(string name,
            ConditionDefinition illuminatingBurstSuppressedCondition)
        {
            return new FeatureDefinitionPowerIlluminatingBurstInitiatorBuilder(name,
                illuminatingBurstSuppressedCondition);
        }

        private static EffectDescription CreateIlluminatingBurstSuppressedPowerEffect(
            ConditionDefinition illuminatingBurstSuppressedCondition)
        {
            var enableIlluminatingBurst = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm = new ConditionForm
                {
                    Operation = ConditionForm.ConditionOperation.Remove,
                    ConditionDefinition = illuminatingBurstSuppressedCondition
                }
            };

            var effectDescriptionBuilder = new EffectDescriptionBuilder();

            effectDescriptionBuilder
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfTurn)
                .AddEffectForm(enableIlluminatingBurst);

            return effectDescriptionBuilder.Build();
        }
    }
}
