using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionFeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {

        public FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet toCopy, string name, string guid,
           GuiPresentation guiPresentation) : base(toCopy, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionFeatureSetBuilder ClearFeatures()
        {
            Definition.FeatureSet.Clear();
            return this;
        }

        public FeatureDefinitionFeatureSetBuilder AddFeature(FeatureDefinition featureDefinition)
        {

            Definition.FeatureSet.Add(featureDefinition);
            return this;
        }

        public FeatureDefinitionFeatureSetBuilder SetMode(FeatureDefinitionFeatureSet.FeatureSetMode mode)
        {

            Definition.SetMode(mode);
            return this;
        }

        public FeatureDefinitionFeatureSetBuilder SetUniqueChoices(bool uniqueChoice)
        {

            Definition.SetUniqueChoices(uniqueChoice);
            return this;
        }

    }
}
