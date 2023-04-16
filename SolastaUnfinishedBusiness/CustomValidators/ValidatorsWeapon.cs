using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ArmorTypeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.CustomValidators;

internal delegate bool IsWeaponValidHandler(
    RulesetAttackMode attackMode,
    RulesetItem rulesetItem,
    RulesetCharacter rulesetCharacter);

internal static class ValidatorsWeapon
{
    internal static readonly IsWeaponValidHandler AlwaysValid = (_, _, _) => true;

    internal static IsWeaponValidHandler IsOfDamageType(string damageType)
    {
        return (attackMode, _, _) => attackMode?.EffectDescription.FindFirstDamageForm()?.DamageType == damageType;
    }

    internal static IsWeaponValidHandler IsOfWeaponTypeWithoutAttackTag(
        string weaponTag, params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        return (attackMode, rulesetItem, _) => attackMode is not { SourceObject: RulesetItem sourceRulesetItem } ||
                                               attackMode.AttackTags.Contains(weaponTag)
            ? IsWeaponType(rulesetItem, weaponTypeDefinitions)
            : IsWeaponType(sourceRulesetItem, weaponTypeDefinitions);
    }

    internal static IsWeaponValidHandler IsOfWeaponType(params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        return (attackMode, rulesetItem, _) =>
            IsWeaponType(attackMode?.sourceObject as RulesetItem ?? rulesetItem, weaponTypeDefinitions);
    }

    internal static bool IsMagical(RulesetAttackMode attackMode, RulesetItem rulesetItem, RulesetCharacter _)
    {
        return attackMode.Magical || (rulesetItem != null &&
                                      (rulesetItem.IsMagicalWeapon() || ShieldAttack.IsMagicalShield(rulesetItem)));
    }

    internal static bool IsMelee([CanBeNull] ItemDefinition itemDefinition)
    {
        return itemDefinition != null &&
               (itemDefinition.WeaponDescription?.WeaponTypeDefinition.WeaponProximity ==
                   AttackProximity.Melee || itemDefinition.IsArmor /* for shields */);
    }

    internal static bool IsMelee([CanBeNull] RulesetItem rulesetItem)
    {
        return rulesetItem != null && IsMelee(rulesetItem.ItemDefinition);
    }

    internal static bool IsMelee([CanBeNull] RulesetAttackMode attackMode)
    {
        return attackMode is { SourceDefinition: ItemDefinition itemDefinition } && IsMelee(itemDefinition);
    }

    internal static bool IsOneHanded([CanBeNull] RulesetAttackMode attackMode)
    {
        return attackMode is { SourceDefinition: ItemDefinition itemDefinition } &&
               !HasAnyWeaponTag(itemDefinition, TagsDefinitions.WeaponTagTwoHanded);
    }

    internal static bool IsShield([CanBeNull] ItemDefinition itemDefinition)
    {
        return itemDefinition != null &&
               itemDefinition.IsArmor &&
               itemDefinition.ArmorDescription != null &&
               itemDefinition.ArmorDescription.ArmorType == ShieldType.Name;
    }

    internal static bool IsShield([CanBeNull] RulesetItem rulesetItem)
    {
        return rulesetItem != null && IsShield(rulesetItem.ItemDefinition);
    }

    internal static bool IsWeaponType(
        [CanBeNull] ItemDefinition itemDefinition,
        params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        return itemDefinition != null &&
               itemDefinition.IsWeapon &&
               itemDefinition.WeaponDescription != null &&
               weaponTypeDefinitions.Contains(itemDefinition.WeaponDescription.WeaponTypeDefinition);
    }

    internal static bool IsWeaponType(
        [CanBeNull] RulesetItem rulesetItem,
        params WeaponTypeDefinition[] weaponTypeDefinitions)
    {
        return rulesetItem != null && IsWeaponType(rulesetItem.ItemDefinition, weaponTypeDefinitions);
    }

    internal static bool IsUnarmed(
        [CanBeNull] ItemDefinition itemDefinition,
        [CanBeNull] RulesetAttackMode attackMode)
    {
        itemDefinition = attackMode?.SourceDefinition as ItemDefinition ?? itemDefinition;

        return itemDefinition == null ||
               (itemDefinition.IsWeapon &&
                itemDefinition.WeaponDescription != null &&
                itemDefinition.WeaponDescription.WeaponTypeDefinition == UnarmedStrikeType);
    }

    internal static bool IsUnarmed(
        [CanBeNull] RulesetCharacter rulesetCharacter,
        [CanBeNull] RulesetAttackMode attackMode)
    {
        return rulesetCharacter is RulesetCharacterMonster || IsUnarmed((ItemDefinition)null, attackMode);
    }

    internal static bool HasAnyWeaponTag([CanBeNull] ItemDefinition itemDefinition, [NotNull] params string[] tags)
    {
        return itemDefinition != null &&
               itemDefinition.IsWeapon &&
               itemDefinition.WeaponDescription != null &&
               tags.Any(t => itemDefinition.WeaponDescription.WeaponTags.Contains(t));
    }

    internal static bool HasAnyWeaponTag([CanBeNull] RulesetItem rulesetItem, [NotNull] params string[] tags)
    {
        return rulesetItem != null && HasAnyWeaponTag(rulesetItem.ItemDefinition, tags);
    }
}
