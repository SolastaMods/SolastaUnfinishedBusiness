using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FeatSubPanelPatcher
{
    [HarmonyPatch(typeof(FeatSubPanel), nameof(FeatSubPanel.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] FeatSubPanel __instance)
        {
            //PATCH: avoids a restart when enabling / disabling feats on the Mod UI panel
            FeatsContext.UpdateRelevantFeatList(__instance);

            //PATCH: sorts the feats panel by Title
            FeatsContext.SortFeats(__instance);

            //PATCH: grouped feats - update children according to current feat list
            FeatsContext.UpdatePanelChildren(__instance);
        }
    }

    //PATCH: enforces the feat selection panel to always display same-width columns
    [HarmonyPatch(typeof(FeatSubPanel), nameof(FeatSubPanel.SetState))]
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
                new Action<RectTransform, bool, FeatSubPanel>(FeatsContext.ForceSameWidth).Method;

            return instructions.ReplaceCalls(forceRebuildLayoutImmediateMethod, "FeatSubPanel.SetState",
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, forceSameWidthMethod));
        }
    }
}
