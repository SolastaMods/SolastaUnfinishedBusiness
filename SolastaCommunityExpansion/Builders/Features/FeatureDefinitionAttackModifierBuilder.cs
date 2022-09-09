using System;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders.Features;

public class FeatureDefinitionAttackModifierBuilder
    : FeatureDefinitionAffinityBuilder<FeatureDefinitionAttackModifier, FeatureDefinitionAttackModifierBuilder>
{
    public FeatureDefinitionAttackModifierBuilder Configure(
        RuleDefinitions.AttackModifierMethod attackRollModifierMethod = RuleDefinitions.AttackModifierMethod.None,
        int attackRollModifier = 0,
        string attackRollAbilityScore = "",
        RuleDefinitions.AttackModifierMethod damageRollModifierMethod = RuleDefinitions.AttackModifierMethod.None,
        int damageRollModifier = 0,
        string damageRollAbilityScore = "",
        bool canAddAbilityBonusToSecondary = false,
        string additionalAttackTag = "")
    {
        Definition.attackRollModifierMethod = attackRollModifierMethod;
        Definition.attackRollModifier = attackRollModifier;
        Definition.attackRollAbilityScore = attackRollAbilityScore;
        Definition.damageRollModifierMethod = damageRollModifierMethod;
        Definition.damageRollModifier = damageRollModifier;
        Definition.damageRollAbilityScore = damageRollAbilityScore;
        Definition.canAddAbilityBonusToSecondary = canAddAbilityBonusToSecondary;
        Definition.additionalBonusUnarmedStrikeAttacksTag = additionalAttackTag;

        return This();
    }

    public FeatureDefinitionAttackModifierBuilder SetAbilityScoreReplacement(
        RuleDefinitions.AbilityScoreReplacement replacement)
    {
        Definition.abilityScoreReplacement = replacement;

        return This();
    }

    public FeatureDefinitionAttackModifierBuilder SetAdditionalAttackTag(string tag)
    {
        Definition.additionalBonusUnarmedStrikeAttacksTag = tag;

        return This();
    }

    public FeatureDefinitionAttackModifierBuilder SetImpactParticleReference(AssetReference asset)
    {
        Definition.impactParticleReference = asset;

        return This();
    }

    public FeatureDefinitionAttackModifierBuilder SetRequiredProperty(
        RuleDefinitions.RestrictedContextRequiredProperty property)
    {
        Definition.requiredProperty = property;

        return This();
    }

    #region Constructors

    protected FeatureDefinitionAttackModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAttackModifierBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionAttackModifierBuilder(FeatureDefinitionAttackModifier original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAttackModifierBuilder(FeatureDefinitionAttackModifier original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
