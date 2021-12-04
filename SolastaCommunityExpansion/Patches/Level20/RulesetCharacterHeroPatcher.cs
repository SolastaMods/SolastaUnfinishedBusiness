using HarmonyLib;
using System;
using System.Collections.Generic;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches
{
    // replaces the hard coded experience
    [HarmonyPatch(typeof(RulesetCharacterHero), "RegisterAttributes")]
    internal static class RulesetCharacterHero_RegisterAttributes
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            if (Main.Settings.EnableLevel20)
            {
                code.Find(x => x.opcode.Name == "ldc.i4.s" && Convert.ToInt32(x.operand) == GAME_MAX_LEVEL).operand = MOD_MAX_LEVEL;
            }

            return code;
        }
    }
}
