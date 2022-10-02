using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionSummoningAffinityBuilder
    : FeatureDefinitionAffinityBuilder<FeatureDefinitionSummoningAffinity,
        FeatureDefinitionSummoningAffinityBuilder>
{
    internal FeatureDefinitionSummoningAffinityBuilder ClearEffectForms()
    {
        Definition.EffectForms.Clear();
        return this;
    }

    internal FeatureDefinitionSummoningAffinityBuilder SetRequiredMonsterTag(string tag)
    {
        Definition.requiredMonsterTag = tag;
        return this;
    }

    internal FeatureDefinitionSummoningAffinityBuilder SetAddedConditions(params ConditionDefinition[] value)
    {
        Definition.AddedConditions.SetRange(value);
        Definition.AddedConditions.Sort(Sorting.Compare);
        return this;
    }

    #region Constructors

    protected FeatureDefinitionSummoningAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionSummoningAffinityBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
