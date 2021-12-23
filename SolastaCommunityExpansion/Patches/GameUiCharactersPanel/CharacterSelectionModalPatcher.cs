using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUiCharactersPanel
{
    internal static class CharacterSelectionModalPatcher
    {
        // ensures MC heroes are correctly offered on adventures with min/max caps on character level
        [HarmonyPatch(typeof(CharacterSelectionModal), "EnumeratePlates")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class CharacterSelectionModal_EnumeratePlates
        {
            public static int MyLevels(int[] levels)
            {
                return levels.Sum();
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var bypass = 0;
                var myLevelMethod = typeof(CharacterSelectionModal_EnumeratePlates).GetMethod("MyLevels");
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
