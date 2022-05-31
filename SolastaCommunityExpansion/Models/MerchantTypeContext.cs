using System.Collections.Generic;
using System.Linq;
using static SolastaModApi.DatabaseHelper.ItemFlagDefinitions;

namespace SolastaCommunityExpansion.Models;

internal class MerchantTypeContext
{
    internal static Dictionary<MerchantDefinition, MerchantType> MerchantTypes = new();

    internal static void Load()
    {
        var rangeWeaponTypes = new[]
        {
            "LightCrossbowType", "HeavyCrossbowType", "ShortbowType", "LongbowType", "DartType"
        };

        var dbMerchantDefinition = DatabaseRepository.GetDatabase<MerchantDefinition>();

        foreach (var merchant in dbMerchantDefinition)
        {
            var isDocumentMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsDocument);

            var isAmmunitionMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsAmmunition
                    && !x.ItemDefinition.Magical);

            var isArmorMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsArmor
                    && !x.ItemDefinition.Magical);

            var isMeleeWeaponMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsWeapon
                    && !rangeWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                    && !x.ItemDefinition.Magical);

            var isRangeWeaponMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsWeapon
                    && rangeWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                    && !x.ItemDefinition.Magical);

            var isMagicalAmmunitionMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsAmmunition
                    && x.ItemDefinition.Magical);

            var isMagicalArmorMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsArmor
                    && x.ItemDefinition.Magical);

            var isMagicalMeleeWeaponMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsWeapon
                    && !rangeWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                    && x.ItemDefinition.Magical);

            var isMagicalRangeWeaponMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsWeapon
                    && rangeWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                    && x.ItemDefinition.Magical);

            var isPrimedArmorMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsArmor
                    && x.ItemDefinition.ItemPresentation.ItemFlags.Contains(ItemFlagPrimed));

            var isPrimedMeleeWeaponMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsWeapon
                    && !rangeWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                    && x.ItemDefinition.ItemPresentation.ItemFlags.Contains(ItemFlagPrimed));

            var isPrimedRangeWeaponMerchant = merchant.StockUnitDescriptions
                .Any(x =>
                    x.ItemDefinition.IsWeapon
                    && rangeWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                    && x.ItemDefinition.ItemPresentation.ItemFlags.Contains(ItemFlagPrimed));

            MerchantTypes.Add(
                merchant,
                new MerchantType
                {
                    IsDocument = isDocumentMerchant,
                    IsAmmunition = isAmmunitionMerchant,
                    IsArmor = isArmorMerchant,
                    IsMeleeWeapon = isMeleeWeaponMerchant,
                    IsRangeWeapon = isRangeWeaponMerchant,
                    IsMagicalAmmunition = isMagicalAmmunitionMerchant,
                    IsMagicalArmor = isMagicalArmorMerchant,
                    IsMagicalMeleeWeapon = isMagicalMeleeWeaponMerchant,
                    IsMagicalRangeWeapon = isMagicalRangeWeaponMerchant,
                    IsPrimedArmor = isPrimedArmorMerchant,
                    IsPrimedMeleeWeapon = isPrimedMeleeWeaponMerchant,
                    IsPrimedRangeWeapon = isPrimedRangeWeaponMerchant
                });
        }
    }

    internal class MerchantType
    {
        public bool IsAmmunition;
        public bool IsArmor;
        public bool IsDocument;

        public bool IsMagicalAmmunition;
        public bool IsMagicalArmor;
        public bool IsMagicalMeleeWeapon;
        public bool IsMagicalRangeWeapon;
        public bool IsMeleeWeapon;

        public bool IsPrimedArmor;
        public bool IsPrimedMeleeWeapon;
        public bool IsPrimedRangeWeapon;
        public bool IsRangeWeapon;
    }
}
