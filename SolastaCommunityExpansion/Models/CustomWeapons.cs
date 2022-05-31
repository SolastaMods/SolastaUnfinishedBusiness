using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.ItemCrafting;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;
using static SolastaModApi.DatabaseHelper;
using static SolastaCommunityExpansion.Models.ItemPropertyDescriptions;

namespace SolastaCommunityExpansion.Models;

public static class CustomWeapons
{
    public static ItemDefinition Halberd, HalberdPrimed, HalberdPlus1, HalberdPlus2, HalberdLightning;
    public static ItemDefinition Pike, PikePrimed, PikePlus1, PikePlus2, PikePsychic;
    public static ItemDefinition LongMace, LongMacePrimed, LongMacePlus1, LongMacePlus2, LongMaceThunder;

    public static readonly MerchantFilter GenericMelee = new() {IsMeleeWeapon = true};
    public static readonly MerchantFilter MagicMelee = new() {IsMagicalMeleeWeapon = true};
    public static readonly MerchantFilter PrimedMelee = new() {IsPrimedMeleeWeapon = true};
    public static readonly MerchantFilter CraftingManual = new() {IsDocument = true};

    public static readonly ShopItemType ShopGenericMelee = new(FactionStatusDefinitions.Indifference, GenericMelee);
    public static readonly ShopItemType ShopPrimedMelee = new(FactionStatusDefinitions.Sympathy, PrimedMelee);
    public static readonly ShopItemType ShopMeleePlus1 = new(FactionStatusDefinitions.Alliance, MagicMelee);
    public static readonly ShopItemType ShopMeleePlus2 = new(FactionStatusDefinitions.Brotherhood, MagicMelee);
    public static readonly ShopItemType ShopCrafting = new(FactionStatusDefinitions.Alliance, CraftingManual);

    #region Halberd Icons

    private static AssetReferenceSprite _halberdIcon,
        _halberdPrimedIcon,
        _halberdP1Icon,
        _halberdP2Icon,
        _halberdLightningIcon;

    private static AssetReferenceSprite HalberdIcon =>
        _halberdIcon ??= CustomIcons.CreateAssetReferenceSprite("Halberd", Resources.Halberd, 128);

    private static AssetReferenceSprite HalberdPrimedIcon => _halberdPrimedIcon ??=
        CustomIcons.CreateAssetReferenceSprite("HalberdPrimed", Resources.HalberdPrimed, 128);

    private static AssetReferenceSprite HalberdP1Icon => _halberdP1Icon ??=
        CustomIcons.CreateAssetReferenceSprite("Halberd_1", Resources.Halberd_1, 128);

    private static AssetReferenceSprite HalberdP2Icon => _halberdP2Icon ??=
        CustomIcons.CreateAssetReferenceSprite("Halberd_2", Resources.Halberd_2, 128);

    private static AssetReferenceSprite HalberdLightningIcon => _halberdLightningIcon ??=
        CustomIcons.CreateAssetReferenceSprite("HalberdLightning", Resources.HalberdLightning, 128);

    #endregion

    private static readonly List<(ItemDefinition, ShopItemType)> ShopItems = new();
    private static StockUnitDescriptionBuilder _stockBuilder;
    private static StockUnitDescriptionBuilder StockBuilder => _stockBuilder ??= BuildStockBuilder();

    public static void Load()
    {
        BuildHalberds();
        BuildPikes();
        BuildLongMaces();
        HandwrapWeaponContext.Load(ShopItems);

        AddToShops();
    }

    public static ItemPresentation BuildPresentation(string unIdentifiedName, ItemPresentation basePresentation,
        float scale = 1.0f)
    {
        var presentation = new ItemPresentation(basePresentation);
        presentation.ItemFlags.Clear();
        presentation.SetAssetReference(basePresentation.AssetReference);
        presentation.SetUnidentifiedTitle(GuiPresentationBuilder.CreateTitleKey(unIdentifiedName, Category.Item));
        presentation.scaleFactorWhileWielded = scale;
        presentation.SetUnidentifiedDescription(
            GuiPresentationBuilder.CreateDescriptionKey(unIdentifiedName, Category.Item));
        return presentation;
    }

