//using HarmonyLib;

//namespace SolastaDungeonMakerPro.Patches.EncountersSpawn
//{
//    // this patch allows an away party member to die without triggering a Game Over
//    class GameLocationCharacterPatcher
//    {
//        [HarmonyPatch(typeof(GameLocationCharacter), "IsCriticalCharacter")]
//        internal static class GameLocationCharacterIsCriticalCharacter
//        {
//            internal static void Postfix(ref bool __result, GameLocationCharacter __instance)
//            {
//                if (Models.EncountersSpawnContext.HasStagedHeroes)
//                {
//                    var gameLocationService = ServiceRepository.GetService<IGameLocationService>();

//                    if (gameLocationService?.GameLocation?.LocationDefinition?.IsUserLocation == true)
//                    {
//                        __instance.RulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionCriticalCharacter>(__instance.RulesetCharacter.FeaturesToBrowse);
//                        __result = !__instance.RulesetCharacter.FeaturesToBrowse.Empty();
//                    }
//                }
//            }
//        }
//    }
//}