using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using TA;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.ElvenAccuracy;

[HarmonyPatch(typeof(RulesetCharacter), "RollAttackMode")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_RollAttackMode
{
    internal static RulesetCharacterHero ElvenAccuracyHero { get; private set; }

    internal static void Prefix(
        RulesetCharacter __instance,
        RulesetAttackMode attackMode,
        bool ignoreAdvantage,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        bool testMode)
    {
        ElvenAccuracyHero = null;

        if (ignoreAdvantage
            || testMode
            || attackMode.abilityScore is AttributeDefinitions.Strength or AttributeDefinitions.Constitution)
        {
            return;
        }

        var advantageType = RuleDefinitions.ComputeAdvantage(advantageTrends);

        if (advantageType != RuleDefinitions.AdvantageType.Advantage)
        {
            return;
        }

        var hero = __instance as RulesetCharacterHero ?? __instance.OriginalFormCharacter as RulesetCharacterHero;

        if (hero != null && hero.TrainedFeats.Any(x => x.Name.Contains(Feats.ZappaFeats.ElvenAccuracyTag)))
        {
            ElvenAccuracyHero = hero;
        }
    }
}

[HarmonyPatch(typeof(RulesetActor), "RollDie")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetActor_RollDie
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var rollDieMethod = typeof(RuleDefinitions).GetMethod("RollDie", BindingFlags.Public | BindingFlags.Static);
        var myRollDieMethod = typeof(RulesetActor_RollDie).GetMethod("RollDie");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(rollDieMethod))
            {
                yield return new CodeInstruction(OpCodes.Call, myRollDieMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static int RollDie(
        RuleDefinitions.DieType diceType,
        RuleDefinitions.AdvantageType advantageType,
        out int firstRoll,
        out int secondRoll,
        float rollAlterationScore)
    {
        var hero = RulesetCharacter_RollAttackMode.ElvenAccuracyHero;

        if (hero == null)
        {
            return RuleDefinitions.RollDie(diceType, advantageType, out firstRoll, out secondRoll, rollAlterationScore);
        }

        var flag = rollAlterationScore != 0.0;
        var rolls = new int[3];

        rolls[0] = flag ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore) : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int) diceType]);
        rolls[1] = flag ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore) : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int) diceType]);
        rolls[2] = flag ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore) : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int) diceType]);

        Array.Sort(rolls);

        //
        // TODO: find a better way to add this message to game console
        //

        Gui.Game.GameConsole.LogSimpleLine($"Elven Accuracy triggered. Roll {rolls[0]} discarded.");

        firstRoll = rolls[1];
        secondRoll = rolls[2];

        return Mathf.Max(firstRoll,secondRoll);
    }
}
