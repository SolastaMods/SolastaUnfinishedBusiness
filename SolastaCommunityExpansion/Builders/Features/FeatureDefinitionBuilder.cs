using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public abstract class FeatureDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : FeatureDefinition
        where TBuilder : FeatureDefinitionBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected FeatureDefinitionBuilder(TDefinition original) : base(original)
        {
        }

        protected FeatureDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        // TODO: methods specific to FeatureDefinition
    }

    // Should this builder be available?  Only used in one place.
    public class FeatureDefinitionBuilder : FeatureDefinitionBuilder<FeatureDefinition, FeatureDefinitionBuilder>
    {
        #region Standard constructors
        protected FeatureDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        protected FeatureDefinitionBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionBuilder(FeatureDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        protected FeatureDefinitionBuilder(FeatureDefinition original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }
        #endregion

        #region Factory methods (create builder)
        public static FeatureDefinitionBuilder Create(string name, string guid)
        {
            return new FeatureDefinitionBuilder(name, guid);
        }

        public static FeatureDefinitionBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionBuilder Create(FeatureDefinition original, string name, string guid)
        {
            return new FeatureDefinitionBuilder(original, name, guid);
        }

        public static FeatureDefinitionBuilder Create(FeatureDefinition original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionBuilder(original, name, namespaceGuid);
        }
        #endregion
    }
}
