using System;
using System.Collections.Generic;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches
{
    // unhides features for levels above 10
    internal static class HigherLevelFeaturesModalPatcher
    {
        // replaces the hard coded experience
        [HarmonyPatch(typeof(HigherLevelFeaturesModal), "Bind")]
        internal static class HigherLevelFeaturesModal_Bind_Patch
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);

                if (Main.Settings.EnableLevel20)
                {
                    code.Find(x => x.opcode.Name == "ldc.i4.s" && Convert.ToInt32(x.operand) == GAME_MAX_LEVEL - 1).operand = MOD_MAX_LEVEL;
                }

                return code;
            }
        }
    }
}