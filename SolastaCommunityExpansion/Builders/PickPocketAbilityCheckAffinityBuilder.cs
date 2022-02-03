using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    // TODO: merge with FeatureDefinitionAbilityCheckAffinity
    public class PickPocketAbilityCheckAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
    {
        protected PickPocketAbilityCheckAffinityBuilder(FeatureDefinitionAbilityCheckAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public static FeatureDefinitionAbilityCheckAffinity CreateCopyFrom(
            FeatureDefinitionAbilityCheckAffinity original, string name, string guid)
        {
            return new PickPocketAbilityCheckAffinityBuilder(original, name, guid).AddToDB();
        }

        public static FeatureDefinitionAbilityCheckAffinity CreateCopyFrom(
            FeatureDefinitionAbilityCheckAffinity original, string name, string guid, string title, string description)
        {
            return new PickPocketAbilityCheckAffinityBuilder(original, name, guid).SetGuiPresentation(title, description).AddToDB();
        }
    }
}
