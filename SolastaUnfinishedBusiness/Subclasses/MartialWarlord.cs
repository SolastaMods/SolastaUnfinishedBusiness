using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;

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

    internal static FeatureDefinitionPower PowerCoordinatedAssault;

    public MartialWarlord()
    {
        //
        // LEVEL 03
        //

        var conditionWisdomInitiative = ConditionDefinitionBuilder
            .Create($"Condition{Name}WisdomInitiative")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}WisdomInitiative")
                    .SetGuiPresentationNoContent(true)
                    .SetModifierAbilityScore(AttributeDefinitions.Initiative, AttributeDefinitions.Wisdom)
                    .SetAddConditionAmount(AttributeDefinitions.Initiative)
                    .AddToDB())
            .AddToDB();

        // Battlefield Experience

        var featureBattlefieldExperience = FeatureDefinitionBuilder
            .Create($"Feature{Name}BattlefieldExperience")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CharacterBattleStartedListenerBattlefieldExperience(conditionWisdomInitiative))
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
                IsPowerPool.Marker,
                new MagicEffectFinishedByMePressTheAdvantage(),
                new RestrictReactionAttackMode((_, attacker, _, mode, _) =>
                    mode != null && // weapon or unarmed only
                    attacker.OnceInMyTurnIsValid(PressTheAdvantageMarker) &&
                    attacker.RulesetCharacter.IsToggleEnabled(PressTheAdvantageToggle)))
            .AddToDB();

        var combatAffinityExploitOpening = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}ExploitOpening")
            .SetGuiPresentation($"Condition{Name}PredictAttack", Category.Condition)
            .SetAttackOnMeAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(
                (SituationalContext)ExtraSituationalContext.IsNotSourceOfCondition)
            .AddToDB();

        var conditionExploitOpening = ConditionDefinitionBuilder
            .Create($"Condition{Name}ExploitOpening")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBaned)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(combatAffinityExploitOpening)
            .AddToDB();

        conditionExploitOpening.AddCustomSubFeatures(
            RemoveConditionOnSourceTurnStart.Mark,
            new PhysicalAttackFinishedOnMeExploitOpening(conditionExploitOpening));

        combatAffinityExploitOpening.requiredCondition = conditionExploitOpening;

        var powerExploitOpening = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ExploitOpening")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.NoCost, powerPressTheAdvantage)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
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
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{Name}PredictAttack")
                                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
                                .SetPossessive()
                                .SetConditionType(ConditionType.Detrimental)
                                .AddFeatures(
                                    FeatureDefinitionCombatAffinityBuilder
                                        .Create($"CombatAffinity{Name}PredictAttack")
                                        .SetGuiPresentation($"Condition{Name}PredictAttack", Category.Condition)
                                        .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                        .AddToDB())
                                .AddCustomSubFeatures(RemoveConditionOnSourceTurnStart.Mark)
                                .SetSpecialInterruptions(ConditionInterruption.Attacks)
                                .AddToDB()))
                    .Build())
            .AddToDB();

        var conditionCoveringStrike = ConditionDefinitionBuilder
            .Create($"Condition{Name}CoveringStrike")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBranded)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
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
            .AddCustomSubFeatures(RemoveConditionOnSourceTurnStart.Mark)
            .AddToDB();

        conditionCoveringStrike.AddCustomSubFeatures(
            RemoveConditionOnSourceTurnStart.Mark,
            new OnConditionAddedOrRemovedCoveringStrike(conditionCoveringStrikeAlly));

        var powerCoveringStrike = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}CoveringStrike")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.NoCost, powerPressTheAdvantage)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
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
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDisengaging))
                    .Build())
            .AddCustomSubFeatures(new CustomBehaviorStrategicReposition())
            .AddToDB();

        //
        // LEVEL 10
        //

        // Coordinated Assault

        PowerCoordinatedAssault = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CoordinatedAssault")
            .SetGuiPresentation($"FeatureSet{Name}CoordinatedAssault", Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.LongRest, 1, 3)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddCustomSubFeatures(
                new PhysicalAttackFinishedByMeCoordinatedAssault(),
                new RestrictReactionAttackMode((_, attacker, _, _, _) =>
                    attacker.OnceInMyTurnIsValid(CoordinatedAssaultMarker) &&
                    attacker.RulesetCharacter.IsToggleEnabled(CoordinatedAssaultToggle)))
            .AddToDB();

        var actionAffinityCoordinatedAssaultToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityCoordinatedAssaultToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(CoordinatedAssaultToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(PowerCoordinatedAssault)))
            .AddToDB();

        var featureSetCoordinatedAssault = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}CoordinatedAssault")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                PowerCoordinatedAssault,
                actionAffinityCoordinatedAssaultToggle)
            .AddToDB();

        //
        // LEVEL 15
        //

        // Battle Plan

        var featureBattlePlan = FeatureDefinitionBuilder
            .Create($"Feature{Name}BattlePlan")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CharacterBattleStartedListenerBattlePlan(conditionWisdomInitiative))
            .AddToDB();

        //
        // LEVEL 18
        //

        // Control the Field

        var powerControlTheField = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ControlTheField")
            .SetGuiPresentation(Category.Feature, FeatureDefinitionPowers.PowerMonkStepOftheWindDisengage)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDisengaging))
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(
                    ValidatorsCharacter.HasNoAvailableBonusAction, ValidatorsCharacter.HasAvailableReaction),
                new CustomBehaviorStrategicReposition(),
                new CharacterBattleStartedListenerControlTheField(PowerCoordinatedAssault))
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
    // Battlefield Experience
    //

    private sealed class CharacterBattleStartedListenerBattlefieldExperience : ICharacterBattleStartedListener
    {
        private readonly ConditionDefinition _conditionBattlefieldExperience;

        public CharacterBattleStartedListenerBattlefieldExperience(ConditionDefinition conditionBattlefieldExperience)
        {
            _conditionBattlefieldExperience = conditionBattlefieldExperience;
        }

        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter.HasAnyConditionOfTypeOrSubType(ConditionIncapacitated, ConditionSurprised))
            {
                return;
            }

            var wisdomModifier = Math.Max(AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Wisdom)), 1);

            rulesetCharacter.InflictCondition(
                _conditionBattlefieldExperience.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.Guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                _conditionBattlefieldExperience.Name,
                wisdomModifier,
                0,
                0);
        }
    }

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

    private sealed class OnConditionAddedOrRemovedCoveringStrike : IOnConditionAddedOrRemoved
    {
        private readonly ConditionDefinition _conditionCoveringStrikeAlly;

        public OnConditionAddedOrRemovedCoveringStrike(ConditionDefinition conditionCoveringStrikeAlly)
        {
            _conditionCoveringStrikeAlly = conditionCoveringStrikeAlly;
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (Gui.Battle == null)
            {
                return;
            }

            foreach (var character in Gui.Battle.PlayerContenders
                         .Where(x => x.Guid != rulesetCondition.SourceGuid))
            {
                character.RulesetCharacter.InflictCondition(
                    _conditionCoveringStrikeAlly.Name,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfSourceTurn,
                    AttributeDefinitions.TagCombat,
                    target.guid,
                    target.CurrentFaction.Name,
                    1,
                    _conditionCoveringStrikeAlly.Name,
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
    // Exploit Opening
    //

    private sealed class PhysicalAttackFinishedOnMeExploitOpening : IPhysicalAttackFinishedOnMe
    {
        private readonly ConditionDefinition _conditionExploitOpening;

        public PhysicalAttackFinishedOnMeExploitOpening(ConditionDefinition conditionExploitOpening)
        {
            _conditionExploitOpening = conditionExploitOpening;
        }

        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, _conditionExploitOpening.Name, out var activeCondition) &&
                attacker.Guid != activeCondition.SourceGuid)
            {
                rulesetDefender.RemoveCondition(activeCondition);
            }
        }
    }

    //
    // Strategic Reposition
    //

    private sealed class CustomBehaviorStrategicReposition :
        ISelectPositionAfterCharacter, IFilterTargetingPosition, IMagicEffectFinishedByMe
    {
        public void EnumerateValidPositions(
            CursorLocationSelectPosition cursorLocationSelectPosition,
            List<int3> validPositions)
        {
            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;

            if (!actingCharacter.UsedSpecialFeatures.TryGetValue("SelectedCharacter", out var targetGuid))
            {
                return;
            }

            var gameLocationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var targetRulesetCharacter = EffectHelpers.GetCharacterByGuid((ulong)targetGuid);
            var targetCharacter = GameLocationCharacter.GetFromActor(targetRulesetCharacter);
            var halfMaxTacticalMoves = (targetCharacter.MaxTacticalMoves + 1) / 2; // half-rounded up
            var boxInt = new BoxInt(
                targetCharacter.LocationPosition,
                new int3(-halfMaxTacticalMoves, -halfMaxTacticalMoves, -halfMaxTacticalMoves),
                new int3(halfMaxTacticalMoves, halfMaxTacticalMoves, halfMaxTacticalMoves));

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (gameLocationPositioningService.CanPlaceCharacter(
                        targetCharacter, position, CellHelpers.PlacementMode.Station) &&
                    gameLocationPositioningService.CanCharacterStayAtPosition_Floor(
                        targetCharacter, position, onlyCheckCellsWithRealGround: true))
                {
                    validPositions.Add(position);
                }
            }
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (!action.ActingCharacter.UsedSpecialFeatures.TryGetValue("SelectedCharacter", out var targetGuid))
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var targetRulesetCharacter = EffectHelpers.GetCharacterByGuid((ulong)targetGuid);
            var targetCharacter = GameLocationCharacter.GetFromActor(targetRulesetCharacter);
            var targetPosition = action.ActionParams.Positions[0];
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var actionParams =
                new CharacterActionParams(targetCharacter, ActionDefinitions.Id.TacticalMove)
                {
                    Positions = { targetPosition }
                };

            targetRulesetCharacter.InflictCondition(
                ConditionDisengaging,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
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

            targetCharacter.CurrentActionRankByType[ActionDefinitions.ActionType.Reaction]++;
            actionService.ExecuteAction(actionParams, null, false);

            if (action.ActionParams.RulesetEffect is RulesetEffectPower rulesetEffectPower &&
                rulesetEffectPower.PowerDefinition.Name == $"Power{Name}ControlTheField")
            {
                actingCharacter.CurrentActionRankByType[ActionDefinitions.ActionType.Reaction]++;
            }
        }
    }

    //
    // Coordinated Assault
    //

    private sealed class PhysicalAttackFinishedByMeCoordinatedAssault : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter.GetRemainingPowerUses(PowerCoordinatedAssault) == 0)
            {
                yield break;
            }

            if (!rulesetCharacter.IsToggleEnabled(CoordinatedAssaultToggle))
            {
                yield break;
            }

            if (!attacker.OncePerTurnIsValid(CoordinatedAssaultMarker))
            {
                yield break;
            }

            var actionParams = action.actionParams;

            // non-reaction melee hits only
            if (attackMode.ranged || attackRollOutcome is RollOutcome.CriticalFailure or RollOutcome.Failure ||
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
                var preferMelee = ValidatorsWeapon.IsMelee(partyCharacter.RulesetCharacter.GetMainWeapon())
                                  || (battleManager != null && battleManager.IsWithin1Cell(partyCharacter, defender));

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

                    if (cantrips == null || cantrips.Empty())
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

            if (reactions.Empty() || battleManager is not { IsBattleInProgress: true })
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

            rulesetCharacter.UpdateUsageForPower(PowerCoordinatedAssault, PowerCoordinatedAssault.CostPerUse);
        }
    }

    //
    // Battle Plan
    //

    private sealed class CharacterBattleStartedListenerBattlePlan : ICharacterBattleStartedListener
    {
        private readonly ConditionDefinition _conditionBattlePlan;

        public CharacterBattleStartedListenerBattlePlan(ConditionDefinition powerConditionBattlePlan)
        {
            _conditionBattlePlan = powerConditionBattlePlan;
        }

        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter.HasAnyConditionOfTypeOrSubType(ConditionIncapacitated, ConditionSurprised))
            {
                return;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                return;
            }


            var wisdomModifier = Math.Max(AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Wisdom)), 1);

            foreach (var player in gameLocationBattleService.Battle.AllContenders
                         .Where(x => x.Side == locationCharacter.Side &&
                                     x != locationCharacter &&
                                     x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                     gameLocationBattleService.IsWithinXCells(locationCharacter, x, 6)))

            {
                player.RulesetCharacter.InflictCondition(
                    _conditionBattlePlan.Name,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagCombat,
                    rulesetCharacter.Guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    _conditionBattlePlan.Name,
                    wisdomModifier,
                    0,
                    0);
            }
        }
    }

    //
    // Control the Field
    //

    private sealed class CharacterBattleStartedListenerControlTheField : ICharacterBattleStartedListener
    {
        private readonly FeatureDefinitionPower _powerCoordinatedAssault;

        public CharacterBattleStartedListenerControlTheField(FeatureDefinitionPower powerCoordinatedAssault)
        {
            _powerCoordinatedAssault = powerCoordinatedAssault;
        }

        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerCoordinatedAssault, rulesetCharacter);

            rulesetCharacter.RepayPowerUse(usablePower);
        }
    }
}
