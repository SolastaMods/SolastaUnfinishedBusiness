using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Multiclass.Cantrips
{
    [HarmonyPatch(typeof(CharacterActionCastSpell), "GetAdvancementData")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterActionCastSpell_GetAdvancementData
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
            var mySpellCastingLevelMethod = typeof(CharacterActionCastSpell_GetAdvancementData).GetMethod("SpellCastingLevel");

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
}
