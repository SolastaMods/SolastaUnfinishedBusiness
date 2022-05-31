using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;
using static SolastaModApi.DatabaseHelper;
using static SolastaCommunityExpansion.Models.ItemPropertyDescriptions;

namespace SolastaCommunityExpansion.Models;

public static class CustomWeapons
{
    public static ItemDefinition Halberd, HalberdPlus1, HalberdPlus2, HalberdLightning;
    public static ItemDefinition Pike, PikePlus1, PikePlus2, PikePsychic;
    public static ItemDefinition LongMace, LongMacePlus1, LongMacePlus2, LongMaceThunder;

    public static readonly MerchantFilter GenericMelee = new() {IsMeleeWeapon = true};
    public static readonly MerchantFilter MagicMelee = new() {IsMagicalMeleeWeapon = true};
    public static readonly MerchantFilter CraftingManual = new() {IsDocument = true};

    public static readonly ShopItemType ShopGenericMelee = new(FactionStatusDefinitions.Indifference, GenericMelee);
    public static readonly ShopItemType ShopMeleePlus1 = new(FactionStatusDefinitions.Alliance, MagicMelee);
    public static readonly ShopItemType ShopMeleePlus2 = new(FactionStatusDefinitions.Brotherhood, MagicMelee);
    public static readonly ShopItemType ShopCrafting = new(FactionStatusDefinitions.Alliance, CraftingManual);

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
        baseDescription ??= baseItem.WeaponDescription;
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
        var icon = ItemDefinitions.Battleaxe.GuiPresentation.SpriteReference;
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
            20, true, RuleDefinitions.ItemRarity.Common, basePresentation, baseDescription, icon);
        Halberd.SetCustomSubFeatures(scale);
        ShopItems.Add((Halberd, ShopGenericMelee));

        HalberdPlus1 = BuildWeapon("CEHalberd+1", Halberd,
            950, true, RuleDefinitions.ItemRarity.Uncommon, properties: new[] {WeaponPlus1});
        HalberdPlus1.SetCustomSubFeatures(scale);
        ShopItems.Add((HalberdPlus1, ShopMeleePlus1));

        var itemDefinition = ItemDefinitions.BattleaxePlus1;
        HalberdPlus2 = BuildWeapon("CEHalberd+2", Halberd,
            2500, true, RuleDefinitions.ItemRarity.Rare, 
            basePresentation: itemDefinition.ItemPresentation, icon: itemDefinition.GuiPresentation.SpriteReference,
            properties: new[] {WeaponPlus2});
        HalberdPlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((HalberdPlus2, ShopMeleePlus2));

        HalberdLightning = BuildWeapon("CEHalberdLightning", Halberd,
            2500, true, RuleDefinitions.ItemRarity.VeryRare,
            basePresentation: itemDefinition.ItemPresentation, icon: itemDefinition.GuiPresentation.SpriteReference,
            properties: new[] {LightningImpactVFX, WeaponPlus1});
        HalberdLightning.SetCustomSubFeatures(scale);
        HalberdLightning.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypeLightning)
            .Build());
        
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

        PikePlus1 = BuildWeapon("CEPike+1", Pike,
            950, true, RuleDefinitions.ItemRarity.Uncommon, properties: new[] {WeaponPlus1});
        PikePlus1.SetCustomSubFeatures(scale);
        ShopItems.Add((PikePlus1, ShopMeleePlus1));
        var itemDefinition = ItemDefinitions.MorningstarPlus2;
        PikePlus2 = BuildWeapon("CEPike+2", Pike,
            2500, true, RuleDefinitions.ItemRarity.Rare, 
            basePresentation: itemDefinition.ItemPresentation, icon: ItemDefinitions.SpearPlus2.GuiPresentation.SpriteReference,
            properties: new[] {WeaponPlus2});
        PikePlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((PikePlus2, ShopMeleePlus2));
        
        PikePsychic = BuildWeapon("CEPikePsychic", Pike,
            2500, true, RuleDefinitions.ItemRarity.VeryRare, 
            basePresentation: itemDefinition.ItemPresentation, icon: ItemDefinitions.SpearPlus2.GuiPresentation.SpriteReference,
            properties: new[] {PsychicImpactVFX, WeaponPlus1});
        PikePsychic.SetCustomSubFeatures(scale);
        PikePsychic.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypePsychic)
            .Build());
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

        LongMacePlus1 = BuildWeapon("CELongMace+1", LongMace,
            950, true, RuleDefinitions.ItemRarity.Uncommon, properties: new[] {WeaponPlus1});
        LongMacePlus1.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMacePlus1, ShopMeleePlus1));

        var itemDefinition = ItemDefinitions.MacePlus2;
        LongMacePlus2 = BuildWeapon("CELongMace+2", LongMace,
            2500, true, RuleDefinitions.ItemRarity.Rare, 
            basePresentation: itemDefinition.ItemPresentation, icon: itemDefinition.GuiPresentation.SpriteReference,
            properties: new[] {WeaponPlus2});
        LongMacePlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMacePlus2, ShopMeleePlus2));
        
        LongMaceThunder = BuildWeapon("CELongMaceThunder", LongMace,
            2500, true, RuleDefinitions.ItemRarity.VeryRare, 
            basePresentation: itemDefinition.ItemPresentation, icon: itemDefinition.GuiPresentation.SpriteReference,
            properties: new[] {ThunderImpactVFX, WeaponPlus1});
        LongMaceThunder.SetCustomSubFeatures(scale);
        LongMaceThunder.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypeThunder)
            .Build());
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

    public static ItemDefinition BuildManual(RecipeDefinition recipe, Guid guid)
    {
        var reference = ItemDefinitions.CraftingManualScrollOfVampiricTouch;
        return ItemDefinitionBuilder
            .Create($"CraftingManual{recipe.Name}", guid)
            .SetGuiPresentation(Category.Item, reference.GuiPresentation.SpriteReference)
            .SetItemPresentation(reference.ItemPresentation)
            .SetMerchantCategory(MerchantCategoryDefinitions.Crafting)
            .SetSlotTypes(SlotTypeDefinitions.ContainerSlot)
            .SetItemTags(TagsDefinitions.ItemTagStandard, TagsDefinitions.ItemTagPaper)
            .SetDocumentInformation(recipe, reference.DocumentDescription.ContentFragments)
            .SetGold(Main.Settings.RecipeCost)
            .AddToDB();
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
