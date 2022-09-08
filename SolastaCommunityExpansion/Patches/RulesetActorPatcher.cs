using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Subclasses.Rogue;
using TA;
using UnityEngine;
using static ConsoleStyleDuplet;
using static FeatureDefinitionAttributeModifier;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetActorPatcher
{
    [HarmonyPatch(typeof(RulesetActor), "ProcessConditionsMatchingOccurenceType")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ProcessConditionsMatchingOccurenceType_Patch
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
            var karmic = rollAlterationScore != 0.0;

            int DoRoll()
            {
                return karmic
                    ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore)
                    : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int)diceType]);
            }

            var roll1 = DoRoll();
            var roll2 = DoRoll();
            var roll3 = DoRoll();

            var kept = Math.Max(roll1, roll2);
            var replaced = Math.Min(roll1, roll2);

            var entry = new GameConsoleEntry("Feedback/&ElvenAccuracyTriggered",
                Gui.Game.GameConsole.consoleTableDefinition);

            entry.AddParameter(ParameterType.Player, roller);
            entry.AddParameter(ParameterType.AttackSpellPower, "Tooltip/&FeatElvenAccuracyBaseTitle",
                tooltipContent: "Tooltip/&FeatElvenAccuracyBaseDescription");
            entry.AddParameter(ParameterType.AbilityInfo, kept.ToString());
            entry.AddParameter(ParameterType.AbilityInfo, replaced.ToString());
            entry.AddParameter(ParameterType.AbilityInfo, roll3.ToString());

            Gui.Game.GameConsole.AddEntry(entry);

            firstRoll = kept;
            secondRoll = roll3;

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

    [HarmonyPatch(typeof(RulesetActor), "RefreshAttributes")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshAttributes_Patch
    {
        private static readonly Regex classPattern = new($"{AttributeDefinitions.TagClass}(.*)\\d+");

        private static void RefrestClassModifiers(RulesetActor actor)
        {
            if (actor is not RulesetCharacterHero hero)
            {
                return;
            }

            foreach (var attribute in hero.Attributes)
            {
                foreach (var modifier in attribute.Value.ActiveModifiers)
                {
                    switch (modifier.Operation)
                    {
                        case AttributeModifierOperation.MultiplyByClassLevel:
                        case AttributeModifierOperation.MultiplyByClassLevelBeforeAdditions:
                            if (attribute.Key == AttributeDefinitions.SorceryPoints)
                            {
                                var level = hero.GetClassLevel(GetClassByTags(modifier.tags));
                                if (level > 0)
                                {
                                    modifier.Value = level;
                                }
                            }

                            break;
                    }
                }
            }
        }

        private static CharacterClassDefinition GetClassByTags(List<string> tags)
        {
            foreach (var tag in tags)
            {
                var matches = classPattern.Matches(tag);
                if (matches.Count <= 0)
                {
                    continue;
                }

                var match = matches[0];
                if (match.Groups.Count < 2)
                {
                    continue;
                }

                return DatabaseRepository
                    .GetDatabase<CharacterClassDefinition>()
                    .GetElement(match.Groups[1].Value, true);
            }

            return null;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: enforce class level dependend modifiers use proper values for MC
            //neede for sorcery points and healing pools to be of proper sizes when multiclassed
            //adds custom method right before the end that recalculates modifier values specifically for class-level modifiers
            var refreshAttributes = typeof(RulesetEntity).GetMethod("RefreshAttributes");
            var custom = new Action<RulesetActor>(RefrestClassModifiers).Method;

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(refreshAttributes))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, custom);
                }

                yield return instruction;
            }
        }
    }
}
