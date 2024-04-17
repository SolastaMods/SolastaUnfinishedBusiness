using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static AttributeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CollegeOfElegance : AbstractSubclass
{
    private const string Name = "CollegeOfElegance";
    private const ActionDefinitions.Id AmazingDisplayToggle = (ActionDefinitions.Id)ExtraActionId.ImpishWrathToggle;

    public CollegeOfElegance()
    {
        // LEVEL 03

        // Grace

        var dieRollModifierGrace = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}Grace")
            .SetGuiPresentation(Category.Feature)
            .SetModifiers(
                RollContext.AbilityCheck,
                0,
                10,
                0,
                "Feedback/&DieRollModifierCollegeOfEleganceGraceReroll",
                // Dexterity Checks
                SkillDefinitions.Acrobatics,
                // Charisma Checks
                SkillDefinitions.Perception)
            .AddToDB();

        // Elegant Fighting

        var conditionElusiveMovement = ConditionDefinitionBuilder
            .Create($"Condition{Name}ElegantFightingInitiative")
            .SetGuiPresentation(Name, Category.Subclass, Gui.NoLocalization)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}ElegantFightingInitiative")
                    .SetGuiPresentation(Name, Category.Subclass, Gui.NoLocalization)
                    .SetAddConditionAmount(Initiative)
                    .SetSituationalContext(SituationalContext.NotWearingArmorOrMageArmorOrShield)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .AddToDB();

        var attributeModifierElegantStepsArmorClass = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ElegantFightingArmorClass")
            .SetGuiPresentation(Name, Category.Subclass, Gui.NoLocalization)
            .SetDexPlusAbilityScore(ArmorClass, Charisma)
            .SetSituationalContext(SituationalContext.NotWearingArmorOrMageArmorOrShield)
            .AddCustomSubFeatures(new CharacterBattleStartedListenerElegantStepsInitiative(conditionElusiveMovement))
            .AddToDB();

        var conditionElegantSteps = ConditionDefinitionBuilder
            .Create($"Condition{Name}ElegantSteps")
            .SetGuiPresentation(Name, Category.Subclass, Gui.NoLocalization)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{Name}ElegantSteps")
                    .SetGuiPresentation(Name, Category.Subclass, Gui.NoLocalization)
                    .SetAuthorizedActions(
                        ActionDefinitions.Id.DashBonus,
                        ActionDefinitions.Id.DisengageBonus,
                        ActionDefinitions.Id.HideBonus)
                    .AddToDB())
            .AddToDB();

        var powerElegantSteps = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ElegantSteps")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.BardicInspiration)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionElegantSteps))
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasNoneOfConditions(conditionElegantSteps.Name)))
            .AddToDB();

        var featureSetElegantFighting = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ElegantFighting")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(attributeModifierElegantStepsArmorClass, powerElegantSteps)
            .AddToDB();

        // LEVEL 06

        // Evasive Footwork

        var conditionEvasiveFootwork = ConditionDefinitionBuilder
            .Create($"Condition{Name}EvasiveFootwork")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}EvasiveFootwork")
                    .SetGuiPresentation(Name, Category.Subclass, Gui.NoLocalization)
                    .SetAddConditionAmount(ArmorClass)
                    .AddToDB())
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
            .AddToDB();

        var powerEvasiveFootwork = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EvasiveFootwork")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.BardicInspiration)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionEvasiveFootwork))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerKnightLeadership)
                    .Build())
            .AddToDB();

        powerEvasiveFootwork.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new AttackBeforeHitPossibleOnMeOrAllyEvasiveFootwork(powerEvasiveFootwork));

        // Extra Attack

        // LEVEL 14

        // Amazing Display

        const string AmazingDisplayName = $"FeatureSet{Name}AmazingDisplay";

        var powerAmazingDisplayEnemy = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AmazingDisplayEnemy")
            .SetGuiPresentation(AmazingDisplayName, Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, Wisdom, true, EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionHindered,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerAmazingDisplay = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AmazingDisplay")
            .SetGuiPresentation(AmazingDisplayName, Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .DelegatedToAction()
            .AddToDB();

        powerAmazingDisplay.AddCustomSubFeatures(
            new CustomBehaviorAmazingDisplay(powerAmazingDisplay, powerAmazingDisplayEnemy));

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "AmazingDisplayToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.ImpishWrathToggle)
            .SetActivatedPower(powerAmazingDisplay)
            .AddToDB();

        var actionAffinityAmazingDisplayToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityAmazingDisplayToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(AmazingDisplayToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerAmazingDisplay)))
            .AddToDB();

        var featureSetAmazingDisplay = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AmazingDisplay")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(powerAmazingDisplayEnemy, powerAmazingDisplay, actionAffinityAmazingDisplayToggle)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CollegeOfWarDancer, 256))
            .AddFeaturesAtLevel(3, dieRollModifierGrace, featureSetElegantFighting)
            .AddFeaturesAtLevel(6, powerEvasiveFootwork, AttributeModifierCasterFightingExtraAttack)
            .AddFeaturesAtLevel(14, featureSetAmazingDisplay)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Bard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CharacterBattleStartedListenerElegantStepsInitiative(
        BaseDefinition conditionElegantStepsInitiative) : ICharacterBattleStartedListener
    {
        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var dieType = rulesetCharacter.GetBardicInspirationDieValue();
            var dieRoll = RollDie(dieType, AdvantageType.None, out _, out _);

            rulesetCharacter.InflictCondition(
                conditionElegantStepsInitiative.Name,
                DurationType.UntilAnyRest,
                1,
                TurnOccurenceType.StartOfTurn,
                TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionElegantStepsInitiative.Name,
                dieRoll,
                0,
                0);
        }
    }

    private class AttackBeforeHitPossibleOnMeOrAllyEvasiveFootwork(FeatureDefinitionPower powerDefensiveDuelist)
        : IAttackBeforeHitPossibleOnMeOrAlly
    {
        public IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            int attackRoll)
        {
            if (rulesetEffect != null &&
                rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;

            if (helper != defender ||
                !defender.CanReact() ||
                !ValidatorsCharacter.HasNoArmor(rulesetHelper) ||
                !ValidatorsCharacter.HasNoShield(rulesetHelper))
            {
                yield break;
            }

            var armorClass = defender.RulesetCharacter.TryGetAttributeValue(ArmorClass);
            var totalAttack =
                attackRoll +
                (attackMode?.ToHitBonus ?? rulesetEffect?.MagicAttackBonus ?? 0) +
                actionModifier.AttackRollModifier;

            if (armorClass > totalAttack)
            {
                yield break;
            }

            var maxDie = DiceMaxValue[(int)rulesetHelper.GetBardicInspirationDieValue()];

            if (armorClass + maxDie <= totalAttack)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerDefensiveDuelist, rulesetHelper);
            var actionParams =
                new CharacterActionParams(helper, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "EvasiveFootwork",
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(actionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!actionParams.ReactionValidated)
            {
            }

            //TODO: update rulesetCondition with bardic die roll
        }
    }

    private class CustomBehaviorAmazingDisplay(
        FeatureDefinitionPower powerAmazingDisplay,
#pragma warning disable CS9113 // Parameter is unread.
        FeatureDefinitionPower powerAmazingDisplayEnemy) : IPhysicalAttackFinishedByMe
#pragma warning restore CS9113 // Parameter is unread.
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled(AmazingDisplayToggle))
            {
                yield break;
            }

            if (Gui.Battle == null)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerAmazingDisplay, rulesetAttacker);

            rulesetAttacker.UsePower(usablePower);

            //TODO: collect targets to use 2nd power
        }
    }
}
