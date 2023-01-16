using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class InvocationSubPanelPatcher
{
    [HarmonyPatch(typeof(InvocationSubPanel), nameof(InvocationSubPanel.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(InvocationSubPanel __instance)
        {
            //PATCH: support for custom invocations and separate sub-panels for them
            //filters only invocations that fit this sub-panel
            CustomInvocationSubPanel.UpdateRelevantInvocations(__instance);

            // //PATCH: sorts the invocations panel by Title
            InvocationsContext.SortInvocations(__instance);
        }
    }

    [HarmonyPatch(typeof(InvocationSubPanel), nameof(InvocationSubPanel.SetState))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SetState_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var forceRebuildLayoutImmediateMethod = typeof(LayoutRebuilder)
                .GetMethod("ForceRebuildLayoutImmediate", BindingFlags.Static | BindingFlags.Public);
            var forceSameWidthMethod =
                new Action<RectTransform, bool, InvocationSubPanel>(InvocationsContext.ForceSameWidth).Method;

            var getInvocationProficiencies = typeof(RulesetCharacterHero).GetMethod("get_InvocationProficiencies");
            var customInvocationsProficiencies =
                new Func<RulesetCharacterHero, List<string>>(CustomInvocationSubPanel.CustomInvocationsProficiencies)
                    .Method;

            return instructions
                //PATCH: enforces the invocations selection panel to always display same-width columns
                .ReplaceCalls(forceRebuildLayoutImmediateMethod, "InvocationSubPanel.SetState.1",
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, forceSameWidthMethod))
                //PATCH: don't offer invocations unlearn on non Warlock classes (MULTICLASS)
                .ReplaceCalls(getInvocationProficiencies,
                    "InvocationSubPanel.SetState.2",
                    new CodeInstruction(OpCodes.Call, customInvocationsProficiencies));
        }
    }
}
