using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using static ConsoleStyleDuplet;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class MirrorImage
{
    private const string ConditionName = "ConditionMirrorImage";
    private const string TargetMirrorImageTag = "TargetsMirrorImage";

    private static readonly string ConditionTitle =
        GuiPresentationBuilder.CreateTitleKey(ConditionName, Category.Condition);

    private static ConditionDefinition _condition;

    private MirrorImage()
    {
    }

    internal static ConditionDefinition Condition => _condition ??= BuildCondition();

    private static MirrorImage Marker { get; } = new();

    private static ConditionDefinition BuildCondition()
    {
        return ConditionDefinitionBuilder
            .Create(ConditionName)
            .SetGuiPresentation(Category.Condition,
                CustomIcons.GetSprite("ConditionMirrorImage", Resources.ConditionMirrorImage, 32))
            .SetCustomSubFeatures(Marker)
            .SetSilent(Silent.WhenAdded)
            .SetAllowMultipleInstances(true)
            .SetPossessive()
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureMirrorImageCounter")
                .SetGuiPresentation(ConditionName, Category.Condition)
                .SetCustomSubFeatures(DuplicateCounter.Mark)
                .AddToDB())
            .AddToDB();
    }

    private static List<RulesetCondition> GetConditions(RulesetCharacter character)
    {
        var conditions = new List<RulesetCondition>();
        if (character == null) { return conditions; }

        character.GetAllConditions(conditions);
        return conditions.FindAll(c =>
                c.ConditionDefinition.HasSubFeatureOfType<MirrorImage>())
            .ToList();
    }

    internal static IEnumerable<CodeInstruction> PatchAttackRoll(IEnumerable<CodeInstruction> instructions)
    {
        var foundAC = false;
        var replaced = false;
        var method = new Func<RulesetAttribute, RulesetActor, List<RuleDefinitions.TrendInfo>, int>(GetAC).Method;

        foreach (var code in instructions)
        {
            var operand = $"{code.operand}";

            if (code.opcode == OpCodes.Ldstr && operand.Contains(AttributeDefinitions.ArmorClass))
            {
                foundAC = true;
                yield return code;
            }
            else if (foundAC && !replaced && code.opcode == OpCodes.Callvirt &&
                     operand.Contains("get_CurrentValue"))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Ldarg, 4);
                yield return new CodeInstruction(OpCodes.Call, method);
                replaced = true;
            }
            else
            {
                yield return code;
            }
        }

        if (!replaced)
        {
            Main.Error("RulesetCharacter.RollAttack PATCH FAILED");
        }
    }

    private static int GetAC(RulesetAttribute attribute, RulesetActor target,
        List<RuleDefinitions.TrendInfo> toHitTrends)
    {
        if (!TargetsMirrorImage(toHitTrends)) { return attribute.CurrentValue; }

        var dexterity = target.TryGetAttributeValue(AttributeDefinitions.Dexterity);
        return 10 + AttributeDefinitions.ComputeAbilityScoreModifier(dexterity);
    }

    private static bool TargetsMirrorImage(List<RuleDefinitions.TrendInfo> toHitTrends)
    {
        return toHitTrends.Any(t => t.sourceName == TargetMirrorImageTag);
    }

    internal static void AttackRollPrefix(RulesetCharacter attacker, RulesetActor target,
        List<RuleDefinitions.TrendInfo> toHitTrends, bool testMode)
    {
        if (!testMode) { return; }

        var conditions = GetConditions(target as RulesetCharacter);

        if (conditions.Empty()) { return; }

        if (attacker.HasConditionOfType(RuleDefinitions.ConditionBlinded))
        {
            ReportAttackerIsBlind(attacker);
            return;
        }

        var distance = (int)attacker.DistanceTo(target);
        foreach (var sense in attacker.SenseModes)
        {
            if (sense.senseType is not (SenseMode.Type.Blindsight or SenseMode.Type.Truesight
                or SenseMode.Type.Tremorsense))
            {
                continue;
            }

            if (sense.senseType is SenseMode.Type.Tremorsense && !target.IsTouchingGround())
            {
                continue;
            }

            if (sense.senseRange >= distance)
            {
                ReportAttackerHasSense(attacker, sense.senseType);
                return;
            }
        }

        //TODO: Bonus points if we can manage to change attack `GameConsole.AttackRolled` to show duplicate, instead of the target

        //TODO: add custom context and modify Halfling's Lucky to include it
        var result = target.RollDie(RuleDefinitions.DieType.D20, RuleDefinitions.RollContext.None, false,
            RuleDefinitions.AdvantageType.None, out var _, out var _, skill: TargetMirrorImageTag);

        var hitImage = false;

        if (conditions.Count >= 3 && result >= 6) { hitImage = true; }
        else if (conditions.Count == 2 && result >= 8) { hitImage = true; }
        else if (conditions.Count == 1 && result >= 11) { hitImage = true; }

        ReportTargetingMirrorImage(attacker, target, result, hitImage);

        if (hitImage)
        {
            toHitTrends.Add(new RuleDefinitions.TrendInfo { value = 0, sourceName = TargetMirrorImageTag });
        }
    }

    internal static void AttackRollPostfix(RulesetCharacter attacker, RulesetAttackMode attackMode, RulesetActor target,
        List<RuleDefinitions.TrendInfo> toHitTrends, ref RuleDefinitions.RollOutcome outcome, ref int successDelta,
        bool testMode)
    {
        //skip custom code if attacker doesn't target mirror image
        if (!TargetsMirrorImage(toHitTrends)) { return; }

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
        if (attackMode != null) { attackMode.AutomaticHit = false; }

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

    private class DuplicateCounter : ICustomConditionFeature
    {
        private DuplicateCounter()
        {
        }

        public static ICustomConditionFeature Mark { get; } = new DuplicateCounter();

        public void ApplyFeature(RulesetCharacter hero)
        {
        }

        public void RemoveFeature(RulesetCharacter hero)
        {
            if (!GetConditions(hero).Empty()) { return; }

            hero.SpellsCastByMe.Find(e => e.SpellDefinition == SpellDefinitions.MirrorImage)?.Terminate(true);
        }
    }

    internal class DuplicateProvider : ICustomConditionFeature
    {
        private DuplicateProvider()
        {
        }

        public static ICustomConditionFeature Mark { get; } = new DuplicateProvider();

        public void ApplyFeature(RulesetCharacter hero)
        {
            for (var i = 0; i < 3; i++)
            {
                var condition = RulesetCondition.CreateActiveCondition(
                    hero.Guid, Condition, RuleDefinitions.DurationType.Minute, 1,
                    RuleDefinitions.TurnOccurenceType.EndOfTurn, hero.Guid, hero.CurrentFaction.Name);

                hero.AddConditionOfCategory(AttributeDefinitions.TagCombat, condition);
            }
        }

        public void RemoveFeature(RulesetCharacter hero)
        {
            var conditions = GetConditions(hero);
            foreach (var condition in conditions)
            {
                hero.RemoveCondition(condition);
            }
        }
    }
}
