using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionOnMagicalAttackDamageEffectBuilder : DefinitionBuilder<
    FeatureDefinitionOnMagicalAttackDamageEffect, FeatureDefinitionOnMagicalAttackDamageEffectBuilder>
{
    internal FeatureDefinitionOnMagicalAttackDamageEffectBuilder SetOnMagicalAttackDamageDelegates(
        OnMagicalAttackDamageDelegate before, OnMagicalAttackDamageDelegate after)
    {
        Definition.SetOnMagicalAttackDamageDelegates(before, after);
        return this;
    }

    #region Constructors

    protected FeatureDefinitionOnMagicalAttackDamageEffectBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionOnMagicalAttackDamageEffectBuilder(
        FeatureDefinitionOnMagicalAttackDamageEffect original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    #endregion
}
