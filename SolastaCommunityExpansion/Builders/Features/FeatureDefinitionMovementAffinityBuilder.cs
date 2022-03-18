using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionMovementAffinityBuilder
        : FeatureDefinitionBuilder<FeatureDefinitionMovementAffinity, FeatureDefinitionMovementAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionMovementAffinityBuilder(FeatureDefinitionMovementAffinity original) : base(original)
        {
        }

        protected FeatureDefinitionMovementAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionMovementAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionMovementAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionMovementAffinityBuilder(FeatureDefinitionMovementAffinity original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionMovementAffinityBuilder(FeatureDefinitionMovementAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionMovementAffinityBuilder(FeatureDefinitionMovementAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
