using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaMulticlass.Patches.Cantrips
{
    internal static class RulesetEffectSpellPatcher
    {
        // enforces cantrips to be cast at character level
        [HarmonyPatch(typeof(RulesetEffectSpell), "ComputeTargetParameter")]
        internal static class RulesetEffectSpellComputeTargetParameter
        {
            public static int SpellCastingLevel(RulesetSpellRepertoire rulesetSpellRepertoire, RulesetEffectSpell rulesetEffectSpell)
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
                var mySpellCastingLevelMethod = typeof(RulesetEffectSpellComputeTargetParameter).GetMethod("SpellCastingLevel");

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
        internal static class RulesetEffectSpellGetClassLevel
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
}
