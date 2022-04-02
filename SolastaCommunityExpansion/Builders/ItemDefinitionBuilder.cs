using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.ItemDefinitions;

namespace SolastaCommunityExpansion.Builders
{
    public class ItemDefinitionBuilder : DefinitionBuilder<ItemDefinition, ItemDefinitionBuilder>
    {
        #region Constructors
        protected ItemDefinitionBuilder(ItemDefinition original) : base(original)
        {
        }

        protected ItemDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected ItemDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected ItemDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected ItemDefinitionBuilder(ItemDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected ItemDefinitionBuilder(ItemDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected ItemDefinitionBuilder(ItemDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public ItemDefinitionBuilder SetDocumentInformation(RecipeDefinition recipeDefinition, params ContentFragmentDescription[] contentFragments)
        {
            SetDocumentInformation(recipeDefinition, contentFragments.AsEnumerable());
            return this;
        }

        public ItemDefinitionBuilder SetDocumentInformation(RecipeDefinition recipeDefinition, IEnumerable<ContentFragmentDescription> contentFragments)
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
            Definition.SetCosts(new int[] { 0, gold, 0, 0, 0 });
            return this;
        }

        public ItemDefinitionBuilder SetCosts(int[] costs)
        {
            Definition.SetCosts(costs);
            return this;
        }

        public ItemDefinitionBuilder MakeMagical()
        {
            Definition.ItemTags.Remove("Standard");
            Definition.SetMagical(true);
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
            Definition.SetUsableDeviceDescription(usableDescription);
            return this;
        }

        public ItemDefinitionBuilder SetUsableDeviceDescription(params FeatureDefinitionPower[] functions)
        {
            return SetUsableDeviceDescription(functions.AsEnumerable());
        }

        public ItemDefinitionBuilder SetUsableDeviceDescription(IEnumerable<FeatureDefinitionPower> functions)
        {
            Definition.IsUsableDevice = true;
            Definition.SetUsableDeviceDescription(new UsableDeviceDescription());
            Definition.UsableDeviceDescription.DeviceFunctions.Clear();

            var deviceFunction = Berry_Ration.UsableDeviceDescription.DeviceFunctions[0];

            foreach (FeatureDefinitionPower power in functions)
            {
                DeviceFunctionDescription functionDescription =
                    deviceFunction.Copy().SetType(DeviceFunctionDescription.FunctionType.Power).SetFeatureDefinitionPower(power);
                Definition.UsableDeviceDescription.DeviceFunctions.Add(functionDescription);
            }
            return this;
        }
    }
}
