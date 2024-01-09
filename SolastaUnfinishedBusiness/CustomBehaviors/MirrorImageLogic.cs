using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static ConsoleStyleDuplet;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class MirrorImageLogic
{
    private const string ConditionName = "ConditionMirrorImage";
    private const string TargetMirrorImageTag = "TargetsMirrorImage";

    private static readonly string ConditionTitle =
        GuiPresentationBuilder.CreateTitleKey(ConditionName, Category.Condition);

    private static ConditionDefinition _condition;

    private MirrorImageLogic()
    {
    }

    internal static ConditionDefinition Condition => _condition ??= BuildCondition();

    private static MirrorImageLogic Marker { get; } = new();

    private static ConditionDefinition BuildCondition()
    {
        return ConditionDefinitionBuilder
            .Create(ConditionName)
            .SetGuiPresentation(Category.Condition,
                Sprites.GetSprite("ConditionMirrorImage", Resources.ConditionMirrorImage, 32))
            .SetSilent(Silent.WhenAdded)
            .AllowMultipleInstances()
            .SetPossessive()
            .AddCustomSubFeatures(Marker, DuplicateCounter.Mark)
            .AddToDB();
    }

    private static List<RulesetCondition> GetConditions(RulesetActor character)
    {
        var conditions = new List<RulesetCondition>();

        if (character == null)
        {
            return conditions;
        }

        character.GetAllConditions(conditions);

        return
        [
            .. conditions.FindAll(c =>
                c.ConditionDefinition.HasSubFeatureOfType<MirrorImageLogic>())
        ];
    }

    // ReSharper disable once InconsistentNaming
    internal static int GetAC(
        RulesetAttribute attribute,
        RulesetActor target,
        IEnumerable<RuleDefinitions.TrendInfo> toHitTrends)
    {
        if (!TargetsMirrorImage(toHitTrends))
        {
            return attribute.CurrentValue;
        }

        var dexterity = target.TryGetAttributeValue(AttributeDefinitions.Dexterity);
        return 10 + AttributeDefinitions.ComputeAbilityScoreModifier(dexterity);
    }

    private static bool TargetsMirrorImage(IEnumerable<RuleDefinitions.TrendInfo> toHitTrends)
    {
        return toHitTrends.Any(t => t.sourceName == TargetMirrorImageTag);
    }

    internal static void AttackRollPrefix(
        RulesetCharacter attacker,
        RulesetActor target,
        List<RuleDefinitions.TrendInfo> toHitTrends,
        bool testMode)
    {
        if (!testMode)
        {
            return;
        }

        var conditions = GetConditions(target as RulesetCharacter);

        if (conditions.Empty())
        {
            return;
        }

        if (attacker.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionBlinded))
        {
            ReportAttackerIsBlind(attacker);
            return;
        }

        var distance = (int)attacker.DistanceTo(target);

        foreach (var sense in attacker.SenseModes
                     .Where(sense => sense.senseType is SenseMode.Type.Blindsight or SenseMode.Type.Truesight
                         or SenseMode.Type.Tremorsense)
                     .Where(sense => sense.senseType is not SenseMode.Type.Tremorsense || target.IsTouchingGround())
                     .Where(sense => sense.senseRange >= distance))
        {
            ReportAttackerHasSense(attacker, sense.senseType);

            return;
        }

        //TODO: Bonus points if we can manage to change attack `GameConsole.AttackRolled` to show duplicate, instead of the target

        //TODO: add custom context and modify Halfling's Lucky to include it
        var result = target.RollDie(RuleDefinitions.DieType.D20, RuleDefinitions.RollContext.None, false,
            RuleDefinitions.AdvantageType.None, out _, out _, skill: TargetMirrorImageTag);

        var hitImage = false;

        switch (conditions.Count)
        {
            case >= 3 when result >= 6:
            case 2 when result >= 8:
            case 1 when result >= 11:
                hitImage = true;
                break;
        }

        ReportTargetingMirrorImage(attacker, target, result, hitImage);

        if (hitImage)
        {
            toHitTrends.Add(new RuleDefinitions.TrendInfo { value = 0, sourceName = TargetMirrorImageTag });
        }
    }

    internal static void AttackRollPostfix(
        RulesetAttackMode attackMode,
        RulesetActor target,
        IEnumerable<RuleDefinitions.TrendInfo> toHitTrends,
        ref RuleDefinitions.RollOutcome outcome,
        ref int successDelta,
        bool testMode)
    {
        //skip custom code if attacker doesn't target mirror image
        if (!TargetsMirrorImage(toHitTrends))
        {
            return;
        }

        if (!testMode
            && outcome is RuleDefinitions.RollOutcome.Success or RuleDefinitions.RollOutcome.CriticalSuccess)
        {
            //attacker hit our mirror image, need to remove one of them
            var conditions = GetConditions(target as RulesetCharacter);

            if (conditions.Count > 0)
            {
                target.RemoveCondition(conditions[0]);
            }
        }

        //disable automatic hit, so target won't get hit
        if (attackMode != null)
        {
            attackMode.AutomaticHit = false;
        }

        outcome = RuleDefinitions.RollOutcome.Failure;
        successDelta = Int32.MaxValue;
    }

    private static void ReportAttackerIsBlind(RulesetActor attacker)
    {
        var console = Gui.Game.GameConsole;
        var entry = new GameConsoleEntry("Feedback/&MirrorImageAttackerIsBlind", console.consoleTableDefinition);

        console.AddCharacterEntry(attacker, entry);
        entry.AddParameter(ParameterType.Negative, "Rules/&ConditionBlindedTitle");
        entry.AddParameter(ParameterType.AttackSpellPower, SpellDefinitions.MirrorImage.FormatTitle());
        console.AddEntry(entry);
    }

    private static void ReportAttackerHasSense(RulesetActor attacker, SenseMode.Type sense)
    {
        var console = Gui.Game.GameConsole;
        var entry = new GameConsoleEntry("Feedback/&MirrorImageAttackerHasSense", console.consoleTableDefinition);

        console.AddCharacterEntry(attacker, entry);
        entry.AddParameter(ParameterType.Positive, Gui.Format(Gui.SenseTypeTitleFormat, $"{sense}"));
        entry.AddParameter(ParameterType.AttackSpellPower, SpellDefinitions.MirrorImage.FormatTitle());
        console.AddEntry(entry);
    }

    private static void ReportTargetingMirrorImage(RulesetActor attacker, RulesetActor target, int roll, bool success)
    {
        var console = Gui.Game.GameConsole;

        //Add line about mirror image roll result
        var entry = new GameConsoleEntry("Feedback/&MirrorImageRetargetRoll", console.consoleTableDefinition);

        string result;
        ParameterType resultType;

        if (success)
        {
            result = GameConsole.SaveSuccessOutcome;
            resultType = ParameterType.SuccessfulRoll;
        }
        else
        {
            result = GameConsole.SaveFailureOutcome;
            resultType = ParameterType.FailedRoll;
        }

        console.AddCharacterEntry(target, entry);
        entry.AddParameter(resultType, Gui.Format(result, roll.ToString()));
        entry.AddParameter(ParameterType.AttackSpellPower, SpellDefinitions.MirrorImage.FormatTitle());
        console.AddEntry(entry);

        //Add line about what attacker will target - defender or decoy
        entry = new GameConsoleEntry("Feedback/&MirrorImageRetargetResult", console.consoleTableDefinition)
        {
            Indent = true
        };

        console.AddCharacterEntry(attacker, entry);

        if (success)
        {
            entry.AddParameter(ParameterType.AttackSpellPower, ConditionTitle);
        }
        else
        {
            console.AddCharacterEntry(target, entry);
        }

        console.AddEntry(entry);
    }

    private class DuplicateCounter : IOnConditionAddedOrRemoved
    {
        private DuplicateCounter()
        {
        }

        public static IOnConditionAddedOrRemoved Mark { get; } = new DuplicateCounter();

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (!GetConditions(target).Empty())
            {
                return;
            }

            target.SpellsCastByMe.Find(e => e.SpellDefinition == SpellDefinitions.MirrorImage)?.Terminate(true);
        }
    }

    internal class DuplicateProvider : IOnConditionAddedOrRemoved
    {
        private DuplicateProvider()
        {
        }

        public static IOnConditionAddedOrRemoved Mark { get; } = new DuplicateProvider();

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            for (var i = 0; i < 3; i++)
            {
                target.InflictCondition(
                    Condition.Name,
                    RuleDefinitions.DurationType.Minute,
                    1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    target.guid,
                    target.CurrentFaction.Name,
                    1,
                    Condition.Name,
                    0,
                    0,
                    0);
            }
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var conditions = GetConditions(target);

            foreach (var condition in conditions)
            {
                target.RemoveCondition(condition);
            }
        }
    }
}
