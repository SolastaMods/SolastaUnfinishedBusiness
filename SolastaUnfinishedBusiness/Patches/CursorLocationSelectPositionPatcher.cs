using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    //BUGFIX: fix vanilla not considering war lists when casting spells with position target
    [HarmonyPatch(typeof(CursorLocationSelectPosition), nameof(CursorLocationSelectPosition.Activate))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Activate_Patch
    {
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

        private static int ComputeAdditionalSummonsBySlotDelta(
            EffectAdvancement __instance,
            int effectAdvancement,
            CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            var result = __instance.ComputeAdditionalSummonsBySlotDelta(effectAdvancement);

            // ReSharper disable once InvertIf
            if (__instance is
                {
                    EffectIncrementMethod: EffectIncrementMethod.PerAdditionalSlotLevel,
                    additionalSummonsPerIncrement: > 0
                } &&
                cursorLocationSelectPosition.ActionParams.RulesetEffect is RulesetEffectSpell rulesetEffectSpell)
            {
                var effectLevel = rulesetEffectSpell.EffectLevel;
                var slotLevel = rulesetEffectSpell.SlotLevel;
                var additionalSummons = __instance.ComputeAdditionalSummonsBySlotDelta(effectLevel - slotLevel);

                result += additionalSummons;
            }

            return result;
        }
    }

    [HarmonyPatch(typeof(CursorLocationSelectPosition), nameof(CursorLocationSelectPosition.ComputeValidPositions))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeValidPositions_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(IEnumerator values, CursorLocationSelectPosition __instance)
        {
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            //PATCH: supports `IFilterTargetingPosition`
            foreach (var iFilter in __instance.ActionParams.ActingCharacter.RulesetCharacter
                         .GetSubFeaturesByType<IFilterTargetingPosition>())
            {
                iFilter?.Filter(__instance);
            }
        }
    }
}
