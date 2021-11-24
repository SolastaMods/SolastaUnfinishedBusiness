using HarmonyLib;
using System;
using System.Collections.Generic;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches
{
    // allows custom dungeons to be set for parties up to level 20
    internal static class UserLocationSettingsModalPatcher
    {
        [HarmonyPatch(typeof(UserLocationSettingsModal), "OnMinLevelEndEdit")]
        public static class UserLocationSettingsModal_OnMinLevelEndEdit
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);

                if (Main.Settings.EnableLevel20)
                {
                    var opcodes = code.FindAll(x => x.opcode.Name == "ldc.i4.s" && Convert.ToInt32(x.operand) == GAME_MAX_LEVEL);

                    foreach (var opcode in opcodes)
                    {
                        opcode.operand = MOD_MAX_LEVEL;
                    }
                }

                return code;
            }
        }

        [HarmonyPatch(typeof(UserLocationSettingsModal), "OnMaxLevelEndEdit")]
        public static class UserLocationSettingsModal_OnMaxLevelEndEdit
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);

                if (Main.Settings.EnableLevel20)
                {
                    var opcodes = code.FindAll(x => x.opcode.Name == "ldc.i4.s" && Convert.ToInt32(x.operand) == GAME_MAX_LEVEL);

                    foreach (var opcode in opcodes)
                    {
                        opcode.operand = MOD_MAX_LEVEL;
                    }
                }

                return code;
            }
        }
    }
}
