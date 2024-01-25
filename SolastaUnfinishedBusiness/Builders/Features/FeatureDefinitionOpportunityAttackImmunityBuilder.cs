using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Definitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionOpportunityAttackImmunityBuilder
    : DefinitionBuilder<FeatureDefinitionOpportunityAttackImmunity, FeatureDefinitionOpportunityAttackImmunityBuilder>
{
    protected FeatureDefinitionOpportunityAttackImmunityBuilder(string name, Guid namespaceGuid)
        : base(name, namespaceGuid)
    {
    }

    [NotNull]
    internal FeatureDefinitionOpportunityAttackImmunityBuilder SetConditionName(string conditionName)
    {
        Definition.ConditionName = conditionName;
        return this;
    }
}
