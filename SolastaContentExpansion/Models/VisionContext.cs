namespace SolastaContentExpansion.Models
{
    internal static class VisionContext
    {
        internal static void Load()
        {
            if (Main.Settings.DisableSenseDarkVisionFromAllRaces)
            {
                foreach (CharacterRaceDefinition characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>().GetAllElements())
                {
                    characterRaceDefinition.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition.name == "SenseDarkvision");
                }
            }

            if (Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces)
            {
                foreach (CharacterRaceDefinition characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>().GetAllElements())
                {
                    characterRaceDefinition.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition.name == "SenseSuperiorDarkvision");
                }
            }
        }
    }
}
