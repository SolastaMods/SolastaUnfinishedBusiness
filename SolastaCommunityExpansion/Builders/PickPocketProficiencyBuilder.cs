using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    // TODO: merge with FeatureDefinitionProficiencyBuilder
    public class PickPocketProficiencyBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        protected PickPocketProficiencyBuilder(string name, string guid, string title, string description, FeatureDefinitionProficiency base_proficiency) : base(base_proficiency, name, guid)
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

        public static FeatureDefinitionProficiency CreateCopyFrom(string name, string guid, string title, string description, FeatureDefinitionProficiency base_proficiency)
        {
            return new PickPocketProficiencyBuilder(name, guid, title, description, base_proficiency).AddToDB();
        }
    }
}
