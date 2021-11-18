using System;
using System.Collections.Generic;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches.Level20
{
    internal static class ArchetypesPreviewModalPatcher
    {
        // replaces the hard coded experience
        [HarmonyPatch(typeof(ArchetypesPreviewModal), "Refresh")]
        internal static class ArchetypesPreviewModal_Refresh_Patch
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
