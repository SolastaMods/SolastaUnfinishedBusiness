using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStageAbilityScoresPanelPatcher
{
    //PATCH: extends the cost buy table to enable `EpicPointsAndArray`
    [HarmonyPatch(typeof(CharacterStageAbilityScoresPanel), nameof(CharacterStageAbilityScoresPanel.Reset))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Reset_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Settings.EnableEpicPointsAndArray)
            {
                return instructions;
            }

            return instructions.ReplaceCode(
                instruction => instruction.opcode == OpCodes.Ldc_I4_S &&
                               instruction.operand.ToString() == RulesContext.GameBuyPoints.ToString(),
                -1, "CharacterStageAbilityScoresPanel.Reset",
                new CodeInstruction(OpCodes.Ldc_I4_S, RulesContext.ModBuyPoints));
        }
    }

    //PATCH: extends the cost buy table to enable `EpicPointsAndArray`
    [HarmonyPatch(typeof(CharacterStageAbilityScoresPanel), nameof(CharacterStageAbilityScoresPanel.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Settings.EnableEpicPointsAndArray)
            {
                return instructions;
            }

            return instructions
                .ReplaceCode(instruction => instruction.opcode == OpCodes.Ldc_R4 &&
                                            instruction.operand.ToString() ==
                                            RulesContext.GameBuyPoints.ToString(),
                    -1, "CharacterStageAbilityScoresPanel.Refresh.1",
                    new CodeInstruction(OpCodes.Ldc_R4, 1f * RulesContext.ModBuyPoints))
                .ReplaceCode(instruction => instruction.opcode == OpCodes.Ldc_I4_S &&
                                            instruction.operand.ToString() ==
                                            RulesContext.GameBuyPoints.ToString(),
                    -1, "CharacterStageAbilityScoresPanel.Refresh.2",
                    new CodeInstruction(OpCodes.Ldc_I4_S, RulesContext.ModBuyPoints))
                .ReplaceCode(instruction => instruction.opcode == OpCodes.Ldc_I4_S &&
                                            instruction.operand.ToString() ==
                                            RulesContext.GameMaxAttribute.ToString(),
                    -1, "CharacterStageAbilityScoresPanel.Refresh.3",
                    new CodeInstruction(OpCodes.Ldc_I4_S, RulesContext.ModMaxAttribute));
        }
    }
}
