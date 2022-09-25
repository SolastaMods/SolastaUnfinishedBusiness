using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionEquipmentAffinityBuilder
    : FeatureDefinitionAffinityBuilder<FeatureDefinitionEquipmentAffinity,
        FeatureDefinitionEquipmentAffinityBuilder>
{
    #region Constructors

    protected FeatureDefinitionEquipmentAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionEquipmentAffinityBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionEquipmentAffinityBuilder(FeatureDefinitionEquipmentAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionEquipmentAffinityBuilder(FeatureDefinitionEquipmentAffinity original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
