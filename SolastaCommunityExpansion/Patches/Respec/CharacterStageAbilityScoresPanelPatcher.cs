using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Respec
{
    // use this patch to avoid issues during RESPEC if a hero with same name is in the pool
    [HarmonyPatch(typeof(CharacterStageIdentityDefinitionPanel), "CanProceedToNextStage")]
    internal static class CharacterStageIdentityDefinitionPanel_CanProceedToNextStage
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
