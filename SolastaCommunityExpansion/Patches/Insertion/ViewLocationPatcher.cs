using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Insertion;

// register our global delegate to keep a tab on some important game state
[HarmonyPatch(typeof(ViewLocation), "Activate")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationCharacterManager_Activate
{
    internal static void Prefix()
    {
        var service = ServiceRepository.GetService<IGameLocationActionService>();

        if (service != null)
        {
            service.ActionStarted += Global.ActionStarted;
        }
    }
}

// unregister our global delegate to keep a tab on some important game state
[HarmonyPatch(typeof(ViewLocation), "Deactivate")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationCharacterManager_Deactivate
{
    internal static void Prefix()
    {
        var service = ServiceRepository.GetService<IGameLocationActionService>();

        if (service != null)
        {
            service.ActionStarted -= Global.ActionStarted;
        }
    }
}
