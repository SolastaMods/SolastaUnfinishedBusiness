using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionEquipmentAffinityBuilder
        : FeatureDefinitionAffinityBuilder<FeatureDefinitionEquipmentAffinity, FeatureDefinitionEquipmentAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionEquipmentAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionEquipmentAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionEquipmentAffinityBuilder(FeatureDefinitionEquipmentAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionEquipmentAffinityBuilder(FeatureDefinitionEquipmentAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionEquipmentAffinityBuilder SetCarryingCapacityMultiplier(float carryingCapacityMultiplier, float? additionalCarryingCapacity)
        {
            Definition.SetCarryingCapacityMultiplier(carryingCapacityMultiplier);

            if (additionalCarryingCapacity != null)
            {
                Definition.SetAdditionalCarryingCapacity(additionalCarryingCapacity.Value);
            }

            return This();
        }
    }
}
