using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class AttributeDefinitionsPatcher
{
    [HarmonyPatch(typeof(AttributeDefinitions), nameof(AttributeDefinitions.ComputeCostToRaiseAbility))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeCostToRaiseAbility_Patch
    {
        private static readonly int[] Array = [15, 16];
        private static readonly int[] Array0 = [17, 18];

        [UsedImplicitly]
        public static void Postfix(int previousValue, ref int __result)
        {
            //PATCH: extends the cost buy table to enable `EpicPointsAndArray`
            if (!Main.Settings.EnableEpicPointsAndArray)
            {
                return;
            }

            if (System.Array.IndexOf(Array, previousValue) != -1)
            {
                __result = 3;
            }
            else if (System.Array.IndexOf(Array0, previousValue) != -1)
            {
                __result = 4;
            }
        }
    }
}
