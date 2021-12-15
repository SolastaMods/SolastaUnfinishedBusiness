using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Encounters
{
    [HarmonyPatch(typeof(GameLocationCharacter), "IsCriticalCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacterManager_IsCriticalCharacter
    {
        internal static void Postfix(GameLocationCharacter __instance, ref bool __result)
        {
            var isUserLocation = Gui.GameLocation.LocationDefinition.IsUserLocation;

            if (isUserLocation && !Models.PlayerControllerContext.PlayerCharacters.Contains(__instance))
            {
                __result = false;
            }
        }
    }
}
