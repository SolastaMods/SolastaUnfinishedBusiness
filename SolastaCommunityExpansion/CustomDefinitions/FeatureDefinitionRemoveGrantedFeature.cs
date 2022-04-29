namespace SolastaCommunityExpansion.CustomDefinitions
{
    //
    // FeatureDefinitionRemoveGrantedFeature allows you to remove a previously granted feature from a hero character
    // It can be useful when defining subclass progressions that require 1 or more features from the base class to be removed or implemented in a different way (i.e: Rogue Thug)
    //
    // As features can be granted by classes or subclasses progression on a specific level we can use the builder in 2 ways:
    // 
    // Replace a class feature - We need to inform the feature to be removed, the level and the class
    // Replace a subclass feature - We need to inform the feature to be removed, the level, the class and the subclass
    //
    public class FeatureDefinitionRemoveGrantedFeature : FeatureDefinition, IFeatureDefinitionCustomCode
    {
        public int ClassLevel { get; set; }
        public FeatureDefinition FeatureToRemove { get; set; }
        public CharacterClassDefinition CharacterClass { get; set; }
        public CharacterSubclassDefinition CharacterSubclass { get; set; }
        private string Tag => CharacterSubclass == null ? AttributeDefinitions.GetClassTag(CharacterClass, ClassLevel) : AttributeDefinitions.GetSubclassTag(CharacterClass, ClassLevel, CharacterSubclass);

        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            var activeFeatures = hero.ActiveFeatures;

            if (activeFeatures.TryGetValue(Tag, out var featureDefinitions) && featureDefinitions.Contains(FeatureToRemove))
            {
                featureDefinitions.Remove(FeatureToRemove);
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            var activeFeatures = hero.ActiveFeatures;

            if (activeFeatures.TryGetValue(Tag, out var featureDefinitions) && !featureDefinitions.Contains(FeatureToRemove))
            {
                featureDefinitions.Add(FeatureToRemove);
            }
        }
    }
}
