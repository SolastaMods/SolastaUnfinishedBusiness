using System;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionPointPoolBuilder : FeatureDefinitionBuilder<FeatureDefinitionPointPool,
    FeatureDefinitionPointPoolBuilder>
{
#if false
    internal FeatureDefinitionPointPoolBuilder Configure(HeroDefinitions.PointsPoolType poolType, int poolAmount,
        bool uniqueChoices, params string[] choices)
    {
        Definition.poolType = poolType;
        Definition.poolAmount = poolAmount;
        Definition.RestrictedChoices.AddRange(choices);
        Definition.uniqueChoices = uniqueChoices;
        Definition.RestrictedChoices.Sort();

        return this;
    }
#endif

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


    internal FeatureDefinitionPointPoolBuilder RestrictChoices(params ToolTypeDefinition[] choices)
    {
        Definition.RestrictedChoices.AddRange(choices.Select(t => t.Name));
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
