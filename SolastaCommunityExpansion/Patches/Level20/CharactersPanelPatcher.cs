using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches.Level20
{
    // replaces the hard-coded level
    [HarmonyPatch(typeof(CharactersPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharactersPanel_Refresh
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            code.Find(x => x.opcode.Name == "ldc.i4.s"
                && Convert.ToInt32(x.operand) == GAME_MAX_LEVEL)
                    .operand = Main.Settings.MaxAllowedLevels;

            return code;
        }
    }
}
