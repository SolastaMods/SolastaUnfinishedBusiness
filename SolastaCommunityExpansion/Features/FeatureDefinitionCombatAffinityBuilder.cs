using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionCombatAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionCombatAffinity>
    {
        public FeatureDefinitionCombatAffinityBuilder(FeatureDefinitionCombatAffinity toCopy, string name, string guid,
            GuiPresentation guiPresentation) : base(toCopy, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

    }
}
