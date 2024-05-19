using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerFeyWanderer : AbstractSubclass
{
    private const string Name = "RangerFeyWanderer";

    public RangerFeyWanderer()
    {
        //
        // LEVEL 03
        //

        // Fey Wanderer Magic

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, CharmPerson),
                BuildSpellGroup(5, MistyStep),
                BuildSpellGroup(9, DispelMagic),
                BuildSpellGroup(13, DimensionDoor),
                BuildSpellGroup(17, SpellsContext.SteelWhirlwind))
            .AddToDB();

        // Dreadful Strikes

        var conditionDreadfulStrikes = ConditionDefinitionBuilder
            .Create($"Condition{Name}DreadfulStrikes")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration()
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var additionalDamageDreadfulStrikes = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DreadfulStrikes")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DreadfulStrikes")
            .SetDamageDice(DieType.D4, 1)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetTargetCondition(conditionDreadfulStrikes, AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetAttackModeOnly()
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = conditionDreadfulStrikes,
                Operation = ConditionOperationDescription.ConditionOperation.Add
            })
            .AddToDB();

        additionalDamageDreadfulStrikes.AddCustomSubFeatures(
            new ModifyAdditionalDamageDreadfulStrikes(additionalDamageDreadfulStrikes));

        // Otherworldly Glamour

        var pointPoolOtherworldlyGlamour = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{Name}OtherworldlyGlamour")
            .SetGuiPresentationNoContent(true)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.Deception,
                SkillDefinitions.Performance,
                SkillDefinitions.Persuasion)
            .AddToDB();

        var abilityCheckAffinityOtherworldlyGlamour = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}OtherworldlyGlamour")
            .SetGuiPresentation($"FeatureSet{Name}OtherworldlyGlamour", Category.Feature, Gui.NoLocalization)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, AttributeDefinitions.Charisma)
            .AddToDB();

        var featureSetOtherworldlyGlamour = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}OtherworldlyGlamour")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(pointPoolOtherworldlyGlamour, abilityCheckAffinityOtherworldlyGlamour)
            .AddToDB();

        //
        // LEVEL 07
        //

        // Beguiling Twist

        var powerBeguilingTwist = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BeguilingTwist")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .AddToDB();

        var powerBeguilingTwistCharmed = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BeguilingTwistCharmed")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.NoCost, powerBeguilingTwist)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerBeguilingTwistFrightened = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BeguilingTwistFrightened")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.NoCost, powerBeguilingTwist)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        powerBeguilingTwist.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorBeguilingTwist(powerBeguilingTwist));

        PowerBundle.RegisterPowerBundle(powerBeguilingTwist, false,
            powerBeguilingTwistCharmed, powerBeguilingTwistFrightened);

        //
        // LEVEL 11
        //

        // Fey Reinforcements

        var powerFeyReinforcements = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}FeyReinforcements")
            .SetGuiPresentation(Category.Feature, ConjureFey)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(ConjureFey)
                    .SetDurationData(DurationType.Minute, 1)
                    .Build())
            .AddToDB();

        //
        // LEVEL 15
        //

        // Misty Wanderer

        var powerMistyWanderer = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MistyWanderer")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(MistyStep)
                    .InviteOptionalAlly()
                    .Build())
            .AddCustomSubFeatures(new CustomBehaviorMistyWanderer())
            .AddToDB();

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CircleOfTheAncientForest, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells, additionalDamageDreadfulStrikes, featureSetOtherworldlyGlamour)
            .AddFeaturesAtLevel(7,
                powerBeguilingTwist, powerBeguilingTwistCharmed, powerBeguilingTwistFrightened)
            .AddFeaturesAtLevel(11, powerFeyReinforcements)
            .AddFeaturesAtLevel(15, powerMistyWanderer)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Ranger;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyAdditionalDamageDreadfulStrikes(
        FeatureDefinitionAdditionalDamage additionalDamageDreadfulStrikes) : IModifyAdditionalDamage
    {
        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm damageForm)
        {
            if (featureDefinitionAdditionalDamage != additionalDamageDreadfulStrikes)
            {
                return;
            }

            var classLevel = attacker.RulesetCharacter.GetClassLevel(CharacterClassDefinitions.Ranger);

            if (classLevel < 11)
            {
                return;
            }

            damageForm.DieType = DieType.D6;
        }
    }

    private sealed class CustomBehaviorBeguilingTwist(
        FeatureDefinitionPower powerBeguilingTwist)
        : IRollSavingThrowInitiated, ITryAlterOutcomeSavingThrow, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActingCharacter.UsedMainSpell = true;

            yield break;
        }

        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref int saveBonus,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            ref int rollModifier,
            ref int saveDC,
            ref bool hasHitVisual,
            RollOutcome outcome,
            int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (HasCharmedOrFrightened(effectForms))
            {
                advantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.Power, powerBeguilingTwist.Name, powerBeguilingTwist));
            }
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (!actionManager ||
                !action.RolledSaveThrow ||
                action.SaveOutcome != RollOutcome.Success ||
                !HasCharmedOrFrightened(
                    action.ActionParams.activeEffect?.EffectDescription.EffectForms ??
                    action.ActionParams.AttackMode?.EffectDescription.EffectForms ??
                    []) ||
                !helper.IsOppositeSide(attacker.Side) ||
                !helper.IsWithinRange(defender, 24) ||
                !helper.CanPerceiveTarget(defender))
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerBeguilingTwist, rulesetHelper);
            var actionParams = new CharacterActionParams(helper, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { actionModifier },
                StringParameter = powerBeguilingTwist.Name,
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { attacker }
            };

            var count = actionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendBundlePower(actionParams);

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);

            if (!reactionRequest.Validated)
            {
                yield break;
            }

            helper.SpendActionType(ActionDefinitions.ActionType.Reaction);
        }

        private static bool HasCharmedOrFrightened(List<EffectForm> effectForms)
        {
            return effectForms.Any(x =>
                x.FormType == EffectForm.EffectFormType.Condition &&
                (x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionCharmed) ||
                 x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionFrightened)));
        }
    }

    private sealed class CustomBehaviorMistyWanderer : IMagicEffectFinishedByMe, IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid =
                target.RulesetCharacter is not RulesetCharacterEffectProxy &&
                __instance.ActionParams.ActingCharacter.IsWithinRange(target, 1);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustBeWithin5ft");
            }

            return isValid;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActingCharacter.UsedBonusSpell = true;

            yield break;
        }
    }
}
