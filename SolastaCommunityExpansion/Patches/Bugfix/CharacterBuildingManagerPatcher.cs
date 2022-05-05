using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.BugFix
{
    //Replacing this method completely to remove weird 'return'. TA confirmed they will remove it in next patch.
    [HarmonyPatch(typeof(CharacterBuildingManager), "BrowseGrantedFeaturesHierarchically")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_BrowseGrantedFeaturesHierarchically
    {
        internal static bool Prefix(CharacterBuildingManager __instance,
            CharacterHeroBuildingData heroBuildingData,
            List<FeatureDefinition> grantedFeatures,
            string tag)
        {
            if (!Main.Settings.BugFixBrowseFeatures)
            {
                return true;
            }

            foreach (FeatureDefinition grantedFeature in grantedFeatures)
            {
                switch (grantedFeature)
                {
                    case FeatureDefinitionCastSpell spell:
                        __instance.SetupSpellPointPools(heroBuildingData, spell, tag);
                        continue; // In original code this was 'return'
                    case FeatureDefinitionBonusCantrips cantrips:
                        foreach (var cantrip in cantrips.BonusCantrips)
                        {
                            __instance.AcquireBonusCantrip(heroBuildingData, cantrip, tag);
                        }
                        continue;
                    case FeatureDefinitionProficiency proficiency:
                        if (proficiency.ProficiencyType == RuleDefinitions.ProficiencyType.FightingStyle)
                        {
                            foreach (var name in proficiency.Proficiencies)
                            {
                                var style = DatabaseRepository.GetDatabase<FightingStyleDefinition>().GetElement(name);
                                __instance.AcquireBonusFightingStyle(heroBuildingData, style, tag);
                            }

                            continue;
                        }
                        else
                            continue;
                    case FeatureDefinitionFeatureSet featureSet:
                        if (featureSet.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        {
                            __instance.BrowseGrantedFeaturesHierarchically(heroBuildingData,
                                featureSet.FeatureSet, tag);
                        }
                        continue;
                    default:
                        continue;
                }
            }

            return false;
        }
    }
}
