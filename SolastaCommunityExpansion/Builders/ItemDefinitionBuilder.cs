using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.Infrastructure;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ItemDefinitions;

namespace SolastaCommunityExpansion.Builders;

public class ItemDefinitionBuilder : DefinitionBuilder<ItemDefinition, ItemDefinitionBuilder>
{
    public ItemDefinitionBuilder SetDocumentInformation(RecipeDefinition recipeDefinition,
        params ContentFragmentDescription[] contentFragments)
    {
        SetDocumentInformation(recipeDefinition, contentFragments.AsEnumerable());
        return this;
    }

    public ItemDefinitionBuilder SetDocumentInformation(RecipeDefinition recipeDefinition,
        IEnumerable<ContentFragmentDescription> contentFragments)
    {
        if (Definition.DocumentDescription == null)
        {
            Definition.documentDescription = new DocumentDescription();
        }

        Definition.IsDocument = true;
        Definition.DocumentDescription.recipeDefinition = recipeDefinition;
        Definition.DocumentDescription.loreType = RuleDefinitions.LoreType.CraftingRecipe;
        Definition.DocumentDescription.destroyAfterReading = true;
        Definition.DocumentDescription.locationKnowledgeLevel = GameCampaignDefinitions.NodeKnowledge.Known;
        Definition.DocumentDescription.ContentFragments.SetRange(contentFragments);
        return this;
    }

    public ItemDefinitionBuilder SetGuiTitleAndDescription(string title, string description)
    {
        Definition.GuiPresentation.Title = title;
        Definition.GuiPresentation.Description = description;
        return this;
    }

    public ItemDefinitionBuilder SetGold(int gold)
    {
        Definition.costs = new[] {0, gold, 0, 0, 0};
        return this;
    }

    public ItemDefinitionBuilder SetCosts(int[] costs)
    {
        Definition.costs = costs;
        return this;
    }

    public ItemDefinitionBuilder SetInDungeonEditor(bool inDungeonEditor)
    {
        Definition.inDungeonEditor = inDungeonEditor;
        return this;
    }

    public ItemDefinitionBuilder MakeMagical()
    {
        Definition.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        Definition.magical = true;
        return this;
    }

    public ItemDefinitionBuilder SetMerchantCategory(MerchantCategoryDefinition category)
    {
        Definition.merchantCategory = category.Name;
        return this;
    }

    public ItemDefinitionBuilder SetItemTags(params string[] tags)
    {
        Definition.ItemTags.SetRange(tags);
        return this;
    }

    public ItemDefinitionBuilder SetSlotTypes(params string[] slotTypes)
    {
        Definition.SlotTypes.SetRange(slotTypes);
        return this;
    }

    public ItemDefinitionBuilder SetSlotTypes(params SlotTypeDefinition[] slotTypes)
    {
        Definition.SlotTypes.SetRange(slotTypes.Select(t => t.Name));
        return this;
    }

    public ItemDefinitionBuilder SetSlotsWhereActive(params string[] slotTypes)
    {
        Definition.SlotsWhereActive.SetRange(slotTypes);
        return this;
    }

    public ItemDefinitionBuilder SetSlotsWhereActive(params SlotTypeDefinition[] slotTypes)
    {
        Definition.SlotsWhereActive.SetRange(slotTypes.Select(t => t.Name));
        return this;
    }

    public ItemDefinitionBuilder SetStaticProperties(params ItemPropertyDescription[] staticProperties)
    {
        Definition.StaticProperties.SetRange(staticProperties);
        return this;
    }

    public ItemDefinitionBuilder SetWeaponDescription(WeaponDescription weapon)
    {
        Definition.isWeapon = true;
        Definition.weaponDefinition = weapon;
        return this;
    }

    public ItemDefinitionBuilder SetItemPresentation(ItemPresentation presentation)
    {
        Definition.itemPresentation = presentation;
        return this;
    }

    public ItemDefinitionBuilder SetItemRarity(RuleDefinitions.ItemRarity rarity)
    {
        Definition.itemRarity = rarity;
        return this;
    }

    public ItemDefinitionBuilder SetRequiresIdentification(bool value)
    {
        Definition.requiresIdentification = value;
        return this;
    }

    public ItemDefinitionBuilder SetRequiresAttunement(bool value)
    {
        Definition.requiresAttunement = value;
        return this;
    }

    public ItemDefinitionBuilder SetStaticProperties(IEnumerable<ItemPropertyDescription> staticProperties)
    {
        Definition.StaticProperties.SetRange(staticProperties);
        return this;
    }

    public ItemDefinitionBuilder MergeStaticProperties(IEnumerable<ItemPropertyDescription> staticProperties)
    {
        Definition.StaticProperties.AddRange(staticProperties);
        return this;
    }

    public ItemDefinitionBuilder AddWeaponEffect(EffectForm effect)
    {
        Definition.WeaponDescription.EffectDescription.EffectForms.Add(effect);
        return this;
    }

    public ItemDefinitionBuilder SetUsableDeviceDescription(UsableDeviceDescription usableDescription)
    {
        Definition.IsUsableDevice = true;
        Definition.usableDeviceDescription = usableDescription;
        return this;
    }

    public ItemDefinitionBuilder SetUsableDeviceDescription(params FeatureDefinitionPower[] functions)
    {
        return SetUsableDeviceDescription(functions.AsEnumerable());
    }

    public ItemDefinitionBuilder SetUsableDeviceDescription(IEnumerable<FeatureDefinitionPower> functions)
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
