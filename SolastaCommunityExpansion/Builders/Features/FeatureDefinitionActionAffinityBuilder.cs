using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionActionAffinityBuilder : FeatureDefinitionBuilder<FeatureDefinitionActionAffinity, FeatureDefinitionActionAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original) : base(original)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public static FeatureDefinitionActionAffinityBuilder Create(FeatureDefinitionActionAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionActionAffinityBuilder(original, name, namespaceGuid);
        }
    }
}
