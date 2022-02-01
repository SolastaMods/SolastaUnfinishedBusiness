using System;
using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionBuilder<TDefinition> : BaseDefinitionBuilder<TDefinition> where TDefinition : FeatureDefinition
    {
        public FeatureDefinitionBuilder(string name, string guid, Action<TDefinition> modifyDefinition = null) : base(name, guid)
        {
            modifyDefinition?.Invoke(Definition);
        }

        public FeatureDefinitionBuilder(string name, string guid, string description, string title, Action<TDefinition> modifyDefinition = null) : base(name, guid)
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

        public static TDefinition Build(string name, string guid, string description, string title, Action<TDefinition> modifyDefinition = null)
        {
            var featureDefinitionBuilder = new FeatureDefinitionBuilder<TDefinition>(name, guid, description, title, modifyDefinition);

            return featureDefinitionBuilder.AddToDB();
        }
    }
}
