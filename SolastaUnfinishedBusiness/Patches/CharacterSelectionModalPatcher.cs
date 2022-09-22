using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CharacterSelectionModalPatcher
{
    //PATCH: this patch changes the min/max requirements on locations or campaigns
    [HarmonyPatch(typeof(CharacterSelectionModal), "SelectFromPool")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SelectFromPool_Patch
    {
        internal static void Prefix(CharacterSelectionModal __instance)
        {
            if (!Main.Settings.OverrideMinMaxLevel)
            {
                return;
            }

            __instance.MinLevel = DungeonMakerContext.DungeonMinLevel;
            __instance.MaxLevel = DungeonMakerContext.DungeonMaxLevel;
        }
    }

    [HarmonyPatch(typeof(CharacterSelectionModal), "EnumeratePlates")]
    internal static class EnumeratePlates_Patch
    {
        //PATCH: correctly offers on adventures with min/max caps on character level (MULTICLASS)
        public static int MyLevels(IEnumerable<int> levels)
        {
            return levels.Sum();
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var bypass = 0;
            var myLevelMethod = typeof(EnumeratePlates_Patch).GetMethod("MyLevels");
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
}
