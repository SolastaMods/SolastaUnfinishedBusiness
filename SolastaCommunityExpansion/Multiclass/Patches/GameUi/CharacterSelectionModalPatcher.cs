using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion;

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
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var bypass = 0;
                var myLevelMethod = typeof(CharacterSelectionModalEnumeratePlates).GetMethod("MyLevels");
                var levelsField = typeof(RulesetCharacterHero.Snapshot).GetField("Levels");

                foreach (var instruction in instructions)
                {
                    if (bypass-- > 0)
                    {
                        continue;
                    }

                    yield return instruction;

                    if (instruction.LoadsField(levelsField))
                    {
                        yield return new CodeInstruction(OpCodes.Call, myLevelMethod);
                        bypass = 2;
                    }
                }
            }
        }
    }
}
