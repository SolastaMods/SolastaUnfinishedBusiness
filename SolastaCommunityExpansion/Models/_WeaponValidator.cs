using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api;

namespace SolastaCommunityExpansion.Models;

public delegate bool IsWeaponValidHandler(RulesetAttackMode attackMode, RulesetItem weapon,
    RulesetCharacter character);

public static class WeaponValidators
{
    public static readonly IsWeaponValidHandler AlwaysValid = (_, _, _) => true;

    public static readonly IsWeaponValidHandler IsUnarmed = IsUnarmedWeapon;
    public static readonly IsWeaponValidHandler IsReactionAttack = IsReactionAttackMode;

    public static readonly IsWeaponValidHandler IsLight = (mode, weapon, _) =>
        HasActiveTag(mode, weapon, TagsDefinitions.WeaponTagLight);

    public static bool IsPolearm(RulesetItem weapon)
    {
        return weapon != null
               && IsPolearm(weapon.ItemDefinition);
    }

    public static bool IsPolearm(ItemDefinition weapon)
    {
        return weapon != null
               && CustomWeaponsContext.PolearmWeaponTypes.Contains(weapon.WeaponDescription?.WeaponType);
    }

    public static bool IsMelee(RulesetItem weapon)
    {
        return HasAnyTag(weapon, TagsDefinitions.WeaponTagMelee);
    }

    public static bool IsRanged(RulesetItem weapon)
    {
        return HasAnyWeaponTag(weapon, TagsDefinitions.WeaponTagRange, TagsDefinitions.WeaponTagThrown);
    }

    public static bool IsOneHanded(RulesetItem weapon)
    {
        return !HasAnyWeaponTag(weapon, TagsDefinitions.WeaponTagTwoHanded);
    }

    public static bool IsUnarmedWeapon(RulesetAttackMode attackMode, RulesetItem weapon, RulesetCharacter character)
    {
        var item = attackMode?.SourceDefinition as ItemDefinition ?? weapon?.ItemDefinition;
        if (item != null)
        {
            return item.WeaponDescription?.WeaponTypeDefinition ==
                   DatabaseHelper.WeaponTypeDefinitions.UnarmedStrikeType;
        }

        return weapon == null;
    }

    public static bool IsUnarmedWeapon(RulesetAttackMode attackMode)
    {
        return IsUnarmedWeapon(attackMode, null, null);
    }

    public static bool IsUnarmedWeapon(RulesetItem weapon)
    {
        return IsUnarmedWeapon(null, weapon, null);
    }

    public static bool IsThrownWeapon(RulesetItem weapon)
    {
        if (weapon == null)
        {
            return false;
        }

        var weaponDescription = weapon.ItemDefinition.WeaponDescription;

        if (weaponDescription == null)
        {
            return false;
        }

        return weaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagThrown);
    }

    public static bool IsReactionAttackMode(RulesetAttackMode attackMode, RulesetItem weapon,
        RulesetCharacter character)
    {
        return attackMode is {ActionType: ActionDefinitions.ActionType.Reaction};
    }

    public static bool HasAnyTag(RulesetItem item, params string[] tags)
    {
        var tagsMap = new Dictionary<string, TagsDefinitions.Criticity>();
        item?.FillTags(tagsMap, null, true);
        return tagsMap.Keys.Any(tags.Contains);
    }

    public static bool HasAnyWeaponTag(RulesetItem item, params string[] tags)
    {
        return item != null && HasAnyWeaponTag(item.ItemDefinition, tags);
    }

    public static bool HasAnyWeaponTag(ItemDefinition item, params string[] tags)
    {
        var weaponTags = GetWeaponTags(item);
        return weaponTags != null && tags.Any(t => weaponTags.Contains(t));
    }

    private static bool HasActiveTag(RulesetAttackMode mode, RulesetItem weapon, string tag)
    {
        var hasTag = false;
        if (mode != null)
        {
            hasTag = mode.AttackTags.Contains(tag);
            if (!hasTag)
            {
                var tags = GetWeaponTags(mode.SourceDefinition as ItemDefinition);
                if (tags != null && tags.Contains(tag))
                {
                    hasTag = true;
                }
            }

            return hasTag;
        }

        if (weapon != null)
        {
            var tags = GetWeaponTags(weapon.ItemDefinition);
            if (tags != null && tags.Contains(tag))
            {
                hasTag = true;
            }
        }

        return hasTag;
    }

    private static List<string> GetWeaponTags(ItemDefinition item)
    {
        if (item != null)
        {
            return item.WeaponDescription?.WeaponTags;
        }

        return null;
    }
}