    public static ItemDefinition BuildWeapon(string name, ItemDefinition baseItem, int goldCost, bool noDescription,
        RuleDefinitions.ItemRarity rarity,
        ItemPresentation basePresentation = null,
        WeaponDescription baseDescription = null,
        AssetReferenceSprite icon = null,
        params ItemPropertyDescription[] properties)
    {
        basePresentation ??= baseItem.ItemPresentation;
        baseDescription ??= new WeaponDescription(baseItem.WeaponDescription);
        icon ??= baseItem.GuiPresentation.SpriteReference;

        var builder = ItemDefinitionBuilder
            .Create(baseItem, name, DefinitionBuilder.CENamespaceGuid)
            .SetGold(goldCost)
            .SetMerchantCategory(MerchantCategoryDefinitions.Weapon)
            .SetStaticProperties(properties)
            .SetSlotTypes(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.ContainerSlot)
            .SetSlotsWhereActive(SlotTypeDefinitions.MainHandSlot)
            .SetWeaponDescription(baseDescription)
            .SetItemPresentation(BuildPresentation($"{name}Unidentified", basePresentation))
            .SetRequiresIdentification(true)
            .SetItemRarity(rarity);

        if (!properties.Empty())
        {
            builder.MakeMagical();
        }

        if (noDescription)
        {
            builder.SetGuiPresentation(Category.Item, Gui.NoLocalization, icon);
        }
        else
        {
            builder.SetGuiPresentation(Category.Item, icon);
        }


        var weapon = builder.AddToDB();

        //TODO: add to editor only if option turned on
        weapon.inDungeonEditor = true;

        return weapon;
    }

