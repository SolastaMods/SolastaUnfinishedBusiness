using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

[HarmonyPatch(typeof(SlotStatusTable), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SlotStatusTable_Bind
{
    //PATCH: Warlock unique case under MC (Multiclass)
    public static bool UniqueLevelSlots(RulesetSpellRepertoire rulesetSpellRepertoire,
        FeatureDefinitionCastSpell featureDefinitionCastSpell)
    {
        return false;

        //TODO: Check why below fails...
        //
        // if (rulesetSpellRepertoire?.CharacterName == null)
        // {
        //     return featureDefinitionCastSpell.UniqueLevelSlots;
        // }
        //
        // var heroWithSpellRepertoire = SharedSpellsContext.GetHero(rulesetSpellRepertoire.CharacterName);
        //
        // if (heroWithSpellRepertoire == null)
        // {
        //     return featureDefinitionCastSpell.UniqueLevelSlots;
        // }
        //
        // return featureDefinitionCastSpell.UniqueLevelSlots && !SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire);
    }

    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var uniqueLevelSlotsMethod = typeof(FeatureDefinitionCastSpell).GetMethod("get_UniqueLevelSlots");
        var myUniqueLevelSlotsMethod = typeof(SlotStatusTable_Bind).GetMethod("UniqueLevelSlots");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(uniqueLevelSlotsMethod))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call, myUniqueLevelSlotsMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    //PATCH: creates different slots colors and pop up messages depending on slot types
    public static void Postfix(
        SlotStatusTable __instance,
        RulesetSpellRepertoire spellRepertoire,
        int spellLevel)
    {
        // spellRepertoire is null during level up...
        if (spellRepertoire == null || spellLevel == 0)
        {
            return;
        }

        var heroWithSpellRepertoire = SharedSpellsContext.GetHero(spellRepertoire.CharacterName);

        if (heroWithSpellRepertoire is null)
        {
            return;
        }

        spellRepertoire.GetSlotsNumber(spellLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);

        MulticlassGameUiContext.PaintPactSlots(
            heroWithSpellRepertoire, totalSlotsCount, totalSlotsRemainingCount, spellLevel, __instance.table, true);
    }
}

//PATCH: ensures slot colors are white before getting back to pool
[HarmonyPatch(typeof(SlotStatusTable), "Unbind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SlotStatusTable_Unbind
{
    public static void Prefix(SlotStatusTable __instance)
    {
        MulticlassGameUiContext.PaintSlotsWhite(__instance.table);
    }
}
