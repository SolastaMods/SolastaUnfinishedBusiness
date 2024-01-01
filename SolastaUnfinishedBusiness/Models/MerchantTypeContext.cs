using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine;
using UnityEngine.UI;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemFlagDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class MerchantTypeContext
{
    internal static readonly List<(MerchantDefinition, MerchantType)> MerchantTypes = [];

    private static readonly string[] RangedWeaponTypes =
    [
        "LightCrossbowType", "HeavyCrossbowType", "ShortbowType", "LongbowType", "DartType"
    ];

    private static readonly string[] EquipmentSlots =
    [
        EquipmentDefinitions.SlotTypeBelt, EquipmentDefinitions.SlotTypeBack, EquipmentDefinitions.SlotTypeFeet,
        EquipmentDefinitions.SlotTypeFinger, EquipmentDefinitions.SlotTypeGloves, EquipmentDefinitions.SlotTypeHead,
        EquipmentDefinitions.SlotTypeNeck, EquipmentDefinitions.SlotTypeShoulders,
        EquipmentDefinitions.SlotTypeWrists
    ];

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

        //Only merchants with at least 5 magic items qualify - some (like Hugo Requer or Priest of Pakri)
        //sell only 1-2 specific items and should not be considered magic item vendors
        var isMagicalEquipment = merchant.StockUnitDescriptions
            .Count(x =>
                x.ItemDefinition.SlotTypes.Any(s => EquipmentSlots.Contains(s))
                && x.ItemDefinition.Magical) >= 5;

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
            IsMagicalEquipment = isMagicalEquipment,
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
        internal bool IsMagicalEquipment;
        internal bool IsMagicalMeleeWeapon;
        internal bool IsMagicalRangeWeapon;
        internal bool IsMeleeWeapon;

        internal bool IsPrimedArmor;
        internal bool IsPrimedMeleeWeapon;
        internal bool IsPrimedRangeWeapon;
        internal bool IsRangeWeapon;
    }
}

internal static class MerchantContext
{
    private static StockUnitDescriptionBuilder _stockBuilder;

    private static readonly List<(ItemDefinition, ShopItemType)> ShopItems = [];
    private static StockUnitDescriptionBuilder StockBuilder => _stockBuilder ??= BuildStockBuilder();

    internal static void Load()
    {
        AddToShops();
        AddToEditor();
    }

    internal static void AddItem(ItemDefinition item, ShopItemType type)
    {
        ShopItems.Add((item, type));
    }

    private static void AddToShops()
    {
        if (Main.Settings.AddNewWeaponsAndRecipesToShops)
        {
            GiveAssortment(ShopItems, MerchantTypeContext.MerchantTypes);
        }
    }

    private static void AddToEditor()
    {
        if (!Main.Settings.AddNewWeaponsAndRecipesToEditor)
        {
            return;
        }

        foreach (var (item, _) in ShopItems)
        {
            item.inDungeonEditor = true;
        }
    }

    internal static void TryAddItemsToUserMerchant(MerchantDefinition merchant)
    {
        if (Main.Settings.AddNewWeaponsAndRecipesToShops)
        {
            GiveAssortment(ShopItems, merchant, MerchantTypeContext.GetMerchantType(merchant));
        }
    }

    private static void GiveAssortment(List<(ItemDefinition, ShopItemType)> items,
        [NotNull] IEnumerable<(MerchantDefinition, MerchantTypeContext.MerchantType)> merchants)
    {
        foreach (var (merchant, type) in merchants)
        {
            GiveAssortment(items, merchant, type);
        }
    }

    private static void GiveAssortment([NotNull] List<(ItemDefinition, ShopItemType)> items,
        MerchantDefinition merchant,
        MerchantTypeContext.MerchantType type)
    {
        foreach (var (item, itemType) in items)
        {
            if (itemType.Filter.Matches(type))
            {
                StockItem(merchant, item, itemType.Status);
            }
        }
    }

    private static void StockItem([NotNull] MerchantDefinition merchant, ItemDefinition item,
        [NotNull] BaseDefinition status)
    {
        merchant.StockUnitDescriptions.Add(
            StockBuilder
                .SetItem(item)
                .SetFaction(merchant.FactionAffinity, status.Name)
                .Build()
        );
    }

