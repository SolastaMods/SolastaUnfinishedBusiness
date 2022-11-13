using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionEquipmentAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionEquipmentAffinity, FeatureDefinitionEquipmentAffinityBuilder>
{
    internal FeatureDefinitionEquipmentAffinityBuilder SetAdditionalCarryingCapacity(int value)
    {
        Definition.additionalCarryingCapacity = value;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionEquipmentAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionEquipmentAffinityBuilder(FeatureDefinitionEquipmentAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
