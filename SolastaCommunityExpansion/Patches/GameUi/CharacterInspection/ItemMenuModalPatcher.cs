using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    // allows mark deity to work with MC heroes
    [HarmonyPatch(typeof(ItemMenuModal), "SetupFromItem")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemMenuModal_SetupFromItem
    {
        public static bool RequiresDeity(ItemMenuModal itemMenuModal)
        {
            return itemMenuModal.GuiCharacter.RulesetCharacterHero.ClassesHistory.Exists(x => x.RequiresDeity);
        }

        public static int MaxSpellLevelOfSpellCastingLevel(RulesetSpellRepertoire repertoire)
        {
            return SharedSpellsContext.GetClassSpellLevel(repertoire);
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var requiresDeityMethod = typeof(CharacterClassDefinition).GetMethod("get_RequiresDeity");
            var myRequiresDeityMethod = typeof(ItemMenuModal_SetupFromItem).GetMethod("RequiresDeity");
            var maxSpellLevelOfSpellCastingLevelMethod =
                typeof(RulesetSpellRepertoire).GetMethod("get_MaxSpellLevelOfSpellCastingLevel");
            var myMaxSpellLevelOfSpellCastingLevelMethod =
                typeof(ItemMenuModal_SetupFromItem).GetMethod("MaxSpellLevelOfSpellCastingLevel");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(requiresDeityMethod))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, myRequiresDeityMethod);
                }
                else if (instruction.Calls(maxSpellLevelOfSpellCastingLevelMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, myMaxSpellLevelOfSpellCastingLevelMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
