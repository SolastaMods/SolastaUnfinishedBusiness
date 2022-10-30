using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

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

    //replaces calls to ItemDefinition's isWeapon and Wea[ponDescription getter with custom ones that account for shield
    internal static IEnumerable<CodeInstruction> MakeShieldCountAsMelee(IEnumerable<CodeInstruction> instructions)
    {
        var weaponDescription = typeof(ItemDefinition).GetMethod("get_WeaponDescription");
        var isWeapon = typeof(ItemDefinition).GetMethod("get_IsWeapon");
        var customWeaponDescription = new Func<ItemDefinition, WeaponDescription>(CustomWeaponDescription).Method;
        var customIsWeapon = new Func<ItemDefinition, bool>(CustomIsWeapon).Method;

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(weaponDescription))
            {
                yield return new CodeInstruction(OpCodes.Call, customWeaponDescription);
            }
            else if (instruction.Calls(isWeapon))
            {
                yield return new CodeInstruction(OpCodes.Call, customIsWeapon);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    private static WeaponDescription CustomWeaponDescription(ItemDefinition item)
    {
        return ShieldStrike.IsShield(item)
            ? ShieldStrike.ShieldWeaponDescription
            : item.WeaponDescription;
    }

    private static bool CustomIsWeapon(ItemDefinition item)
    {
        return item.IsWeapon || ShieldStrike.IsShield(item);
    }

    internal static bool IsMagicShield(RulesetItem shield)
    {
        if (shield == null) { return false; }

        if (shield.ItemDefinition.Magical) { return true; }

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
