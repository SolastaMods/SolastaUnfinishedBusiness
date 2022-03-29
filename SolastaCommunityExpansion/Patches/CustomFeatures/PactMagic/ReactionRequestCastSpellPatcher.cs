using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using static SolastaCommunityExpansion.Classes.Warlock.Warlock;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    [HarmonyPatch(typeof(ReactionRequestCastSpell), "BuildSlotSubOptions")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReactionRequestCastSpell_BuildSlotSubOptions
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var maxSpellLevelOfSpellCastingLevelMethod = typeof(RulesetSpellRepertoire).GetMethod("get_MaxSpellLevelOfSpellCastingLevel");
            var myMaxSpellLevelOfSpellCastingLevelMethod = typeof(ReactionRequestCastSpell_BuildSlotSubOptions).GetMethod("MaxSpellLevelOfSpellCastingLevel");

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(maxSpellLevelOfSpellCastingLevelMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_1); // spellLevel
                    yield return new CodeInstruction(OpCodes.Call, myMaxSpellLevelOfSpellCastingLevelMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public static int MaxSpellLevelOfSpellCastingLevel(RulesetSpellRepertoire rulesetSpellRepertoire, int spellLevel)
        {
            if (rulesetSpellRepertoire.SpellCastingClass != ClassWarlock)
            {
                return rulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel;
            }
            else
            {
                if (spellLevel >= MYSTIC_ARCANUM_SPELL_LEVEL)
                {
                    return Math.Min(spellLevel, rulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel);
                }
                else
                {
                    return Math.Min(MYSTIC_ARCANUM_SPELL_LEVEL - 1, rulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel);
                }
            }
        }
    }
}
