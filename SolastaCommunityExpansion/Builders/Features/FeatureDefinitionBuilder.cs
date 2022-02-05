using System;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionBuilder<TDefinition> : BaseDefinitionBuilder<TDefinition> where TDefinition : FeatureDefinition
    {
        public FeatureDefinitionBuilder(string name, string guid, Action<TDefinition> modifyDefinition) : base(name, guid)
        {
            modifyDefinition?.Invoke(Definition);
        }

        public FeatureDefinitionBuilder(string name, string guid, string title, string description, Action<TDefinition> modifyDefinition = null) : base(name, guid)
        {
            Definition.SetGuiPresentation(title, description);

            modifyDefinition?.Invoke(Definition);
        }

        public FeatureDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionBuilder(TDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        public static TDefinition Build(string name, string guid, Action<TDefinition> modifyDefinition = null)
        {
            var featureDefinitionBuilder = new FeatureDefinitionBuilder<TDefinition>(name, guid, modifyDefinition);

            return featureDefinitionBuilder.AddToDB();
        }

        public static TDefinition Build(string name, string guid, string title, string description, Action<TDefinition> modifyDefinition = null)
        {
            var featureDefinitionBuilder = new FeatureDefinitionBuilder<TDefinition>(name, guid, title, description, modifyDefinition);

            return featureDefinitionBuilder.AddToDB();
        }
    }
}
