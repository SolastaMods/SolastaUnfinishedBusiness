using System;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

internal abstract class
    FeatureDefinitionPointPoolBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatureDefinitionPointPool
    where TBuilder : FeatureDefinitionPointPoolBuilder<TDefinition, TBuilder>
{
#if false
    internal TBuilder Configure(HeroDefinitions.PointsPoolType poolType, int poolAmount,
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

    internal TBuilder SetPool(HeroDefinitions.PointsPoolType poolType, int poolAmount)
    {
        Definition.poolType = poolType;
        Definition.poolAmount = poolAmount;
        return This();
    }

    internal TBuilder RestrictChoices(params string[] choices)
    {
        Definition.RestrictedChoices.AddRange(choices);
        Definition.RestrictedChoices.Sort();
        return This();
    }


    internal TBuilder RestrictChoices(params ToolTypeDefinition[] choices)
    {
        Definition.RestrictedChoices.AddRange(choices.Select(t => t.Name));
        Definition.RestrictedChoices.Sort();
        return This();
    }

    internal TBuilder OnlyUniqueChoices()
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

    internal FeatureDefinitionPointPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionPointPoolBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    internal FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    internal FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
