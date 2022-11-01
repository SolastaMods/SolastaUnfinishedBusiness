using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class MirrorImage
{
    private const string ConditionName = "ConditionMirrorImage";
    private const string TargetMirrorImageTag = "TargetsMirrorImage";
    private static string ConditionTitle = GuiPresentationBuilder.CreateTitleKey(ConditionName, Category.Condition);
    private static ConditionDefinition _condition;
    internal static ConditionDefinition Condition => _condition ??= BuildCondition();

    private static ConditionDefinition BuildCondition()
    {
        return ConditionDefinitionBuilder
            .Create(MirrorImage.ConditionName)
            .SetGuiPresentation(Category.Condition,
                CustomIcons.GetSprite("ConditionMirrorImage", Resources.ConditionMirrorImage, 32))
            .SetCustomSubFeatures(MirrorImage.Marker)
            .SetSilent(Silent.WhenAdded)
            .SetAllowMultipleInstances(true)
            .SetPossessive()
            .AddToDB();
    }

    public static MirrorImage Marker { get; } = new();

    private MirrorImage()
    {
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

        //TODO: make Blindsight, Tremorsense and True Sight ignore mirror image

        //TODO: Bonus points if we can manage to change attack `GameConsole.AttackRolled` to show duplicate, instead of the target

        //TODO: adde custom context and modify Halfling's Lucky to include it
        var result = target.RollDie(RuleDefinitions.DieType.D20, RuleDefinitions.RollContext.None, false,
            RuleDefinitions.AdvantageType.None, out var _, out var _, skill: TargetMirrorImageTag);

        var hitImage = false;

        if (conditions.Count >= 3 && result >= 6) { hitImage = true; }
        else if (conditions.Count == 2 && result >= 8) { hitImage = true; }
        else if (conditions.Count == 1 && result >= 11) { hitImage = true; }

        ReportTargetingMirrorImage(attacker, target, result, hitImage);

        if (hitImage)
        {
            toHitTrends.Add(new RuleDefinitions.TrendInfo() {value = 0, sourceName = TargetMirrorImageTag});
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

    private static void ReportTargetingMirrorImage(RulesetActor attacker, RulesetActor target, int roll, bool success)
    {
        //TODO: add proper translation
        var line = success
            ? "{0} rolls {2}, {1} will target {3}"
            : "{0} rolls {2}, {1} will not target {3}";
        var console = Gui.Game.GameConsole;
        var entry = new GameConsoleEntry(line, console.consoleTableDefinition);

        string result;
        ConsoleStyleDuplet.ParameterType resultType;
        if (success)
        {
            result = GameConsole.SaveSuccessOutcome;
            resultType = ConsoleStyleDuplet.ParameterType.SuccessfulRoll;
        }
        else
        {
            result = GameConsole.SaveFailureOutcome;
            resultType = ConsoleStyleDuplet.ParameterType.FailedRoll;
        }

        console.AddCharacterEntry(target, entry);
        console.AddCharacterEntry(attacker, entry);

        entry.AddParameter(resultType, Gui.Format(result, roll.ToString()));

        entry.AddParameter(ConsoleStyleDuplet.ParameterType.AttackSpellPower, ConditionTitle);

        console.AddEntry(entry);
    }

    internal class DuplicateProvider : ICustomConditionFeature
    {
        private DuplicateProvider()
        {
        }

        public static ICustomConditionFeature Mark { get; } = new DuplicateProvider();

        //TODO: when all 3 duplictes are gone, stop spell effect
        public void ApplyFeature(RulesetCharacter hero)
        {
            for (var i = 0; i < 3; i++)
            {
                var condition = RulesetCondition.CreateActiveCondition(
                    hero.Guid, MirrorImage.Condition, RuleDefinitions.DurationType.Minute, 1,
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
