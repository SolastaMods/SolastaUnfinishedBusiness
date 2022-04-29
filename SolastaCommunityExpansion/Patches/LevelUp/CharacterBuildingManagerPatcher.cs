//using HarmonyLib;
//using SolastaCommunityExpansion.CustomDefinitions;

//namespace SolastaCommunityExpansion.Patches.LevelUp
//{
//    [HarmonyPatch(typeof(CharacterBuildingManager), "FinalizeCharacter")]
//    internal static class CharacterBuildingManagerFinalizeCharacter
//    {
//        internal static void Prefix(RulesetCharacterHero hero)
//        {
//            // remove the feature from the Replace drop down
//            foreach (var featureDescriptionItem in FeatureDescriptionItemPatcher.FeatureDescriptionItems.Keys)
//            {
//                if (featureDescriptionItem.Feature.Name.EndsWith("Replace"))
//                {
//                    var feature = featureDescriptionItem.GetCurrentFeature();

//                    foreach (var activeFeature in hero.ActiveFeatures)
//                    {
//                        activeFeature.Value.RemoveAll(x => x == feature);
//                    }

//                    if (feature is FeatureDefinitionFreeBonusCantrips)
//                    {
//                        var db = DatabaseRepository.GetDatabase<SpellDefinition>();
//                        var cantripToRemove = db.GetElement(feature.Name.Replace("BonusCantrip",""));

//                        foreach (var spellRepertoire in hero.SpellRepertoires)
//                        {
//                            spellRepertoire.KnownCantrips.Remove(cantripToRemove);
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
