using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionSummoningAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionSummoningAffinity,
        FeatureDefinitionSummoningAffinityBuilder>
{
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

    protected FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
