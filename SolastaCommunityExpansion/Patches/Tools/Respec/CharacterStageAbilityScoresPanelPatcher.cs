using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Tools.Respec;

// use this patch to avoid issues during RESPEC if a hero with same name is in the pool
[HarmonyPatch(typeof(CharacterStageIdentityDefinitionPanel), "CanProceedToNextStage")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageIdentityDefinitionPanel_CanProceedToNextStage
{
    internal static void Postfix(CharacterStageIdentityDefinitionPanel __instance, ref bool __result)
    {
        if (Main.Settings.EnableRespec && RespecContext.FunctorRespec.IsRespecing &&
            !string.IsNullOrEmpty(__instance.currentHero.Name))
        {
            __result = true;
        }
    }
}
