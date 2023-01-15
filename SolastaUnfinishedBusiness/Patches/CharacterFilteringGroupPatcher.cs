using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterFilteringGroupPatcher
{
    [HarmonyPatch(typeof(CharacterFilteringGroup), "Compare")]
    [UsedImplicitly]
    public static class Compare_Patch
    {
        //PATCH: correctly offers on adventures with min/max caps on character level (MULTICLASS)
        private static int MyLevels(IEnumerable<int> levels)
        {
            return levels.Sum();
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var myLevelMethod = new Func<IEnumerable<int>, int>(MyLevels).Method;
            var levelsField = typeof(RulesetCharacterHero.Snapshot).GetField("Levels");

            return instructions.ReplaceLoadField(levelsField,
                -1,
                2, "CharacterFilteringGroup.Compare",
                new CodeInstruction(OpCodes.Ldfld, levelsField),
                new CodeInstruction(OpCodes.Call, myLevelMethod));
        }
    }
}
