using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Cantrips
{
    // enforces cantrips to be cast at character level
    [HarmonyPatch(typeof(RulesetEffectSpell), "ComputeTargetParameter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetEffectSpell_ComputeTargetParameter
    {
        public static int SpellCastingLevel(RulesetSpellRepertoire rulesetSpellRepertoire,
            RulesetEffectSpell rulesetEffectSpell)
        {
            if (rulesetEffectSpell.Caster is RulesetCharacterHero hero
                && rulesetEffectSpell.SpellDefinition.SpellLevel == 0)
            {
                return hero.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
            }

            return rulesetSpellRepertoire.SpellCastingLevel;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var spellCastingLevelMethod = typeof(RulesetSpellRepertoire).GetMethod("get_SpellCastingLevel");
            var mySpellCastingLevelMethod =
                typeof(RulesetEffectSpell_ComputeTargetParameter).GetMethod("SpellCastingLevel");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(spellCastingLevelMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                    yield return new CodeInstruction(OpCodes.Call, mySpellCastingLevelMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    // enforces cantrips to be cast at character level
    [HarmonyPatch(typeof(RulesetEffectSpell), "GetClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetEffectSpell_GetClassLevel
    {
        internal static void Postfix(RulesetEffectSpell __instance, ref int __result, RulesetCharacter character)
        {
            if (character is RulesetCharacterHero hero
                && __instance.SpellDefinition.SpellLevel == 0)
            {
                __result = hero.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
            }
        }
    }
}
