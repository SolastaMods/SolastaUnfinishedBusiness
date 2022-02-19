using System;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
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

    public abstract class FeatureDefinitionCustomCodeBuilder<TDefinition, TBuilder> : FeatureDefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : FeatureDefinitionCustomCode
        where TBuilder : FeatureDefinitionCustomCodeBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected FeatureDefinitionCustomCodeBuilder(TDefinition original) : base(original)
        {
        }

        protected FeatureDefinitionCustomCodeBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionCustomCodeBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionCustomCodeBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionCustomCodeBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionCustomCodeBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionCustomCodeBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
