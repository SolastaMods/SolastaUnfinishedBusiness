using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FeatItemPatcher
{
    [HarmonyPatch(typeof(FeatItem), nameof(FeatItem.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            FeatItem __instance,
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
            __instance.Bind(
                inspectedCharacter,
                featDefinition,
                _ =>
                {
                    var selector = SubFeatSelectionModal.Get();

                    selector.Cancel();
                    selector.Bind(inspectedCharacter, __instance, featDefinition, group, onItemClicked,
                        __instance.RectTransform);
                    selector.Show();
                },
                flexibleWidth);
            __instance.GuiFeatDefinition.SetupTooltip(__instance.Tooltip, inspectedCharacter);
            __instance.OnItemHoverChanged = onItemHoverChanged;
            SubFeatSelectionModal.SetColor(__instance, SubFeatSelectionModal.HeaderColor);

            return false;
        }
    }

    [HarmonyPatch(typeof(FeatItem), nameof(FeatItem.Unbind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Unbind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(FeatItem __instance)
        {
            //PATCH: sets FeatItem's color back to default
            SubFeatSelectionModal.SetColor(__instance, SubFeatSelectionModal.DefaultColor);
        }
    }
}
