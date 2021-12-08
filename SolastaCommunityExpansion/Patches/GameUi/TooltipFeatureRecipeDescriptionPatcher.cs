using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi
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
            if (tooltip.DataProvider is IItemDefinitionProvider)
            {
                ItemDefinition item = (tooltip.DataProvider as IItemDefinitionProvider).ItemDefinition;
                if (item.IsDocument && item.DocumentDescription.LoreType == RuleDefinitions.LoreType.CraftingRecipe)
                {
                    foreach (ContentFragmentDescription contentFragmentDescription in item.DocumentDescription.ContentFragments)
                    {
                        if (contentFragmentDescription.Type == ContentFragmentDescription.FragmentType.Body)
                        {
                            GuiRecipeDefinition guiRecipeDefinition = ServiceRepository.GetService<IGuiWrapperService>().GetGuiRecipeDefinition(item.DocumentDescription.RecipeDefinition.Name);

                            __instance.DescriptionLabel.Text = Gui.Format(contentFragmentDescription.Text, new string[]
                            {
                                guiRecipeDefinition.Title,
                                guiRecipeDefinition.IngredientsText
                            });
                        }
                    }
                }
            }
        }
    }
}