    private static void BuildHalberds()
    {
        var scale = new CustomScale(z: 3.5f);
        var baseWeaponType = WeaponTypeDefinitionBuilder
            .Create(WeaponTypeDefinitions.GreataxeType, "CEHalberdType", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Equipment, Gui.NoLocalization)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .AddToDB();
        var baseItem = ItemDefinitions.Greataxe;
        var basePresentation = ItemDefinitions.Battleaxe.ItemPresentation;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 2,
            weaponType = baseWeaponType.Name,
            weaponTags = new List<string>
            {
                TagsDefinitions.WeaponTagHeavy,
                TagsDefinitions.WeaponTagReach,
                TagsDefinitions.WeaponTagTwoHanded,
            }
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;
        damageForm.dieType = RuleDefinitions.DieType.D10;
        damageForm.diceNumber = 1;

        Halberd = BuildWeapon("CEHalberd", baseItem,
            20, true, RuleDefinitions.ItemRarity.Common, basePresentation, baseDescription, HalberdIcon);
        Halberd.SetCustomSubFeatures(scale);
        ShopItems.Add((Halberd, ShopGenericMelee));

        HalberdPrimed = BuildWeapon("CEHalberdPrimed", baseItem,
            40, true, RuleDefinitions.ItemRarity.Uncommon, basePresentation, baseDescription, HalberdPrimedIcon);
        HalberdPrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        HalberdPrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        HalberdPrimed.SetCustomSubFeatures(scale);
        ShopItems.Add((HalberdPrimed, ShopPrimedMelee));
        ShopItems.Add((BuildPrimingManual(Halberd, HalberdPrimed), ShopCrafting));

        HalberdPlus1 = BuildWeapon("CEHalberd+1", Halberd,
            950, true, RuleDefinitions.ItemRarity.Rare, icon: HalberdP1Icon, properties: new[] {WeaponPlus1});
        HalberdPlus1.SetCustomSubFeatures(scale);
        ShopItems.Add((HalberdPlus1, ShopMeleePlus1));
        ShopItems.Add((BuildRecipeManual(HalberdPlus1, 24, 10,
                HalberdPrimed,
                ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness),
            ShopCrafting));

        var itemDefinition = ItemDefinitions.BattleaxePlus1;
        HalberdPlus2 = BuildWeapon("CEHalberd+2", Halberd,
            2500, true, RuleDefinitions.ItemRarity.VeryRare,
            basePresentation: itemDefinition.ItemPresentation, icon: HalberdP2Icon,
            properties: new[] {WeaponPlus2});
        HalberdPlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((HalberdPlus2, ShopMeleePlus2));
        ShopItems.Add((BuildRecipeManual(HalberdPlus2, 48, 16,
                HalberdPrimed,
                ItemDefinitions.Ingredient_Enchant_Blood_Gem),
            ShopCrafting));

        HalberdLightning = BuildWeapon("CEHalberdLightning", Halberd,
            2500, true, RuleDefinitions.ItemRarity.VeryRare,
            basePresentation: itemDefinition.ItemPresentation, icon: HalberdLightningIcon,
            properties: new[] {LightningImpactVFX, WeaponPlus1});
        HalberdLightning.SetCustomSubFeatures(scale);
        HalberdLightning.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypeLightning)
            .Build());
        ShopItems.Add((BuildRecipeManual(HalberdLightning, 48, 16,
                HalberdPrimed,
                ItemDefinitions.Ingredient_Enchant_Stardust),
            ShopCrafting));
    }

    private static void BuildPikes()
    {
        var scale = new CustomScale(z: 3.5f);
        var baseWeaponType = WeaponTypeDefinitionBuilder
            .Create(WeaponTypeDefinitions.SpearType, "CEPikeType", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Equipment, Gui.NoLocalization)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .AddToDB();
        var baseItem = ItemDefinitions.Spear;
        var icon = ItemDefinitions.Spear.GuiPresentation.SpriteReference;
        var basePresentation = ItemDefinitions.Morningstar.ItemPresentation;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 2,
            weaponType = baseWeaponType.Name,
            weaponTags = new List<string>
            {
                TagsDefinitions.WeaponTagHeavy,
                TagsDefinitions.WeaponTagReach,
                TagsDefinitions.WeaponTagTwoHanded,
            }
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;
        damageForm.dieType = RuleDefinitions.DieType.D10;
        damageForm.diceNumber = 1;

        Pike = BuildWeapon("CEPike", baseItem,
            20, true, RuleDefinitions.ItemRarity.Common, basePresentation, baseDescription, icon);
        Pike.SetCustomSubFeatures(scale);
        ShopItems.Add((Pike, ShopGenericMelee));

        PikePrimed = BuildWeapon("CEPikePrimed", baseItem,
            40, true, RuleDefinitions.ItemRarity.Uncommon, basePresentation, baseDescription, icon);
        PikePrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        PikePrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        PikePrimed.SetCustomSubFeatures(scale);
        ShopItems.Add((PikePrimed, ShopPrimedMelee));
        ShopItems.Add((BuildPrimingManual(Pike, PikePrimed), ShopCrafting));

        PikePlus1 = BuildWeapon("CEPike+1", Pike,
            950, true, RuleDefinitions.ItemRarity.Rare, properties: new[] {WeaponPlus1});
        PikePlus1.SetCustomSubFeatures(scale);
        ShopItems.Add((PikePlus1, ShopMeleePlus1));
        ShopItems.Add((BuildRecipeManual(PikePlus1, 24, 10,
                PikePrimed,
                ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness),
            ShopCrafting));

        var itemDefinition = ItemDefinitions.MorningstarPlus2;
        PikePlus2 = BuildWeapon("CEPike+2", Pike,
            2500, true, RuleDefinitions.ItemRarity.VeryRare,
            basePresentation: itemDefinition.ItemPresentation,
            icon: ItemDefinitions.SpearPlus2.GuiPresentation.SpriteReference,
            properties: new[] {WeaponPlus2});
        PikePlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((PikePlus2, ShopMeleePlus2));
        ShopItems.Add((BuildRecipeManual(PikePlus2, 48, 16,
                PikePrimed,
                ItemDefinitions.Ingredient_Enchant_Blood_Gem),
            ShopCrafting));

        PikePsychic = BuildWeapon("CEPikePsychic", Pike,
            2500, true, RuleDefinitions.ItemRarity.VeryRare,
            basePresentation: itemDefinition.ItemPresentation,
            icon: ItemDefinitions.SpearPlus2.GuiPresentation.SpriteReference,
            properties: new[] {PsychicImpactVFX, WeaponPlus1});
        PikePsychic.SetCustomSubFeatures(scale);
        PikePsychic.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypePsychic)
            .Build());
        ShopItems.Add((BuildRecipeManual(PikePsychic, 48, 16,
                PikePrimed,
                ItemDefinitions.Ingredient_Enchant_Stardust),
            ShopCrafting));
    }

    private static void BuildLongMaces()
    {
        var scale = new CustomScale(z: 3.5f);
        var baseWeaponType = WeaponTypeDefinitionBuilder
            .Create(WeaponTypeDefinitions.MaulType, "CELongMaceType", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Equipment, Gui.NoLocalization)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .AddToDB();
        var baseItem = ItemDefinitions.Mace;
        var icon = ItemDefinitions.Mace.GuiPresentation.SpriteReference;
        var basePresentation = ItemDefinitions.Mace.ItemPresentation;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 2,
            weaponType = baseWeaponType.Name,
            weaponTags = new List<string>
            {
                TagsDefinitions.WeaponTagHeavy,
                TagsDefinitions.WeaponTagReach,
                TagsDefinitions.WeaponTagTwoHanded,
            }
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;
        damageForm.dieType = RuleDefinitions.DieType.D10;
        damageForm.diceNumber = 1;

        LongMace = BuildWeapon("CELongMace", baseItem,
            20, true, RuleDefinitions.ItemRarity.Common, basePresentation, baseDescription, icon);
        LongMace.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMace, ShopGenericMelee));

        LongMacePrimed = BuildWeapon("CELongMacePrimed", baseItem,
            40, true, RuleDefinitions.ItemRarity.Uncommon, basePresentation, baseDescription, icon);
        LongMacePrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        LongMacePrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        LongMacePrimed.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMacePrimed, ShopPrimedMelee));
        ShopItems.Add((BuildPrimingManual(LongMace, LongMacePrimed), ShopCrafting));

        LongMacePlus1 = BuildWeapon("CELongMace+1", LongMace,
            950, true, RuleDefinitions.ItemRarity.Rare, properties: new[] {WeaponPlus1});
        LongMacePlus1.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMacePlus1, ShopMeleePlus1));
        ShopItems.Add((BuildRecipeManual(LongMacePlus1, 24, 10,
                LongMacePrimed,
                ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness),
            ShopCrafting));

        var itemDefinition = ItemDefinitions.MacePlus2;
        LongMacePlus2 = BuildWeapon("CELongMace+2", LongMace,
            2500, true, RuleDefinitions.ItemRarity.VeryRare,
            basePresentation: itemDefinition.ItemPresentation, icon: itemDefinition.GuiPresentation.SpriteReference,
            properties: new[] {WeaponPlus2});
        LongMacePlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMacePlus2, ShopMeleePlus2));
        ShopItems.Add((BuildRecipeManual(LongMacePlus2, 48, 16,
                LongMacePrimed,
                ItemDefinitions.Ingredient_Enchant_Blood_Gem),
            ShopCrafting));

        LongMaceThunder = BuildWeapon("CELongMaceThunder", LongMace,
            2500, true, RuleDefinitions.ItemRarity.VeryRare,
            basePresentation: itemDefinition.ItemPresentation, icon: itemDefinition.GuiPresentation.SpriteReference,
            properties: new[] {ThunderImpactVFX, WeaponPlus1});
        LongMaceThunder.SetCustomSubFeatures(scale);
        LongMaceThunder.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypeThunder)
            .Build());
        ShopItems.Add((BuildRecipeManual(LongMaceThunder, 48, 16,
                LongMacePrimed,
                ItemDefinitions.Ingredient_Enchant_Stardust),
            ShopCrafting));
    }

    private static void AddToShops()
    {
        //TODO: do this only if mod option is toggled
        GiveAssortment(ShopItems);
    }

    //TODO: move this to the separate shop context file
    private static void GiveAssortment(List<(ItemDefinition, ShopItemType)> items)
    {
        foreach (var e in MerchantTypeContext.MerchantTypes)
        {
            foreach (var (item, itemType) in items)
            {
                if (itemType.filter.Matches(e.Value))
                {
                    StockItem(e.Key, item, itemType.status);
                }
            }
        }
    }

    private static void StockItem(MerchantDefinition merchant, ItemDefinition item, FactionStatusDefinition status)
    {
        merchant.StockUnitDescriptions.Add(StockBuilder
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

    public static RecipeDefinition BuildRecipe(ItemDefinition item, int hours, int difficulty,
        params ItemDefinition[] ingredients)
    {
        return BuildRecipe(item, hours, difficulty, DefinitionBuilder.CENamespaceGuid, ingredients);
    }

    public static RecipeDefinition BuildRecipe(ItemDefinition item, int hours, int difficulty, Guid guid,
        params ItemDefinition[] ingredients)
    {
        return RecipeDefinitionBuilder
            .Create($"RecipeEnchant{item.Name}", guid)
            .SetGuiPresentation(item.GuiPresentation.Title, Gui.NoLocalization)
            .SetCraftedItem(item)
            .SetCraftingCheckData(hours, difficulty, ToolTypeDefinitions.EnchantingToolType)
            .AddIngredients(ingredients)
            .AddToDB();
    }

    public static ItemDefinition BuildRecipeManual(ItemDefinition item, int hours, int difficulty,
        params ItemDefinition[] ingredients)
    {
        return BuildManual(BuildRecipe(item, hours, difficulty, DefinitionBuilder.CENamespaceGuid, ingredients),
            DefinitionBuilder.CENamespaceGuid);
    }

    public static ItemDefinition BuildManual(RecipeDefinition recipe)
    {
        return BuildManual(recipe, DefinitionBuilder.CENamespaceGuid);
    }

    public static ItemDefinition BuildManual(RecipeDefinition recipe, Guid guid)
    {
        var reference = ItemDefinitions.CraftingManualScrollOfVampiricTouch;
        var manual = ItemDefinitionBuilder
            .Create($"CraftingManual{recipe.Name}", guid)
            .SetGuiPresentation(Category.Item, reference.GuiPresentation.SpriteReference)
            .SetItemPresentation(reference.ItemPresentation)
            .SetMerchantCategory(MerchantCategoryDefinitions.Crafting)
            .SetSlotTypes(SlotTypeDefinitions.ContainerSlot)
            .SetItemTags(TagsDefinitions.ItemTagStandard, TagsDefinitions.ItemTagPaper)
            .SetDocumentInformation(recipe, reference.DocumentDescription.ContentFragments)
            .SetGold(Main.Settings.RecipeCost)
            .AddToDB();

        //TODO: add only if option enabled in mod settings
        manual.inDungeonEditor = true;

        return manual;
    }

    public static ItemDefinition BuildPrimingManual(ItemDefinition item, ItemDefinition primed, Guid guid)
    {
        return BuildManual(ItemRecipeGenerationHelper.CreatePrimingRecipe(guid, item, primed), guid);
    }

    public static ItemDefinition BuildPrimingManual(ItemDefinition item, ItemDefinition primed)
    {
        return BuildPrimingManual(item, primed, DefinitionBuilder.CENamespaceGuid);
    }
}

//TODO: move this to the separate shop context file
public class ShopItemType
{
    public readonly FactionStatusDefinition status;
    public readonly MerchantFilter filter;

    public ShopItemType(FactionStatusDefinition status, MerchantFilter filter)
    {
        this.status = status;
        this.filter = filter;
    }
}

public class MerchantFilter
{
    public bool? IsAmmunition = null;
    public bool? IsArmor = null;
    public bool? IsDocument = null;

    public bool? IsMagicalAmmunition = null;
    public bool? IsMagicalArmor = null;
    public bool? IsMagicalMeleeWeapon = null;
    public bool? IsMagicalRangeWeapon = null;
    public bool? IsMeleeWeapon = null;

    public bool? IsPrimedArmor = null;
    public bool? IsPrimedMeleeWeapon = null;
    public bool? IsPrimedRangeWeapon = null;
    public bool? IsRangeWeapon = null;

    public bool Matches(MerchantTypeContext.MerchantType merchantType)
    {
        return (IsAmmunition == null || IsAmmunition == merchantType.IsAmmunition) &&
               (IsArmor == null || IsArmor == merchantType.IsArmor) &&
               (IsDocument == null || IsDocument == merchantType.IsDocument) &&
               (IsMagicalAmmunition == null || IsMagicalAmmunition == merchantType.IsMagicalAmmunition) &&
               (IsMagicalArmor == null || IsMagicalArmor == merchantType.IsMagicalArmor) &&
               (IsMagicalMeleeWeapon == null || IsMagicalMeleeWeapon == merchantType.IsMagicalMeleeWeapon) &&
               (IsMagicalRangeWeapon == null || IsMagicalRangeWeapon == merchantType.IsMagicalRangeWeapon) &&
               (IsMeleeWeapon == null || IsMeleeWeapon == merchantType.IsMeleeWeapon) &&
               (IsPrimedArmor == null || IsPrimedArmor == merchantType.IsPrimedArmor) &&
               (IsPrimedMeleeWeapon == null || IsPrimedMeleeWeapon == merchantType.IsPrimedMeleeWeapon) &&
               (IsPrimedRangeWeapon == null || IsPrimedRangeWeapon == merchantType.IsPrimedRangeWeapon) &&
               (IsRangeWeapon == null || IsRangeWeapon == merchantType.IsRangeWeapon);
    }
}

public class CustomScale
{
    public readonly float X, Y, Z;

    public CustomScale(float x = 1f, float y = 1f, float z = 1f)
    {
        X = x;
        Y = y;
        Z = z;
    }
}
