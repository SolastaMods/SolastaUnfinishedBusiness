using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUiTooltip
{
    [HarmonyPatch(typeof(TooltipFeatureDescription), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TooltipFeatureDescription_Bind
    {
        internal static void Postfix(TooltipFeatureDescription __instance, ITooltip tooltip)
        {
            if (!Main.Settings.RecipeTooltipShowsRecipe)
            {
                return;
            }

            if (!(tooltip.DataProvider is IItemDefinitionProvider itemDefinitionProvider))
            {
                return;
            }

            var item = itemDefinitionProvider.ItemDefinition;

            if (!item.IsDocument || item.DocumentDescription.LoreType != RuleDefinitions.LoreType.CraftingRecipe)
            {
                return;
            }

            var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>();

            foreach (ContentFragmentDescription contentFragmentDescription in item.DocumentDescription.ContentFragments
                .Where(x => x.Type == ContentFragmentDescription.FragmentType.Body))
            {
                var guiRecipeDefinition = guiWrapperService.GetGuiRecipeDefinition(item.DocumentDescription.RecipeDefinition.Name);

                __instance.DescriptionLabel.Text = Gui.Format(contentFragmentDescription.Text, new string[]
                {
                    guiRecipeDefinition.Title,
                    guiRecipeDefinition.IngredientsText
                });
            }
        }
    }
}
