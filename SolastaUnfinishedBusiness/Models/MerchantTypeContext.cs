using System.Collections.Generic;
using System.Linq;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemFlagDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class MerchantTypeContext
{
    internal static readonly List<(MerchantDefinition, MerchantType)> MerchantTypes = new();

    private static readonly string[] RangedWeaponTypes =
    {
        "LightCrossbowType", "HeavyCrossbowType", "ShortbowType", "LongbowType", "DartType"
    };

    internal static void Load()
    {
        var dbMerchantDefinition = DatabaseRepository.GetDatabase<MerchantDefinition>();

        foreach (var merchant in dbMerchantDefinition)
        {
            MerchantTypes.Add((merchant, GetMerchantType(merchant)));
        }
    }

    internal static MerchantType GetMerchantType(MerchantDefinition merchant)
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
                && !RangedWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                && !x.ItemDefinition.Magical);

        var isRangeWeaponMerchant = merchant.StockUnitDescriptions
            .Any(x =>
                x.ItemDefinition.IsWeapon
                && RangedWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
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
                && !RangedWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                && x.ItemDefinition.Magical);

        var isMagicalRangeWeaponMerchant = merchant.StockUnitDescriptions
            .Any(x =>
                x.ItemDefinition.IsWeapon
                && RangedWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                && x.ItemDefinition.Magical);

        var isPrimedArmorMerchant = merchant.StockUnitDescriptions
            .Any(x =>
                x.ItemDefinition.IsArmor
                && x.ItemDefinition.ItemPresentation.ItemFlags.Contains(ItemFlagPrimed));

        var isPrimedMeleeWeaponMerchant = merchant.StockUnitDescriptions
            .Any(x =>
                x.ItemDefinition.IsWeapon
                && !RangedWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                && x.ItemDefinition.ItemPresentation.ItemFlags.Contains(ItemFlagPrimed));

        var isPrimedRangeWeaponMerchant = merchant.StockUnitDescriptions
            .Any(x =>
                x.ItemDefinition.IsWeapon
                && RangedWeaponTypes.Contains(x.ItemDefinition.WeaponDescription.WeaponType)
                && x.ItemDefinition.ItemPresentation.ItemFlags.Contains(ItemFlagPrimed));

        return new MerchantType
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
        };
    }

    internal sealed class MerchantType
    {
        internal bool IsAmmunition;
        internal bool IsArmor;
        internal bool IsDocument;

        internal bool IsMagicalAmmunition;
        internal bool IsMagicalArmor;
        internal bool IsMagicalMeleeWeapon;
        internal bool IsMagicalRangeWeapon;
        internal bool IsMeleeWeapon;

        internal bool IsPrimedArmor;
        internal bool IsPrimedMeleeWeapon;
        internal bool IsPrimedRangeWeapon;
        internal bool IsRangeWeapon;
    }
}
