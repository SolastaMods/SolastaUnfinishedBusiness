using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.CustomSpecificBehaviors;

internal static class ShieldStrike
{
    private static WeaponDescription _shieldWeaponDescription;
    private static WeaponTypeDefinition _shieldWeaponType;

    [NotNull]
    internal static WeaponDescription ShieldWeaponDescription =>
        _shieldWeaponDescription ??= BuildShieldWeaponDescription();

    [NotNull] internal static WeaponTypeDefinition ShieldWeaponType => _shieldWeaponType ??= BuildShieldWeaponType();

    [NotNull]
    private static WeaponTypeDefinition BuildShieldWeaponType()
    {
        var shieldType = WeaponTypeDefinitionBuilder
            .Create(WeaponTypeDefinitions.UnarmedStrikeType, "CEShieldStrikeType")
            .AddToDB();

        shieldType.soundEffectOnHitDescription = WeaponTypeDefinitions.ClubType.SoundEffectOnHitDescription;

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

        damage.DieType = DieType.D4;

        return description;
    }
}
