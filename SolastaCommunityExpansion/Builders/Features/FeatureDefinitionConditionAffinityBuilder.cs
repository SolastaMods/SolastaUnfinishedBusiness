using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionConditionAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionConditionAffinity>
    {
        public FeatureDefinitionConditionAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionConditionAffinityBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity original, string name, string guid,
            GuiPresentation guiPresentation) : base(original, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public static FeatureDefinitionConditionAffinity CreateAndAddToDB(FeatureDefinitionConditionAffinity toCopy, string name, string guid, GuiPresentation guiPresentation)
        {
            return new FeatureDefinitionConditionAffinityBuilder(toCopy, name, guid, guiPresentation).AddToDB();
        }
    }
}
