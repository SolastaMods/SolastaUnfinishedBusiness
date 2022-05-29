using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Models
{
    public static class ShieldStrikeContext
    {
        private static WeaponDescription _shieldWeaponDescription;
        private static WeaponTypeDefinition _shieldWeaponType;

        public static WeaponDescription ShieldWeaponDescription =>
            _shieldWeaponDescription ??= BuildShieldWeaponDescription();

        public static WeaponTypeDefinition ShieldWeaponType => _shieldWeaponType ??= BuildShieldWeaponType();

        public static void Load()
        {
            if (_shieldWeaponType == null)
            {
                _shieldWeaponType = BuildShieldWeaponType();
            }
        }

        public static bool IsShield(RulesetItem item)
        {
            return item != null && IsShield(item.ItemDefinition);
        }

        public static bool IsShield(ItemDefinition item)
        {
            if (item == null || !item.IsArmor)
            {
                return false;
            }

            var armorDescription = item.ArmorDescription;

            return armorDescription.ArmorType == ArmorTypeDefinitions.ShieldType.Name;
        }

        private static WeaponTypeDefinition BuildShieldWeaponType()
        {
            var shieldType = new WeaponTypeDefinitionBuilder(
                    WeaponTypeDefinitions.UnarmedStrikeType,
                    "ShieldStrikeType",
                    DefinitionBuilder.CENamespaceGuid)
                .AddToDB();

            shieldType.soundEffectOnHitDescription =
                WeaponTypeDefinitions.ClubType.SoundEffectOnHitDescription;

            return shieldType;
        }

        private static WeaponDescription BuildShieldWeaponDescription()
        {
            var description = new WeaponDescription(ItemDefinitions.UnarmedStrikeBase.WeaponDescription);
            description.SetWeaponType(ShieldWeaponType.Name);

            var damage = description.EffectDescription.FindFirstDamageForm();
            damage.DieType = RuleDefinitions.DieType.D4;

            return description;
        }
    }
}
