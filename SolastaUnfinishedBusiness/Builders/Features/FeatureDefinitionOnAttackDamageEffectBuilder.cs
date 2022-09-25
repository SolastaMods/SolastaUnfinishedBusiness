using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionOnAttackDamageEffectBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionOnAttackDamageEffect, FeatureDefinitionOnAttackDamageEffectBuilder>
{
    public FeatureDefinitionOnAttackDamageEffectBuilder SetOnAttackDamageDelegates(OnAttackDamageDelegate before,
        OnAttackDamageDelegate after)
    {
        Definition.SetOnAttackDamageDelegates(before, after);
        return this;
    }

    #region Constructors

    protected FeatureDefinitionOnAttackDamageEffectBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionOnAttackDamageEffectBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionOnAttackDamageEffectBuilder(FeatureDefinitionOnAttackDamageEffect original,
        string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionOnAttackDamageEffectBuilder(FeatureDefinitionOnAttackDamageEffect original,
        string name, string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
