using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialWarlord : AbstractSubclass
{
    private const string Name = "MartialWarlord";
    private const string CoordinatedAssaultMarker = "CoordinatedAssault";
    private const string PressTheAdvantageMarker = "PressTheAdvantage";

    private const ActionDefinitions.Id CoordinatedAssaultToggle =
        (ActionDefinitions.Id)ExtraActionId.CoordinatedAssaultToggle;

    private const ActionDefinitions.Id PressTheAdvantageToggle =
        (ActionDefinitions.Id)ExtraActionId.PressTheAdvantageToggle;

    public MartialWarlord()
    {
        //
        // LEVEL 03
        //

        var conditionStrengthInitiative = ConditionDefinitionBuilder
            .Create($"Condition{Name}WisdomInitiative")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}WisdomInitiative")
                    .SetGuiPresentationNoContent(true)
                    .SetAddConditionAmount(AttributeDefinitions.Initiative)
                    .AddToDB())
            .AddToDB();

        // Relentlessness

        var featureBattlefieldExperience = FeatureDefinitionBuilder
            .Create($"Feature{Name}BattlefieldExperience")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Press the Advantage

        var powerPressTheAdvantage = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PressTheAdvantage")
            .SetGuiPresentation($"FeatureSet{Name}PressTheAdvantage", Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackHit)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.EndOfSourceTurn)
                    .Build())
            .AddCustomSubFeatures(
                new MagicEffectFinishedByMePressTheAdvantage(),
                new RestrictReactionAttackMode((_, attacker, _, mode, _) =>
                    mode != null && // IsWeaponOrUnarmedAttack
                    attacker.OnceInMyTurnIsValid(PressTheAdvantageMarker) &&
                    attacker.RulesetCharacter.IsToggleEnabled(PressTheAdvantageToggle)))
            .AddToDB();

        var combatAffinityExploitOpening = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}ExploitOpening")
            .SetGuiPresentation($"Condition{Name}PredictAttack", Category.Condition)
            .SetAttackOnMeAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(ExtraSituationalContext.IsNotConditionSource)
            .AddToDB();

        var conditionExploitOpening = ConditionDefinitionBuilder
            .Create($"Condition{Name}ExploitOpening")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionTargetedByGuidingBolt)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(combatAffinityExploitOpening)
            .SetSpecialInterruptions(ExtraConditionInterruption.AttackedNotBySource)
            .CopyParticleReferences(ConditionDefinitions.ConditionLeadByExampleMarked)
            .AddToDB();

        combatAffinityExploitOpening.requiredCondition = conditionExploitOpening;

        var powerExploitOpening = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ExploitOpening")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.NoCost, powerPressTheAdvantage)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionExploitOpening))
                    .Build())
            .AddToDB();

        var powerPredictAttack = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PredictAttack")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.NoCost, powerPressTheAdvantage)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{Name}PredictAttack")
                                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPossessed)
                                .SetPossessive()
                                .SetConditionType(ConditionType.Detrimental)
                                .AddFeatures(
                                    FeatureDefinitionCombatAffinityBuilder
                                        .Create($"CombatAffinity{Name}PredictAttack")
                                        .SetGuiPresentation($"Condition{Name}PredictAttack", Category.Condition)
                                        .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                        .AddToDB())
                                .SetSpecialInterruptions(ConditionInterruption.Attacks)
                                .CopyParticleReferences(ConditionDefinitions.ConditionInsane)
                                .AddToDB()))
                    .Build())
            .AddToDB();

        var conditionCoveringStrike = ConditionDefinitionBuilder
            .Create($"Condition{Name}CoveringStrike")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConfused)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .CopyParticleReferences(ConditionDefinitions.ConditionConfused)
            .AddToDB();

        var conditionCoveringStrikeAlly = ConditionDefinitionBuilder
            .Create($"Condition{Name}CoveringStrikeAlly")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}CoveringStrikeAlly")
                    .SetGuiPresentationNoContent(true)
                    .SetAttackOfOpportunityImmunity(true)
                    .SetSituationalContext(SituationalContext.SourceHasCondition, conditionCoveringStrike)
                    .AddToDB())
            .AddToDB();

        conditionCoveringStrike.AddCustomSubFeatures(
            new OnConditionAddedOrRemovedCoveringStrike(conditionCoveringStrikeAlly));

        var powerCoveringStrike = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}CoveringStrike")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.NoCost, powerPressTheAdvantage)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionCoveringStrike))
                    .Build())
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerPressTheAdvantage, true,
            powerCoveringStrike, powerExploitOpening, powerPredictAttack);

        var actionAffinityPressTheAdvantageToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityPressTheAdvantageToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(PressTheAdvantageToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerPressTheAdvantage)))
            .AddToDB();

        var featureSetPressTheAdvantage = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PressTheAdvantage")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerPressTheAdvantage,
                powerExploitOpening, powerPredictAttack, powerCoveringStrike,
                actionAffinityPressTheAdvantageToggle)
            .AddToDB();

        //
        // LEVEL 07
        //

        // Strategic Reposition

        var powerStrategicRepositioning = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}StrategicReposition")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerMonkStepOftheWindDisengage)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .ExcludeCaster()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .Build())
            .AddCustomSubFeatures(new CustomBehaviorStrategicReposition())
            .AddToDB();

        //
        // LEVEL 10
        //

        // Coordinated Assault

        var powerCoordinatedAssault = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CoordinatedAssault")
            .SetGuiPresentation($"FeatureSet{Name}CoordinatedAssault", Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .DelegatedToAction()
            .AddToDB();

        powerCoordinatedAssault.AddCustomSubFeatures(
            new PhysicalAttackFinishedByMeCoordinatedAssault(powerCoordinatedAssault));

        _ = ActionDefinitionBuilder
            .Create(MetamagicToggle, "CoordinatedAssaultToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CoordinatedAssaultToggle)
            .SetActivatedPower(powerCoordinatedAssault)
            .AddToDB();

        var actionAffinityCoordinatedAssaultToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityCoordinatedAssaultToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(CoordinatedAssaultToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerCoordinatedAssault)))
            .AddToDB();

        var featureSetCoordinatedAssault = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}CoordinatedAssault")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerCoordinatedAssault,
                actionAffinityCoordinatedAssaultToggle)
            .AddToDB();

        //
        // LEVEL 15
        //

        // Battle Plan

        var featureBattlePlan = FeatureDefinitionBuilder
            .Create($"Feature{Name}BattlePlan")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureBattlefieldExperience.AddCustomSubFeatures(
            new CharacterBattleStartedListenerBattlefieldExperienceBattlePlan(
                conditionStrengthInitiative, featureBattlefieldExperience, featureBattlePlan));

        //
        // LEVEL 18
        //

        // Control the Field

        var powerControlTheField = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ControlTheField")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerMonkStepOftheWindDisengage)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round)
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasUnavailableBonusAction),
                new CustomBehaviorStrategicReposition(),
                new CharacterBattleStartedListenerControlTheField(powerCoordinatedAssault))
            .AddToDB();

        var featureSetControlTheField = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ControlTheField")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerControlTheField)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.MartialMarshal, 256))
            .AddFeaturesAtLevel(3, featureBattlefieldExperience, featureSetPressTheAdvantage)
            .AddFeaturesAtLevel(7, powerStrategicRepositioning)
            .AddFeaturesAtLevel(10, featureSetCoordinatedAssault)
            .AddFeaturesAtLevel(15, featureBattlePlan)
            .AddFeaturesAtLevel(18, featureSetControlTheField)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Press the Advantage
    //

    private sealed class MagicEffectFinishedByMePressTheAdvantage : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionSpendPower characterActionSpendPower
                || characterActionSpendPower.activePower.PowerDefinition.Name is not (
                    "PowerMartialWarlordCoveringStrike" or
                    "PowerMartialWarlordExploitOpening" or
                    "PowerMartialWarlordPredictAttack"))
            {
                yield break;
            }

            action.ActingCharacter.UsedSpecialFeatures.TryAdd(PressTheAdvantageMarker, 1);
        }
    }

    //
    // Covering Strike
    //

    private sealed class OnConditionAddedOrRemovedCoveringStrike(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionCoveringStrikeAlly)
        : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (Gui.Battle == null)
            {
                return;
            }

            var sourceCharacter = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (sourceCharacter == null)
            {
                return;
            }

            foreach (var character in Gui.Battle.PlayerContenders
                         .Where(x => x.Guid != rulesetCondition.SourceGuid))
            {
                character.RulesetActor.InflictCondition(
                    conditionCoveringStrikeAlly.Name,
                    DurationType.Round,
                    1,
                    (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn,
                    AttributeDefinitions.TagEffect,
                    sourceCharacter.guid,
                    sourceCharacter.CurrentFaction.Name,
                    1,
                    conditionCoveringStrikeAlly.Name,
                    0,
                    0,
                    0);
            }
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    //
    // Strategic Reposition
    //

    private sealed class CustomBehaviorStrategicReposition :
        IFilterTargetingCharacter, ISelectPositionAfterCharacter, IFilterTargetingPosition, IPowerOrSpellFinishedByMe,
        IIgnoreInvisibilityInterruptionCheck
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid =
                !target.RulesetCharacter
                    .HasAnyConditionOfTypeOrSubType(ConditionIncapacitated, ConditionParalyzed, ConditionRestrained);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&SelfOrTargetCannotAct");
            }

            return isValid;
        }

        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            cursorLocationSelectPosition.validPositionsCache.Clear();

            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var targetCharacter = cursorLocationSelectPosition.ActionParams.TargetCharacters[0];
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var visibilityService =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            var halfMaxTacticalMoves = (targetCharacter.MaxTacticalMoves + 1) / 2; // half-rounded up
            var boxInt = new BoxInt(targetCharacter.LocationPosition, int3.zero, int3.zero);

            boxInt.Inflate(halfMaxTacticalMoves, 0, halfMaxTacticalMoves);

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (!visibilityService.MyIsCellPerceivedByCharacter(position, actingCharacter) ||
                    !positioningService.CanPlaceCharacter(
                        actingCharacter, position, CellHelpers.PlacementMode.Station) ||
                    !positioningService.CanCharacterStayAtPosition_Floor(
                        actingCharacter, position, onlyCheckCellsWithRealGround: true))
                {
                    continue;
                }

                cursorLocationSelectPosition.validPositionsCache.Add(position);

                if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                {
                    yield return null;
                }
            }
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActionParams.activeEffect.EffectDescription.rangeParameter = 6;

            var actingCharacter = action.ActingCharacter;
            var targetCharacter = action.ActionParams.TargetCharacters[0];
            var targetRulesetCharacter = targetCharacter.RulesetCharacter;
            var targetPosition = action.ActionParams.Positions[0];
            var actionParams =
                new CharacterActionParams(targetCharacter, ActionDefinitions.Id.TacticalMove)
                {
                    Positions = { targetPosition }
                };

            targetCharacter.UsedTacticalMoves = 0;
            targetRulesetCharacter.InflictCondition(
                ConditionDisengaging,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                // all disengaging in game is set under TagCombat (why?)
                AttributeDefinitions.TagCombat,
                targetRulesetCharacter.Guid,
                targetRulesetCharacter.CurrentFaction.Name,
                1,
                ConditionDisengaging,
                0,
                0,
                0);

            EffectHelpers.StartVisualEffect(actingCharacter, targetCharacter,
                FeatureDefinitionPowers.PowerDomainSunHeraldOfTheSun, EffectHelpers.EffectType.Effect);

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);

            yield break;
        }

        public int PositionRange => 12;

        public bool EnforcePositionSelection(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            return true;
        }
    }

    //
    // Coordinated Assault
    //

    private sealed class PhysicalAttackFinishedByMeCoordinatedAssault(FeatureDefinitionPower powerCoordinatedAssault)
        : IPhysicalAttackFinishedByMe
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
            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter.GetRemainingPowerUses(powerCoordinatedAssault) == 0)
            {
                yield break;
            }

            if (!rulesetCharacter.IsToggleEnabled(CoordinatedAssaultToggle))
            {
                yield break;
            }

            if (!attacker.OnceInMyTurnIsValid(CoordinatedAssaultMarker))
            {
                yield break;
            }

            var actionParams = action.actionParams;

            // non-reaction melee hits only
            if (attackMode.ranged || rollOutcome is RollOutcome.CriticalFailure or RollOutcome.Failure ||
                actionParams.actionDefinition.Id == ActionDefinitions.Id.AttackOpportunity)
            {
                yield break;
            }

            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var allies = new List<GameLocationCharacter>();

            foreach (var guestCharacter in characterService.GuestCharacters.ToList())
            {
                if (guestCharacter.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
                {
                    continue;
                }

                if (!rulesetCharacterMonster.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagConjure,
                        ConditionConjuredCreature,
                        out var activeCondition)
                    || activeCondition.SourceGuid != attacker.Guid)
                {
                    continue;
                }

                if (guestCharacter.CanReact())
                {
                    allies.Add(guestCharacter);
                }
            }

            allies.AddRange(characterService.PartyCharacters
                .Where(partyCharacter => partyCharacter.CanReact() && partyCharacter != attacker));

            var reactions = new List<CharacterActionParams>();

            foreach (var partyCharacter in allies)
            {
                RulesetAttackMode mode;
                ActionModifier modifier;

                //prefer melee if main hand is melee or if enemy is close
                var preferMelee =
                    ValidatorsWeapon.IsMelee(partyCharacter.RulesetCharacter.GetMainWeapon()) ||
                    partyCharacter.IsWithinRange(defender, 1);

                var (meleeMode, meleeModifier) = partyCharacter.GetFirstMeleeModeThatCanAttack(defender);
                var (rangedMode, rangedModifier) = partyCharacter.GetFirstRangedModeThatCanAttack(defender);

                if (preferMelee)
                {
                    mode = meleeMode ?? rangedMode;
                    modifier = meleeModifier ?? rangedModifier;
                }
                else
                {
                    mode = rangedMode ?? meleeMode;
                    modifier = rangedModifier ?? meleeModifier;
                }

                if (mode == null)
                {
                    var cantrips = ReactionRequestWarcaster.GetValidCantrips(battleManager, partyCharacter, defender);

                    if (cantrips == null || cantrips.Count == 0)
                    {
                        continue;
                    }
                }

                var reactionParams = new CharacterActionParams(partyCharacter, ActionDefinitions.Id.AttackOpportunity)
                {
                    StringParameter2 = "CoordinatedAssault", BoolParameter4 = mode == null // true means no attack
                };

                reactionParams.targetCharacters.Add(defender);
                reactionParams.actionModifiers.Add(modifier ?? new ActionModifier());

                if (mode != null)
                {
                    reactionParams.attackMode = RulesetAttackMode.AttackModesPool.Get();
                    reactionParams.attackMode.Copy(mode);
                }

                reactions.Add(reactionParams);
            }

            if (reactions.Count == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var count = actionService.PendingReactionRequestGroups.Count;

            foreach (var reaction in reactions)
            {
                actionService.ReactForOpportunityAttack(reaction);
            }

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            var firstValidatedReaction = reactions.FirstOrDefault(x => x.ReactionValidated);

            if (firstValidatedReaction == null)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerCoordinatedAssault, rulesetCharacter);

            attacker.UsedSpecialFeatures.TryAdd(CoordinatedAssaultMarker, 0);
            rulesetCharacter.UsePower(usablePower);
        }
    }

    //
    // Battlefield Experience / Battle Plan
    //

    private sealed class CharacterBattleStartedListenerBattlefieldExperienceBattlePlan(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionStrengthInitiative,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureBattlefieldExperience,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureBattlePlan)
        : ICharacterBattleStartedListener, IRollSavingThrowInitiated
    {
        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            if (Gui.Battle == null)
            {
                return;
            }

            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var levels = rulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Fighter, Name);
            var strengthModifier = Math.Max(AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Strength)), 1);

            rulesetCharacter.InflictCondition(
                conditionStrengthInitiative.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.Guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionStrengthInitiative.Name,
                strengthModifier,
                0,
                0);

            if (levels < 15)
            {
                rulesetCharacter.LogCharacterUsedFeature(featureBattlefieldExperience);
            }
            else
            {
                foreach (var player in Gui.Battle
                             .GetContenders(locationCharacter, isOppositeSide: false, withinRange: 6))
                {
                    player.RulesetCharacter.InflictCondition(
                        conditionStrengthInitiative.Name,
                        DurationType.Round,
                        1,
                        TurnOccurenceType.EndOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetCharacter.Guid,
                        rulesetCharacter.CurrentFaction.Name,
                        1,
                        conditionStrengthInitiative.Name,
                        (strengthModifier + 1) / 2,
                        0,
                        0);
                }

                rulesetCharacter.LogCharacterUsedFeature(featureBattlePlan); 
            }
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
            var hasCharmedOrFrightened = effectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Condition)
                .Select(effectForm => effectForm.ConditionForm.ConditionDefinition)
                .Any(condition =>
                    condition == ConditionDefinitions.ConditionCharmed ||
                    condition.parentCondition == ConditionDefinitions.ConditionCharmed ||
                    condition == ConditionDefinitions.ConditionFrightened ||
                    condition.parentCondition == ConditionDefinitions.ConditionFrightened);

            if (!hasCharmedOrFrightened)
            {
                return;
            }

            advantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, featureBattlePlan.Name, featureBattlePlan));
        }
    }

    //
    // Control the Field
    //

    private sealed class CharacterBattleStartedListenerControlTheField(FeatureDefinitionPower powerCoordinatedAssault)
        : ICharacterBattleStartedListener
    {
        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerCoordinatedAssault, rulesetCharacter);

            rulesetCharacter.RepayPowerUse(usablePower);
        }
    }
}
