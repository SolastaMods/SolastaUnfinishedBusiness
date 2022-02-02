using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    // TODO: merge with FeatureDefinitionAbilityCheckAffinity
    public class PickPocketAbilityCheckAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
    {
        protected PickPocketAbilityCheckAffinityBuilder(string name, string guid, string title, string description, FeatureDefinitionAbilityCheckAffinity base_check_affinity) : base(base_check_affinity, name, guid)
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
        public static FeatureDefinitionAbilityCheckAffinity CreateCopyFrom(string name, string guid, string title, string description, FeatureDefinitionAbilityCheckAffinity base_check_affinity)
        {
            return new PickPocketAbilityCheckAffinityBuilder(name, guid, title, description, base_check_affinity).AddToDB();
        }
    }
}
