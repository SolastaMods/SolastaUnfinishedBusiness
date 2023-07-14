using System;
using System.Linq;
using SolastaUnfinishedBusiness.CustomBehaviors;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.CustomUI;

internal static class Tooltips
{
    private static GameObject _tooltipInfoCharacterDescription;
    private static GameObject _distanceTextObject;
    private static TextMeshProUGUI _tmpUGui;

    internal static void AddContextToRecoveredFeature(RecoveredFeatureItem item, RulesetCharacterHero character)
    {
        item.GuiTooltip.Context = character;
    }

    internal static void AddContextToPowerBoxTooltip(UsablePowerBox box)
    {
        CharacterControlPanel panel = box.GetComponentInParent<CharacterControlPanelExploration>();
        panel ??= box.GetComponentInParent<CharacterControlPanelBattle>();

        if (panel != null)
        {
            box.GuiTooltip.Context = panel.GuiCharacter?.RulesetCharacter;
        }
    }

    internal static void UpdatePowerUses(ITooltip tooltip, TooltipFeaturePowerParameters parameters)
    {
        if (tooltip.DataProvider is not GuiPowerDefinition guiPowerDefinition)
        {
            return;
        }

        if (tooltip.Context is not RulesetCharacter character)
        {
            return;
        }

        var power = guiPowerDefinition.PowerDefinition;
        var usesLabel = parameters.usesLabel;

        usesLabel.Text = FormatUses(power, character, usesLabel.Text);
    }

    private static string FormatUses(FeatureDefinitionPower power, RulesetCharacter character, string def)
    {
        if (power.UsesDetermination != RuleDefinitions.UsesDetermination.Fixed)
        {
            return def;
        }

        if (power.RechargeRate == RuleDefinitions.RechargeRate.AtWill)
        {
            return def;
        }

        if (power.CostPerUse == 0)
        {
            return def;
        }

        var usablePower = UsablePowersProvider.Get(power, character);
        var maxUses = PowerBundle.GetMaxUsesForPool(usablePower, character);
        var remainingUses = character.GetRemainingUsesOfPower(usablePower);

        return $"{remainingUses}/{maxUses}";
    }

    internal static void UpdateCraftingTooltip(TooltipFeatureDescription description, ITooltip tooltip)
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

            description.DescriptionLabel.Text = Gui.Format(contentFragmentDescription.Text,
                guiRecipeDefinition.Title, guiRecipeDefinition.IngredientsText);
        }
    }

    internal static void AddDistanceToTooltip(EntityDescription entityDescription)
    {
        _tooltipInfoCharacterDescription ??= GameObject.Find("TooltipFeatureCharacterDescription");

        if (_tooltipInfoCharacterDescription is null)
        {
            return;
        }

        if (Main.Settings.EnableDistanceOnTooltip &&
            ServiceRepository.GetService<IGameLocationBattleService>().Battle?.ActiveContender?.Side ==
            RuleDefinitions.Side.Ally)
        {
            entityDescription.header += "<br><br>";

            var distance = GetDistanceToCharacter();

            // don't use ? on a type deriving from an unity object
            if (_tooltipInfoCharacterDescription != null)
            {
                _tmpUGui ??= _tooltipInfoCharacterDescription.transform.GetComponentInChildren<TextMeshProUGUI>();
            }

            if (_distanceTextObject == null)
            {
                GenerateDistanceText(distance, _tmpUGui);
            }
            else
            {
                UpdateDistanceText(distance);
            }

            // don't use ? on a type deriving from an unity object
            if (_distanceTextObject != null)
            {
                _distanceTextObject.SetActive(true);
            }
        }
        else if (!Main.Settings.EnableDistanceOnTooltip ||
                 ServiceRepository.GetService<IGameLocationBattleService>().Battle?.ActiveContender?.Side ==
                 RuleDefinitions.Side.Enemy)
        {
            // don't use ? on a type deriving from an unity object
            if (_distanceTextObject != null)
            {
                _distanceTextObject.SetActive(false);
            }
        }
    }

    private static void GenerateDistanceText(int distance, TextMeshProUGUI tmpUGui)
    {
        var anchorObject = new GameObject();

        anchorObject.transform.SetParent(tmpUGui.transform);
        anchorObject.transform.localPosition = Vector3.zero;
        _distanceTextObject = Object.Instantiate(tmpUGui).gameObject;
        _distanceTextObject.name = "DistanceTextObject";
        _distanceTextObject.transform.SetParent(anchorObject.transform);
        _distanceTextObject.transform.position = Vector3.zero;
        _distanceTextObject.transform.localPosition = new Vector3(0, -10, 0);

        UpdateDistanceText(distance);
    }

    private static int GetDistanceToCharacter()
    {
        var gameLocationSelectionService = ServiceRepository.GetService<IGameLocationSelectionService>();

        if (gameLocationSelectionService.SelectedCharacters.Count is 0 ||
            gameLocationSelectionService.HoveredCharacters.Count is 0)
        {
            return 0;
        }

        var selectedCharacter = gameLocationSelectionService.SelectedCharacters[0];
        var hoveredCharacter = gameLocationSelectionService.HoveredCharacters[0];
        var rawDistance = selectedCharacter.LocationPosition - hoveredCharacter.LocationPosition;
        var distance = Math.Max(Math.Max(Math.Abs(rawDistance.x), Math.Abs(rawDistance.z)), Math.Abs(rawDistance.y));

        return distance;
    }

    private static void UpdateDistanceText(int distance)
    {
        _distanceTextObject.GetComponent<TextMeshProUGUI>().text =
            Gui.Format("UI/&DistanceFormat", Gui.FormatDistance(distance));
    }
}
