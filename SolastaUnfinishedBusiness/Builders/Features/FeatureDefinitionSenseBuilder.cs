using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class
    FeatureDefinitionSenseBuilder : FeatureDefinitionBuilder<FeatureDefinitionSense, FeatureDefinitionSenseBuilder>
{
    internal FeatureDefinitionSenseBuilder SetSenseRange(int senseRange)
    {
        Definition.senseRange = senseRange;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionSenseBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionSenseBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionSenseBuilder(FeatureDefinitionSense original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionSenseBuilder(FeatureDefinitionSense original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}
