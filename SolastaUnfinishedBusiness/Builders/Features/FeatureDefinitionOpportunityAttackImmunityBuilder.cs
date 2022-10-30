using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
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
