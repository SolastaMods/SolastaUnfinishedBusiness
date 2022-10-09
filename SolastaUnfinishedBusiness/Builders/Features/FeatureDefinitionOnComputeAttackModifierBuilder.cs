using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionOnComputeAttackModifierBuilder : DefinitionBuilder<
    FeatureDefinitionOnComputeAttackModifier,
    FeatureDefinitionOnComputeAttackModifierBuilder>
{
    internal FeatureDefinitionOnComputeAttackModifierBuilder SetOnComputeAttackModifierDelegate(
        OnComputeAttackModifier del)
    {
        Definition.SetOnRollAttackModeDelegate(del);
        return this;
    }

    #region Constructors

    protected FeatureDefinitionOnComputeAttackModifierBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionOnComputeAttackModifierBuilder(FeatureDefinitionOnComputeAttackModifier original,
        string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