    private static StockUnitDescriptionBuilder BuildStockBuilder()
    {
        return new StockUnitDescriptionBuilder()
            .SetStock(initialAmount: 1)
            .SetRestock(1);
    }
}

internal static class RecipeHelper
{
    private static RecipeDefinition BuildRecipe(
        [NotNull] ItemDefinition item, int hours, int difficulty, params ItemDefinition[] ingredients)
    {
        return RecipeDefinitionBuilder
            .Create($"RecipeEnchant{item.Name}")
            .SetGuiPresentation(item.GuiPresentation.Title, GuiPresentationBuilder.EmptyString, item)
            .SetCraftedItem(item)
            .SetCraftingCheckData(hours, difficulty, ToolTypeDefinitions.EnchantingToolType)
            .AddIngredients(ingredients)
            .AddToDB();
    }

    public static RecipeDefinition BuildPrimeRecipe(ItemDefinition item, ItemDefinition primed)
    {
        if (primed.itemPresentation.ItemFlags.Contains(ItemFlagPrimed))
        {
            return RecipeDefinitionBuilder
                .Create($"RecipePrime{item.Name}")
                .SetGuiPresentation(primed.GuiPresentation.Title, GuiPresentationBuilder.EmptyString, primed)
                .SetCraftedItem(primed)
                .SetCraftingCheckData(8, 15, ToolTypeDefinitions.EnchantingToolType)
                .AddIngredients(item)
                .AddToDB();
        }

        var presentation = new ItemPresentation(primed.itemPresentation);

        presentation.ItemFlags.Add(ItemFlagPrimed);
        primed.itemPresentation = presentation;

        return RecipeDefinitionBuilder
            .Create($"RecipePrime{item.Name}")
            .SetGuiPresentation(primed.GuiPresentation.Title, GuiPresentationBuilder.EmptyString, primed)
            .SetCraftedItem(primed)
            .SetCraftingCheckData(8, 15, ToolTypeDefinitions.EnchantingToolType)
            .AddIngredients(item)
            .AddToDB();
    }

    [NotNull]
    // ReSharper disable once SuggestBaseTypeForParameter
    private static ItemDefinition BuildManual([NotNull] RecipeDefinition recipe, ItemDefinition item, string termTag)
    {
        var reference = ItemDefinitions.CraftingManualScrollOfVampiricTouch;
        var manual = ItemDefinitionBuilder
            .Create($"CraftingManual{recipe.Name}")
            .SetGuiPresentation(
                Gui.Format($"Item/&{termTag}Title", item.FormatTitle()),
                item.guiPresentation.description,
                reference)
            .SetItemPresentation(reference.ItemPresentation)
            .SetMerchantCategory(MerchantCategoryDefinitions.Crafting)
            .SetSlotTypes(SlotTypeDefinitions.ContainerSlot)
            .SetItemTags(TagsDefinitions.ItemTagStandard, TagsDefinitions.ItemTagPaper)
            .SetDocumentInformation(recipe, reference.DocumentDescription.ContentFragments)
            .SetGold(Main.Settings.RecipeCost)
            .AddToDB();

        manual.inDungeonEditor = false;

        return manual;
    }

