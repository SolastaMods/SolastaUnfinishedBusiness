using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionCraftingAffinityBuilder
        : FeatureDefinitionAffinityBuilder<FeatureDefinitionCraftingAffinity, FeatureDefinitionCraftingAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionCraftingAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionCraftingAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionCraftingAffinityBuilder(FeatureDefinitionCraftingAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionCraftingAffinityBuilder(FeatureDefinitionCraftingAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
