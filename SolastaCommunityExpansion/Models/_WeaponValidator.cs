using SolastaModApi;

namespace SolastaCommunityExpansion.Models;

public delegate bool IsWeaponValidHandler(RulesetAttackMode attackMode, RulesetItem weapon);

public static class WeaponValidators
{
    public static IsWeaponValidHandler IsUnarmed = IsUnarmedWeapon;

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
}