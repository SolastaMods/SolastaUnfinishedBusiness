using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FeatureDefinitionFeatureSetPatcher
{
#if false
    [HarmonyPatch(typeof(FeatureDefinitionFeatureSet), nameof(FeatureDefinitionFeatureSet.FormatDescription))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class FormatDescription_Patch
    {
        [UsedImplicitly] public static void Postfix([NotNull] FeatureDefinitionFeatureSet __instance, ref string __result)
        {
            //PATCH: improves formatting of feature sets description by including descriptions of its sub-features
            if (__instance.Mode != FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            {
                return;
            }

            var description = Gui.Localize(__instance.GuiPresentation.Description);
            var featureSet = __instance.FeatureSet.ToList();

            featureSet.RemoveAll(f => f.GuiPresentation.Hidden);

            if (!featureSet.Count == 0)
            {
                description += "\n\n" + string.Join("\n\n", featureSet.Select(f =>
                    $"{Gui.Colorize(f.FormatTitle(), Gui.ColorBrightBlue)}\n{f.FormatDescription()}"));
            }

            __result = description;
        }
    }
#endif

    //PATCH: Support ancestries custom selection UI that grant them trough invocations
    [HarmonyPatch(typeof(FeatureDefinitionFeatureSet),
        nameof(FeatureDefinitionFeatureSet.TryGetAncestryFeatureFromFeatureSetAndHeroAncestry))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TryGetAncestryFeatureFromFeatureSetAndHeroAncestry_Patch
    {
        [UsedImplicitly]
        public static void Prefix(List<FeatureDefinition> alreadyOwnedFeatures)
        {
            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
            var currentHero = characterBuildingService.CurrentLocalHeroCharacter;

            //TODO: consider recursive dive into FeatureSets if ever required
            alreadyOwnedFeatures.AddRange(currentHero.TrainedInvocations.Select(x => x.GrantedFeature));
        }
    }
}
