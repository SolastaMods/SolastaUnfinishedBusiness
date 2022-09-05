using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class CharacterStageAbilityScoresPanelPatcher
{
    //PATCH: extends the cost buy table to enable `EpicPointsAndArray`
    [HarmonyPatch(typeof(CharacterStageAbilityScoresPanel), "Reset")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Reset_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (Main.Settings.EnableEpicPointsAndArray)
                {
                    if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.ToString() ==
                        InitialChoicesContext.GameBuyPoints.ToString())
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_S, InitialChoicesContext.ModBuyPoints);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

//PATCH: extends the cost buy table to enable `EpicPointsAndArray`
    [HarmonyPatch(typeof(CharacterStageAbilityScoresPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Refresh_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (Main.Settings.EnableEpicPointsAndArray)
                {
                    if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand.ToString() ==
                        InitialChoicesContext.GameBuyPoints.ToString())
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, 1f * InitialChoicesContext.ModBuyPoints);
                    }
                    else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.ToString() ==
                             InitialChoicesContext.GameBuyPoints.ToString())
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_S, InitialChoicesContext.ModBuyPoints);
                    }
                    else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.ToString() ==
                             InitialChoicesContext.GameMaxAttribute.ToString())
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_S, InitialChoicesContext.ModMaxAttribute);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
