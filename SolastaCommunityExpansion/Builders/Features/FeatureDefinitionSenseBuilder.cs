using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionSenseBuilder : FeatureDefinitionBuilder<FeatureDefinitionSense, FeatureDefinitionSenseBuilder>
    {
        #region Constructors

        protected FeatureDefinitionSenseBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSenseBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionSenseBuilder(FeatureDefinitionSense original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSenseBuilder(FeatureDefinitionSense original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
