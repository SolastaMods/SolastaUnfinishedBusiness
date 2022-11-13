using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionDamageAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionDamageAffinity, FeatureDefinitionDamageAffinityBuilder>
{
    internal FeatureDefinitionDamageAffinityBuilder SetDamageType(string damageType)
    {
        Definition.DamageType = damageType;
        return this;
    }

    internal FeatureDefinitionDamageAffinityBuilder SetAncestryDefinesDamageType(bool ancestryDefinesDamageType)
    {
        Definition.ancestryDefinesDamageType = ancestryDefinesDamageType;
        return this;
    }

    internal FeatureDefinitionDamageAffinityBuilder SetDamageAffinityType(
        RuleDefinitions.DamageAffinityType damageAffinityType)
    {
        Definition.DamageAffinityType = damageAffinityType;
        return this;
    }

    internal FeatureDefinitionDamageAffinityBuilder SetRetaliate(
        FeatureDefinitionPower featureDefinitionPower,
        int rangeCells,
        bool retaliateWhenHit = false)
    {
        Definition.retaliatePower = featureDefinitionPower;
        Definition.retaliateRangeCells = rangeCells;
        Definition.retaliateWhenHit = retaliateWhenHit;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionDamageAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
