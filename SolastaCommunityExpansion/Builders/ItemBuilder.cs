using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    internal static class ItemBuilder
    {
        public static ItemDefinition BuilderCopyFromItemSetRecipe(Guid collectionGuid, RecipeDefinition recipeDefinition, ItemDefinition toCopy, string name, GuiPresentation guiPresentation, int gold)
        {
            return ItemDefinitionBuilder
                .Create(toCopy, name, collectionGuid)
                .SetDocumentInformation(recipeDefinition, toCopy.DocumentDescription.ContentFragments)
                .SetGuiPresentation(guiPresentation)
                .SetGold(gold)
                .AddToDB();
        }

        public static ItemDefinition BuildNewMagicWeapon(Guid collectionGuid, ItemDefinition baseItem, ItemDefinition magicalExample, string name)
        {
            string itemName = "Enchanted_" + baseItem.Name + "_" + name;
            ItemDefinitionBuilder builder = new ItemDefinitionBuilder(baseItem, itemName, GuidHelper.Create(collectionGuid, itemName).ToString());
            // Set is magical
            // Remove "Standard" from item tags
            builder.MakeMagical();
            // Copy over static properties from example enchanted
            builder.SetStaticProperties(magicalExample.StaticProperties);
            if (magicalExample.IsUsableDevice)
            {
                builder.SetUsableDeviceDescription(magicalExample.UsableDeviceDescription);
            }
            // If example enchated has multiple forms, copy over extra forms
            if (magicalExample.WeaponDescription.EffectDescription.EffectForms.Count > 1)
            {
                for (int i = 1; i < magicalExample.WeaponDescription.EffectDescription.EffectForms.Count; i++)
                {
                    builder.AddWeaponEffect(magicalExample.WeaponDescription.EffectDescription.EffectForms[i]);
                }
            }
            // Set new GuiPresentation
            builder.SetGuiTitleAndDescription("Equipment/&" + itemName + "_Title",
                "Equipment/&" + itemName + "_Description");
            // Copy over price from example enchanted
            builder.SetCosts(magicalExample.Costs);
            return builder.AddToDB();
        }

        public static ItemDefinition BuildNewMagicArmor(Guid collectionGuid, ItemDefinition baseItem, ItemDefinition magicalExample, string name)
        {
            string itemName = "Enchanted_" + baseItem.Name + "_" + name;
            ItemDefinitionBuilder builder = new ItemDefinitionBuilder(baseItem, itemName, GuidHelper.Create(collectionGuid, itemName).ToString());
            // Set is magical
            // Remove "Standard" from item tags
            builder.MakeMagical();
            // Copy over static properties from example enchanted, but remove stealth disadvantage since that is determined by the armor and not the enchantment.
            builder.MergeStaticProperties(FilterItemProperty(magicalExample.StaticProperties, DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityStealthDisadvantage));
            if (magicalExample.IsUsableDevice)
            {
                builder.SetUsableDeviceDescription(magicalExample.UsableDeviceDescription);
            }
            // Set new GuiPresentation
            builder.SetGuiTitleAndDescription("Equipment/&" + itemName + "_Title",
                "Equipment/&" + itemName + "_Description");
            // Copy over price from example enchanted
            builder.SetCosts(magicalExample.Costs);
            return builder.AddToDB();
        }

        private static List<ItemPropertyDescription> FilterItemProperty(List<ItemPropertyDescription> listToFilter, FeatureDefinition toFilter)
        {
            return listToFilter.Where(ip => !ip.FeatureDefinition.GUID.Equals(toFilter.GUID)).ToList();
        }

        public static ItemDefinition CopyFromItemSetFunctions(Guid baseGuid, List<FeatureDefinitionPower> functions, ItemDefinition toCopy, string name, GuiPresentation guiPresentation)
        {
            ItemDefinitionBuilder builder = new ItemDefinitionBuilder(toCopy, name, GuidHelper.Create(baseGuid, name).ToString());

            // Set new GuiPresentation
            builder.SetGuiTitleAndDescription(guiPresentation.Title, guiPresentation.Description);
            builder.SetUsableDeviceDescription(functions);

            return builder.AddToDB();
        }
    }
}
