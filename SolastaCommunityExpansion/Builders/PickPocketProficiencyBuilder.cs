using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    // TODO: merge with FeatureDefinitionProficiencyBuilder
    public class PickPocketProficiencyBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        protected PickPocketProficiencyBuilder(FeatureDefinitionProficiency original, string name, string guid) : base(original, name, guid)
        {
        }

        public static FeatureDefinitionProficiency CreateCopyFrom(
            FeatureDefinitionProficiency original, string name, string guid)
        {
            return new PickPocketProficiencyBuilder(original, name, guid).AddToDB();
        }

        public static FeatureDefinitionProficiency CreateCopyFrom(
            FeatureDefinitionProficiency original, string name, string guid, string title, string description)
        {
            return new PickPocketProficiencyBuilder(original, name, guid).SetGuiPresentation(title, description).AddToDB();
        }
    }
}
