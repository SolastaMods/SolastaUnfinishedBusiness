using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    // TODO: merge with FeatDefinitionBuilder
    public class PickPocketFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
    {
        protected PickPocketFeatBuilder(FeatDefinition original, string name, string guid) : base(original, name, guid)
        {
        }

        public static FeatDefinition CreateCopyFrom(FeatDefinition original, string name, string guid)
        {
            return new PickPocketFeatBuilder(original, name, guid).AddToDB();
        }

        public static FeatDefinition CreateCopyFrom(FeatDefinition original, string name, string guid, string title, string description)
        {
            return new PickPocketFeatBuilder(original, name, guid).SetGuiPresentation(title, description).AddToDB();
        }
    }
}
