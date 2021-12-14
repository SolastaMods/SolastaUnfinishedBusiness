using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Encounters
{
    [HarmonyPatch(typeof(GameLocationCharacter), "IsCriticalCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacterManager_IsCriticalCharacter
    {
        internal static bool Prefix(GameLocationCharacter __instance, ref bool __result)
        {
            if (Models.EncountersSpawnContext.HasStagedHeroes && !Models.PlayerControllerContext.PartyCharacters.Contains(__instance))
            {
                __result = false;

                return false;
            }

            return true;
        }
    }
}
