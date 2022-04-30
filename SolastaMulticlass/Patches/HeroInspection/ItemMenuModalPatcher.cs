using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaMulticlass.Patches.HeroInspection
{
    internal static class ItemMenuModalPatcher
    {
        public static bool RequiresDeity(ItemMenuModal itemMenuModal)
        {
            return itemMenuModal.GuiCharacter.RulesetCharacterHero.ClassesHistory.Exists(x => x.RequiresDeity);
        }

        // allows mark deity to work with MC heroes
        [HarmonyPatch(typeof(ItemMenuModal), "SetupFromItem")]
        internal static class ItemMenuModalSetupFromItem
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var requiresDeityMethod = typeof(CharacterClassDefinition).GetMethod("get_RequiresDeity");
                var myRequiresDeityMethod = typeof(ItemMenuModalPatcher).GetMethod("RequiresDeity");

                var code = instructions.ToList();
                var index = code.FindIndex(x => x.Calls(requiresDeityMethod));

                // final sequence is pop, ldarg_0, call
                code[index] = new CodeInstruction(OpCodes.Call, myRequiresDeityMethod);
                code.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));
                code.Insert(index, new CodeInstruction(OpCodes.Pop));

                return code;
            }
        }
    }
}
