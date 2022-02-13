using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionBuilder<TDefinition> : DefinitionBuilder<TDefinition> where TDefinition : FeatureDefinition
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

        #region Factory methods (create definition)

        public static TDefinition Build(string name, string guid, string title, string description, Action<TDefinition> modifyDefinition = null)
        {
            var featureDefinitionBuilder = Create(name, guid).SetGuiPresentation(title, description);
            if (modifyDefinition != null)
            {
                featureDefinitionBuilder.Configure(modifyDefinition);
            }
            return featureDefinitionBuilder.AddToDB();
        }
        #endregion

        public FeatureDefinitionBuilder<TDefinition> Configure(Action<TDefinition> modifyDefinition)
        {
            Assert.IsNotNull(modifyDefinition);
            modifyDefinition.Invoke(Definition);
            return this;
        }
    }
}
