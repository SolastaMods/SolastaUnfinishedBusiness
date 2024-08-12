using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ContainerPanelPatcher
{
    private static IEnumerable<CodeInstruction> ReplaceSlotsGetter(IEnumerable<CodeInstruction> instructions,
        string context)
    {
        var oldMethod = typeof(RulesetContainer).GetProperty(nameof(RulesetContainer.InventorySlots))!.GetGetMethod();
        var newMethod =
            typeof(InventoryManagementContext).GetMethod(nameof(InventoryManagementContext.GetFilteredSlots));

        return instructions.ReplaceCalls(oldMethod, context,
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, newMethod));
    }

    //PATCH: Enable Inventory Filtering and Sorting
    [HarmonyPatch(typeof(ContainerPanel), nameof(ContainerPanel.RefreshVisibleSlots))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshVisibleSlots_Patch
    {
        [UsedImplicitly]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var oldMethod = typeof(RectTransform).GetProperty(nameof(RectTransform.anchoredPosition))!.GetSetMethod();
            var newMethod = new Action<RectTransform, Vector2>(Noop).Method;

            return ReplaceSlotsGetter(instructions, "ContainerPanel.RefreshVisibleSlots.1")
                .ReplaceCall(oldMethod, 1, "ContainerPanel.RefreshVisibleSlots.2",
                    new CodeInstruction(OpCodes.Call, newMethod));
        }

        [UsedImplicitly]
        internal static void Prefix(ContainerPanel __instance)
        {
            __instance.slotsTable.anchoredPosition = new Vector2(__instance.slotsTable.anchoredPosition.x, 0);
        }
    }

    //PATCH: Enable Inventory Filtering and Sorting
    [HarmonyPatch(typeof(ContainerPanel), nameof(ContainerPanel.ComputeTableHeight))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeTableHeight_Patch
    {
        [UsedImplicitly]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceSlotsGetter(instructions, "ContainerPanel.ComputeTableHeight");
        }
    }

    private static void Noop(RectTransform instance, Vector2 c)
    {
        //Do nothing
    }
}
