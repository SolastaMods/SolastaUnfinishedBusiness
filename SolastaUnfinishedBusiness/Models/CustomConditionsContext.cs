using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;

namespace SolastaUnfinishedBusiness.Models;

/**
 * Place for generic conditions that may be reused between several features
 */
internal static class CustomConditionsContext
{
    internal static ConditionDefinition Distracted;

    internal static ConditionDefinition FlightSuspended;

    internal static ConditionDefinition InvisibilityEveryRound;

    internal static ConditionDefinition LightSensitivity;

    internal static ConditionDefinition StopMovement;

    internal static ConditionDefinition Taunted;

    private static ConditionDefinition _taunter;
    private static FeatureDefinitionPower _flightSuspend;
    private static FeatureDefinitionPower _flightResume;
    private static ConditionDefinition _conditionInvisibilityEveryRoundRevealed;
    private static ConditionDefinition _conditionInvisibilityEveryRoundHidden;

    internal static void Load()
    {
        Distracted = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionDistractedByAlly")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityDistractedByAlly")
                    .SetGuiPresentation("ConditionDistractedByAlly", Category.Condition, Gui.NoLocalization)
                    .SetAttackOnMeAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .AddToDB();

        FlightSuspended = BuildFlightSuspended();

        InvisibilityEveryRound = BuildInvisibilityEveryRound();

        LightSensitivity = BuildLightSensitivity();

