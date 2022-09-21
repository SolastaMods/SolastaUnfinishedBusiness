using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Models;

public static class ShieldStrikeContext
{
    private static WeaponDescription _shieldWeaponDescription;
    private static WeaponTypeDefinition _shieldWeaponType;

    [NotNull]
    public static WeaponDescription ShieldWeaponDescription =>
        _shieldWeaponDescription ??= BuildShieldWeaponDescription();

    [NotNull] public static WeaponTypeDefinition ShieldWeaponType => _shieldWeaponType ??= BuildShieldWeaponType();

    public static bool IsShield([CanBeNull] RulesetItem item)
    {
        return item != null && IsShield(item.ItemDefinition);
    }

    public static bool IsShield([CanBeNull] ItemDefinition item)
    {
        if (item == null || !item.IsArmor)
        {
            return false;
        }

        var armorDescription = item.ArmorDescription;

        return armorDescription.ArmorType == ArmorTypeDefinitions.ShieldType.Name;
    }

    [NotNull]
    private static WeaponTypeDefinition BuildShieldWeaponType()
    {
        var shieldType = new WeaponTypeDefinitionBuilder(
                WeaponTypeDefinitions.UnarmedStrikeType,
                "CEShieldStrikeType",
                DefinitionBuilder.CeNamespaceGuid)
            .AddToDB();

        shieldType.soundEffectOnHitDescription =
            WeaponTypeDefinitions.ClubType.SoundEffectOnHitDescription;

        return shieldType;
    }

    [NotNull]
    private static WeaponDescription BuildShieldWeaponDescription()
    {
        var description =
            new WeaponDescription(ItemDefinitions.UnarmedStrikeBase.WeaponDescription)
            {
                weaponType = ShieldWeaponType.Name
            };

        var damage = description.EffectDescription.FindFirstDamageForm();
        damage.DieType = RuleDefinitions.DieType.D4;

        return description;
    }
}
