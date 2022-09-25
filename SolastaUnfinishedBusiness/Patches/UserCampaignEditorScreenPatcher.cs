using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Models.Level20Context;

namespace SolastaUnfinishedBusiness.Patches;

internal static class UserCampaignEditorScreenPatcher
{
    //PATCH: Allows Campaigns to be created with min level 20 requirement (DMP)
    [HarmonyPatch(typeof(UserCampaignEditorScreen), "OnMinLevelEndEdit")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnMinLevelEndEdit_Patch
    {
        [NotNull]
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            if (Main.Settings.EnableLevel20)
            {
                code
                    .FindAll(x => x.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(x.operand) == GameFinalMaxLevel)
                    .ForEach(x => x.operand = ModMaxLevel);
            }

            return code;
        }
    }

//PATCH: Allows Campaigns to be created with max level 20 requirement (DMP)
    [HarmonyPatch(typeof(UserCampaignEditorScreen), "OnMaxLevelEndEdit")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnMaxLevelEndEdit_Patch
    {
        [NotNull]
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            if (Main.Settings.EnableLevel20)
            {
                code
                    .FindAll(x => x.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(x.operand) == GameFinalMaxLevel)
                    .ForEach(x => x.operand = ModMaxLevel);
            }

            return code;
        }
    }
}
