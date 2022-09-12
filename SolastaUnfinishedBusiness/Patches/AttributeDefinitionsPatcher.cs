using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class AttributeDefinitionsPatcher
{
    //PATCH: extends the cost buy table to enable `EpicPointsAndArray`
    [HarmonyPatch(typeof(AttributeDefinitions), "ComputeCostToRaiseAbility")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ComputeCostToRaiseAbility_Patch
    {
        internal static void Postfix(int previousValue, ref int __result)
        {
            if (!Main.Settings.EnableEpicPointsAndArray)
            {
                return;
            }

            if (Array.IndexOf(new[] { 15, 16 }, previousValue) != -1)
            {
                __result = 3;
            }
            else if (Array.IndexOf(new[] { 17, 18 }, previousValue) != -1)
            {
                __result = 4;
            }
        }
    }
}
