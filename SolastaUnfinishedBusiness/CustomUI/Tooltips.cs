using System;
using System.Linq;
using SolastaUnfinishedBusiness.CustomBehaviors;
using TMPro;
using UnityEngine;

namespace SolastaUnfinishedBusiness.CustomUI;

internal static class Tooltips
{
    internal static GameObject TooltipInfoCharacterDescription = null;
    internal static GameObject DistanceObject = null;
    internal static GameObject DistanceTextObject;
    internal static GameObject DistanceImageObject;
    internal static GameObject CellImageObject = null;
    internal static TextMeshProUGUI TMPUGUI = null;

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

    internal static void AddDistanceToTooltip(GameLocationCharacter instance, EntityDescription entityDescription)
    {
        TooltipInfoCharacterDescription ??= GameObject.Find("TooltipFeatureCharacterDescription");

        if (TooltipInfoCharacterDescription is null || !TooltipInfoCharacterDescription.activeInHierarchy)
            return;

        if (Main.Settings.EnableDistanceOnTooltip && TooltipInfoCharacterDescription.activeInHierarchy && ServiceRepository.GetService<IGameLocationBattleService>().Battle.ActiveContender.Side == RuleDefinitions.Side.Ally)
        {
            var distance = GetDistanceToCharacter();
            entityDescription.header += "<br><br>";

            TMPUGUI ??= TooltipInfoCharacterDescription.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            CellImageObject ??= GameObject.Find("CellImage").gameObject;

            if (DistanceObject is null)
            {
                GenerateDistanceObjects(distance, TMPUGUI, CellImageObject);
            }
            else
            {
                DistanceTextObject.GetComponent<TextMeshProUGUI>().text = "Distance : " + distance.ToString();
                DistanceImageObject.transform.localPosition = GetCellIconCorrectPosition(distance);
            }

            DistanceObject?.SetActive(true);
        }
        else if (!Main.Settings.EnableDistanceOnTooltip || (GameObject.Find("TooltipFeatureCharacterDescription") && ServiceRepository.GetService<IGameLocationBattleService>().Battle.ActiveContender.Side == RuleDefinitions.Side.Enemy))
        {
            DistanceObject?.SetActive(false);
        }
    }

    private static void GenerateDistanceObjects(int distance, TextMeshProUGUI TMPUGUI, GameObject CellImageObject)
    {
        DistanceObject = new GameObject("DistanceObject");
        DistanceObject.transform.SetParent(TMPUGUI.transform);
        DistanceObject.transform.position = Vector3.zero;
        DistanceObject.transform.localPosition = new Vector3(0, -10, 0);
        DistanceTextObject = GameObject.Instantiate(TMPUGUI).gameObject;
        DistanceTextObject.name = "DistanceTextObject";
        DistanceTextObject.GetComponent<TextMeshProUGUI>().text = "Distance : " + distance.ToString();
        DistanceTextObject.transform.SetParent(DistanceObject.transform);
        DistanceTextObject.transform.position = Vector3.zero;
        DistanceTextObject.transform.localPosition = Vector3.zero;
        DistanceImageObject = GameObject.Instantiate(CellImageObject);
        DistanceImageObject.name = "DistanceImageObject";
        DistanceImageObject.transform.SetParent(DistanceObject.transform);
        DistanceImageObject.transform.position = Vector3.zero;
        DistanceImageObject.transform.localPosition = GetCellIconCorrectPosition(distance);
    }

    private static Vector3 GetCellIconCorrectPosition(int distance)
        => new Vector3(-47 + (int)distance.ToString().Length * 9, 7, 0);

    private static int GetDistanceToCharacter()
    {
        var gameLocationSelectionService = ServiceRepository.GetService<IGameLocationSelectionService>();

        if (gameLocationSelectionService.SelectedCharacters.Count is 0 || gameLocationSelectionService.HoveredCharacters.Count is 0)
            return 0;

        GameLocationCharacter selectedCharacter = gameLocationSelectionService.SelectedCharacters[0];
        GameLocationCharacter hoveredCharacter = gameLocationSelectionService.HoveredCharacters[0];

        var rawDistance = selectedCharacter.LocationPosition - hoveredCharacter.LocationPosition;
        var distance = Math.Max(Math.Max(Math.Abs(rawDistance.x), Math.Abs(rawDistance.z)), Math.Abs(rawDistance.y));

        return distance;
    }
}
