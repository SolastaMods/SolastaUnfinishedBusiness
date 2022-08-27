using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class GameManagerPatcher
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BindPostDatabase_Patch
    {
        internal static void Postfix()
        {
            //PATCH: loads all mod contexts
            BootContext.Startup();
        }
    }
}
