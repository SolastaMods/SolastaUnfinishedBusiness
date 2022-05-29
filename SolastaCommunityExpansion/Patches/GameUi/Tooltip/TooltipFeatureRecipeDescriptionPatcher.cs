using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Tooltip
{
    [HarmonyPatch(typeof(TooltipFeatureDescription), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TooltipFeatureDescription_Bind
    {
        internal static void Postfix(TooltipFeatureDescription __instance, ITooltip tooltip)
        {
            if (!Main.Settings.ShowCraftingRecipeInDetailedTooltips)
            {
                return;
            }

            if (tooltip.DataProvider is not IItemDefinitionProvider itemDefinitionProvider)
            {
                return;
            }

            var item = itemDefinitionProvider.ItemDefinition;

            if (!item.IsDocument || item.DocumentDescription.LoreType != RuleDefinitions.LoreType.CraftingRecipe)
            {
                return;
            }

            var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>();

            foreach (var contentFragmentDescription in item.DocumentDescription.ContentFragments
                         .Where(x => x.Type == ContentFragmentDescription.FragmentType.Body))
            {
                var guiRecipeDefinition =
                    guiWrapperService.GetGuiRecipeDefinition(item.DocumentDescription.RecipeDefinition.Name);

                __instance.DescriptionLabel.Text = Gui.Format(contentFragmentDescription.Text,
                    guiRecipeDefinition.Title, guiRecipeDefinition.IngredientsText);
            }
        }
    }
}
