using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUiCharactersPanel
{
    internal static class CharacterFilteringGroupPatcher
    {
        // ensures MC heroes are correctly sorted by level on character selection modal
        [HarmonyPatch(typeof(CharacterFilteringGroup), "Compare")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class CharacterFilteringGroup_Compare
        {
            public static int MyLevels(int[] levels)
            {
                return levels.Sum();
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var bypass = 0;
                var myLevelMethod = typeof(CharacterFilteringGroup_Compare).GetMethod("MyLevels");
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
