using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetContainerPatcher
{
    //PATCH: Enable Inventory Filtering and Sorting
    [HarmonyPatch(typeof(RulesetContainer), nameof(RulesetContainer.FitSlotsToContent))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FitSlotsToContent_Patch
    {
        [UsedImplicitly]
        internal static bool Prefix(RulesetContainer __instance)
        {
            int lastItem = __instance.inventorySlots.FindLastIndex(x => x.EquipedItem != null);
            var count = ((lastItem + 1) / RulesetContainer.SlotsPerRow + 1) * RulesetContainer.SlotsPerRow;

            __instance.ReserveSlots(Mathf.Max(__instance.minSlotsNumber, count));

            var containerContentModified = __instance.ContainerContentModified;
            containerContentModified?.Invoke();
            return false;
        }
    }
}
