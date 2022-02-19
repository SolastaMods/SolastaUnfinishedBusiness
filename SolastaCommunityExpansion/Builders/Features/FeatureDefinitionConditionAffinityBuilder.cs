using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionConditionAffinityBuilder : FeatureDefinitionAffinityBuilder<FeatureDefinitionConditionAffinity, FeatureDefinitionConditionAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity original) : base(original)
        {
        }

        protected FeatureDefinitionConditionAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionConditionAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionConditionAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public static FeatureDefinitionConditionAffinityBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionConditionAffinityBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionConditionAffinityBuilder Create(FeatureDefinitionConditionAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionConditionAffinityBuilder(original, name, namespaceGuid);
        }
    }
}
