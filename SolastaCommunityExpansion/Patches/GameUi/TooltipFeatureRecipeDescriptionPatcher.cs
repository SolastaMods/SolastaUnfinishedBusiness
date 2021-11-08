using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.GameUi
{

    [HarmonyPatch(typeof(TooltipFeatureDescription), "Bind")]
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
                            string text = string.Empty;
                            GuiRecipeDefinition guiRecipeDefinition = ServiceRepository.GetService<IGuiWrapperService>().GetGuiRecipeDefinition(item.DocumentDescription.RecipeDefinition.Name);
                            text = Gui.Format(contentFragmentDescription.Text, new string[]
                            {
                                guiRecipeDefinition.Title,
                                guiRecipeDefinition.IngredientsText
                            });
                            __instance.DescriptionLabel.Text = text;
                        }
                    }
                }
            }
        }
    }
}
