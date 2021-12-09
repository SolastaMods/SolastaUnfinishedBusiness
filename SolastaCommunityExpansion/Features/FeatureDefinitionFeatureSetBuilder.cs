using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using SolastaCommunityExpansion.Helpers;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionFeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        public FeatureDefinitionFeatureSetBuilder(string name, string guid, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet toCopy, string name, string guid,
           GuiPresentation guiPresentation) : base(toCopy, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }
        public FeatureDefinitionFeatureSetBuilder SetFeature(FeatureDefinition featureDefinition)
        {
            
            // IMPROVE THIS AND SEARCH FOR FEATURESET TYPE... THERE IS NO SETTER FOR FEATURESET
            Definition.FeatureSet[0] = featureDefinition;
            return this;
        }

    }
}
