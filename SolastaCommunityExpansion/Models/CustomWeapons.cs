using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Models;

public static class CustomWeapons
{
    public static ItemDefinition Halberd;
    public static ItemDefinition Pike;
    public static ItemDefinition LongMace;

    private static readonly List<(ItemDefinition, FactionStatusDefinition)> GenericWeapons = new();
    private static readonly List<(ItemDefinition, FactionStatusDefinition)> MagicWeapons = new();
    private static readonly List<(ItemDefinition, FactionStatusDefinition)> CraftingManuals = new();
    private static StockUnitDescriptionBuilder _stockBuilder;
    private static StockUnitDescriptionBuilder StockBuilder => _stockBuilder ??= BuildStockBuilder();

    public static void Load()
    {
        BuildHalberds();
        BuildPikes();
        BuildLongMaces();
        HandwrapWeaponContext.Load(GenericWeapons, MagicWeapons, CraftingManuals);

        AddToShops();
    }

    private static ItemPresentation BuildPresentation(string unIdentifiedName, ItemPresentation basePresentation,
        float scale = 1.0f,
        bool emptyAsset = false)
    {
        var presentation = new ItemPresentation(basePresentation);
        presentation.ItemFlags.Clear();
        var assetReference = emptyAsset ? new AssetReference() : basePresentation.AssetReference;
        presentation.SetAssetReference(assetReference);
        presentation.SetUnidentifiedTitle(GuiPresentationBuilder.CreateTitleKey(unIdentifiedName, Category.Item));
        presentation.scaleFactorWhileWielded = scale;
        presentation.SetUnidentifiedDescription(
            GuiPresentationBuilder.CreateDescriptionKey(unIdentifiedName, Category.Item));
        return presentation;
    }

    private static ItemDefinition BuildWeapon(string name, ItemDefinition baseItem, int goldCost, bool noDescription,
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
            .Create(name, DefinitionBuilder.CENamespaceGuid)
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

        return builder
            .AddToDB();
    }

    private static void BuildHalberds()
    {
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
        Halberd.SetCustomSubFeatures(new CustomScale(z: 3.5f));
        Halberd.inDungeonEditor = true;
        GenericWeapons.Add((Halberd, FactionStatusDefinitions.Indifference));
    }

    private static void BuildPikes()
    {
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
        Pike.SetCustomSubFeatures(new CustomScale(z: 3.5f));
        Pike.inDungeonEditor = true;
        GenericWeapons.Add((Pike, FactionStatusDefinitions.Indifference));
    }

    private static void BuildLongMaces()
    {
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
        LongMace.SetCustomSubFeatures(new CustomScale(z: 3.5f));
        LongMace.inDungeonEditor = true;
        GenericWeapons.Add((LongMace, FactionStatusDefinitions.Indifference));
    }

    private static void AddToShops()
    {
        //TODO: do this only if mod option is toggled

        GiveAssortment(GenericWeapons,
            MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore //Caer Cyflen
            //TODO: find weapon merchants in Lost Valley
        );

        GiveAssortment(MagicWeapons,
            MerchantDefinitions.Store_Merchant_CircleOfDanantar_Joriel_Foxeye //Caer Cyflen
            //TODO: find magic weapon merchants in Lost Valley
        );

        GiveAssortment(CraftingManuals,
            MerchantDefinitions.Store_Merchant_Circe //Manacalon Ruins
            //TODO: find crafting manuals merchants in Lost Valley
        );
    }

    private static void GiveAssortment(List<(ItemDefinition, FactionStatusDefinition)> items,
        params MerchantDefinition[] merchants)
    {
        foreach (var merchant in merchants)
        {
            foreach (var (item, status) in items)
            {
                StockItem(merchant, item, status);
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
