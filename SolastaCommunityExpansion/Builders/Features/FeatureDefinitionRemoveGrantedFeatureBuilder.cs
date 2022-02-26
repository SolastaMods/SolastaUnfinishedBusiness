using System;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    //
    // RemoveGrantedFeature allows you to remove a previously granted feature from a hero character
    // It can be useful when defining subclass progressions that require 1 or more features from the base class to be removed or implemented in a different way (i.e: Rogue Thug)
    //
    // As features can be granted by classes or subclasses progression on a specific level we can use the builder in 2 ways:
    // 
    // Replace a class feature - We need to inform the feature to be removed, the level and the class
    // Replace a subclass feature - We need to inform the feature to be removed, the level, the class and the subclass
    //
    public sealed class FeatureDefinitionRemoveGrantedFeatureBuilder : FeatureDefinitionBuilder<FeatureDefinitionRemoveGrantedFeature, FeatureDefinitionRemoveGrantedFeatureBuilder>
    {
        private FeatureDefinitionRemoveGrantedFeatureBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid) { }

        public static FeatureDefinitionRemoveGrantedFeatureBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionRemoveGrantedFeatureBuilder(name, namespaceGuid);
        }

        public FeatureDefinitionRemoveGrantedFeatureBuilder SetFeatureInfo(FeatureDefinition featureToRemove, int classLevel, CharacterClassDefinition characterClass, CharacterSubclassDefinition characterSubclass = null)
        {
            Definition.ClassLevel = classLevel;
            Definition.FeatureToRemove = featureToRemove;
            Definition.CharacterClass = characterClass;
            Definition.CharacterSubclass = characterSubclass;

            return This();
        }
    }
}
