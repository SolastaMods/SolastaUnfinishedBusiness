using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaMulticlass.Patches.GameUi
{
    internal static class CharacterSelectionModalPatcher
    {
        // ensures MC heroes are correctly offered on adventures with min/max caps on character level
        [HarmonyPatch(typeof(CharacterSelectionModal), "EnumeratePlates")]
        internal static class CharacterSelectionModalEnumeratePlates
        {
            public static int MyLevels(int[] levels)
            {
                return levels.Sum();
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var myLevelMethod = typeof(CharacterSelectionModalEnumeratePlates).GetMethod("MyLevels");
                var levelsField = typeof(RulesetCharacterHero.Snapshot).GetField("Levels");

                var code = instructions.ToList();
                var index = code.FindIndex(x => x.LoadsField(levelsField));

                // final sequence is call, pop
                code[index + 1] = new CodeInstruction(OpCodes.Call, myLevelMethod);
                code[index + 2] = new CodeInstruction(OpCodes.Nop);

                return code;
            }
        }
    }
}
