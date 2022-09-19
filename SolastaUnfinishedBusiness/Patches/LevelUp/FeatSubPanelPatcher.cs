using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches.LevelUp;

internal static class FeatSubPanelPatcher
{
    [HarmonyPatch(typeof(FeatSubPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatSubPanel_Bind
    {
        internal static void Prefix([NotNull] FeatSubPanel __instance)
        {
            //PATCH: avoids a restart when enabling / disabling feats on the Mod UI panel
            FeatsContext.UpdateRelevantFeatList(__instance);

            //PATCH: sorts the feats panel by Title
            FeatsContext.SortFeats(__instance);

            //PATCH: grouped feats - select sub-feats into a separate list
            FeatsContext.ProcessFeatGroups(__instance);

            //PATCH: grouped feats - update children according to current feat list
            FeatsContext.UpdatePanelChildren(__instance);
        }
    }

    //PATCH: enforces the feat selection panel to always display same-width columns
    [HarmonyPatch(typeof(FeatSubPanel), "SetState")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatSubPanel_SetState
    {
        [NotNull]
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var forceRebuildLayoutImmediateMethod = typeof(LayoutRebuilder)
                .GetMethod("ForceRebuildLayoutImmediate", BindingFlags.Static | BindingFlags.Public);
            var forceSameWidthMethod = new Action<RectTransform, bool>(FeatsContext.ForceSameWidth).Method;

            var code = instructions.ToList();
            var index = code.FindIndex(x => x.Calls(forceRebuildLayoutImmediateMethod));

            code[index] = new CodeInstruction(OpCodes.Ldarg_1);
            code.Insert(index + 1, new CodeInstruction(OpCodes.Call, forceSameWidthMethod));

            return code;
        }
    }
}
