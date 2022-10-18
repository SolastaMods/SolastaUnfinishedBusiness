using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class
    FeatureDefinitionSenseBuilder : DefinitionBuilder<FeatureDefinitionSense, FeatureDefinitionSenseBuilder>
{
    internal FeatureDefinitionSenseBuilder SetSense(SenseMode.Type type, int senseRange, int stealthBreakerRange = 0)
    {
        Definition.senseType = type;
        Definition.senseRange = senseRange;
        Definition.stealthBreakerRange = stealthBreakerRange;
        return this;
    }

    #region Constructors

    internal FeatureDefinitionSenseBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionSenseBuilder(FeatureDefinitionSense original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    #endregion
}
