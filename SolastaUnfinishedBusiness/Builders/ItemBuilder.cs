using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Builders;

internal static class ItemBuilder
{
    internal static ItemDefinition BuilderCopyFromItemSetRecipe(ItemDefinition original, string name,
        RecipeDefinition recipeDefinition, int gold, GuiPresentation guiPresentation)
    {
        return ItemDefinitionBuilder
            .Create(original, name)
            .SetGuiPresentation(guiPresentation)
            .SetDocumentInformation(recipeDefinition, original.DocumentDescription.ContentFragments)
            .SetGold(gold)
            .AddToDB();
    }

    internal static ItemDefinition BuildNewMagicWeapon(ItemDefinition original, ItemDefinition presentation,
        string name,
        ItemDefinition magicalExample)
    {
        var itemName = original.Name + "_" + name;

        var builder = ItemDefinitionBuilder
            .Create(original, itemName)
            .SetOrUpdateGuiPresentation(itemName + "_", Category.Item)
            .SetItemPresentation(presentation.ItemPresentation.DeepCopy())
            // Set is magical
            // Remove "Standard" from item tags
            .MakeMagical()
            // Copy over static properties from example enchanted
            .SetStaticProperties(magicalExample.StaticProperties)
            // Copy over price from example enchanted
            .SetCosts(magicalExample.Costs);

        if (magicalExample.IsUsableDevice)
        {
            builder.SetUsableDeviceDescription(magicalExample.UsableDeviceDescription);
        }

        // If example enchanted has multiple forms, copy over extra forms
        if (magicalExample.WeaponDescription.EffectDescription.EffectForms.Count <= 1)
        {
            return builder.AddToDB();
        }

        for (var i = 1; i < magicalExample.WeaponDescription.EffectDescription.EffectForms.Count; i++)
        {
            builder.AddWeaponEffect(magicalExample.WeaponDescription.EffectDescription.EffectForms[i]);
        }

        return builder.AddToDB();
    }

    internal static ItemDefinition BuildNewMagicArmor(ItemDefinition original, ItemDefinition presentation, string name,
        ItemDefinition magicalExample)
    {
        var itemName = original.Name + "_" + name;

        var builder = ItemDefinitionBuilder
            .Create(original, itemName)
            .SetOrUpdateGuiPresentation(itemName + "_", Category.Item)
            .SetItemPresentation(presentation.ItemPresentation.DeepCopy())
            // Set is magical
            // Remove "Standard" from item tags
            .MakeMagical()
            // Copy over price from example enchanted
            .SetCosts(magicalExample.Costs)
            // Copy over static properties from example enchanted, but remove stealth disadvantage since that is determined by the armor and not the enchantment.
            .MergeStaticProperties(FilterItemProperty(magicalExample.StaticProperties,
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityStealthDisadvantage));

        if (magicalExample.IsUsableDevice)
        {
            builder.SetUsableDeviceDescription(magicalExample.UsableDeviceDescription);
        }

        var newItem = builder.AddToDB();

        if (newItem.ItemPresentation.armorAddressableName != String.Empty)
        {
            return newItem;
        }

        newItem.ItemPresentation.armorAddressableName = original.Name;
        newItem.ItemPresentation.useArmorAddressableName = true;

        return newItem;
    }

    [NotNull]
    // ReSharper disable once ReturnTypeCanBeEnumerable.Local
    private static List<ItemPropertyDescription> FilterItemProperty(
        [NotNull] IEnumerable<ItemPropertyDescription> listToFilter, BaseDefinition toFilter)
    {
        return listToFilter.Where(ip => !ip.FeatureDefinition.GUID.Equals(toFilter.GUID)).ToList();
    }
}
