﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetEffectSpellPatcher
{
    [HarmonyPatch(typeof(RulesetEffectSpell), "EffectDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EffectDescription_Patch
    {
        internal static void Postfix(RulesetEffectSpell __instance, ref EffectDescription __result)
        {
            //PATCH: support for `ICustomMagicEffectBasedOnCaster` and `IModifySpellEffect` 
            // allowing to pick and/or tweak spell effect depending on some caster properties
            __result = CustomFeaturesContext.ModifySpellEffect(__result, __instance);
        }
    }

    [HarmonyPatch(typeof(RulesetEffectSpell), "GetClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GetClassLevel_Patch
    {
        internal static void Postfix(RulesetEffectSpell __instance, ref int __result, RulesetCharacter character)
        {
            //PATCH: Multicass: enforces cantrips to be cast at character level 
            if (character is RulesetCharacterHero hero
                && __instance.SpellDefinition.SpellLevel == 0)
            {
                __result = hero.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
            }
        }
    }

    // enforces cantrips to be cast at character level
    [HarmonyPatch(typeof(RulesetEffectSpell), "ComputeTargetParameter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ComputeTargetParameter_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Multicass: enforces cantrips to be cast at character level 
            //replaces repertoire's SpellCastingLevel with character level for cantrips

            var spellCastingLevelMethod = typeof(RulesetSpellRepertoire).GetMethod("get_SpellCastingLevel");
            var SpellCastingLevel =
                new Func<RulesetSpellRepertoire, RulesetEffectSpell, int>(MulticlassPatchingContext.SpellCastingLevel)
                    .Method;

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(spellCastingLevelMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                    yield return new CodeInstruction(OpCodes.Call, SpellCastingLevel);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
