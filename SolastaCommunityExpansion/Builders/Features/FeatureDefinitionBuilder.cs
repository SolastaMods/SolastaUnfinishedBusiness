using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionBuilder<TDefinition> : DefinitionBuilder<TDefinition> where TDefinition : FeatureDefinition
    {
        #region Standard constructors
        private FeatureDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        private FeatureDefinitionBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        private FeatureDefinitionBuilder(TDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        private FeatureDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }
        #endregion

        #region Factory methods (create builder)
        public static FeatureDefinitionBuilder<TDefinition> Create(string name, string guid)
        {
            return new FeatureDefinitionBuilder<TDefinition>(name, guid);
        }

        public static FeatureDefinitionBuilder<TDefinition> Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionBuilder<TDefinition>(name, namespaceGuid);
        }

        public static FeatureDefinitionBuilder<TDefinition> Create(TDefinition original, string name, string guid)
        {
            return new FeatureDefinitionBuilder<TDefinition>(original, name, guid);
        }

        public static FeatureDefinitionBuilder<TDefinition> Create(TDefinition original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionBuilder<TDefinition>(original, name, namespaceGuid);
        }
        #endregion
    }
}
