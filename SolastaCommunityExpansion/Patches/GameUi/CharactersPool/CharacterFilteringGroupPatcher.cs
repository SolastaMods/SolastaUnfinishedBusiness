using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.CharactersPool;

// ensures MC heroes are correctly offered on adventures with min/max caps on character level
[HarmonyPatch(typeof(CharacterFilteringGroup), "Compare")]
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

            if (!instruction.LoadsField(levelsField))
            {
                continue;
            }

            yield return new CodeInstruction(OpCodes.Call, myLevelMethod);
            bypass = 2;
        }
    }
}
