﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Subclasses.Rogue;
using TA;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetActorPatcher
{
    [HarmonyPatch(typeof(RulesetActor), "ProcessConditionsMatchingOccurenceType")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetActor_ProcessConditionsMatchingOccurenceType
    {
        internal static void Postfix(RulesetActor __instance, RuleDefinitions.TurnOccurenceType occurenceType)
        {
            //PATCH: support for `IConditionRemovedOnSourceTurnStart` - removes appropriately marked conditions
            ConditionRemovedOnSourceTurnStartPatch.RemoveConditionIfNeeded(__instance, occurenceType);
        }
    }

    [HarmonyPatch(typeof(RulesetActor), "RollDie")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RollDie_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollDieMethod = typeof(RuleDefinitions).GetMethod("RollDie", BindingFlags.Public | BindingFlags.Static);
            var myRollDieMethod = typeof(RollDie_Patch).GetMethod("RollDie");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(rollDieMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // this (RulesetActor)
                    yield return new CodeInstruction(OpCodes.Ldarg_2); // rollContext
                    yield return new CodeInstruction(OpCodes.Call, myRollDieMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static int RollDie(
            RuleDefinitions.DieType dieType,
            RuleDefinitions.AdvantageType advantageType,
            out int firstRoll,
            out int secondRoll,
            float rollAlterationScore,
            RulesetActor actor,
            RuleDefinitions.RollContext rollContext
        )
        {
            if (rollContext == RuleDefinitions.RollContext.AttackRoll &&
                advantageType == RuleDefinitions.AdvantageType.Advantage && IsElvenPrecisionContextQualified(actor))
            {
                return Roll3DicesAndKeepBest(actor.Name, dieType, out firstRoll, out secondRoll, rollAlterationScore);
            }

            return RuleDefinitions.RollDie(dieType, advantageType, out firstRoll, out secondRoll,
                rollAlterationScore);
        }

        private static bool IsElvenPrecisionContextQualified(RulesetActor actor)
        {
            var character = GameLocationCharacter.GetFromActor(actor);
            return character.RulesetCharacter is RulesetCharacterHero hero && (from feat in hero.TrainedFeats
                where feat.Name.Contains(ZappaFeats.ElvenAccuracyTag)
                select feat.GetFirstSubFeatureOfType<ElvenPrecisionContext>()
                into context
                where context != null
                select context).Any(sub => sub.Qualified);
        }

        private static int Roll3DicesAndKeepBest(
            string roller,
            RuleDefinitions.DieType diceType,
            out int firstRoll,
            out int secondRoll,
            float rollAlterationScore
        )
        {
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
                roller, rolls[2].ToString(), rolls[1].ToString(),
                rolls[0].ToString());

            Gui.Game.GameConsole.LogSimpleLine(line);

            firstRoll = rolls[1];
            secondRoll = rolls[2];

            return Mathf.Max(firstRoll, secondRoll);
        }

        // TODO: make this more generic
        internal static void Prefix(RulesetActor __instance, RuleDefinitions.RollContext rollContext,
            ref bool enumerateFeatures, ref bool canRerollDice)
        {
            //PATCH: support for `Raven` Rogue subclass

            if (!__instance.HasSubFeatureOfType<Raven.RavenRerollAnyDamageDieMarker>() ||
                rollContext != RuleDefinitions.RollContext.AttackDamageValueRoll)
            {
                return;
            }

            enumerateFeatures = true;
            canRerollDice = true;
        }
    }
}
