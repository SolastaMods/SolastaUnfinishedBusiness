using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    class NegativeFeature
    {
    }

    public class NegativeFeatureDefinition : FeatureDefinition
    {
        public string Tag;
        public FeatureDefinition FeatureToRemove;
    }

    public sealed class NegativeFeatureBuilder : BaseDefinitionBuilder<NegativeFeatureDefinition>
    {
        public NegativeFeatureBuilder(string name, string guid, string tag, FeatureDefinition featureToRemove)
            : base(name, guid)
        {
            Definition.Tag = tag;
            Definition.FeatureToRemove = featureToRemove;
            Definition.GuiPresentation.SetHidden(true);
        }
    }
}
