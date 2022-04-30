using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaMulticlass.Patches.Cantrips
{
    internal static class CharacterActionCastSpellPatcher
    {
        [HarmonyPatch(typeof(CharacterActionCastSpell), "GetAdvancementData")]
        internal static class CharacterActionCastSpellGetAdvancementData
        {
            public static int SpellCastingLevel(RulesetSpellRepertoire rulesetSpellRepertoire, CharacterActionCastSpell characterActionCastSpell)
            {
                if (characterActionCastSpell.ActingCharacter.RulesetCharacter is RulesetCharacterHero hero
                    && characterActionCastSpell.ActiveSpell.SpellDefinition.SpellLevel == 0)
                {
                    return hero.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
                }

                return rulesetSpellRepertoire.SpellCastingLevel;
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var spellCastingLevelMethod = typeof(RulesetSpellRepertoire).GetMethod("get_SpellCastingLevel");
                var mySpellCastingLevelMethod = typeof(CharacterActionCastSpellGetAdvancementData).GetMethod("SpellCastingLevel");

                var code = instructions.ToList();
                var index = code.FindIndex(x => x.Calls(spellCastingLevelMethod));

                // final sequence is ldarg_0, call
                code[index] = new CodeInstruction(OpCodes.Call, mySpellCastingLevelMethod);
                code.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));

                return code;
            }
        }
    }
}
