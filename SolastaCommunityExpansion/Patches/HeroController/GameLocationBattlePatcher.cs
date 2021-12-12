using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.HeroController
{
    internal static class GameLocationBattlePatcher
    {
        [HarmonyPatch(typeof(GameLocationBattle), "CheckVictoryOrDefeat")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class GameLocationBattle_CheckVictoryOrDefeat
        {
            internal static void Prefix()
            {
                Models.HeroControllerContext.Stop();
            }
        }
    }
}
