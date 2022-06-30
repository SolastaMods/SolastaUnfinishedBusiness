using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Insertion;

internal static class CharacterActionReadyPatcher
{
    [HarmonyPatch(typeof(CharacterActionReady), "ExecuteImpl")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ExecuteImpl
    {
        internal static void Prefix(CharacterActionReady __instance)
        {
            CustomReactionsContext.ReadReadyActionPreferedCantripPatch(__instance.actionParams);
        }
    }
}
