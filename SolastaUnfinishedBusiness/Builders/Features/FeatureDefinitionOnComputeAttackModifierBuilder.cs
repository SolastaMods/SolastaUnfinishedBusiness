using System;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Builders.Features;

public class FeatureDefinitionOnComputeAttackModifierBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionOnComputeAttackModifier,
    FeatureDefinitionOnComputeAttackModifierBuilder>
{
    public FeatureDefinitionOnComputeAttackModifierBuilder SetOnComputeAttackModifierDelegate(
        OnComputeAttackModifier del)
    {
        Definition.SetOnComputeAttackModifierDelegate(del);
        return this;
    }

    #region Constructors

    protected FeatureDefinitionOnComputeAttackModifierBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionOnComputeAttackModifierBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionOnComputeAttackModifierBuilder(FeatureDefinitionOnComputeAttackModifier original,
        string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionOnComputeAttackModifierBuilder(FeatureDefinitionOnComputeAttackModifier original,
        string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
