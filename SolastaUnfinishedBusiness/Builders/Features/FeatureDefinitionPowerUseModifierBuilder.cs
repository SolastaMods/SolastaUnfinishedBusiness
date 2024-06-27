using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Behaviors;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionPowerUseModifierBuilder
    : FeatureDefinitionBuilder<FeatureDefinitionPowerUseModifier, FeatureDefinitionPowerUseModifierBuilder>
{
    internal FeatureDefinitionPowerUseModifierBuilder SetFixedValue(
        FeatureDefinitionPower poolPower,
        int powerPoolModifier)
    {
        // ReSharper disable once InvocationIsSkipped
        PreConditions.ArgumentIsNotNull(poolPower, $"{GetType().Name}[{Definition.Name}] poolPower is null.");

        var modifier = Definition.Modifier;

        modifier.PowerPool = poolPower;
        modifier.Type = PowerPoolBonusCalculationType.Fixed;
        modifier.Value = powerPoolModifier;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionPowerUseModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPowerUseModifierBuilder(FeatureDefinitionPowerUseModifier original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
