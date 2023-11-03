using System;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionConditionAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionConditionAffinity, FeatureDefinitionConditionAffinityBuilder>
{
    internal FeatureDefinitionConditionAffinityBuilder SetConditionAffinityType(
        ConditionAffinityType value)
    {
        Definition.conditionAffinityType = value;
        return this;
    }

    internal FeatureDefinitionConditionAffinityBuilder SetConditionType(ConditionDefinition value)
    {
        Definition.conditionType = value.Name;
        return this;
    }

    internal FeatureDefinitionConditionAffinityBuilder SetSavingThrowAdvantageType(AdvantageType value)
    {
        Definition.savingThrowAdvantageType = value;
        return this;
    }


    #region Constructors

    protected FeatureDefinitionConditionAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
