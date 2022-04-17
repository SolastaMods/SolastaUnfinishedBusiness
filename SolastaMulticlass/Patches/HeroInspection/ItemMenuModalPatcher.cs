using System.Collections.Generic;
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
}
