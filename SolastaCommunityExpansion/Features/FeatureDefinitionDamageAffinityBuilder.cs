using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionDamageAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionDamageAffinity>
    {
        public FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity toCopy, string name, string guid,
            GuiPresentation guiPresentation) : base(toCopy, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
