using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Validators;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

internal static class ShieldAttack
{
    internal static void UseOffhandForShieldAttackAnimation(
        RulesetAttackMode attackMode,
        ref string animation,
        ref bool isThrown,
        ref bool leftHand)
    {
        if (!ValidatorsWeapon.IsShield(attackMode.SourceDefinition as ItemDefinition))
        {
            return;
        }

        leftHand = true;
        isThrown = false;
        animation = ShieldStrike.ShieldWeaponType.AnimationTag;
    }

    internal static WeaponDescription EnhancedWeaponDescription(ItemDefinition itemDefinition)
    {
        return ValidatorsWeapon.IsShield(itemDefinition)
            ? ShieldStrike.ShieldWeaponDescription
            : itemDefinition.WeaponDescription;
    }

    internal static bool IsWeaponOrShield(ItemDefinition itemDefinition)
    {
        return itemDefinition.IsWeapon || ValidatorsWeapon.IsShield(itemDefinition);
    }

    internal static bool IsMagicalShield(RulesetItem shield)
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

        var sum = features.OfType<FeatureDefinitionAttributeModifier>()
            .Where(x => x.ModifiedAttribute == AttributeDefinitions.ArmorClass &&
                        x.ModifierOperation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive)
            .Sum(feature => feature.ModifierValue);

        return sum > 0;
    }
}