        StopMovement = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrained, "ConditionStopMovement")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained)
            .AddToDB();

        Taunted = ConditionDefinitionBuilder
            .Create("ConditionTaunted")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConfused)
            .SetConditionType(ConditionType.Detrimental)
            .SetConditionParticleReference(
                ConditionDefinitions.ConditionUnderDemonicInfluence.conditionParticleReference)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityTaunted")
                    .SetGuiPresentation("ConditionTaunted", Category.Condition, Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                    .SetSituationalContext(ExtraSituationalContext.TargetIsNotEffectSource)
                    .AddToDB())
            .AddCustomSubFeatures(new CustomBehaviorTaunted())
            .AddToDB();

        _taunter = ConditionDefinitionBuilder
            .Create("ConditionTaunter")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(new ActionFinishedByMeTaunter())
            .AddToDB();
    }

    private static ConditionDefinition BuildInvisibilityEveryRound()
    {
        _conditionInvisibilityEveryRoundRevealed = ConditionDefinitionBuilder
            .Create("ConditionInvisibilityEveryRoundRevealed")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        _conditionInvisibilityEveryRoundHidden = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionInvisible, "ConditionInvisibilityEveryRoundHidden")
            .SetCancellingConditions(_conditionInvisibilityEveryRoundRevealed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(
                ConditionInterruption.Attacks,
                ConditionInterruption.CastSpell,
                ConditionInterruption.UsePower)
            .AddToDB();

        var conditionInvisibilityEveryRound = ConditionDefinitionBuilder
            .Create("ConditionInvisibilityEveryRound")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddCustomSubFeatures(new InvisibilityEveryRoundBehavior())
            .AddToDB();

        return conditionInvisibilityEveryRound;
    }

    private static ConditionDefinition BuildLightSensitivity()
    {
        var abilityCheckAffinityLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage, DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        var combatAffinityDarkelfLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(CombatAffinitySensitiveToLight, "CombatAffinityLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .SetMyAttackAdvantage(AdvantageType.None)
            .SetMyAttackModifierSign(AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(DieType.D4)
            .AddToDB();

        var conditionLightSensitive = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionLightSensitive, "ConditionLightSensitivity")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(abilityCheckAffinityLightSensitivity, combatAffinityDarkelfLightSensitivity)
            .AddToDB();

        conditionLightSensitive.GuiPresentation.description = Gui.NoLocalization;

        return conditionLightSensitive;
    }

    private static ConditionDefinition BuildFlightSuspended()
    {
        const string Name = "FlightSuspend";

        var conditionFlightSuspendConcentrationTracker = ConditionDefinitionBuilder
            .Create("ConditionFlightSuspendedConcentrationTracker")
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1)
            .AddToDB();

        var conditionFlightSuspend = ConditionDefinitionBuilder
            .Create("ConditionFlightSuspended")
            .SetGuiPresentation(Name, Category.Condition,
                Sprites.GetSprite("ConditionFlightSuspended", Resources.ConditionFlightSuspended, 32))
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1)
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedFlightSuspendBehavior())
            .AddToDB();

        // I ran into sync issues if I didn't generate the actions here
        BuildFlightSuspendAction(conditionFlightSuspend, conditionFlightSuspendConcentrationTracker);

        return conditionFlightSuspend;
    }

    private static void BuildFlightSuspendAction(ConditionDefinition conditionFlightSuspend,
        ConditionDefinition conditionFlightSuspendConcentrationTracker)
    {
        const string FlightSuspendName = "FlightSuspend";
        const string FlightResumeName = "FlightResume";

        _flightSuspend = FeatureDefinitionPowerBuilder
            .Create($"Power{FlightSuspendName}")
            .SetGuiPresentation(FlightSuspendName, Category.Feature,
                Sprites.GetSprite("ActionFlightSuspend", Resources.ActionFlightSuspend, 80), 71)
            .SetUsesFixed(ActivationTime.NoCost)
            .DelegatedToAction()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionFlightSuspend),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionFlying,
                            ConditionForm.ConditionOperation.Remove))
                    .UseQuickAnimations()
                    .Build())
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(
                ValidatorsCharacter.HasNoneOfConditions(conditionFlightSuspend.Name)))
            .AddToDB();

        ActionDefinitionBuilder
            .Create($"Action{FlightSuspendName}")
            .SetGuiPresentation(FlightSuspendName, Category.Action,
                Sprites.GetSprite("ActionFlightSuspend", Resources.ActionFlightSuspend, 80), 71)
            .SetActionId(ExtraActionId.FlightSuspend)
            .OverrideClassName("UsePower")
            .SetActionScope(ActionScope.All)
            .SetActionType(ActionType.NoCost)
            .SetFormType(ActionFormType.Small)
            .SetActivatedPower(_flightSuspend)
            .AddToDB();

        _flightResume = FeatureDefinitionPowerBuilder
            .Create($"Power{FlightResumeName}")
            .SetGuiPresentation(FlightResumeName, Category.Feature,
                Sprites.GetSprite("ActionFlightResume", Resources.ActionFlightResume, 80), 71)
            .SetUsesFixed(ActivationTime.NoCost)
            .DelegatedToAction()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionFlightSuspend,
                            ConditionForm.ConditionOperation.Remove),
                        EffectFormBuilder.ConditionForm(conditionFlightSuspendConcentrationTracker,
                            ConditionForm.ConditionOperation.Remove))
                    .UseQuickAnimations()
                    .Build())
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(
                ValidatorsCharacter.HasAnyOfConditions(conditionFlightSuspend.Name)))
            .AddToDB();

        ActionDefinitionBuilder
            .Create($"Action{FlightResumeName}")
            .SetGuiPresentation(FlightResumeName, Category.Action,
                Sprites.GetSprite("ActionFlightResume", Resources.ActionFlightResume, 80), 71)
            .SetActionId(ExtraActionId.FlightResume)
            .OverrideClassName("UsePower")
            .SetActionScope(ActionScope.All)
            .SetActionType(ActionType.NoCost)
            .SetFormType(ActionFormType.Small)
            .SetActivatedPower(_flightResume)
            .AddToDB();
    }

    private static bool TryAddFlightCondition(
        RulesetEntity source,
        RulesetCharacter target,
        RulesetCondition rulesetCondition,
        out RulesetCondition condition)
    {
        condition = null;

        try
        {
            condition = target.InflictCondition(
                rulesetCondition.effectDefinitionName,
                DurationType.Round,
                rulesetCondition.remainingRounds,
                rulesetCondition.endOccurence,
                AttributeDefinitions.TagEffect,
                source.guid,
                target.CurrentFaction.Name,
                1,
                rulesetCondition.effectDefinitionName,
                0,
                0,
                0);

            return true;
        }
        catch (Exception e)
        {
            // ReSharper disable once InvocationIsSkipped
            Main.Log($">>>> TryAddFlightSuspendedCondition EXCEPTION {e.Message} {e.StackTrace}");
        }

        return false;
    }

    private static bool TryAddFlightSuspendedConcentrationTracker(
        RulesetEntity source,
        RulesetCharacter target,
        RulesetEntity effect,
        out RulesetCondition condition)
    {
        condition = null;

        try
        {
            condition = target.InflictCondition(
                "ConditionFlightSuspendedConcentrationTracker",
                DurationType.Permanent,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                source.guid,
                target.CurrentFaction.Name,
                1,
                "ConditionFlightSuspendedConcentrationTracker",
                0,
                0,
                0);

            condition.effectDefinitionName = effect.Name;

            return true;
        }
        catch (Exception e)
        {
            // ReSharper disable once InvocationIsSkipped
            Main.Log($">>>> TryAddFlightSuspendedConcentrationTracker EXCEPTION {e.Message} {e.StackTrace}");
        }

        return false;
    }

    private static int GetTauntedRange(string effectDescriptionName)
    {
        return effectDescriptionName switch
        {
            "PowerGambitGoadingReact" or "ActionAffinityMartialGuardianCompellingStrike" => 1,
            "PowerMartialGuardianGrandChallenge" => 6,
            _ => 18 // Thunder Gauntlet
        };
    }

    private sealed class InvisibilityEveryRoundBehavior : IActionFinishedByMe, IOnConditionAddedOrRemoved
    {
        private const string CategoryRevealed = "InvisibilityEveryRoundRevealed";
        private const string CategoryHidden = "InvisibilityEveryRoundHidden";

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not (CharacterActionUsePower or CharacterActionCastSpell or CharacterActionAttack))
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var actionParams = action.ActionParams;
            var rulesetEffect = actionParams.RulesetEffect;

            if (rulesetEffect == null || !IsAllowedEffect(rulesetEffect.EffectDescription))
            {
                BecomeRevealed(rulesetCharacter);
            }
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not RulesetCharacterMonster &&
                !target.HasConditionOfType(_conditionInvisibilityEveryRoundRevealed))
            {
                BecomeHidden(target);
            }
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not RulesetCharacterMonster)
            {
                target.RemoveAllConditionsOfCategory(CategoryHidden, false);
            }
        }

        // returns true if effect is self teleport or any self targeting spell that is self-buff
        private static bool IsAllowedEffect(EffectDescription effect)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (effect.TargetType)
            {
                case TargetType.Position:
                {
                    foreach (var form in effect.EffectForms)
                    {
                        if (form.FormType != EffectForm.EffectFormType.Motion)
                        {
                            return false;
                        }

                        if (form.MotionForm.Type != MotionForm.MotionType.TeleportToDestination)
                        {
                            return false;
                        }
                    }

                    break;
                }
                case TargetType.Self:
                {
                    foreach (var form in effect.EffectForms)
                    {
                        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                        switch (form.FormType)
                        {
                            case EffectForm.EffectFormType.Damage:
                            case EffectForm.EffectFormType.Healing:
                            case EffectForm.EffectFormType.ShapeChange:
                            case EffectForm.EffectFormType.Summon:
                            case EffectForm.EffectFormType.Counter:
                            case EffectForm.EffectFormType.Motion:
                                return false;
                        }
                    }

                    break;
                }
                default:
                    return false;
            }

            return true;
        }

        private static void BecomeRevealed(RulesetCharacter hero)
        {
            hero.AddConditionOfCategory(CategoryRevealed,
                RulesetCondition.CreateActiveCondition(
                    hero.Guid,
                    _conditionInvisibilityEveryRoundRevealed,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.StartOfTurn,
                    hero.Guid,
                    hero.CurrentFaction.Name
                ));
        }

        private static void BecomeHidden(RulesetCharacter hero)
        {
            hero.AddConditionOfCategory(CategoryHidden,
                RulesetCondition.CreateActiveCondition(
                    hero.Guid,
                    _conditionInvisibilityEveryRoundHidden,
                    DurationType.Permanent,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    hero.Guid,
                    hero.CurrentFaction.Name),
                false);
        }
    }

    private sealed class OnConditionAddedOrRemovedFlightSuspendBehavior : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is RulesetCharacterMonster monster)
            {
                if (monster.MoveModes.TryGetValue((int)MoveMode.Walk, out _))
                {
                    //WILDSHAPE CHANGE TO MOVE MODE WALKING GOES HERE
                    //For now, remove to avoid issues when wildshape is cancelled while flight suspended
                    monster.RemoveCondition(rulesetCondition);
                }
            }
            else
            {
                var conditions = target.allConditionsForEnumeration;

                foreach (var condition in conditions
                             .Where(condition => condition.ConditionDefinition.IsSubtypeOf("ConditionFlying")))
                {
                    //We are not interested in permanent effects
                    if (condition.DurationType == DurationType.Permanent)
                    {
                        target.RemoveCondition(rulesetCondition);

                        return;
                    }

                    var source = target;

                    try
                    {
                        //Ensure we keep tabs on source and target in case someone else is concentrating on us
                        if (condition.sourceGuid != condition.TargetGuid &&
                            RulesetEntity.TryGetEntity(condition.sourceGuid, out RulesetCharacter newSource))
                        {
                            source = newSource;
                        }

                        //Check if there is an effect tracked by the source for concentration purposes
                        var effect = source.FindEffectTrackingCondition(condition);

                        if (effect is RulesetEffectSpell spell &&
                            spell.SpellDefinition.RequiresConcentration)
                        {
                            spell.TrackCondition(source, source.Guid, target, target.Guid, rulesetCondition,
                                "FlightSuspended");

                            if (TryAddFlightSuspendedConcentrationTracker(
                                    source, target, spell, out var trackerCondition))
                            {
                                trackerCondition.remainingRounds = condition.remainingRounds;
                                spell.TrackCondition(source, source.Guid, target, target.Guid, trackerCondition,
                                    "FlightSuspendedConcentrationTracker");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // ReSharper disable once InvocationIsSkipped
                        Main.Log(
                            $"OnConditionAddedOrRemovedFlightSuspendBehavior ApplyFeature EXCEPTION Tracker {ex} {ex.StackTrace}");
                        return;
                    }

                    //Using effectDefinitionName to store suspended condition, safe?
                    rulesetCondition.effectDefinitionName = condition.Name;
                    rulesetCondition.sourceGuid = source.Guid;
                    rulesetCondition.targetGuid = target.Guid;
                    rulesetCondition.remainingRounds = condition.remainingRounds;
                    rulesetCondition.endOccurence = condition.endOccurence;

                    if (Main.Settings.FlightSuspendWingedBoots
                        && condition.Name == "ConditionFlyingBootsWinged")
                    {
                        //Stop duration counting for Winged Boots
                        rulesetCondition.durationType = DurationType.Permanent;
                    }

                    break;
                }
            }
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is RulesetCharacterMonster)
            {
                //Do nothing for now for wildshape
            }
            else
            {
                try
                {
                    //Ensure we keep tabs on source and target in case someone else is concentrating on us
                    var source = target;

                    if (rulesetCondition.sourceGuid != rulesetCondition.TargetGuid)
                    {
                        if (RulesetEntity.TryGetEntity(rulesetCondition.sourceGuid, out RulesetCharacter newSource))
                        {
                            source = newSource;
                        }
                    }

                    //If we have a concentration tracker, but the source is no longer concentrating on our effect, do not renew flight
                    if (target.HasConditionOfType("ConditionFlightSuspendedConcentrationTracker"))
                    {
                        var conditions = new List<RulesetCondition>();

                        target.GetAllConditionsOfType(conditions, "ConditionFlightSuspendedConcentrationTracker");

                        if (conditions.Count > 0)
                        {
                            var trackerCondition = conditions.First();
                            var concentratedSpell = source.concentratedSpell;

                            if (concentratedSpell == null || concentratedSpell.Name !=
                                trackerCondition.effectDefinitionName)
                            {
                                target.RemoveCondition(trackerCondition);
                                return;
                            }
                        }
                    }

                    //Add the flight condition, and track if there is an effect
                    if (!TryAddFlightCondition(source, target, rulesetCondition, out var newFlightCondition))
                    {
                        return;
                    }

                    //Restore concentration tracking
                    var effect = source.FindEffectTrackingCondition(rulesetCondition);

                    if (effect is RulesetEffectSpell spell &&
                        spell.SpellDefinition.RequiresConcentration)
                    {
                        effect.TrackCondition(source, source.Guid, target, target.Guid, newFlightCondition,
                            "FlightResumed");
                    }
                }
                catch (Exception ex)
                {
                    // ReSharper disable once InvocationIsSkipped
                    Main.Log(
                        $"OnConditionAddedOrRemovedFlightSuspendBehavior RemoveFeature EXCEPTION {ex} {ex.StackTrace}");
                }
            }
        }
    }

    private sealed class ActionFinishedByMeTaunter : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            if (characterAction.ActionType != ActionType.Move &&
                // also checking attack action to handle reach > 1 scenarios or throw or ranged
                characterAction.ActionId is not (
                    Id.AttackMain or
                    Id.AttackOff or
                    Id.AttackFree or
                    Id.AttackReadied or
                    Id.AttackOpportunity))
            {
                yield break;
            }

            var actingCharacter = characterAction.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var targets = Gui.Battle.GetContenders(actingCharacter);

            foreach (var target in targets)
            {
                var rulesetCondition = target.RulesetCharacter.AllConditions.FirstOrDefault(x =>
                    x.ConditionDefinition.Name == Taunted.Name &&
                    x.SourceGuid == rulesetCharacter.Guid);

                if (rulesetCondition == null)
                {
                    continue;
                }

                // ruleset amount carries the max range for the condition
                if (!actingCharacter.IsWithinRange(target, rulesetCondition.Amount))
                {
                    target.RulesetCharacter.RemoveCondition(rulesetCondition);
                }
            }
        }
    }

    //
    // Taunted
    //

    private sealed class CustomBehaviorTaunted : IActionFinishedByMe, IOnConditionAddedOrRemoved
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction.ActionType != ActionType.Move)
            {
                yield break;
            }

            var actingCharacter = characterAction.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            foreach (var rulesetCondition in rulesetCharacter.AllConditions
                         .Where(x => x.ConditionDefinition.Name == Taunted.Name)
                         .ToList()
                         .Select(a => new { a, rulesetCaster = EffectHelpers.GetCharacterByGuid(a.SourceGuid) })
                         .Where(t => t.rulesetCaster != null)
                         .Select(b => new { b, caster = GameLocationCharacter.GetFromActor(b.rulesetCaster) })
                         .Where(t =>
                             // ruleset amount carries the max range for the condition
                             t.caster != null && !t.caster.IsWithinRange(actingCharacter, t.b.a.Amount))
                         .Select(c => c.b.a))
            {
                rulesetCharacter.RemoveCondition(rulesetCondition);
            }
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var sourceCharacter = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);
            var tauntedRange = GetTauntedRange(rulesetCondition.effectDefinitionName);

            // for some reason when conditions are refreshed the original effect name and amount aren't updating
            // also, when condition is applied from an item (gauntlet), the end occurence gets set to StartOfTurn
            // below is a hack for these particular scenarios
            if (target.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, Taunted.Name, out var activeCondition))
            {
                activeCondition.amount = tauntedRange;
                activeCondition.effectDefinitionName = rulesetCondition.effectDefinitionName;
                activeCondition.sourceGuid = rulesetCondition.sourceGuid;
                activeCondition.durationType = DurationType.Round;
                activeCondition.durationParameter = 1;
                activeCondition.EndOccurence = TurnOccurenceType.EndOfSourceTurn;
            }

            sourceCharacter?.InflictCondition(
                _taunter.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                sourceCharacter.Guid,
                sourceCharacter.CurrentFaction.Name,
                1,
                _taunter.Name,
                tauntedRange,
                0,
                0);
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var sourceCharacter = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (sourceCharacter == null)
            {
                return;
            }

            if (sourceCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, _taunter.Name, out var activeCondition))
            {
                sourceCharacter.RemoveCondition(activeCondition);
            }
        }
    }
}
