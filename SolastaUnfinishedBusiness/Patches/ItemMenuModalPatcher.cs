using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class ItemMenuModalPatcher
{
    //PATCH: allows mark deity to work with MC heroes (Multiclass)
    [HarmonyPatch(typeof(ItemMenuModal), "SetupFromItem")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SetupFromItem_Patch
    {
        public static bool RequiresDeity(ItemMenuModal itemMenuModal)
        {
            return itemMenuModal.GuiCharacter.RulesetCharacterHero.ClassesHistory.Exists(x => x.RequiresDeity);
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var requiresDeityMethod = typeof(CharacterClassDefinition).GetMethod("get_RequiresDeity");
            var myRequiresDeityMethod = typeof(SetupFromItem_Patch).GetMethod("RequiresDeity");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(requiresDeityMethod))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, myRequiresDeityMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
