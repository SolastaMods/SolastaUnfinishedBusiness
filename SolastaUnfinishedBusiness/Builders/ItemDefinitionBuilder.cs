using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

internal class ItemDefinitionBuilder : DefinitionBuilder<ItemDefinition, ItemDefinitionBuilder>
{
    internal ItemDefinitionBuilder SetDocumentInformation(RecipeDefinition recipeDefinition,
        IEnumerable<ContentFragmentDescription> contentFragments)
    {
        Definition.IsDocument = true;
        Definition.documentDescription ??= new DocumentDescription();
        Definition.DocumentDescription.recipeDefinition = recipeDefinition;
        Definition.DocumentDescription.loreType = RuleDefinitions.LoreType.CraftingRecipe;
        Definition.DocumentDescription.destroyAfterReading = true;
        Definition.DocumentDescription.locationKnowledgeLevel = GameCampaignDefinitions.NodeKnowledge.Known;
        Definition.DocumentDescription.ContentFragments.SetRange(contentFragments);
        return this;
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
        Definition.costs = new[] { 0, gold, 0, 0, 0 };
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

    internal ItemDefinitionBuilder SetItemRarity(RuleDefinitions.ItemRarity rarity)
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

    internal ItemDefinitionBuilder SetUsableDeviceDescription(IEnumerable<FeatureDefinitionPower> functions)
    {
        Definition.IsUsableDevice = true;
        Definition.usableDeviceDescription = new UsableDeviceDescription();
        Definition.UsableDeviceDescription.DeviceFunctions.Clear();

        var deviceFunction = Berry_Ration.UsableDeviceDescription.DeviceFunctions[0];

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

    protected ItemDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected ItemDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected ItemDefinitionBuilder(ItemDefinition original, string name, Guid namespaceGuid) : base(original, name,
        namespaceGuid)
    {
    }

    protected ItemDefinitionBuilder(ItemDefinition original, string name, string definitionGuid) : base(original,
        name, definitionGuid)
    {
    }

    #endregion
}
