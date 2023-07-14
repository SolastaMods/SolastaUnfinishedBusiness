using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
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

    internal static ConditionDefinition InvisibilityEveryRound;

    internal static ConditionDefinition LightSensitivity;

    internal static ConditionDefinition StopMovement;

    internal static ConditionDefinition FlightSuspended;

    internal static FeatureDefinitionPower FlightSuspend { get; private set; }
    internal static FeatureDefinitionPower FlightResume { get; private set; }

    private static ConditionDefinition ConditionInvisibilityEveryRoundRevealed { get; set; }

    private static ConditionDefinition ConditionInvisibilityEveryRoundHidden { get; set; }

    internal static void Load()
    {
        StopMovement = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrained, "ConditionStopMovement")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained)
            .AddToDB();

        InvisibilityEveryRound = BuildInvisibilityEveryRound();

        LightSensitivity = BuildLightSensitivity();

        Distracted = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionDistractedByAlly")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityDistractedByAlly")
                    .SetGuiPresentation("ConditionDistractedByAlly", Category.Condition)
                    .SetAttackOnMeAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .AddToDB();

        FlightSuspended = BuildFlightSuspended();
    }

    private static ConditionDefinition BuildInvisibilityEveryRound()
    {
        ConditionInvisibilityEveryRoundRevealed = ConditionDefinitionBuilder
            .Create("ConditionInvisibilityEveryRoundRevealed")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        ConditionInvisibilityEveryRoundHidden = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionInvisible, "ConditionInvisibilityEveryRoundHidden")
            .SetCancellingConditions(ConditionInvisibilityEveryRoundRevealed)
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
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureInvisibilityEveryRound")
                .SetGuiPresentationNoContent()
                .SetCustomSubFeatures(new InvisibilityEveryRoundBehavior())
                .AddToDB())
            .AddToDB();

        return conditionInvisibilityEveryRound;
    }

    private static ConditionDefinition BuildLightSensitivity()
    {
        var abilityCheckAffinityLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityLightSensitivity")
            .SetGuiPresentation(CombatAffinitySensitiveToLight.GuiPresentation)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage, DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        abilityCheckAffinityLightSensitivity.AffinityGroups[0].lightingContext = LightingContext.BrightLight;

        var combatAffinityDarkelfLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(CombatAffinitySensitiveToLight, "CombatAffinityLightSensitivity")
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

        return conditionLightSensitive;
    }

    private sealed class InvisibilityEveryRoundBehavior : IActionFinishedByMe, ICustomConditionFeature
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

        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not RulesetCharacterMonster &&
                !target.HasConditionOfType(ConditionInvisibilityEveryRoundRevealed))
            {
                BecomeHidden(target);
            }
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
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
                    ConditionInvisibilityEveryRoundRevealed,
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
                    ConditionInvisibilityEveryRoundHidden,
                    DurationType.Permanent,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    hero.Guid,
                    hero.CurrentFaction.Name),
                false);
        }
    }


    private static ConditionDefinition BuildFlightSuspended()
    {
        const string NAMELAND = "FlightSuspend";

        var conditionFlightSuspendConcentrationTracker = ConditionDefinitionBuilder
            .Create("ConditionFlightSuspendedConcentrationTracker")
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1)
            .AddToDB();

        var conditionFlightSuspend = ConditionDefinitionBuilder
            .Create("ConditionFlightSuspended")
            .SetGuiPresentation(NAMELAND, Category.Condition, Sprites.GetSprite("ConditionFlightSuspended", Properties.Resources.ConditionFlightSuspended, 32))
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureFlightSuspended")
                .SetGuiPresentationNoContent()
                .SetCustomSubFeatures(new FlightSuspendBehavior())
                .AddToDB())
            .AddToDB();

        // I ran into sync issues if I didn't generate the actions here
        BuildFlightSuspendAction(conditionFlightSuspend, conditionFlightSuspendConcentrationTracker);

        return conditionFlightSuspend;
    }

    private static void BuildFlightSuspendAction(ConditionDefinition conditionFlightSuspend, ConditionDefinition conditionFlightSuspendConcentrationTracker)
    {

        const string NAMELAND = "FlightSuspend";
        const string NAMETAKEOFF = "FlightResume";

        FlightSuspend = FeatureDefinitionPowerBuilder
            .Create($"Power{NAMELAND}")
            .SetGuiPresentation(NAMELAND, Category.Feature, Sprites.GetSprite("ActionFlightSuspend", Properties.Resources.ActionFlightSuspend, 80), 71)
            .SetUsesFixed(ActivationTime.NoCost)
            .DelegatedToAction()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionFlightSuspend,
                            ConditionForm.ConditionOperation.Add),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionFlying,
                            ConditionForm.ConditionOperation.Remove)
                    )
                    .UseQuickAnimations()
                    .Build())
            .SetCustomSubFeatures(new ValidatorsPowerUse(
                ValidatorsCharacter.HasNoneOfConditions(conditionFlightSuspend.Name)))
            .AddToDB();

        ActionDefinitionBuilder
            .Create($"Action{NAMELAND}")
            .SetGuiPresentation(NAMELAND, Category.Action, Sprites.GetSprite("ActionFlightSuspend", Properties.Resources.ActionFlightSuspend, 80), 71)
            .SetActionId(ExtraActionId.FlightSuspend)
            .OverrideClassName("UsePower")
            .SetActionScope(ActionScope.All)
            .SetActionType(ActionType.NoCost)
            .SetFormType(ActionFormType.Small)
            .SetActivatedPower(FlightSuspend)
            .AddToDB();

        FlightResume = FeatureDefinitionPowerBuilder
            .Create($"Power{NAMETAKEOFF}")
            .SetGuiPresentation(NAMETAKEOFF, Category.Feature, Sprites.GetSprite("ActionFlightResume", Properties.Resources.ActionFlightResume, 80), 71)
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
                            ConditionForm.ConditionOperation.Remove)
                    )
                    .UseQuickAnimations()
                    .Build())
            .SetCustomSubFeatures(new ValidatorsPowerUse(
                ValidatorsCharacter.HasAnyOfConditions(conditionFlightSuspend.Name)))
            .AddToDB();

        ActionDefinitionBuilder
            .Create($"Action{NAMETAKEOFF}")
            .SetGuiPresentation(NAMETAKEOFF, Category.Action, Sprites.GetSprite("ActionFlightResume", Properties.Resources.ActionFlightResume, 80), 71)
            .SetActionId(ExtraActionId.FlightResume)
            .OverrideClassName("UsePower")
            .SetActionScope(ActionScope.All)
            .SetActionType(ActionType.NoCost)
            .SetFormType(ActionFormType.Small)
            .SetActivatedPower(FlightResume)
            .AddToDB();
    }

    private static bool TryAddFlightCondition(RulesetCharacter source, RulesetCharacter target, RulesetCondition rulesetCondition, out RulesetCondition condition)
    {
        condition = null;
        try
        {
            condition = target.InflictCondition(
                rulesetCondition.effectDefinitionName,
                DurationType.Round,
                rulesetCondition.remainingRounds,
                rulesetCondition.endOccurence,
                "11Effect",
                source.guid,
                target.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);

            return true;
        }
        catch (Exception e)
        {
            Main.Log($">>>> TryAddFlightSuspendedCondition EXCEPTION {e.Message} {e.StackTrace}");
        }

        return false;

    }

    private static bool TryAddFlightSuspendedConcentrationTracker(RulesetCharacter source, RulesetCharacter target, RulesetEffectSpell effect, out RulesetCondition condition)
    {
        condition = null;
        try
        {
            condition = target.InflictCondition(
                "ConditionFlightSuspendedConcentrationTracker",
                DurationType.Permanent,
                0,
                TurnOccurenceType.EndOfTurn,
                "11Effect",
                source.guid,
                target.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);

            condition.effectDefinitionName = effect.Name;

            return true;
        }
        catch (Exception e)
        {
            Main.Log($">>>> TryAddFlightSuspendedConcentrationTracker EXCEPTION {e.Message} {e.StackTrace}");
        }

        return false;

    }

    private sealed class FlightSuspendBehavior : ICustomConditionFeature, INotifyConditionRemoval
    {
        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {

            if (target is RulesetCharacterMonster m)
            {
                var monster = target as RulesetCharacterMonster;

                int speedWalk;
                if (monster.MoveModes.TryGetValue((int)MoveMode.Walk, out speedWalk))
                {
                    //WILDSHAPE CHANGE TO MOVE MODE WALKING GOES HERE
                    //For now, remove to avoid issues when wildshape is cancelled while flight suspended
                    monster.RemoveCondition(rulesetCondition);
                }
            }
            else
            {
                List<RulesetCondition> conditions = target.allConditionsForEnumeration;
                foreach (var condition in conditions)
                {
                    if (condition.ConditionDefinition.IsSubtypeOf("ConditionFlying"))
                    {
                        //We are not interested in permanent effects
                        if (condition.DurationType == DurationType.Permanent)
                        {
                            target.RemoveCondition(rulesetCondition);
                            return;
                        }

                        RulesetCharacter source = target;
                        try
                        {
                            //Ensure we keep tabs on source and target in case someone else is concentrating on us
                            if (condition.sourceGuid != condition.TargetGuid)
                            {
                                RulesetCharacter newSource = null;
                                if (RulesetEntity.TryGetEntity<RulesetCharacter>(condition.sourceGuid, out newSource))
                                {
                                    source = newSource;
                                }
                            }

                            //Check if there is an effect tracked by the source for concentration purposes
                            RulesetEffect effect = source.FindEffectTrackingCondition(condition);
                            if (effect != null && effect is RulesetEffectSpell spell && spell.SpellDefinition.RequiresConcentration)
                            {
                                effect.TrackCondition(source, source.Guid, target, target.Guid, rulesetCondition, "FlightSuspended");

                                RulesetCondition trackerCondition = null;
                                if (TryAddFlightSuspendedConcentrationTracker(source, target, effect as RulesetEffectSpell, out trackerCondition))
                                {
                                    trackerCondition.remainingRounds = condition.remainingRounds;
                                    effect.TrackCondition(source, source.Guid, target, target.Guid, trackerCondition, "FlightSuspendedConcentrationTracker");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Main.Log($"FlightSuspendBehavior ApplyFeature EXCEPTION Tracker {ex} {ex.StackTrace}");
                            return;
                        }

                        //Using effectDefinitionName to store suspended condition, safe?
                        rulesetCondition.effectDefinitionName = condition.Name;
                        rulesetCondition.sourceGuid = source.Guid;
                        rulesetCondition.targetGuid = target.Guid;
                        rulesetCondition.remainingRounds = condition.remainingRounds;
                        rulesetCondition.endOccurence = condition.endOccurence;
                        break;
                    }
                }
            }
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is RulesetCharacterMonster m)
            {
                //Do nothing for now for wildshape
            }
            else
            {

                try
                {
                    //Ensure we keep tabs on source and target in case someone else is concentrating on us
                    RulesetCharacter source = target;
                    if (rulesetCondition.sourceGuid != rulesetCondition.TargetGuid)
                    {
                        RulesetCharacter newSource = null;
                        if (RulesetEntity.TryGetEntity<RulesetCharacter>(rulesetCondition.sourceGuid, out newSource))
                        {
                            source = newSource;
                        }
                    }

                    //If we have a concentration tracker, but the source is no longer concentrating on our effect, do not renew flight
                    if (target.HasConditionOfType("ConditionFlightSuspendedConcentrationTracker"))
                    {
                        RulesetCondition trackerCondition = null;
                        List<RulesetCondition> conditions = new List<RulesetCondition>();
                        target.GetAllConditionsOfType(conditions, "ConditionFlightSuspendedConcentrationTracker");
                        if (conditions.Count > 0)
                        {
                            trackerCondition = conditions.First();
                            RulesetEffectSpell concentratedSpell = source.concentratedSpell;
                            if (concentratedSpell == null || (concentratedSpell != null && concentratedSpell.Name != trackerCondition.effectDefinitionName))
                            {
                                target.RemoveCondition(trackerCondition);
                                return;
                            }
                        }
                    }
                    
                    //Add the flight condition, and track if there is an effect
                    RulesetCondition newFlightCondition = null;
                    if (TryAddFlightCondition(source, target, rulesetCondition, out newFlightCondition))
                    {
                        //Restore concentration tracking
                        RulesetEffect effect = source.FindEffectTrackingCondition(rulesetCondition);
                        if (effect != null && effect is RulesetEffectSpell spell && spell.SpellDefinition.RequiresConcentration)
                        {
                            effect.TrackCondition(source, source.Guid, target, target.Guid, newFlightCondition, "FlightResumed");
                        }
                    }
                } catch (Exception ex)
                {
                    Main.Log($"FlightSuspendBehavior RemoveFeature EXCEPTION {ex} {ex.StackTrace}");
                    return;
                }
            }
        }

        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            removedFrom.RemoveCondition(rulesetCondition);
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            rulesetActor.RemoveCondition( rulesetCondition );
        }

    }
}
