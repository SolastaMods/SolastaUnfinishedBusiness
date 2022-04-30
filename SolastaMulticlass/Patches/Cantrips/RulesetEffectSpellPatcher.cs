using System.Collections.Generic;
using System.Linq;
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

                var code = instructions.ToList();
                var index = code.FindIndex(x => x.Calls(spellCastingLevelMethod));

                // final sequence is ldarg_0, call
                code[index] = new CodeInstruction(OpCodes.Call, mySpellCastingLevelMethod);
                code.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));

                return code;
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
