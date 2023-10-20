using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CursorLocationSelectPositionPatcher
{
    [HarmonyPatch(typeof(CursorLocationSelectPosition), nameof(CursorLocationSelectPosition.Activate))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Activate_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var computeAdditionalSummonsBySlotDeltaMethod =
                typeof(EffectAdvancement).GetMethod("ComputeAdditionalSummonsBySlotDelta");
            var myComputeAdditionalSummonsBySlotDeltaMethod =
                new Func<EffectAdvancement, int, CursorLocationSelectPosition, int>(ComputeAdditionalSummonsBySlotDelta)
                    .Method;

            return instructions.ReplaceCall(computeAdditionalSummonsBySlotDeltaMethod, 1,
                "CursorLocationSelectPosition.Activate",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, myComputeAdditionalSummonsBySlotDeltaMethod));
        }

        //BUGFIX: fix vanilla not considering war lists when casting spells with position target
        private static int ComputeAdditionalSummonsBySlotDelta(
            EffectAdvancement __instance,
            int effectAdvancement,
            CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            var result = __instance.ComputeAdditionalSummonsBySlotDelta(effectAdvancement);

            if (__instance is
                    not
                    {
                        EffectIncrementMethod: EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalSummonsPerIncrement: > 0
                    } ||
                cursorLocationSelectPosition.ActionParams.RulesetEffect is not RulesetEffectSpell rulesetEffectSpell)
            {
                return result;
            }

            var effectLevel = rulesetEffectSpell.EffectLevel;
            var slotLevel = rulesetEffectSpell.SlotLevel;
            var additionalSummons = __instance.ComputeAdditionalSummonsBySlotDelta(effectLevel - slotLevel);

            result += additionalSummons;

            return result;
        }
    }

    //PATCH: supports `IFilterTargetingPosition`
    [HarmonyPatch(typeof(CursorLocationSelectPosition), nameof(CursorLocationSelectPosition.OnClickMainPointer))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnClickMainPointer_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            CursorLocationSelectPosition __instance,
            out CursorDefinitions.CursorActionResult actionResult)
        {
            actionResult = CursorDefinitions.CursorActionResult.None;

            var actionParams = __instance.ActionParams;

            if (actionParams.RulesetEffect is RulesetEffectPower rulesetEffectPower
                && rulesetEffectPower.PowerDefinition.HasSubFeatureOfType<IFilterTargetingPosition>())
            {
                return __instance.validPositionsCache.Contains(__instance.HoveredLocation);
            }

            return true;
        }
    }

    //PATCH: supports `IFilterTargetingPosition`
    [HarmonyPatch(typeof(CursorLocationSelectPosition), nameof(CursorLocationSelectPosition.RefreshHover))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshHover_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CursorLocationSelectPosition __instance)
        {
            var actionParams = __instance.ActionParams;

            if (actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower)
            {
                return;
            }

            var filterTargetingPosition =
                rulesetEffectPower.PowerDefinition.GetFirstSubFeatureOfType<IFilterTargetingPosition>();

            if (filterTargetingPosition == null)
            {
                return;
            }

            filterTargetingPosition.EnumerateValidPositions(__instance, __instance.validPositionsCache);
            __instance.movementHelper.RefreshValidDestinations(__instance.validPositionsCache);
            __instance.validCellsComputationCoroutine.Reset();
        }
    }
}
