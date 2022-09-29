using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

public abstract class
    FeatureDefinitionPointPoolBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatureDefinitionPointPool
    where TBuilder : FeatureDefinitionPointPoolBuilder<TDefinition, TBuilder>
{
#if false
    public TBuilder Configure(HeroDefinitions.PointsPoolType poolType, int poolAmount,
        bool uniqueChoices, params string[] choices)
    {
        Definition.poolType = poolType;
        Definition.poolAmount = poolAmount;
        Definition.RestrictedChoices.AddRange(choices);
        Definition.uniqueChoices = uniqueChoices;
        Definition.RestrictedChoices.Sort();

        return This();
    }
#endif

    public TBuilder SetPool(HeroDefinitions.PointsPoolType poolType, int poolAmount)
    {
        Definition.poolType = poolType;
        Definition.poolAmount = poolAmount;
        return This();
    }

    public TBuilder RestrictChoices(params string[] choices)
    {
        Definition.RestrictedChoices.AddRange(choices);
        Definition.RestrictedChoices.Sort();
        return This();
    }

    public TBuilder OnlyUniqueChoices()
    {
        Definition.uniqueChoices = true;
        return This();
    }

    #region Constructors

    protected FeatureDefinitionPointPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPointPoolBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionPointPoolBuilder(TDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPointPoolBuilder(TDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}

[UsedImplicitly]
internal class FeatureDefinitionPointPoolBuilder : FeatureDefinitionPointPoolBuilder<FeatureDefinitionPointPool,
    FeatureDefinitionPointPoolBuilder>
{
    #region Constructors

    public FeatureDefinitionPointPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    public FeatureDefinitionPointPoolBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    public FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    public FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
