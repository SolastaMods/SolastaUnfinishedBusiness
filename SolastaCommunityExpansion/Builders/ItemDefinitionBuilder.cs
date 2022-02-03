using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders
{
    public class ItemDefinitionBuilder : BaseDefinitionBuilder<ItemDefinition>
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
}
