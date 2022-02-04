using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders
{
    public class ItemDefinitionBuilder : BaseDefinitionBuilder<ItemDefinition>
    {
        public ItemDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public ItemDefinitionBuilder(string name, Guid namespaceGuid, Category category)
            : base(name, namespaceGuid, category)
        {
        }

        public ItemDefinitionBuilder(ItemDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public ItemDefinitionBuilder(ItemDefinition original, string name, Guid namespaceGuid, Category category)
            : base(original, name, namespaceGuid, category)
        {
        }

        public void SetDocumentInformation(RecipeDefinition recipeDefinition, params ContentFragmentDescription[] contentFragments)
        {
            SetDocumentInformation(recipeDefinition, contentFragments.AsEnumerable());
        }

        public void SetDocumentInformation(RecipeDefinition recipeDefinition, IEnumerable<ContentFragmentDescription> contentFragments)
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
            Definition.DocumentDescription.ContentFragments.SetRange(contentFragments);
        }

        public void SetGuiTitleAndDescription(string title, string description)
        {
            Definition.GuiPresentation.Title = title;
            Definition.GuiPresentation.Description = description;
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

        public void SetStaticProperties(IEnumerable<ItemPropertyDescription> staticProperties)
        {
            Definition.StaticProperties.SetRange(staticProperties);
        }

        public void MergeStaticProperties(IEnumerable<ItemPropertyDescription> staticProperties)
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

        public void SetUsableDeviceDescription(IEnumerable<FeatureDefinitionPower> functions)
        {
            Definition.IsUsableDevice = true;
            Definition.SetUsableDeviceDescription(new UsableDeviceDescription());
            Definition.UsableDeviceDescription.DeviceFunctions.Clear();
            foreach (FeatureDefinitionPower power in functions)
            {
                DeviceFunctionDescription functionDescription = new DeviceFunctionDescription(DatabaseHelper.ItemDefinitions.Berry_Ration.UsableDeviceDescription.DeviceFunctions[0]);
                functionDescription.SetType(DeviceFunctionDescription.FunctionType.Power);
                functionDescription.SetFeatureDefinitionPower(power);
                Definition.UsableDeviceDescription.DeviceFunctions.Add(functionDescription);
            }
        }
    }
}