    internal static void AddRecipeIcons()
    {
        var flag = ItemFlagDefinitionBuilder
            .Create("ItemFlagRecipeIcon")
            .SetGuiPresentationNoContent()
            .AddCustomSubFeatures(new TooltipModifier<ItemDefinition>((tooltip, img, obj, definition, context) =>
            {
                var item = Main.Settings.ShowCraftedItemOnRecipeIcon
                    ? GetCraftedItem(definition)
                    : null;

                if (item == null)
                {
                    return;
                }

                if (img != null)
                {
                    if (img.sprite != null)
                    {
                        Gui.ReleaseAddressableAsset(img.sprite);
                        img.sprite = null;
                    }

                    var spriteReference = item.GuiPresentation.SpriteReference;
                    if (spriteReference != null && spriteReference.RuntimeKeyIsValid())
                    {
                        img.sprite = Gui.LoadAssetSync<Sprite>(spriteReference);
                        if (obj != null)
                        {
                            obj.gameObject.SetActive(true);
                            obj.localScale = new Vector3(2f, 2f, 1f);
                        }
                    }
                }

                if (tooltip != null)
                {
                    ServiceRepository.GetService<IGuiWrapperService>()
                        .GetGuiItemDefinition(item.Name)
                        .SetupTooltip(tooltip, context);
                }
            }))
            .AddToDB();

        var recipes = DatabaseRepository.GetDatabase<ItemDefinition>()
            .Where(d => d.IsDocument)
            .Where(d => d.DocumentDescription != null)
            .Where(d => d.DocumentDescription.RecipeDefinition != null);

        foreach (var recipe in recipes)
        {
            var presentation = new ItemPresentation(recipe.ItemPresentation);
            presentation.ItemFlags.Add(flag);
            recipe.itemPresentation = presentation;
        }
    }

    [NotNull]
    public static ItemDefinition BuildRecipeManual(
        [NotNull] ItemDefinition item, int hours, int difficulty, params ItemDefinition[] ingredients)
    {
        return BuildManual(BuildRecipe(item, hours, difficulty, ingredients), item, "Enchant");
    }

    [NotNull]
    public static ItemDefinition BuildPrimeManual(ItemDefinition item, ItemDefinition primed)
    {
        return BuildManual(BuildPrimeRecipe(item, primed), item, "Prime");
    }

    public static ItemDefinition GetCraftedItem(ItemDefinition item)
    {
        if (!item.IsDocument
            || item.DocumentDescription == null
            || item.DocumentDescription.RecipeDefinition == null)
        {
            return null;
        }

        return item.DocumentDescription.RecipeDefinition.CraftedItem;
    }

    public static bool RecipeIsKnown(ItemDefinition item)
    {
        if (!item.IsDocument
            || item.DocumentDescription == null
            || item.DocumentDescription.RecipeDefinition == null)
        {
            return false;
        }

        var service = ServiceRepository.GetService<IGameLoreService>();

        return service != null && service.KnownRecipes.Contains(item.DocumentDescription.RecipeDefinition);
    }

    internal delegate void TooltipModifier<in T>(GuiTooltip tooltip, Image img, Transform obj, T definition,
        object context)
        where T : BaseDefinition;
}

internal sealed class MerchantFilter
{
    internal static readonly MerchantFilter GenericMelee = new() { _isMeleeWeapon = true };
    internal static readonly MerchantFilter MagicMelee = new() { _isMagicalMeleeWeapon = true };
    internal static readonly MerchantFilter PrimedMelee = new() { _isPrimedMeleeWeapon = true };
    internal static readonly MerchantFilter GenericRanged = new() { _isRangeWeapon = true };
    internal static readonly MerchantFilter MagicRanged = new() { _isMagicalRangeWeapon = true };
    internal static readonly MerchantFilter PrimedRanged = new() { _isPrimedRangeWeapon = true };
    internal static readonly MerchantFilter MagicEquipment = new() { _isMagicalEquipment = true };
    internal static readonly MerchantFilter CraftingManual = new() { _isDocument = true };
    private readonly bool? _isAmmunition = null;
    private readonly bool? _isArmor = null;
    private readonly bool? _isMagicalAmmunition = null;
    private readonly bool? _isMagicalArmor = null;
    private readonly bool? _isPrimedArmor = null;
    private bool? _isDocument;
    private bool? _isMagicalEquipment;
    private bool? _isMagicalMeleeWeapon;
    private bool? _isMagicalRangeWeapon;
    private bool? _isMeleeWeapon;
    private bool? _isPrimedMeleeWeapon;
    private bool? _isPrimedRangeWeapon;
    private bool? _isRangeWeapon;

