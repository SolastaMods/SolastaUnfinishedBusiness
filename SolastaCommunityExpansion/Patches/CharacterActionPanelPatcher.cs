using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.CustomUI;

namespace SolastaCommunityExpansion.Patches;

internal static class CharacterActionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterActionPanel), "RefreshActions")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshActions_Patch
    {
        internal static void Postfix(CharacterActionPanel __instance)
        {
            //PATCH: Adds extra items to the action panel if character has more than 1 attack mode available for action type of this panel
            ExtraAttacksOnActionPanel.AddExtraAttackItems(__instance);
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), "ReadyActionEngaged")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReadyActionEngaged_Patch
    {
        internal static void Prefix(CharacterActionPanel __instance, ActionDefinitions.ReadyActionType readyActionType)
        {
            //PATCH: used for `force preferred cantrip` option
            CustomReactionsContext.SaveReadyActionPreferredCantrip(__instance.actionParams, readyActionType);
        }
    }
}
