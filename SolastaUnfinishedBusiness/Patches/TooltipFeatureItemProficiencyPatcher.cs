using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class TooltipFeatureItemProficiencyPatcher
{
    [HarmonyPatch(typeof(TooltipFeatureItemProficiency), nameof(TooltipFeatureItemProficiency.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureItemProficiency __instance, ITooltip tooltip)
        {
            var obj = __instance.gameObject;
            if (obj.activeSelf)
            {
                return;
            }

            if (tooltip.DataProvider is not IItemDefinitionProvider data)
            {
                return;
            }

            if (!RecipeHelper.RecipeIsKnown(data.ItemDefinition))
            {
                return;
            }

            __instance.notProficientLabel.Text = "Failure/&FailureFlagRecipeAlreadyKnown";
            obj.SetActive(true);
        }
    }
}
