using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.RecursiveGrantCustomFeatures
{
    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        /**
         * When a character is being granted features, this patch will apply the effect of custom features.
         */

        internal static void Postfix(RulesetCharacterHero hero, List<FeatureDefinition> grantedFeatures, bool clearPrevious = true)
        {
            if (!clearPrevious)
            {
                Models.CustomFeaturesContext.RecursiveGrantCustomFeatures(hero, grantedFeatures);
            }
            else
            {
                //
                // TODO: Move this to CustomFeaturesContext and cover all undo cases there...
                //

                var heroBuildingData = hero.GetHeroBuildingData();

                foreach (var feature in heroBuildingData.AllActiveFeatures
                    .OfType<FeatureDefinitionCustomCode>())
                {
                    feature.RemoveFeature(hero);
                }
            }
        }
    }
}
