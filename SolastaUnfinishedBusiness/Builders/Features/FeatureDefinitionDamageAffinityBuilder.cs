using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

internal abstract class
    FeatureDefinitionDamageAffinityBuilder<TDefinition, TBuilder> : FeatureDefinitionAffinityBuilder<TDefinition,
        TBuilder>
    where TDefinition : FeatureDefinitionDamageAffinity
    where TBuilder : FeatureDefinitionDamageAffinityBuilder<TDefinition, TBuilder>
{
    #region Constructors

    protected FeatureDefinitionDamageAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionDamageAffinityBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionDamageAffinityBuilder(TDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionDamageAffinityBuilder(TDefinition original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}

[UsedImplicitly]
internal class FeatureDefinitionDamageAffinityBuilder : FeatureDefinitionDamageAffinityBuilder<
    FeatureDefinitionDamageAffinity, FeatureDefinitionDamageAffinityBuilder>
{
    internal FeatureDefinitionDamageAffinityBuilder SetDamageType(string damageType)
    {
        Definition.DamageType = damageType;

        return This();
    }

    internal FeatureDefinitionDamageAffinityBuilder SetAncestryDefinesDamageType(bool ancestryDefinesDamageType)
    {
        Definition.ancestryDefinesDamageType = ancestryDefinesDamageType;

        return This();
    }

    internal FeatureDefinitionDamageAffinityBuilder SetDamageAffinityType(
        RuleDefinitions.DamageAffinityType damageAffinityType)
    {
        Definition.DamageAffinityType = damageAffinityType;

        return This();
    }

    internal FeatureDefinitionDamageAffinityBuilder SetRetaliate(FeatureDefinitionPower featureDefinitionPower,
        int rangeCells, bool retaliateWhenHit = false)
    {
        Definition.retaliatePower = featureDefinitionPower;
        Definition.retaliateRangeCells = rangeCells;
        Definition.retaliateWhenHit = retaliateWhenHit;

        return This();
    }

    #region Constructors

    protected FeatureDefinitionDamageAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionDamageAffinityBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
