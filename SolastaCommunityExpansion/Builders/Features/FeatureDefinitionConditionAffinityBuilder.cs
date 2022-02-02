using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionConditionAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionConditionAffinity>
    {
        public FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity toCopy, string name, string guid,
            GuiPresentation guiPresentation) : base(toCopy, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public static FeatureDefinitionConditionAffinity CreateAndAddToDB(FeatureDefinitionConditionAffinity toCopy, string name, string guid, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionConditionAffinityBuilder(toCopy, name, guid, guiPresentation).AddToDB();
        }
    }
}
