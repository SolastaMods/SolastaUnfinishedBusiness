using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ArmorTypeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Models.CustomWeaponsContext;

namespace SolastaUnfinishedBusiness.CustomValidators;

internal delegate bool IsWeaponValidHandler(
    RulesetAttackMode attackMode,
    RulesetItem rulesetItem,
    RulesetCharacter rulesetCharacter);

internal static class ValidatorsWeapon
{
    internal static readonly IsWeaponValidHandler AlwaysValid = (_, _, _) => true;

    internal static readonly IsWeaponValidHandler IsZenArrowAttack =
        (attackMode, _, character) => attackMode is { Ranged: true }
                                      && character.IsMonkWeapon(attackMode.SourceDefinition as ItemDefinition);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IsWeaponValidHandler IsOfDamageType(string damageType)
    {
        return (attackMode, _, _) => attackMode?.EffectDescription.FindFirstDamageForm()?.DamageType == damageType;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IsWeaponValidHandler IsOfWeaponTypeWithoutAttackTag(
        string weaponTag, params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        return (attackMode, rulesetItem, _) => attackMode is not { SourceObject: RulesetItem sourceRulesetItem }
                                               || attackMode.AttackTags.Contains(weaponTag)
            ? IsWeaponType(rulesetItem, weaponTypeDefinitions)
            : IsWeaponType(sourceRulesetItem, weaponTypeDefinitions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IsWeaponValidHandler IsOfWeaponType(params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        return (attackMode, rulesetItem, _) =>
            IsWeaponType(attackMode?.sourceObject as RulesetItem ?? rulesetItem, weaponTypeDefinitions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMagical(RulesetAttackMode attackMode, RulesetItem rulesetItem, RulesetCharacter _)
    {
        return attackMode.Magical || (rulesetItem != null
                                      && (rulesetItem.IsMagicalWeapon() || ShieldAttack.IsMagicalShield(rulesetItem)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsTwoHandedRanged(RulesetAttackMode attackMode, RulesetItem rulesetItem, RulesetCharacter _)
    {
        return IsTwoHandedRanged(attackMode?.SourceDefinition as ItemDefinition ?? rulesetItem?.ItemDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsTwoHandedRanged([CanBeNull] RulesetAttackMode attackMode)
    {
        return IsTwoHandedRanged(attackMode?.SourceDefinition as ItemDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsTwoHandedRanged([CanBeNull] ItemDefinition itemDefinition)
    {
        return IsWeaponType(itemDefinition, LongbowType, ShortbowType, HeavyCrossbowType, LightCrossbowType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool HasTwoHandedTag([CanBeNull] RulesetAttackMode attackMode)
    {
        return attackMode is { SourceDefinition: ItemDefinition itemDefinition } &&
               HasAnyWeaponTag(itemDefinition, TagsDefinitions.WeaponTagTwoHanded);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsMelee([CanBeNull] ItemDefinition itemDefinition)
    {
        return itemDefinition != null
               && ((itemDefinition.WeaponDescription != null
                    && itemDefinition.WeaponDescription.WeaponTypeDefinition.WeaponProximity == AttackProximity.Melee)
                   || itemDefinition.IsArmor /* for shields */);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMelee(
        [CanBeNull] RulesetAttackMode attackMode, [CanBeNull] RulesetItem rulesetItem, RulesetCharacter _)
    {
        return attackMode != null ? IsMelee(attackMode) : IsMelee(rulesetItem);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMelee([CanBeNull] RulesetItem rulesetItem)
    {
        return rulesetItem != null && IsMelee(rulesetItem.ItemDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMelee([CanBeNull] RulesetAttackMode attackMode)
    {
        return !(attackMode == null || attackMode.Ranged || attackMode.Thrown)
               && attackMode.SourceDefinition is ItemDefinition itemDefinition && IsMelee(itemDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsShield(
        RulesetAttackMode attackMode, RulesetItem rulesetItem, RulesetCharacter rulesetCharacter)
    {
        return (attackMode is { SourceDefinition: ItemDefinition itemDefinition } && IsShield(itemDefinition))
               || IsShield(rulesetItem)
               || IsShield(rulesetCharacter?.GetOffhandWeapon());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsShield([CanBeNull] ItemDefinition itemDefinition)
    {
        return itemDefinition != null
               && itemDefinition.IsArmor
               && itemDefinition.ArmorDescription != null
               && itemDefinition.ArmorDescription.ArmorType == ShieldType.Name;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsShield([CanBeNull] RulesetItem rulesetItem)
    {
        return rulesetItem != null && IsShield(rulesetItem.ItemDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsWeaponType(
        [CanBeNull] ItemDefinition itemDefinition,
        params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        return itemDefinition != null
               && itemDefinition.IsWeapon
               && itemDefinition.WeaponDescription != null
               && weaponTypeDefinitions.Contains(itemDefinition.WeaponDescription.WeaponTypeDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsWeaponType(
        [CanBeNull] RulesetItem rulesetItem,
        params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        return rulesetItem != null && IsWeaponType(rulesetItem.ItemDefinition, weaponTypeDefinitions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsPolearmType([CanBeNull] RulesetAttackMode attackMode)
    {
        return IsOfWeaponType(QuarterstaffType, SpearType, HalberdWeaponType, PikeWeaponType, LongMaceWeaponType)(
            attackMode, null, null);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsUnarmed(
        [CanBeNull] ItemDefinition itemDefinition,
        [CanBeNull] RulesetAttackMode attackMode)
    {
        itemDefinition = attackMode?.SourceDefinition as ItemDefinition ?? itemDefinition;

        return itemDefinition == null
               || (itemDefinition.IsWeapon
                   && itemDefinition.WeaponDescription != null
                   && itemDefinition.WeaponDescription.WeaponTypeDefinition == UnarmedStrikeType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsUnarmed(
        [CanBeNull] RulesetCharacter rulesetCharacter,
        [CanBeNull] RulesetAttackMode attackMode)
    {
        return (rulesetCharacter is RulesetCharacterMonster && IsMelee(attackMode))
               || IsUnarmed((ItemDefinition)null, attackMode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool HasAnyWeaponTag([CanBeNull] ItemDefinition itemDefinition, [NotNull] params string[] tags)
    {
        return itemDefinition != null
               && itemDefinition.IsWeapon
               && itemDefinition.WeaponDescription != null
               && tags.Any(t => itemDefinition.WeaponDescription.WeaponTags.Contains(t));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool HasAnyWeaponTag([CanBeNull] RulesetItem rulesetItem, [NotNull] params string[] tags)
    {
        return rulesetItem != null && HasAnyWeaponTag(rulesetItem.ItemDefinition, tags);
    }
}
