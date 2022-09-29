using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionMovementAffinityBuilder
    : FeatureDefinitionBuilder<FeatureDefinitionMovementAffinity, FeatureDefinitionMovementAffinityBuilder>
{
    public FeatureDefinitionMovementAffinityBuilder SetBaseSpeedMultiplicativeModifier(float value)
    {
        Definition.baseSpeedMultiplicativeModifier = value;
        return This();
    }
    #region Constructors

    protected FeatureDefinitionMovementAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionMovementAffinityBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionMovementAffinityBuilder(FeatureDefinitionMovementAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionMovementAffinityBuilder(FeatureDefinitionMovementAffinity original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
