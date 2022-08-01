using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Models;
using TA;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.ElvenAccuracy;

[HarmonyPatch(typeof(RulesetActor), "RollDie")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetActor_RollDie
{
    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
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

    // ReSharper disable once UnusedMember.Global
    public static int RollDie(
        RuleDefinitions.DieType diceType,
        RuleDefinitions.AdvantageType advantageType,
        out int firstRoll,
        out int secondRoll,
        float rollAlterationScore)
    {
        var hero = Global.ElvenAccuracyHero;

        if (hero == null)
        {
            return RuleDefinitions.RollDie(diceType, advantageType, out firstRoll, out secondRoll, rollAlterationScore);
        }

        var flag = rollAlterationScore != 0.0;
        var rolls = new int[3];

        rolls[0] = flag
            ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore)
            : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int)diceType]);
        rolls[1] = flag
            ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore)
            : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int)diceType]);
        rolls[2] = flag
            ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore)
            : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int)diceType]);

        Array.Sort(rolls);

        var line = Gui.Format("Feedback/&ElvenAccuracyTriggered",
            hero.name, rolls[2].ToString(), rolls[1].ToString(),
            rolls[0].ToString());

        Gui.Game.GameConsole.LogSimpleLine(line);

        firstRoll = rolls[1];
        secondRoll = rolls[2];

        return Mathf.Max(firstRoll, secondRoll);
    }
}
