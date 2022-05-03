using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomUI;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    [HarmonyPatch(typeof(CharacterInformationPanel), "EnumerateFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterInformationPanel_EnumerateFeatures
    {
        private static bool TryFindChoiceFeature(
            CharacterInformationPanel panel,
            FeatureDefinition subFeature,
            out FeatureDefinition choiceFeature)
        {
            choiceFeature = null;

            foreach (var featureUnlock in panel.InspectedCharacter.MainClassDefinition.FeatureUnlocks)
            {
                var featureDefinition = featureUnlock.FeatureDefinition;

                if (featureDefinition is CustomFeatureDefinitionSet set && set.AllFeatures.Contains(subFeature))
                {
                    choiceFeature = featureDefinition;

                    return true;
                }
                else if (featureDefinition is FeatureDefinitionFeatureSet definitionFeatureSet
                    && definitionFeatureSet.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion
                    && definitionFeatureSet.FeatureSet.Contains(subFeature))
                {
                    choiceFeature = featureDefinition;

                    return true;
                }
            }

            return false;
        }

        internal static bool Prefix(
            CharacterInformationPanel __instance,
            RectTransform table,
            List<FeatureUnlockByLevel> features,
            string insufficientLevelFormat,
            TooltipDefinitions.AnchorMode tooltipAnchorMode,
            GameObject ___featurePrefab)
        {
            while (table.childCount < features.Count)
            {
                Gui.GetPrefabFromPool(___featurePrefab, table);
            }

            int index = 0;

            foreach (var feature in features)
            {
                var child = table.GetChild(index);

                child.gameObject.SetActive(true);

                var label = child.GetComponent<GuiLabel>();
                var noLevel = feature.Level == 0;
                var title = feature.FeatureDefinition.FormatTitle();

                label.Text = title + (!noLevel ? $" ({feature.Level})" : string.Empty);
                Gui.HexaKeyToColor(noLevel ? Gui.ColorAlmostWhite : Gui.ColorNegative, out var color);
                label.TMP_Text.color = color;

                var tooltip = child.GetComponent<GuiTooltip>();
                var provider = new CustomTooltipProvider(feature.FeatureDefinition, null);

                if (feature.FeatureDefinition is FeatureDefinitionPower)
                {
                    GuiPowerDefinition guiPowerDefinition = ServiceRepository.GetService<IGuiWrapperService>()
                        .GetGuiPowerDefinition(feature.FeatureDefinition.Name);
                    tooltip.Content = guiPowerDefinition.Description;
                }
                else if (TryFindChoiceFeature(__instance, feature.FeatureDefinition, out var choiceFeature))
                {
                    label.Text = Gui.Format("{1} ({0})", choiceFeature.FormatTitle(), 
                        feature.FeatureDefinition.FormatTitle());
                    tooltip.Content = feature.FeatureDefinition.FormatDescription();
                    provider.SetSubtitle(choiceFeature.GuiPresentation.Title);
                }
                else
                {
                    tooltip.Content = feature.FeatureDefinition.FormatDescription();
                }

                tooltip.TooltipClass = "FeatDefinition";
                tooltip.DataProvider = provider;
                tooltip.Context = __instance.InspectedCharacter?.RulesetCharacter;

                tooltip.AnchorMode = tooltipAnchorMode;

                if (!noLevel)
                {
                    var levelRequirement = Gui.Format(insufficientLevelFormat, feature.Level.ToString());

                    provider.SetPrerequisites(levelRequirement);
                }

                ++index;
            }

            for (int count = features.Count; count < table.childCount; ++count)
            {
                table.GetChild(count).gameObject.SetActive(false);
            }

            return false;
        }
    }
}
