using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Insertion
{
    internal static class CharacterActionSpendPowerPatcher
    {
        [HarmonyPatch(typeof(CharacterActionSpendPower), "ExecuteImpl")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class CharacterActionSpendPower_ExecuteImpl
        {
            internal static void Prefix(CharacterActionSpendPower __instance)
            {
                PowerBundleContext.SpendBundledPowerIfNeeded(__instance);
            }
        }
    }
}
