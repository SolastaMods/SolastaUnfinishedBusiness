
namespace SolastaCommunityExpansion.Builders.Features
{
    /**
     * This adds the ability to do fully custom code when a feature is granted. Where possible you should use more targeted features,
     * but some things just need more flexibility.
     * 
     * Currently this supports features granted through Class, Subclass, Race, and Feats.
     */
    public abstract class FeatureDefinitionCustomCode : FeatureDefinition
    {
        // Use this to add the feature to the character.
        public abstract void ApplyFeature(RulesetCharacterHero hero);

        // Use this to remove the feature from the character. In particular this is used to allow level down functionality.
        public abstract void RemoveFeature(RulesetCharacterHero hero);
    }
}
