using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ArmorTypeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Models.CustomWeaponsContext;

namespace SolastaUnfinishedBusiness.Validators;

internal delegate bool IsWeaponValidHandler(
    RulesetAttackMode attackMode,
    RulesetItem rulesetItem,
    RulesetCharacter rulesetCharacter);

internal static class ValidatorsWeapon
{
    internal static readonly IsWeaponValidHandler AlwaysValid = (_, _, _) => true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IsWeaponValidHandler IsOfWeaponType(params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        return (attackMode, rulesetItem, _) =>
            IsWeaponType(attackMode?.sourceObject as RulesetItem ?? rulesetItem, weaponTypeDefinitions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMagical(RulesetAttackMode attackMode, RulesetItem rulesetItem, RulesetCharacter _)
    {
        return attackMode.Magical ||
               (rulesetItem != null &&
                (rulesetItem.IsMagicalWeapon() ||
                 ShieldAttack.IsMagicalShield(rulesetItem)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsTwoHandedRanged([CanBeNull] RulesetAttackMode attackMode)
    {
        return attackMode is { SourceDefinition: ItemDefinition itemDefinition, Ranged: true } &&
               HasAnyWeaponTag(itemDefinition, TagsDefinitions.WeaponTagTwoHanded);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsTwoHanded([CanBeNull] RulesetAttackMode attackMode)
    {
        return attackMode is { SourceDefinition: ItemDefinition itemDefinition } &&
               HasAnyWeaponTag(itemDefinition, TagsDefinitions.WeaponTagTwoHanded);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMelee([CanBeNull] ItemDefinition itemDefinition)
    {
        if (!itemDefinition)
        {
            return false;
        }

        return itemDefinition.IsArmor ||
               (itemDefinition.WeaponDescription != null &&
                itemDefinition.WeaponDescription.WeaponTypeDefinition != UnarmedStrikeType &&
                itemDefinition.WeaponDescription.WeaponTypeDefinition.WeaponProximity == AttackProximity.Melee);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMelee(
        [CanBeNull] RulesetAttackMode attackMode,
        [CanBeNull] RulesetItem rulesetItem,
        RulesetCharacter rulesetCharacter)
    {
        var finalRulesetItem =
            attackMode?.SourceObject as RulesetItem ?? rulesetItem ?? rulesetCharacter?.GetMainWeapon();

        return IsMelee(finalRulesetItem?.ItemDefinition) || InnovationArmor.InGuardianMode(rulesetCharacter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMelee([CanBeNull] RulesetAttackMode attackMode)
    {
        if (attackMode is { SourceDefinition: MonsterAttackDefinition { proximity: AttackProximity.Melee } })
        {
            return true;
        }

        return
            attackMode is { SourceDefinition: ItemDefinition itemDefinition, Ranged: false } &&
            IsMelee(itemDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsShield([CanBeNull] ItemDefinition itemDefinition)
    {
        if (!itemDefinition)
        {
            return false;
        }

        return itemDefinition.IsArmor &&
               itemDefinition.ArmorDescription != null &&
               itemDefinition.ArmorDescription.ArmorType == ShieldType.Name;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsWeaponType(
        [CanBeNull] ItemDefinition itemDefinition,
        params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        if (!itemDefinition)
        {
            return false;
        }

        return itemDefinition.IsWeapon &&
               itemDefinition.WeaponDescription != null &&
               weaponTypeDefinitions.Contains(itemDefinition.WeaponDescription.WeaponTypeDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsWeaponType(
        [CanBeNull] RulesetItem rulesetItem,
        params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        return IsWeaponType(rulesetItem?.ItemDefinition, weaponTypeDefinitions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsPolearmType([CanBeNull] RulesetAttackMode attackMode)
    {
        return IsOfWeaponType(QuarterstaffType, SpearType, HalberdWeaponType, PikeWeaponType, LongMaceWeaponType)(
            attackMode, null, null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsUnarmed([CanBeNull] ItemDefinition itemDefinition)
    {
        if (!itemDefinition)
        {
            return true;
        }

        return
            itemDefinition.IsWeapon &&
            itemDefinition.WeaponDescription != null &&
            itemDefinition.WeaponDescription.WeaponTypeDefinition.Name == UnarmedStrikeType.Name;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsUnarmed([CanBeNull] RulesetItem rulesetItem)
    {
        return rulesetItem == null || (rulesetItem.ItemDefinition is { } itemDefinition && IsUnarmed(itemDefinition));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsUnarmed([CanBeNull] RulesetAttackMode attackMode)
    {
        if (attackMode is { SourceDefinition: MonsterAttackDefinition { proximity: AttackProximity.Melee } })
        {
            return true;
        }

        return attackMode?.SourceDefinition is ItemDefinition itemDefinition && IsUnarmed(itemDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMeleeOrUnarmed([CanBeNull] RulesetAttackMode attackMode)
    {
        return IsMelee(attackMode) || IsUnarmed(attackMode);
    }

    //ATT: don't use this to check on unarmed weapon types as there itemDefinition are null if not from an attackMode
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool HasAnyWeaponTag([CanBeNull] ItemDefinition itemDefinition, [NotNull] params string[] tags)
    {
        if (!itemDefinition)
        {
            return false;
        }

        return itemDefinition.IsWeapon &&
               itemDefinition.WeaponDescription != null &&
               tags.Any(t => itemDefinition.WeaponDescription.WeaponTags.Contains(t));
    }
}
