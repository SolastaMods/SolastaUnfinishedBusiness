using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class ShieldAttack
{
    internal static void UseOffhandForShieldAttackAnimation(
        RulesetAttackMode attackMode,
        ref string animation,
        ref bool isThrown,
        ref bool leftHand)
    {
        if (!ShieldStrike.IsShield(attackMode.SourceDefinition as ItemDefinition))
        {
            return;
        }

        leftHand = true;
        isThrown = false;
        animation = ShieldStrike.ShieldWeaponType.AnimationTag;
    }

    internal static WeaponDescription CustomWeaponDescription(ItemDefinition item)
    {
        return ShieldStrike.IsShield(item)
            ? ShieldStrike.ShieldWeaponDescription
            : item.WeaponDescription;
    }

    internal static bool CustomIsWeapon(ItemDefinition item)
    {
        return item.IsWeapon || ShieldStrike.IsShield(item);
    }

    internal static bool IsMagicShield(RulesetItem shield)
    {
        if (shield == null)
        {
            return false;
        }

        if (shield.ItemDefinition.Magical)
        {
            return true;
        }

        var features = new List<FeatureDefinition>();

        shield.EnumerateFeaturesToBrowse<FeatureDefinitionAttributeModifier>(features);

        var sum = 0;

        foreach (var feature in features)
        {
            var modifier = feature as FeatureDefinitionAttributeModifier;

            if (modifier == null)
            {
                continue;
            }

            if (modifier.ModifiedAttribute != AttributeDefinitions.ArmorClass)
            {
                continue;
            }

            if (modifier.ModifierOperation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive)
            {
                sum += modifier.ModifierValue;
            }
        }

        return sum > 0;
    }
}
