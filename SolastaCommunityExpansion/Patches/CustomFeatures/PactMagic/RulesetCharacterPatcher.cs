using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using static SolastaCommunityExpansion.Classes.Warlock.Warlock;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    [HarmonyPatch(typeof(RulesetCharacter), "ApplyRest")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_ApplyRest
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (Main.IsMulticlassInstalled)
            {
                foreach (var instruction in instructions)
                {
                    yield return instruction;
                }

                yield break;
            }

            var restoreAllSpellSlotsMethod = typeof(RulesetSpellRepertoire).GetMethod("RestoreAllSpellSlots");
            var myRestoreAllSpellSlotsMethod = typeof(RulesetCharacter_ApplyRest).GetMethod("RestoreAllSpellSlots");

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(restoreAllSpellSlotsMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1); // restType
                    yield return new CodeInstruction(OpCodes.Call, myRestoreAllSpellSlotsMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static void RestoreAllSpellSlots(RulesetSpellRepertoire rulesetSpellRepertoire, RuleDefinitions.RestType restType)
        {
            if (rulesetSpellRepertoire.SpellCastingClass != ClassWarlock || restType == RuleDefinitions.RestType.LongRest)
            {
                rulesetSpellRepertoire.RestoreAllSpellSlots();

                return;
            }

            // restores Warlock short rest slots
            var usedSpellsSlots = rulesetSpellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

            for (var i = 1; i < MYSTIC_ARCANUM_SPELL_LEVEL; i++)
            {
                if (usedSpellsSlots.ContainsKey(i))
                {
                    usedSpellsSlots[i] = 0;
                }
            }

            rulesetSpellRepertoire.RepertoireRefreshed?.Invoke(rulesetSpellRepertoire);
        }
    }
}
