using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class CharacterStageLevelGainsPanelPatcher
    {
        //
        // WILL FIX THIS LATER
        //

        //// patches the method to get my own class and level for level up
        //[HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "EnterStage")]
        //internal static class CharacterStageLevelGainsPanelEnterStage
        //{
        //    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        //    {
        //        if (!Main.Settings.EnableMulticlass)
        //        {
        //            foreach (var instruction in instructions)
        //            {
        //                yield return instruction;
        //            }

        //            yield break;
        //        }

        //        var getLastAssignedClassAndLevelMethod = typeof(ICharacterBuildingService).GetMethod("GetLastAssignedClassAndLevel");
        //        var getLastAssignedClassAndLevelCustomMethod = typeof(Models.LevelUpContext).GetMethod("GetLastAssignedClassAndLevel");

        //        foreach (var instruction in instructions)
        //        {
        //            if (instruction.Calls(getLastAssignedClassAndLevelMethod))
        //            {
        //                yield return new CodeInstruction(OpCodes.Call, getLastAssignedClassAndLevelCustomMethod);
        //            }
        //            else
        //            {
        //                yield return instruction;
        //            }
        //        }
        //    }
        //}

        // only displays spell casting features from the current class
        [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "RefreshSpellcastingFeatures")]
        internal static class CharacterStageLevelGainsPanelRefreshSpellcastingFeatures
        {
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

                var spellRepertoiresMethod = typeof(RulesetCharacter).GetMethod("get_SpellRepertoires");
                var filteredSpellRepertoiresMethod = typeof(Models.LevelUpContext).GetMethod("SpellRepertoires");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(spellRepertoiresMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Call, filteredSpellRepertoiresMethod);
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
