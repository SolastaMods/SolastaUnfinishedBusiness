using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionEquipmentAffinityBuilder
        : FeatureDefinitionAffinityBuilder<FeatureDefinitionEquipmentAffinity, FeatureDefinitionEquipmentAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionEquipmentAffinityBuilder(FeatureDefinitionEquipmentAffinity original) : base(original)
        {
        }

        protected FeatureDefinitionEquipmentAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionEquipmentAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionEquipmentAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionEquipmentAffinityBuilder(FeatureDefinitionEquipmentAffinity original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionEquipmentAffinityBuilder(FeatureDefinitionEquipmentAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionEquipmentAffinityBuilder(FeatureDefinitionEquipmentAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
