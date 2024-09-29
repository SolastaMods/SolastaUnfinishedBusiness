using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.FightingStyles;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class GrappleContext
{
    private const string GrappleName = "Grapple";
    private const string DisableGrappleName = "DisableGrapple";

    internal const string ConditionGrappleSourceName = $"Condition{GrappleName}Source";
    private const string ConditionGrappleTargetName = $"Condition{GrappleName}Target";

    private static readonly FeatureDefinitionPower PowerGrapple = FeatureDefinitionPowerBuilder
        .Create($"Power{GrappleName}")
        .SetGuiPresentation($"Action{GrappleName}", Category.Action, hidden: true)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                .SetImpactEffectParameters(Knock)
                .Build())
        .AddCustomSubFeatures(new CustomBehaviorGrapple())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerDisableGrapple = FeatureDefinitionPowerBuilder
        .Create($"Power{DisableGrappleName}")
        .SetGuiPresentation($"Action{DisableGrappleName}", Category.Action, hidden: true)
        .SetShowCasting(false)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetImpactEffectParameters(Slow)
                .Build())
        .AddCustomSubFeatures(new PowerOrSpellFinishedByMeDisableGrapple())
        .AddToDB();

    private static readonly ActionDefinition ActionGrapple = ActionDefinitionBuilder
        .Create($"Action{GrappleName}")
        .SetGuiPresentation(Category.Action, AttackFree)
        .OverrideClassName("UsePower")
        .SetActivatedPower(PowerGrapple)
        .SetActionId(ExtraActionId.Grapple)
        .SetActionScope(ActionDefinitions.ActionScope.All)
        .SetActionType(ActionDefinitions.ActionType.NoCost)
        .SetFormType(ActionDefinitions.ActionFormType.Large)
        .AddToDB();

    private static readonly ActionDefinition ActionDisableGrapple = ActionDefinitionBuilder
        .Create($"Action{DisableGrappleName}")
        .SetGuiPresentation(Category.Action, AttackFree)
        .OverrideClassName("UsePower")
        .SetActivatedPower(PowerDisableGrapple)
        .SetActionId(ExtraActionId.DisableGrapple)
        .SetActionScope(ActionDefinitions.ActionScope.All)
        .SetActionType(ActionDefinitions.ActionType.NoCost)
        .SetFormType(ActionDefinitions.ActionFormType.Large)
        .AddToDB();

    internal static void LoadGrapple()
    {
        var battlePackage = AiContext.BuildDecisionPackageBreakFree(ConditionGrappleTargetName);

        var conditionGrappleTarget = ConditionDefinitionBuilder
            .Create(ConditionGrappleTargetName)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHindered)
            .SetConditionType(ConditionType.Detrimental)
            .SetFixedAmount((int)AiContext.BreakFreeType.DoStrengthOrDexterityContestCheckAgainstStrengthAthletics)
            .SetBrain(battlePackage, true)
            .SetSpecialDuration(DurationType.UntilAnyRest)
            .SetFeatures(
                FeatureDefinitionActionAffinitys.ActionAffinityGrappled,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained)
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedConditionGrappleTarget())
            .SetConditionParticleReference(ConditionDefinitions.ConditionRestrained)
            .AddToDB();

        _ = ConditionDefinitionBuilder
            .Create(ConditionGrappleSourceName)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionEncumbered)
            .SetConditionType(ConditionType.Neutral)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityGrappleSource")
                    .SetGuiPresentationNoContent(true)
                    .SetForbiddenActions(ActionDefinitions.Id.Climb, ActionDefinitions.Id.Jump)
                    .AddToDB(),
                //TODO: improve this as it will prevent any grappled creature from attacking any grappler
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityGrappleSource")
                    .SetGuiPresentationNoContent(true)
                    .SetSituationalContext(SituationalContext.TargetHasCondition, conditionGrappleTarget)
                    .SetAttackOfOpportunityImmunity(true)
                    .AddToDB(),
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionSlowed)
            .AddCustomSubFeatures(new CustomBehaviorConditionGrappleSource())
            .SetCancellingConditions(ConditionDefinitions.ConditionIncapacitated)
            .SetConditionParticleReference(ConditionDefinitions.ConditionSlowed)
            .AddToDB();
    }

    internal static void SwitchGrappleAction()
    {
        ActionGrapple.formType = Main.Settings.AddGrappleActionToAllRaces
            ? ActionDefinitions.ActionFormType.Large
            : ActionDefinitions.ActionFormType.Invisible;

        ActionDisableGrapple.formType = Main.Settings.AddGrappleActionToAllRaces
            ? ActionDefinitions.ActionFormType.Large
            : ActionDefinitions.ActionFormType.Invisible;
    }

    internal static void ValidateGrappleAfterForcedMove(GameLocationCharacter target)
    {
        var rulesetTarget = target.RulesetCharacter;

        if (rulesetTarget is not { IsDeadOrDying: false })
        {
            return;
        }

        if (GetGrappledActor(rulesetTarget, out var rulesetGrappled, out var activeCondition))
        {
            var grappled = GameLocationCharacter.GetFromActor(rulesetGrappled);
            var allowedRange = GetUnarmedReachRange(target);

            if (!target.IsWithinRange(grappled, allowedRange))
            {
                rulesetGrappled.RemoveCondition(activeCondition);
            }
        }

        // ReSharper disable once InvertIf
        if (rulesetTarget.TryGetConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, ConditionGrappleTargetName, out var activeConditionTarget))
        {
            var rulesetGrappler = EffectHelpers.GetCharacterByGuid(activeConditionTarget.SourceGuid);
            var grappler = GameLocationCharacter.GetFromActor(rulesetGrappler);
            var allowedRange = GetUnarmedReachRange(grappler);

            if (!target.IsWithinRange(grappler, allowedRange))
            {
                rulesetTarget.RemoveCondition(activeConditionTarget);
            }
        }
    }

    private static bool GetGrappledActor(
        RulesetCharacter rulesetSource,
        out RulesetCharacter rulesetTarget,
        out RulesetCondition rulesetTargetCondition)
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

        RulesetCondition foundCondition = null;

        rulesetTarget = contenders.FirstOrDefault(x =>
            x.RulesetCharacter.TryGetConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, ConditionGrappleTargetName, out foundCondition) &&
            foundCondition.SourceGuid == rulesetSource.Guid)?.RulesetCharacter;

        var found = rulesetTarget != null;

        rulesetTargetCondition = found ? foundCondition : null;

        return found;
    }

    private static int GetUnarmedReachRange(GameLocationCharacter character)
    {
        var hero = character.RulesetCharacter.GetOriginalHero();

        if (hero != null &&
            // only Astral Reach grants reach on unarmed
            hero.GetFeaturesByType<FeatureDefinition>().Any(x => x.Name == AstralReach.AstralReachFeatureName))
        {
            return 2;
        }

        return 1;
    }

    private sealed class CustomBehaviorGrapple : IFilterTargetingCharacter, IPowerOrSpellFinishedByMe
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var rulesetTarget = target.RulesetCharacter;

            if (rulesetTarget == null)
            {
                return false;
            }

            var actingCharacter = __instance.ActionParams.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var isValid =
                rulesetCharacter.SizeDefinition.WieldingSize - rulesetTarget.SizeDefinition.WieldingSize >= -1;

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Failure/&TargetMustBeNoMoreThanOneSizeLarger");

                return false;
            }

            var allowedRange = GetUnarmedReachRange(actingCharacter);

            isValid = actingCharacter.IsWithinRange(target, allowedRange);

            if (isValid)
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Failure/&FailureFlagNoReachForTargetDescription");

            return false;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;
            var defender = action.ActionParams.TargetCharacters[0];
            var abilityCheckData = new AbilityCheckData();

            attacker.BurnOneMainAttack();

            yield return TryAlterOutcomeAttributeCheck.ResolveRolls(
                attacker, defender, ActionDefinitions.Id.NoAction, abilityCheckData);

            var success =
                abilityCheckData.AbilityCheckRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess;

            if (!success)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            // should only be grappled by one grappler at a time, last wins
            var conditionsToRemove = rulesetDefender.ConditionsByCategory
                .SelectMany(x => x.Value)
                .Where(x => x.ConditionDefinition.Name == ConditionGrappleTargetName)
                .ToArray();

            foreach (var condition in conditionsToRemove)
            {
                rulesetDefender.RemoveCondition(condition);
            }

            // apply new grappler condition
            rulesetAttacker.InflictCondition(
                ConditionGrappleSourceName,
                DurationType.UntilAnyRest,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionGrappleSourceName,
                0,
                0,
                0);

            // apply new grappled condition
            rulesetDefender.InflictCondition(
                ConditionGrappleTargetName,
                DurationType.UntilAnyRest,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionGrappleTargetName,
                40,
                0,
                0);
        }
    }

    private sealed class PowerOrSpellFinishedByMeDisableGrapple : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            if (GetGrappledActor(rulesetAttacker, out var rulesetDefender, out var activeConditionTarget))
            {
                // this will also take care of removing the source condition at OnConditionAddedOrRemovedConditionGrappleTarget
                rulesetDefender.RemoveCondition(activeConditionTarget);
            }

            yield break;
        }
    }

    private sealed class OnConditionAddedOrRemovedConditionGrappleTarget : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        // should remove source tracker condition as well
        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var rulesetSource = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (rulesetSource.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionGrappleSourceName, out var activeConditionSource) &&
                activeConditionSource.SourceGuid == rulesetCondition.SourceGuid)
            {
                rulesetSource.RemoveCondition(activeConditionSource);
            }
        }
    }

    private sealed class CustomBehaviorConditionGrappleSource
        : IModifyWeaponAttackMode, IMoveStepStarted, IOnItemEquipped, IPhysicalAttackInitiatedByMe
    {
        // should not use a versatile weapon in two-handed mode
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            attackMode.UseVersatileDamage = false;
        }

        // should drag target whenever move
        public void MoveStepStarted(GameLocationCharacter mover, int3 source, int3 destination)
        {
            var rulesetMover = mover.RulesetCharacter;

            if (!GetGrappledActor(rulesetMover, out var rulesetTarget, out var activeCondition))
            {
                return;
            }

            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var target = GameLocationCharacter.GetFromActor(rulesetTarget);

            // be safe and if it cannot place target, get rid of grapple to avoid exploits
            if (positioningService.CanPlaceCharacter(target, source, CellHelpers.PlacementMode.IgnoreOccupantsMoving))
            {
                target.StartTeleportTo(source, mover.Orientation, false);
                target.FinishMoveTo(source, mover.Orientation);
                target.StopMoving(mover.Orientation);

                var isLastStep = DistanceCalculation.GetDistanceFromCharacter(mover, mover.DestinationPosition) <= 1;

                if (isLastStep)
                {
                    EjectCharactersInArea(mover, target);
                }
            }
            else if (DistanceCalculation.GetDistanceFromCharacter(target, mover.DestinationPosition) >
                     GetUnarmedReachRange(mover))
            {
                rulesetTarget.RemoveCondition(activeCondition);
            }
        }

        // should lose grapple if no free hand anymore
        public void OnItemEquipped(RulesetCharacterHero hero)
        {
            if (!ValidatorsCharacter.HasFreeHand(hero) &&
                GetGrappledActor(hero, out var rulesetTarget, out var activeCondition))
            {
                rulesetTarget.RemoveCondition(activeCondition);
            }
        }

        // should lose grapple if attacks with two-handed or with any free hand
        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if ((ValidatorsWeapon.HasAnyWeaponTag(
                     attackMode.SourceDefinition as ItemDefinition, TagsDefinitions.WeaponTagTwoHanded) ||
                 ValidatorsWeapon.IsUnarmed(attackMode)) &&
                GetGrappledActor(rulesetAttacker, out var rulesetTarget, out var activeCondition))
            {
                rulesetTarget.RemoveCondition(activeCondition);
            }

            yield break;
        }

        private static void EjectCharactersInArea(
            GameLocationCharacter grappleSource, GameLocationCharacter grappleTarget)
        {
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var targetingService = ServiceRepository.GetService<IGameLocationTargetingService>();
            var boxArea = grappleTarget.LocationBoundingBox;
            var affectedCharacters = new List<GameLocationCharacter>();

            targetingService.ComputeTargetsOfAreaOfEffect(boxArea, affectedCharacters, null);

            foreach (var affectedCharacter in affectedCharacters)
            {
                if (affectedCharacter == grappleSource ||
                    affectedCharacter == grappleTarget ||
                    affectedCharacter.RulesetCharacter == null ||
                    !affectedCharacter.RulesetActor.CanReceiveMotion)
                {
                    continue;
                }

                var horizontalBoxArea = boxArea;

                horizontalBoxArea.Min.y = affectedCharacter.LocationPosition.y;
                horizontalBoxArea.Max.y = affectedCharacter.LocationPosition.y;

                if (!ComputeEjectDestination(
                        horizontalBoxArea,
                        affectedCharacter,
                        affectedCharacter.LocationPosition,
                        false,
                        positioningService,
                        grappleSource.DestinationPosition,
                        out var destination))
                {
                    continue;
                }

                affectedCharacter.MyExecuteActionTacticalMove(destination);
            }
        }

        private static bool ComputeEjectDestination(
            BoxInt boxArea,
            GameLocationCharacter character,
            int3 currentPosition,
            bool canStandOnly,
            IGameLocationPositioningService positioningService,
            int3 forbiddenPosition,
            out int3 destination)
        {
            var boxInt = boxArea;
            var ejectDestination = false;

            boxInt.Inflate(1, 0, 1);

            destination = currentPosition;

            foreach (var candidateDestination in boxInt.EnumerateAllPositionsWithin())
            {
                if (candidateDestination == currentPosition ||
                    candidateDestination == forbiddenPosition ||
                    boxArea.Contains(candidateDestination))
                {
                    continue;
                }

                if (!positioningService.CanPlaceCharacter(
                        character, candidateDestination, CellHelpers.PlacementMode.Station) ||
                    positioningService.RaycastGrid(
                        currentPosition,
                        candidateDestination,
                        CellFlags.Surface.MovementBlocker,
                        CellFlags.Side.AllHorizontalSides) ||
                    (canStandOnly &&
                     !positioningService.CanCharacterStayAtPosition_Floor(character, candidateDestination, true)))
                {
                    continue;
                }

                if (ejectDestination)
                {
                    var magnitudeCandidate = (candidateDestination - currentPosition).magnitude2DSqr;
                    var magnitudeDestination = (destination - currentPosition).magnitude2DSqr;

                    if (!magnitudeCandidate.IsReallyInferior(magnitudeDestination))
                    {
                        continue;
                    }
                }

                destination = candidateDestination;
                ejectDestination = true;
            }

            return ejectDestination;
        }
    }
}
