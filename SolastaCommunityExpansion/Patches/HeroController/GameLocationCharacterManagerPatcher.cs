using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.HeroController
{
    //
    // TODO: double-check if this indeed required on latest game version. The test scenario is a hero dies when under AI control.
    //
    [HarmonyPatch(typeof(GameLocationCharacterManager), "KillCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacterManager_KillCharacter
    {
        internal static void Prefix()
        {
            Models.HeroControllerContext.Stop();
        }
    }
}
