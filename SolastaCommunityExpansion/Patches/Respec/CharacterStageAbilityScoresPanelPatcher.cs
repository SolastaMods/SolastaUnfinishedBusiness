using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Respec
{
    // use this patch to avoid issues during RESPEC if a hero with same name is in the pool
    [HarmonyPatch(typeof(CharacterStageIdentityDefinitionPanel), "CanProceedToNextStage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageIdentityDefinitionPanel_CanProceedToNextStage
    {
        internal static void Postfix(RulesetCharacterHero ___currentHero, ref bool __result)
        {
            if (Main.Settings.EnableRespec && Models.RespecContext.FunctorRespec.IsRespecing && !string.IsNullOrEmpty(___currentHero.Name))
            {
                __result = true;
            }
        }
    }
}
