using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Classes.Warlock;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Models.SpellsHelper;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.SharedSpells;

[HarmonyPatch(typeof(RulesetCharacter), "ApplyRest")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_ApplyRest
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var restoreAllSpellSlotsMethod = typeof(RulesetSpellRepertoire).GetMethod("RestoreAllSpellSlots");
        var myRestoreAllSpellSlotsMethod = typeof(RulesetCharacter_ApplyRest).GetMethod("RestoreAllSpellSlots");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(restoreAllSpellSlotsMethod))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0); // rulesetCharacter
                yield return new CodeInstruction(OpCodes.Ldarg_1); // restType
                yield return new CodeInstruction(OpCodes.Call, myRestoreAllSpellSlotsMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static void RestoreAllSpellSlots(RulesetSpellRepertoire __instance, RulesetCharacter rulesetCharacter,
        RuleDefinitions.RestType restType)
    {
        if (restType == RuleDefinitions.RestType.LongRest
            || rulesetCharacter is not RulesetCharacterHero heroWithSpellRepertoire)
        {
            rulesetCharacter.RestoreAllSpellSlots();

            return;
        }

        var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
        var slotsToRestore = SharedSpellsContext.GetWarlockUsedSlots(heroWithSpellRepertoire);

        foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                     .Where(x => x.SpellCastingRace == null))
        {
            var usedSpellsSlots =
                spellRepertoire.usedSpellsSlots;

            for (var i = WarlockSpells.PactMagicSlotTabIndex; i <= warlockSpellLevel; i++)
            {
                if (usedSpellsSlots.ContainsKey(i))
                {
                    usedSpellsSlots[i] -= slotsToRestore;
                }
            }

            spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
        }
    }
}
