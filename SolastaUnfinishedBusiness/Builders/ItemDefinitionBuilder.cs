using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

internal class ItemDefinitionBuilder : DefinitionBuilder<ItemDefinition, ItemDefinitionBuilder>
{
    internal ItemDefinitionBuilder SetDocumentInformation(RecipeDefinition recipeDefinition,
        IEnumerable<ContentFragmentDescription> contentFragments)
    {
        Definition.IsDocument = true;
        Definition.documentDescription ??= new DocumentDescription();
        Definition.DocumentDescription.recipeDefinition = recipeDefinition;
        Definition.DocumentDescription.loreType = LoreType.CraftingRecipe;
        Definition.DocumentDescription.destroyAfterReading = true;
        Definition.DocumentDescription.locationKnowledgeLevel = GameCampaignDefinitions.NodeKnowledge.Known;
        Definition.DocumentDescription.ContentFragments.SetRange(contentFragments);
        return this;
    }

    protected override void Initialise()
    {
        if (!Definition.isWeapon)
        {
            Definition.weaponDefinition = null;
        }

        if (!Definition.isDocument)
        {
            // must need empty DocumentDescription
            Definition.documentDescription = new DocumentDescription();
        }

        if (!Definition.isMusicalInstrument)
        {
            Definition.musicalInstrumentDefinition = null;
        }

        if (!Definition.isUsableDevice)
        {
            Definition.usableDeviceDescription = null;
        }

        if (!Definition.isContainerItem)
        {
            Definition.containerItemDefinition = null;
        }

        if (!Definition.isStarterPack)
        {
            Definition.starterPackDefinition = null;
        }

        if (!Definition.isLightSourceItem)
        {
            Definition.lightSourceItemDefinition = null;
        }

        if (!Definition.isTool)
        {
            Definition.toolDefinition = null;
        }

        if (!Definition.isArmor)
        {
            Definition.armorDefinition = null;
        }

        if (!Definition.isAmmunition)
        {
            Definition.ammunitionDefinition = null;
        }

        if (!Definition.isFocusItem)
        {
            Definition.focusItemDefinition = null;
        }

        if (!Definition.isSpellbook)
        {
            Definition.spellbookDefinition = null;
        }

        if (!Definition.isWealthPile)
        {
            Definition.wealthPileDefinition = null;
        }

        if (!Definition.isFood)
        {
            Definition.foodDescription = null;
        }

        if (!Definition.isFactionRelic)
        {
            Definition.factionRelicDescription = null;
        }
    }

    internal ItemDefinitionBuilder HideFromDungeonEditor()
    {
        Definition.inDungeonEditor = false;
        return this;
    }

#if false
    internal ItemDefinitionBuilder SetGuiTitleAndDescription(string title, string description)
    {
        Definition.GuiPresentation.Title = title;
        Definition.GuiPresentation.Description = description;
        return this;
    }
#endif

    internal ItemDefinitionBuilder SetGold(int gold)
    {
        Definition.costs = [0, gold, 0, 0, 0];
        return this;
    }

    internal ItemDefinitionBuilder SetCosts(int[] costs)
    {
        Definition.costs = costs;
        return this;
    }

    internal ItemDefinitionBuilder SetWeight(float weight)
    {
        Definition.weight = weight;
        return this;
    }

    internal ItemDefinitionBuilder MakeMagical()
    {
        Definition.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        Definition.magical = true;
        return this;
    }

    internal ItemDefinitionBuilder SetMerchantCategory(MerchantCategoryDefinition category)
    {
        Definition.merchantCategory = category.Name;
        return this;
    }

    internal ItemDefinitionBuilder SetItemTags(params string[] tags)
    {
        Definition.ItemTags.SetRange(tags);
        return this;
    }

    internal ItemDefinitionBuilder AddItemTags(params string[] tags)
    {
        Definition.ItemTags.AddRange(tags);
        return this;
    }

    internal ItemDefinitionBuilder SetSlotTypes(params SlotTypeDefinition[] slotTypes)
    {
        Definition.SlotTypes.SetRange(slotTypes.Select(t => t.Name));
        return this;
    }

