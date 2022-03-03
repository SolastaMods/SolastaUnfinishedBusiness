using System;
using System.Collections.Generic;
using System.Linq;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Builders
{
    internal static class ItemBuilder
    {
        public static ItemDefinition BuilderCopyFromItemSetRecipe(ItemDefinition original, string name, Guid collectionGuid,
            RecipeDefinition recipeDefinition, int gold, GuiPresentation guiPresentation)
        {
            return ItemDefinitionBuilder
                .Create(original, name, collectionGuid)
                .SetGuiPresentation(guiPresentation)
                .SetDocumentInformation(recipeDefinition, original.DocumentDescription.ContentFragments)
                .SetGold(gold)
                .AddToDB();
        }

        public static ItemDefinition BuildNewMagicWeapon(ItemDefinition original, string name, Guid collectionGuid, ItemDefinition magicalExample)
        {
            string itemName = "Enchanted_" + original.Name + "_" + name;

            ItemDefinitionBuilder builder = ItemDefinitionBuilder
                .Create(original, itemName, collectionGuid)
                .SetGuiPresentation(itemName + "_", Category.Equipment)
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

            // If example enchated has multiple forms, copy over extra forms
            if (magicalExample.WeaponDescription.EffectDescription.EffectForms.Count > 1)
            {
                for (int i = 1; i < magicalExample.WeaponDescription.EffectDescription.EffectForms.Count; i++)
                {
                    builder.AddWeaponEffect(magicalExample.WeaponDescription.EffectDescription.EffectForms[i]);
                }
            }

            return builder.AddToDB();
        }

        public static ItemDefinition BuildNewMagicArmor(ItemDefinition original, Guid collectionGuid, string name, ItemDefinition magicalExample)
        {
            string itemName = "Enchanted_" + original.Name + "_" + name;

            ItemDefinitionBuilder builder = ItemDefinitionBuilder
                .Create(original, itemName, collectionGuid)
                .SetGuiPresentation(itemName + "_", Category.Equipment)
                // Set is magical
                // Remove "Standard" from item tags
                .MakeMagical()
                // Copy over price from example enchanted
                .SetCosts(magicalExample.Costs)
                // Copy over static properties from example enchanted, but remove stealth disadvantage since that is determined by the armor and not the enchantment.
                .MergeStaticProperties(FilterItemProperty(magicalExample.StaticProperties, FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityStealthDisadvantage));

            if (magicalExample.IsUsableDevice)
            {
                builder.SetUsableDeviceDescription(magicalExample.UsableDeviceDescription);
            }

            return builder.AddToDB();
        }

        private static List<ItemPropertyDescription> FilterItemProperty(IEnumerable<ItemPropertyDescription> listToFilter, FeatureDefinition toFilter)
        {
            return listToFilter.Where(ip => !ip.FeatureDefinition.GUID.Equals(toFilter.GUID)).ToList();
        }
    }
}
