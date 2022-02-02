using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    // TODO: merge with FeatDefinitionBuilder
    public class PickPocketFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
    {
        protected PickPocketFeatBuilder(string name, string guid, string title, string description, FeatDefinition base_Feat) : base(base_Feat, name, guid)
        {
            if (title != "")
            {
                Definition.GuiPresentation.Title = title;
            }
            if (description != "")
            {
                Definition.GuiPresentation.Description = description;
            }
        }
        public static FeatDefinition CreateCopyFrom(string name, string guid, string title, string description, FeatDefinition base_Feat)
        {
            return new PickPocketFeatBuilder(name, guid, title, description, base_Feat).AddToDB();
        }
    }
}
