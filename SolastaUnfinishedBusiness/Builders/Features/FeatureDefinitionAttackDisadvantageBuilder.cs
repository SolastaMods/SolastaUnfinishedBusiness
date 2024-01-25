using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Definitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAttackDisadvantageBuilder
    : DefinitionBuilder<FeatureDefinitionAttackDisadvantage, FeatureDefinitionAttackDisadvantageBuilder>
{
    protected FeatureDefinitionAttackDisadvantageBuilder(string name, Guid namespaceGuid)
        : base(name, namespaceGuid)
    {
    }

    [NotNull]
    internal FeatureDefinitionAttackDisadvantageBuilder SetConditionName(string conditionName)
    {
        Definition.ConditionName = conditionName;
        return this;
    }
}
