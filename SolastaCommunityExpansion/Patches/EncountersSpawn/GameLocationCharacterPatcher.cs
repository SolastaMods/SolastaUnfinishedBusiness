using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.PlayerController
{
    // this patch allows an away party member to die without triggering a Game Over
    [HarmonyPatch(typeof(GameLocationCharacter), "IsCriticalCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacter_IsCriticalCharacter
    {
        internal static void Postfix(ref bool __result, GameLocationCharacter __instance)
        {
            if (Models.EncountersSpawnContext.HasStagedHeroes && Gui.GameLocation.LocationDefinition.IsUserLocation)
            {
                __instance.RulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionCriticalCharacter>(__instance.RulesetCharacter.FeaturesToBrowse);
                __result = !__instance.RulesetCharacter.FeaturesToBrowse.Empty();
            }
        }
    }
}
