using System;
using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionBuilder<TDefinition> : BaseDefinitionBuilder<TDefinition> where TDefinition : FeatureDefinition
    {
        public FeatureDefinitionBuilder(string name, string guid, Action<TDefinition> modifyDefinition = null) : base(name, guid)
        {
            modifyDefinition?.Invoke(Definition);
        }

        public FeatureDefinitionBuilder(string name, string guid, string title, string description, Action<TDefinition> modifyDefinition = null) : base(name, guid)
        {
            var guiPresentationBuilder = new GuiPresentationBuilder(title, description);

            Definition.SetGuiPresentation(guiPresentationBuilder.Build());

            modifyDefinition?.Invoke(Definition);
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
