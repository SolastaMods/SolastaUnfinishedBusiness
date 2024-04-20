using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PathOfTheLight : AbstractSubclass
{
    private const string ConditionPathOfTheLightIlluminatedName = "ConditionPathOfTheLightIlluminated";

    private const string AdditionalDamagePathOfTheLightIlluminatingStrikeName =
        "AdditionalDamagePathOfTheLightIlluminatingStrike";

    private const string PowerPathOfTheLightIlluminatingBurstName = "PowerPathOfTheLightIlluminatingBurst";

    public PathOfTheLight()
    {
        var faerieFireLightSource =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var invisibleConditions = DatabaseRepository.GetDatabase<ConditionDefinition>()
            .Where(x => x.IsSubtypeOf(ConditionInvisibleBase.Name))
            .ToList();

        var attackDisadvantageAgainstNonSourcePathOfTheLightIlluminated =
            FeatureDefinitionCombatAffinityBuilder
                .Create("AttackDisadvantageAgainstNonSourcePathOfTheLightIlluminated")
                .SetGuiPresentation(ConditionPathOfTheLightIlluminatedName, Category.Condition,
                    Gui.NoLocalization)
                .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                .SetSituationalContext(ExtraSituationalContext.IsNotConditionSource)
                .AddToDB();

        var featureSetPathOfTheLightIlluminatedPreventInvisibility = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightIlluminatedPreventInvisibility")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                invisibleConditions.Select(x =>
                        FeatureDefinitionConditionAffinityBuilder
                            .Create("ConditionAffinityPathOfTheLightIlluminatedPrevent" +
                                    x.Name.Replace("Condition", string.Empty))
                            .SetGuiPresentationNoContent(true)
                            .SetConditionAffinityType(ConditionAffinityType.Immunity)
                            .SetConditionType(x)
                            .AddToDB())
                    .OfType<FeatureDefinition>()
                    .ToArray())
            .AddToDB();

        var conditionPathOfTheLightIlluminated = ConditionDefinitionBuilder
            .Create(ConditionPathOfTheLightIlluminatedName)
            .SetGuiPresentation(Category.Condition, ConditionBranded)
            .AllowMultipleInstances()
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddFeatures(
                attackDisadvantageAgainstNonSourcePathOfTheLightIlluminated,
                featureSetPathOfTheLightIlluminatedPreventInvisibility)
            .AddCustomSubFeatures(OnConditionAddedOrRemovedIlluminatedOrIlluminatedByBurst.Marker)
            .AddToDB();

        attackDisadvantageAgainstNonSourcePathOfTheLightIlluminated.requiredCondition =
            conditionPathOfTheLightIlluminated;

        var lightSourceForm = new LightSourceForm();

        lightSourceForm.Copy(faerieFireLightSource.LightSourceForm);
        lightSourceForm.brightRange = 4;
        lightSourceForm.dimAdditionalRange = 4;

        var additionalDamagePathOfTheLightIlluminatingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamagePathOfTheLightIlluminatingStrikeName)
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("IlluminatingStrike")
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 10)
            .SetSpecificDamageType(DamageTypeRadiant)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AlwaysActive)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetConditionOperations(
                invisibleConditions.Select(x =>
                    new ConditionOperationDescription
                    {
                        Operation = ConditionOperationDescription.ConditionOperation.Remove, ConditionDefinition = x
                    }).ToArray())
            .AddConditionOperation(
                ConditionOperationDescription.ConditionOperation.Add, conditionPathOfTheLightIlluminated)
            .SetAddLightSource(true)
            .SetLightSourceForm(lightSourceForm)
            .AddCustomSubFeatures(ModifyAdditionalDamageClassLevelBarbarian.Instance)
            .AddToDB();

        additionalDamagePathOfTheLightIlluminatingStrike.DiceByRankTable[9].diceNumber = 2;

        var featureSetPathOfTheLightIlluminatingStrike = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightIlluminatingStrike")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionPowerBuilder
                    .Create("PowerPathOfTheLightIlluminatingStrike")
                    .SetGuiPresentationNoContent(true)
                    .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
                    .SetShowCasting(false)
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
                                            .SetTerminateWhenRemoved()
                                            .SetSilent(Silent.WhenAddedOrRemoved)
                                            .SetSpecialInterruptions(ConditionInterruption.RageStop)
                                            .SetFeatures(additionalDamagePathOfTheLightIlluminatingStrike)
                                            .AddToDB(),
                                        ConditionForm.ConditionOperation.Add)
                                    .Build())
                            .Build())
                    .AddToDB())
            .AddToDB();

        var featureSetPathOfTheLightPierceTheDarkness = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightPierceTheDarkness")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(FeatureDefinitionSenses.SenseSuperiorDarkvision)
            .AddToDB();

        var featureSetPathOfTheLightLightsProtection = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightLightsProtection")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionBuilder
                    .Create("OpportunityAttackImmunityIfAttackerHasConditionPathOfTheLightLightsProtection")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(new IgnoreAoOOnMeLightsProtection())
                    .AddToDB())
            .AddToDB();

        var pathOfTheLightIlluminatingStrikeImprovement = FeatureDefinitionBuilder
            .Create("FeaturePathOfTheLightIlluminatingStrikeImprovement")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var powerPathOfTheLightEyesOfTruth = FeatureDefinitionPowerBuilder
            .Create("PowerPathOfTheLightEyesOfTruth")
            .SetGuiPresentation(Category.Feature, SeeInvisibility)
            .SetUsesFixed(ActivationTime.Permanent)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create("ConditionPathOfTheLightEyesOfTruth")
                                    .SetGuiPresentation(Category.Condition, ConditionSeeInvisibility)
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
            .SetConditionType(ConditionType.Neutral)
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
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
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

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Constitution,
                false,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Constitution)
            .SetDurationData(DurationType.Minute, 1)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique, 3)
            .SetSpeed(SpeedType.CellsPerSeconds, 9.5f)
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
                        ConditionDefinitionBuilder
                            .Create("ConditionPathOfTheLightIlluminatedByBurst")
                            .SetGuiPresentation("ConditionPathOfTheLightIlluminated", Category.Condition,
                                ConditionBranded)
                            .AllowMultipleInstances()
                            .SetConditionType(ConditionType.Detrimental)
                            .SetParentCondition(conditionPathOfTheLightIlluminated)
                            .SetSilent(Silent.WhenAdded)
                            .AddCustomSubFeatures(OnConditionAddedOrRemovedIlluminatedOrIlluminatedByBurst.Marker)
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add)
                    .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
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

        var featureSetPathOfTheLightIlluminatingBurst = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPathOfTheLightIlluminatingBurst")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionPowerBuilder
                    .Create("PowerPathOfTheLightIlluminatingBurstInitiator")
                    .SetGuiPresentationNoContent(true)
                    .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
                    .SetShowCasting(false)
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
                    .AddToDB(),
                FeatureDefinitionPowerBuilder
                    .Create(PowerPathOfTheLightIlluminatingBurstName)
                    .SetGuiPresentation(Category.Feature, PowerDomainSunHeraldOfTheSun)
                    .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
                    .SetEffectDescription(effectDescription)
                    .SetDisableIfConditionIsOwned(conditionPathOfTheLightSuppressedIlluminatingBurst)
                    .SetShowCasting(false)
                    .AddToDB(),
                powerPathOfTheLightIlluminatingBurstSuppressor)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PathOfTheLight")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("PathOfTheLight", Resources.PathOfTheLight, 256))
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

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // behavior classes
    //

    private sealed class IgnoreAoOOnMeLightsProtection : IIgnoreAoOOnMe
    {
        public bool CanIgnoreAoOOnSelf(RulesetCharacter defender, RulesetCharacter attacker)
        {
            return attacker.HasConditionOfType(ConditionPathOfTheLightIlluminatedName);
        }
    }

    private sealed class OnConditionAddedOrRemovedIlluminatedOrIlluminatedByBurst : IOnConditionAddedOrRemoved
    {
        internal static readonly OnConditionAddedOrRemovedIlluminatedOrIlluminatedByBurst Marker = new();

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            HandleAfterIlluminatedConditionRemoved(target);

            if (target is { IsDeadOrDyingOrUnconscious: true })
            {
                ApplyLightsProtectionHealing(rulesetCondition.SourceGuid);
            }
        }

        private static void ApplyLightsProtectionHealing(ulong sourceGuid)
        {
            if (RulesetEntity.GetEntity<RulesetCharacter>(sourceGuid) is not RulesetCharacterHero conditionSource ||
                conditionSource.IsDeadOrDyingOrUnconscious)
            {
                return;
            }

            var levels = conditionSource.GetClassLevel(CharacterClassDefinitions.Barbarian);
            var amountHealed = (levels + 1) / 2;

            conditionSource.ReceiveHealing(amountHealed, true, sourceGuid);
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

            if (gameLocationCharacter != null)
            {
                visibilityService.RemoveCharacterLightSource(gameLocationCharacter, character.PersonalLightSource);
            }

            character.PersonalLightSource = null;
        }
    }
}
