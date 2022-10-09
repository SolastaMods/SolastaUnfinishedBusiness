using System;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAttackModifierBuilder
    : FeatureDefinitionBuilder<FeatureDefinitionAttackModifier, FeatureDefinitionAttackModifierBuilder>
{
    internal FeatureDefinitionAttackModifierBuilder Configure(
        AttackModifierMethod attackRollModifierMethod = AttackModifierMethod.None,
        int attackRollModifier = 0,
        string attackRollAbilityScore = "",
        AttackModifierMethod damageRollModifierMethod = AttackModifierMethod.None,
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

    internal FeatureDefinitionAttackModifierBuilder SetAbilityScoreReplacement(
        AbilityScoreReplacement replacement)
    {
        Definition.abilityScoreReplacement = replacement;
        return This();
    }

    internal FeatureDefinitionAttackModifierBuilder SetAttackRollModifier(
        int value,
        AttackModifierMethod method = AttackModifierMethod.FlatValue)
    {
        Definition.attackRollModifierMethod = method;
        Definition.attackRollModifier = value;
        return This();
    }

    internal FeatureDefinitionAttackModifierBuilder SetDamageRollModifier(
        int value,
        AttackModifierMethod method = AttackModifierMethod.FlatValue)
    {
        Definition.damageRollModifierMethod = method;
        Definition.damageRollModifier = value;
        return This();
    }

    internal FeatureDefinitionAttackModifierBuilder SetMagicalWeapon(bool value = true)
    {
        Definition.magicalWeapon = value;
        return This();
    }

    internal FeatureDefinitionAttackModifierBuilder SetAdditionalAttackTag(string tag)
    {
        Definition.additionalBonusUnarmedStrikeAttacksTag = tag;
        return This();
    }

    internal FeatureDefinitionAttackModifierBuilder SetImpactParticleReference(AssetReference asset)
    {
        Definition.impactParticleReference = asset;
        return This();
    }

    internal FeatureDefinitionAttackModifierBuilder SetRequiredProperty(
        RestrictedContextRequiredProperty property)
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
