using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

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
                        CharacterContext.GameBuyPoints.ToString())
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_S, CharacterContext.ModBuyPoints);
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
                        CharacterContext.GameBuyPoints.ToString())
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, 1f * CharacterContext.ModBuyPoints);
                    }
                    else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.ToString() ==
                             CharacterContext.GameBuyPoints.ToString())
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_S, CharacterContext.ModBuyPoints);
                    }
                    else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.ToString() ==
                             CharacterContext.GameMaxAttribute.ToString())
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_S, CharacterContext.ModMaxAttribute);
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
