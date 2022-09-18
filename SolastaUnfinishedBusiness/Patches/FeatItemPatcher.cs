using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

internal static class FeatItemPatcher
{
    [HarmonyPatch(typeof(FeatItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        public static bool Prefix(FeatItem __instance,
            RulesetCharacterHero inspectedCharacter,
            FeatDefinition featDefinition,
            ProficiencyBaseItem.OnItemClickedHandler onItemClicked,
            ProficiencyBaseItem.OnItemHoverChangedHandler onItemHoverChanged,
            bool flexibleWidth)
        {
            var group = featDefinition.GetFirstSubFeatureOfType<IGroupedFeat>();
            if (group == null)
            {
                return true;
            }

            __instance.GuiFeatDefinition = ServiceRepository.GetService<IGuiWrapperService>()
                .GetGuiFeatDefinition(featDefinition.Name);
            __instance.Bind(inspectedCharacter, featDefinition, (subItem) =>
            {
                var selector = SubFeatSelectionModal.Get();

                selector.Bind(inspectedCharacter, __instance, group, onItemClicked, __instance.RectTransform);
                selector.Show();
                
            }, flexibleWidth);
            __instance.GuiFeatDefinition.SetupTooltip(__instance.Tooltip);
            __instance.Tooltip.Context = inspectedCharacter;
            __instance.OnItemHoverChanged = onItemHoverChanged;

            return false;
        }
    }
}