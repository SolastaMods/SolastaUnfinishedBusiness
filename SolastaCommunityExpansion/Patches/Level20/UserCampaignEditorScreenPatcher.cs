using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches.Level20
{
    [HarmonyPatch(typeof(UserCampaignEditorScreen), "OnMinLevelEndEdit")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class UserCampaignEditorScreen_OnMinLevelEndEdit
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            if (Main.Settings.EnableLevel20)
            {
                code
                    .FindAll(x => x.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(x.operand) == GAME_MAX_LEVEL)
                    .ForEach(x => x.operand = MOD_MAX_LEVEL);
            }

            return code;
        }
    }

    [HarmonyPatch(typeof(UserCampaignEditorScreen), "OnMaxLevelEndEdit")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class UserCampaignEditorScreen_OnMaxLevelEndEdit
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            if (Main.Settings.EnableLevel20)
            {
                code
                    .FindAll(x => x.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(x.operand) == GAME_MAX_LEVEL)
                    .ForEach(x => x.operand = MOD_MAX_LEVEL);
            }

            return code;
        }
    }
}
