using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionDamageAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionDamageAffinity, FeatureDefinitionDamageAffinityBuilder>
{
    internal FeatureDefinitionDamageAffinityBuilder SetAncestryType(ExtraAncestryType ancestryType)
    {
        Definition.ancestryDefinesDamageType = true;
        Definition.ancestryType = (AncestryType)ancestryType;
        return this;
    }

    internal FeatureDefinitionDamageAffinityBuilder SetDamageType(string damageType)
    {
        Definition.DamageType = damageType;
        return this;
    }

#if false
    internal FeatureDefinitionDamageAffinityBuilder SetAncestryDefinesDamageType(bool ancestryDefinesDamageType)
    {
        Definition.ancestryDefinesDamageType = ancestryDefinesDamageType;
        return this;
    }
#endif

    internal FeatureDefinitionDamageAffinityBuilder SetDamageAffinityType(
        DamageAffinityType damageAffinityType)
    {
        Definition.DamageAffinityType = damageAffinityType;
        return this;
    }

    internal FeatureDefinitionDamageAffinityBuilder SetRetaliate(
        FeatureDefinitionPower featureDefinitionPower,
        int rangeCells)
    {
        Definition.damageAffinityType = DamageAffinityType.None;
        Definition.retaliatePower = featureDefinitionPower;
        Definition.retaliateRangeCells = rangeCells;
        Definition.retaliateWhenHit = true;
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
