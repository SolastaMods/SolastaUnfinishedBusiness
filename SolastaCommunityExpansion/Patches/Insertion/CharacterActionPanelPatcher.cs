using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Insertion;

internal static class CharacterActionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterActionPanel), "ReadyActionEngaged")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReadyActionEngaged
    {
        internal static void Prefix(CharacterActionPanel __instance, ActionDefinitions.ReadyActionType readyActionType)
        {
            CustomReactionsContext.SaveReadyActionPreferedCantripPatch(__instance.actionParams, readyActionType);
        }
    }
}
