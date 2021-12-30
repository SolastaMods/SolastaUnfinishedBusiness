
namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    /**
     * This adds the ability to do fully custom EffectForms. If possible you should use the standard EffectForms.
     * Damage and healing done through this CustomEffectForm will not trigger the proper events.
     */
    public abstract class FeatureDefinitionCustomCode : FeatureDefinition
    {
        // Use this to add the feature to the character.
        public abstract void ApplyFeature(RulesetCharacterHero hero);

        // Use this to remove the feature from the character. In particular this is used to allow level down functionality.
        public abstract void RemoveFeature(RulesetCharacterHero hero);
    }
}
