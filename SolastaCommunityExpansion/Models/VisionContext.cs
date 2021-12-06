using System.Linq;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionSenses;

﻿namespace SolastaCommunityExpansion.Models
{
    internal static class VisionContext
    {
        internal static void Load()
        {
            if (Main.Settings.DisableSenseDarkVisionFromAllRaces)
            {
                foreach (var featureUnlocks in DatabaseRepository.GetDatabase<CharacterRaceDefinition>().Select(crd => crd.FeatureUnlocks))
                {
                    featureUnlocks.RemoveAll(x => x.FeatureDefinition.name == "SenseDarkvision");
                    // Half-orcs have a different darkvisition.
                    featureUnlocks.RemoveAll(x => x.FeatureDefinition.name == "SenseDarkvision12");
                }
            }

            if (Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces)
            {
                foreach (CharacterRaceDefinition characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>())
                {
                    characterRaceDefinition.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition.name == "SenseSuperiorDarkvision");
                }
            }

            if (Main.Settings.IncreaseNormalVisionSenseRange)
            {
                SenseNormalVision.SetSenseRange(120);
            }
        }
    }
}
