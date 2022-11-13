using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionPointPoolBuilder
    : DefinitionBuilder<FeatureDefinitionPointPool, FeatureDefinitionPointPoolBuilder>
{
    internal FeatureDefinitionPointPoolBuilder SetPool(HeroDefinitions.PointsPoolType poolType, int poolAmount)
    {
        Definition.poolType = poolType;
        Definition.poolAmount = poolAmount;
        return this;
    }

    internal FeatureDefinitionPointPoolBuilder RestrictChoices(params string[] choices)
    {
        Definition.RestrictedChoices.AddRange(choices);
        Definition.RestrictedChoices.Sort();
        return this;
    }

    internal FeatureDefinitionPointPoolBuilder OnlyUniqueChoices()
    {
        Definition.uniqueChoices = true;
        return this;
    }

    #region Constructors

    internal FeatureDefinitionPointPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    #endregion
}
