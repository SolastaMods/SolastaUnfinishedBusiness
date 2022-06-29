using System;

namespace SolastaCommunityExpansion.Builders.Features;

public abstract class
    FeatureDefinitionCombatAffinityBuilder<TDefinition, TBuilder> : FeatureDefinitionAffinityBuilder<TDefinition,
        TBuilder>
    where TDefinition : FeatureDefinitionCombatAffinity
    where TBuilder : FeatureDefinitionCombatAffinityBuilder<TDefinition, TBuilder>
{
    // Methods specific to FeatureDefinitionCombatAffinity

    public TBuilder SetMyAttackModifierDieType(RuleDefinitions.DieType dieType)
    {
        Definition.myAttackModifierDieType = dieType;
        return This();
    }

    public TBuilder SetMyAttackModifierSign(RuleDefinitions.AttackModifierSign modifierSign)
    {
        Definition.myAttackModifierSign = modifierSign;
        return This();
    }

    public TBuilder SetMyAttackAdvantage(RuleDefinitions.AdvantageType advantage)
    {
        Definition.myAttackAdvantage = advantage;
        return This();
    }

    #region Constructors

    protected FeatureDefinitionCombatAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionCombatAffinityBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionCombatAffinityBuilder(TDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionCombatAffinityBuilder(TDefinition original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}

public class FeatureDefinitionCombatAffinityBuilder
    : FeatureDefinitionCombatAffinityBuilder<FeatureDefinitionCombatAffinity,
        FeatureDefinitionCombatAffinityBuilder>
{
    #region Constructors

    protected FeatureDefinitionCombatAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionCombatAffinityBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionCombatAffinityBuilder(FeatureDefinitionCombatAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionCombatAffinityBuilder(FeatureDefinitionCombatAffinity original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
