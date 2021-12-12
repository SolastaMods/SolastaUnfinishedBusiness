using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.HeroController
{
    internal static class GameLocationCharacterManagerPatcher
    {
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
}
