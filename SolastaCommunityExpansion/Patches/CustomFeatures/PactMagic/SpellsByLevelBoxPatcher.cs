using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic;

// Guarantee Warlock Spell Level will be used whenever possible on SC Warlocks
[HarmonyPatch(typeof(SpellsByLevelBox), "OnActivateStandardBox")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SpellsByLevelBox_OnActivateStandardBox
{
    public static int MySpellLevel(SpellDefinition spellDefinition, SpellsByLevelBox spellsByLevelBox)
    {
        var rulesetSpellRepertoire =
            spellsByLevelBox.spellRepertoire;
        var isWarlockSpell = SharedSpellsContext.IsWarlock(rulesetSpellRepertoire.SpellCastingClass);

        if (!isWarlockSpell || spellDefinition.SpellLevel <= 0)
        {
            return spellDefinition.SpellLevel;
        }

        var hero = SharedSpellsContext.GetHero(rulesetSpellRepertoire.CharacterName);
        var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);

        return warlockSpellLevel;
    }

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var spellLevelMethod = typeof(SpellDefinition).GetMethod("get_SpellLevel");
        var mySpellLevelMethod = typeof(SpellsByLevelBox_OnActivateStandardBox).GetMethod("MySpellLevel");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(spellLevelMethod))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, mySpellLevelMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }
}
