using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    internal static class CharacterInformationPanelPatcher
    {
        internal static bool TryFindChoiceFeature(CharacterInformationPanel panel, FeatureDefinition subFeature,
            out FeatureDefinition choiceFeature)
        {
            choiceFeature = null;
            foreach (FeatureUnlockByLevel featureUnlock in panel.InspectedCharacter.MainClassDefinition.FeatureUnlocks)
            {
                FeatureDefinition featureDefinition = featureUnlock.FeatureDefinition;
                if (featureDefinition is CustomFeatureDefinitionSet set && set.AllFeatures.Contains(subFeature))
                {
                    choiceFeature = featureDefinition;
                    return true;
                }
                else if (featureDefinition is FeatureDefinitionFeatureSet definitionFeatureSet &&
                         definitionFeatureSet.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion &&
                         definitionFeatureSet.FeatureSet.Contains(subFeature))
                {
                    choiceFeature = featureDefinition;
                    return true;
                }
            }

            return false;
        }

        [HarmonyPatch(typeof(CharacterInformationPanel), "EnumerateFeatures")]
        internal static class CharacterInformationPanel_EnumerateFeatures
        {
            internal static bool Prefix(
                CharacterInformationPanel __instance,
                RectTransform table,
                List<FeatureUnlockByLevel> features,
                int currentLevel,
                string insufficientLevelFormat,
                TooltipDefinitions.AnchorMode tooltipAnchorMode)
            {
                var prefab = __instance.GetField<GameObject>("featurePrefab");
                while (table.childCount < features.Count)
                {
                    Gui.GetPrefabFromPool(prefab, table);
                }

                int index = 0;
                foreach (FeatureUnlockByLevel feature in features)
                {
                    Transform child = table.GetChild(index);
                    child.gameObject.SetActive(true);
                    var label = child.GetComponent<GuiLabel>();
                    bool noLevel = feature.Level == 0;
                    string title = feature.FeatureDefinition.FormatTitle();
                    label.Text = title + (!noLevel ? $" ({feature.Level.ToString()})" : string.Empty);
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
                        tooltip.Content = feature.FeatureDefinition.FormatDescription();
                    
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
                    table.GetChild(count).gameObject.SetActive(false);
                return false;
            }
        }
    }
}
