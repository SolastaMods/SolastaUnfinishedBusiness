using System.Collections.Generic;
using SolastaModApi;

namespace SolastaCommunityExpansion.Models;

public delegate bool IsWeaponValidHandler(RulesetAttackMode attackMode, RulesetItem weapon);

public static class WeaponValidators
{
    public static readonly IsWeaponValidHandler IsUnarmed = IsUnarmedWeapon;

    public static readonly IsWeaponValidHandler IsLight = (mode, weapon) =>
        HasActiveTag(mode, weapon, TagsDefinitions.WeaponTagLight);

    public static bool IsUnarmedWeapon(RulesetAttackMode attackMode, RulesetItem weapon)
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
        return IsUnarmedWeapon(attackMode, null);
    }

    public static bool IsUnarmedWeapon(RulesetItem weapon)
    {
        return IsUnarmedWeapon(null, weapon);
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