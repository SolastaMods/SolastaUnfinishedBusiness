using System;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAttackModifierBuilder
    : DefinitionBuilder<FeatureDefinitionAttackModifier, FeatureDefinitionAttackModifierBuilder>
{
    internal FeatureDefinitionAttackModifierBuilder SetAttackRollModifier(
        int value = 0,
        AttackModifierMethod method = AttackModifierMethod.FlatValue,
        string ability = "")
    {
        Definition.attackRollModifierMethod = method;
        Definition.attackRollModifier = value;
        Definition.attackRollAbilityScore = ability;
        return this;
    }

    internal FeatureDefinitionAttackModifierBuilder SetDamageRollModifier(
        int value = 0,
        AttackModifierMethod method = AttackModifierMethod.FlatValue,
        string ability = "")
    {
        Definition.damageRollModifierMethod = method;
        Definition.damageRollModifier = value;
        Definition.damageRollAbilityScore = ability;
        return this;
    }

    internal FeatureDefinitionAttackModifierBuilder SetUnarmedStrike(int additionalBonusUnarmedStrikeAttacksCount)
    {
        Definition.additionalBonusUnarmedStrikeAttacksCount = additionalBonusUnarmedStrikeAttacksCount;
        return this;
    }

    internal FeatureDefinitionAttackModifierBuilder SetMagicalWeapon()
    {
        Definition.magicalWeapon = true;
        return this;
    }

    internal FeatureDefinitionAttackModifierBuilder SetDualWield(bool canDualWieldNonLight = true,
        bool canAddAbilityBonusToSecondary = false)
    {
        Definition.canDualWieldNonLight = canDualWieldNonLight;
        Definition.canAddAbilityBonusToSecondary = canAddAbilityBonusToSecondary;
        return this;
    }

    internal FeatureDefinitionAttackModifierBuilder SetImpactParticleReference(AssetReference asset)
    {
        Definition.impactParticleReference = asset;
        return this;
    }

    internal FeatureDefinitionAttackModifierBuilder SetRequiredProperty(
        RestrictedContextRequiredProperty property)
    {
        Definition.requiredProperty = property;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionAttackModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAttackModifierBuilder(FeatureDefinitionAttackModifier original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
