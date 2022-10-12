using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

// note if you want to use a modifier for the power pool later you should set uses determination to fixed or ability bonus plus fixed
[UsedImplicitly]
internal class FeatureDefinitionPowerPoolBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPower,
    FeatureDefinitionPowerPoolBuilder>
{
    #region Constructors

    protected FeatureDefinitionPowerPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
        Definition.overriddenPower = Definition;
    }

    protected FeatureDefinitionPowerPoolBuilder(FeatureDefinitionPower original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
        Definition.overriddenPower = Definition;
    }

    #endregion
}
