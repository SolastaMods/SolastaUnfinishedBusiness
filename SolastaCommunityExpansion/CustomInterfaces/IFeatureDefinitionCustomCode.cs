namespace SolastaCommunityExpansion.CustomInterfaces
{
    public interface IFeatureDefinitionCustomCode
    {
        // Use this to add the feature to the character.
        public void ApplyFeature(RulesetCharacterHero hero, string tag);

        // Use this to remove the feature from the character. In particular this is used to allow level down functionality.
        public void RemoveFeature(RulesetCharacterHero hero, string tag);
    }
}
