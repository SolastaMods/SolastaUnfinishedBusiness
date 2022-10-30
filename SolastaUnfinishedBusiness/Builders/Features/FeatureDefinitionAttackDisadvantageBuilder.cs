using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
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
