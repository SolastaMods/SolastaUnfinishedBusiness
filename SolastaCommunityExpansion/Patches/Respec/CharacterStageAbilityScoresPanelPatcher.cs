using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    // use this patch to avoid issues during RESPEC if a hero with same name is in the pool
    [HarmonyPatch(typeof(CharacterStageIdentityDefinitionPanel), "CanProceedToNextStage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageIdentityDefinitionPanel_UpdateRelevance
    {
        internal static void Postfix(CharacterStageIdentityDefinitionPanel __instance, ref bool __result)
        {
            if (Main.Settings.EnableRespec && Functors.FunctorRespec.IsRespecing && !string.IsNullOrEmpty(__instance.CharacterBuildingService.HeroCharacter.Name))
            {
                __result = true;
            }
        }
    }
}
