using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
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
    internal static IsWeaponValidHandler IsOfDamageType(string damageType)
    {
        return (attackMode, _, _) => attackMode?.EffectDescription.FindFirstDamageForm()?.DamageType == damageType;
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

#if false
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsTwoHandedRanged(RulesetAttackMode attackMode, RulesetItem rulesetItem, RulesetCharacter _)
    {
        return IsTwoHandedRanged(attackMode?.SourceDefinition as ItemDefinition ?? rulesetItem?.ItemDefinition);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsTwoHandedRanged([CanBeNull] RulesetAttackMode attackMode)
    {
        return IsTwoHandedRanged(attackMode?.SourceDefinition as ItemDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsTwoHandedRanged([CanBeNull] ItemDefinition itemDefinition)
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
        return itemDefinition
               && ((itemDefinition.WeaponDescription != null
                    && itemDefinition.WeaponDescription.WeaponTypeDefinition.WeaponProximity == AttackProximity.Melee)
                   || itemDefinition.IsArmor /* for shields */);
    }

#pragma warning disable IDE0060
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMelee(
        [UsedImplicitly] [CanBeNull] RulesetAttackMode attackMode,
        [CanBeNull] RulesetItem rulesetItem,
        [UsedImplicitly] RulesetCharacter rulesetCharacter)
    {
        RulesetItem attackModeRulesetItem = null;

        if (attackMode?.SourceObject is RulesetItem rulesetItem1)
        {
            attackModeRulesetItem = rulesetItem1;
        }

        var item = attackModeRulesetItem ?? rulesetItem;

        // don't use IsMelee(attackMode) in here as these are used before an attack initiates
        return IsMelee(item ?? rulesetCharacter?.GetMainWeapon());
    }
#pragma warning restore IDE0060

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMelee([CanBeNull] RulesetItem rulesetItem)
    {
        return rulesetItem != null && IsMelee(rulesetItem.ItemDefinition);
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
            attackMode.AttackTags.Contains(TagsDefinitions.WeaponTagMelee) &&
            IsMelee(itemDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsShield([CanBeNull] ItemDefinition itemDefinition)
    {
        return itemDefinition
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
        return itemDefinition
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
    internal static bool IsUnarmed([CanBeNull] ItemDefinition itemDefinition, [CanBeNull] RulesetAttackMode attackMode)
    {
        if (attackMode is { SourceDefinition: MonsterAttackDefinition { proximity: AttackProximity.Melee } })
        {
            return true;
        }

        itemDefinition = attackMode?.SourceDefinition as ItemDefinition ?? itemDefinition;

        if (!itemDefinition)
        {
            return false;
        }

        return
            itemDefinition.IsWeapon &&
            itemDefinition.WeaponDescription != null &&
            itemDefinition.WeaponDescription.WeaponTypeDefinition.Name == UnarmedStrikeType.Name;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsUnarmed([CanBeNull] RulesetAttackMode attackMode)
    {
        return IsUnarmed(null, attackMode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool HasAnyWeaponTag([CanBeNull] ItemDefinition itemDefinition, [NotNull] params string[] tags)
    {
        return itemDefinition
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
