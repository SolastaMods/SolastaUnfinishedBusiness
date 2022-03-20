namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    /// <summary>
    /// Implement on a FeatureDefinition and add it to Feat to be able to enforce custom validations on Feat prerequisites
    /// </summary>
    public interface IValidateFeatPrerequisites
    {
        // typo on purpose to match typo on official game
        public bool IsFeatMacthingPrerequisites(
          FeatDefinition feat,
          RulesetCharacterHero hero,
          ref string prerequisiteOutput);
    }
}