    internal ItemDefinitionBuilder SetSlotsWhereActive(params SlotTypeDefinition[] slotTypes)
    {
        Definition.SlotsWhereActive.SetRange(slotTypes.Select(t => t.Name));
        return this;
    }

    internal ItemDefinitionBuilder SetStaticProperties(params ItemPropertyDescription[] staticProperties)
    {
        Definition.StaticProperties.SetRange(staticProperties);
        return this;
    }

    internal ItemDefinitionBuilder RequireAttunement(params CharacterClassDefinition[] requiredClasses)
    {
        Definition.requiresAttunement = true;
        Definition.requiredAttunementClasses.SetRange(requiredClasses);
        return this;
    }

    internal ItemDefinitionBuilder NoAttunement()
    {
        Definition.requiresAttunement = false;
        Definition.requiredAttunementClasses.Clear();
        return this;
    }

    internal ItemDefinitionBuilder SetWeaponDescription(WeaponDescription weapon)
    {
        Definition.isWeapon = true;
        Definition.weaponDefinition = weapon;
        return this;
    }

    internal ItemDefinitionBuilder SetItemPresentation(ItemPresentation presentation)
    {
        Definition.itemPresentation = presentation;
        return this;
    }

    internal ItemDefinitionBuilder SetItemPresentation(ItemDefinition presentation)
    {
        Definition.itemPresentation = presentation.ItemPresentation;
        return this;
    }

    internal ItemDefinitionBuilder SetItemRarity(ItemRarity rarity)
    {
        Definition.itemRarity = rarity;
        return this;
    }

    internal ItemDefinitionBuilder SetRequiresIdentification(bool value)
    {
        Definition.requiresIdentification = value;
        return this;
    }

    internal ItemDefinitionBuilder SetStaticProperties(IEnumerable<ItemPropertyDescription> staticProperties)
    {
        Definition.StaticProperties.SetRange(staticProperties);
        return this;
    }

    internal ItemDefinitionBuilder MergeStaticProperties(IEnumerable<ItemPropertyDescription> staticProperties)
    {
        Definition.StaticProperties.AddRange(staticProperties);
        return this;
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    internal ItemDefinitionBuilder AddWeaponEffect(EffectForm effect)
    {
        Definition.WeaponDescription.EffectDescription.EffectForms.Add(effect);
        return this;
    }

    internal ItemDefinitionBuilder SetUsableDeviceDescription(UsableDeviceDescription usableDescription)
    {
        Definition.IsUsableDevice = true;
        Definition.usableDeviceDescription = usableDescription;
        return this;
    }

    internal ItemDefinitionBuilder SetFoodDescription(FoodDescription foodDescription)
    {
        Definition.IsFood = true;
        Definition.foodDescription = foodDescription;
        return this;
    }

#if false
    internal ItemDefinitionBuilder SetFocusDescription(FocusItemDescription focus)
    {
        Definition.isFocusItem = focus != null;
        Definition.focusItemDefinition = focus;
        return this;
    }
#endif

    internal ItemDefinitionBuilder SetUsableDeviceDescription(params FeatureDefinitionPower[] functions)
    {
        Definition.IsUsableDevice = true;
        Definition.usableDeviceDescription = new UsableDeviceDescription();
        Definition.UsableDeviceDescription.DeviceFunctions.Clear();

        var deviceFunction = DatabaseHelper.ItemDefinitions.Berry_Ration.UsableDeviceDescription.DeviceFunctions[0];

        foreach (var power in functions)
        {
            var functionDescription = new DeviceFunctionDescription(deviceFunction)
            {
                type = DeviceFunctionDescription.FunctionType.Power, featureDefinitionPower = power
            };

            Definition.UsableDeviceDescription.DeviceFunctions.Add(functionDescription);
        }

        return this;
    }


    #region Constructors

    [UsedImplicitly]
    protected ItemDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected ItemDefinitionBuilder(ItemDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
