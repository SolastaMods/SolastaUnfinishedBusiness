using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionMovementAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionMovementAffinity, FeatureDefinitionMovementAffinityBuilder>
{
    internal FeatureDefinitionMovementAffinityBuilder SetAdditiveModifierAdvancement(
        RuleDefinitions.MovementAffinityAdvancement value)
    {
        Definition.additiveModifierAdvancement = value;
        return This();
    }

    internal FeatureDefinitionMovementAffinityBuilder SetBaseSpeedAdditiveModifier(int value)
    {
        Definition.baseSpeedAdditiveModifier = value;
        return This();
    }

    internal FeatureDefinitionMovementAffinityBuilder SetSituationalContext(ExtraSituationalContext situationalContext)
    {
        Definition.situationalContext = (RuleDefinitions.SituationalContext)situationalContext;
        return This();
    }

    internal FeatureDefinitionMovementAffinityBuilder SetBaseSpeedMultiplicativeModifier(float value)
    {
        Definition.baseSpeedMultiplicativeModifier = value;
        return This();
    }

    #region Constructors

    protected FeatureDefinitionMovementAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionMovementAffinityBuilder(FeatureDefinitionMovementAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