    internal bool Matches(MerchantTypeContext.MerchantType merchantType)
    {
        return (_isAmmunition == null || _isAmmunition == merchantType.IsAmmunition) &&
               (_isArmor == null || _isArmor == merchantType.IsArmor) &&
               (_isDocument == null || _isDocument == merchantType.IsDocument) &&
               (_isMagicalAmmunition == null || _isMagicalAmmunition == merchantType.IsMagicalAmmunition) &&
               (_isMagicalArmor == null || _isMagicalArmor == merchantType.IsMagicalArmor) &&
               (_isMagicalMeleeWeapon == null || _isMagicalMeleeWeapon == merchantType.IsMagicalMeleeWeapon) &&
               (_isMagicalRangeWeapon == null || _isMagicalRangeWeapon == merchantType.IsMagicalRangeWeapon) &&
               (_isMagicalEquipment == null || _isMagicalEquipment == merchantType.IsMagicalEquipment) &&
               (_isMeleeWeapon == null || _isMeleeWeapon == merchantType.IsMeleeWeapon) &&
               (_isPrimedArmor == null || _isPrimedArmor == merchantType.IsPrimedArmor) &&
               (_isPrimedMeleeWeapon == null || _isPrimedMeleeWeapon == merchantType.IsPrimedMeleeWeapon) &&
               (_isPrimedRangeWeapon == null || _isPrimedRangeWeapon == merchantType.IsPrimedRangeWeapon) &&
               (_isRangeWeapon == null || _isRangeWeapon == merchantType.IsRangeWeapon);
    }
}

internal sealed class ShopItemType
{
    internal static readonly ShopItemType ShopGenericMelee =
        new(FactionStatusDefinitions.Indifference, MerchantFilter.GenericMelee);

    internal static readonly ShopItemType ShopPrimedMelee =
        new(FactionStatusDefinitions.Sympathy, MerchantFilter.PrimedMelee);

    internal static readonly ShopItemType ShopMeleePlus1 =
        new(FactionStatusDefinitions.Alliance, MerchantFilter.MagicMelee);

    internal static readonly ShopItemType ShopMeleePlus2 =
        new(FactionStatusDefinitions.Brotherhood, MerchantFilter.MagicMelee);

    internal static readonly ShopItemType ShopMeleePlus3 =
        new(FactionStatusDefinitions.Brotherhood, MerchantFilter.MagicMelee);

    internal static readonly ShopItemType ShopGenericRanged =
        new(FactionStatusDefinitions.Indifference, MerchantFilter.GenericRanged);

    internal static readonly ShopItemType ShopPrimedRanged =
        new(FactionStatusDefinitions.Sympathy, MerchantFilter.PrimedRanged);

    internal static readonly ShopItemType ShopRangedPlus1 =
        new(FactionStatusDefinitions.Alliance, MerchantFilter.MagicRanged);

    internal static readonly ShopItemType ShopRangedPlus2 =
        new(FactionStatusDefinitions.Brotherhood, MerchantFilter.MagicRanged);

    internal static readonly ShopItemType ShopRangedPlus3 =
        new(FactionStatusDefinitions.Brotherhood, MerchantFilter.MagicRanged);

    internal static readonly ShopItemType ShopCrafting =
        new(FactionStatusDefinitions.Alliance, MerchantFilter.CraftingManual);

    internal static readonly ShopItemType MagicItemUncommon =
        new(FactionStatusDefinitions.Sympathy, MerchantFilter.MagicEquipment);

    internal static readonly ShopItemType MagicItemRare =
        new(FactionStatusDefinitions.Alliance, MerchantFilter.MagicEquipment);

#if false
    internal static readonly ShopItemType MagicItemCommon =
        new(FactionStatusDefinitions.Indifference, MerchantFilter.MagicEquipment);

    internal static readonly ShopItemType MagicItemVeryRare =
        new(FactionStatusDefinitions.Brotherhood, MerchantFilter.MagicEquipment);

    internal static readonly ShopItemType MagicItemLegendary =
        new(FactionStatusDefinitions.LivingLegend, MerchantFilter.MagicEquipment);
#endif

    internal readonly MerchantFilter Filter;
    internal readonly FactionStatusDefinition Status;

    private ShopItemType(FactionStatusDefinition status, MerchantFilter filter)
    {
        Status = status;
        Filter = filter;
    }
}
