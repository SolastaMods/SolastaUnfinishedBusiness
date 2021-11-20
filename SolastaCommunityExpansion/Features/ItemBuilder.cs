using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Features
{
    internal static class ItemBuilder
    {
        private class ItemDefinitionBuilder : BaseDefinitionBuilder<ItemDefinition>
        {
            public ItemDefinitionBuilder(ItemDefinition original, string name, string guid) : base(original, name, guid)
            {
            }

            public void SetDocumentInformation(RecipeDefinition recipeDefinition, List<ContentFragmentDescription> contentFragments)
            {
                if (Definition.DocumentDescription == null)
                {
                    Definition.SetDocumentDescription(new DocumentDescription());
                }
                Definition.IsDocument = true;
                Definition.DocumentDescription.SetRecipeDefinition(recipeDefinition);
                Definition.DocumentDescription.SetLoreType(RuleDefinitions.LoreType.CraftingRecipe);
                Definition.DocumentDescription.SetDestroyAfterReading(true);
                Definition.DocumentDescription.SetLocationKnowledgeLevel(GameCampaignDefinitions.NodeKnowledge.Known);
                Definition.DocumentDescription.SetField("contentFragments", contentFragments);
            }

            public void SetGuiTitleAndDescription(string title, string description)
            {
                Definition.GuiPresentation.Title = title;
                Definition.GuiPresentation.Description = description;
            }

            public void SetGuiPresentation(GuiPresentation guiPresentation)
            {
                Definition.SetGuiPresentation(guiPresentation);
            }

            public void SetGold(int gold)
            {
                Definition.SetCosts(new int[] { 0, gold, 0, 0, 0 });
            }

            public void SetCosts(int[] costs)
            {
                Definition.SetCosts(costs);
            }

            public void MakeMagical()
            {
                Definition.ItemTags.Remove("Standard");
                Definition.SetMagical(true);
            }

            public void SetStaticProperties(List<ItemPropertyDescription> staticProperties)
            {
                Definition.SetField("staticProperties", staticProperties);
            }

            public void MergeStaticProperties(List<ItemPropertyDescription> staticProperties)
            {
                Definition.StaticProperties.AddRange(staticProperties);
            }

            public void AddWeaponEffect(EffectForm effect)
            {
                Definition.WeaponDescription.EffectDescription.EffectForms.Add(effect);
            }

            public void SetUsableDeviceDescription(UsableDeviceDescription usableDescription)
            {
                Definition.IsUsableDevice = true;
                Definition.SetUsableDeviceDescription(usableDescription);
            }

            public void SetUsableDeviceDescription(List<FeatureDefinitionPower> functions)
            {
                Definition.IsUsableDevice = true;
                Definition.SetUsableDeviceDescription(new UsableDeviceDescription());
                Definition.UsableDeviceDescription.SetField("deviceFunctions", new List<DeviceFunctionDescription>());
                foreach (FeatureDefinitionPower power in functions)
                {
                    DeviceFunctionDescription functionDescription = new DeviceFunctionDescription(DatabaseHelper.ItemDefinitions.Berry_Ration.UsableDeviceDescription.DeviceFunctions[0]);
                    functionDescription.SetType(DeviceFunctionDescription.FunctionType.Power);
                    functionDescription.SetFeatureDefinitionPower(power);
                    Definition.UsableDeviceDescription.DeviceFunctions.Add(functionDescription);
                }
            }
        }

        public static ItemDefinition BuilderCopyFromItemSetRecipe(Guid collectionGuid, RecipeDefinition recipeDefinition, ItemDefinition toCopy, string name, GuiPresentation guiPresentation, int gold)
        {
            ItemDefinitionBuilder builder = new ItemDefinitionBuilder(toCopy, name, GuidHelper.Create(collectionGuid, name).ToString());
            builder.SetDocumentInformation(recipeDefinition, toCopy.DocumentDescription.ContentFragments);
            builder.SetGuiPresentation(guiPresentation);
            builder.SetGold(gold);
            return builder.AddToDB();
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