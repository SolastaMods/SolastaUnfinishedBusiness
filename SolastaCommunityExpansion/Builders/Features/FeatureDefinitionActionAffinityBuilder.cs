using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionActionAffinityBuilder : DefinitionBuilder<FeatureDefinitionActionAffinity, FeatureDefinitionActionAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionActionAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }
        #endregion

        public static FeatureDefinitionActionAffinityBuilder Create(FeatureDefinitionActionAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionActionAffinityBuilder(original, name, namespaceGuid);
        }
    }
}
