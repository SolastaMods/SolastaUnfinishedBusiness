using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using TA;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Models;

internal static class ToolsContext
{
    internal const int GamePartySize = 4;

    internal const int MinPartySize = 1;
    internal const int MaxPartySize = 6;

    internal const float CustomScale = 0.85f;

    internal const int DungeonMinLevel = 1;
    internal const int DungeonMaxLevel = 20;

    private const RestActivityDefinition.ActivityCondition ActivityConditionDisabled =
        (RestActivityDefinition.ActivityCondition)(-1001);

    private const string RespecName = "RestActivityRespec";

    private static List<string> BuiltInHeroNames { get; } = new();

    private static RestActivityDefinition RestActivityRespec { get; } = RestActivityDefinitionBuilder
        .Create(RespecName)
        .SetGuiPresentation(Category.RestActivity)
        .SetRestData(
            RestDefinitions.RestStage.AfterRest,
            RuleDefinitions.RestType.LongRest,
            RestActivityDefinition.ActivityCondition.None,
            RespecName,
            string.Empty)
        .AddToDB();

    internal static bool IsBuiltIn(string name)
    {
        return BuiltInHeroNames.Contains(name);
    }

    internal static void Load()
    {
        var gameBuiltInCharactersDirectory = TacticalAdventuresApplication.GameBuiltInCharactersDirectory;

        BuiltInHeroNames.AddRange(Directory
            .GetFiles(gameBuiltInCharactersDirectory)
            .Select(x => Path
                .GetFileName(x)
                .Replace(".chr", "")));

        ServiceRepository.GetService<IFunctorService>().RegisterFunctor(RespecName, new FunctorRespec());
        SwitchRespec();
        SwitchEncounterPercentageChance();
    }

    internal static void SwitchRespec()
    {
        RestActivityRespec.condition = Main.Settings.EnableRespec
            ? RestActivityDefinition.ActivityCondition.None
            : ActivityConditionDisabled;
    }

    internal static void SwitchEncounterPercentageChance()
    {
        foreach (var travelEventProbabilityDescription in DatabaseRepository.GetDatabase<TravelActivityDefinition>()
                     .SelectMany(x => x.RandomEvents)
                     .Where(x => x.EventDefinition.Name == "TravelEventEncounter"))
        {
            travelEventProbabilityDescription.basePercent = Main.Settings.EncounterPercentageChance;
        }
    }

    public static void Rebase(Transform parent, int max)
    {
        while (Main.Settings.DefaultPartyHeroes.Count > max)
        {
            var heroToDelete = Main.Settings.DefaultPartyHeroes.ElementAt(0);

            var child = parent.FindChildRecursive(heroToDelete);

            if (child)
            {
                child.GetComponentInChildren<Toggle>().isOn = false;
            }
        }
    }

    public static Transform CreateHeroCheckbox(Component character)
    {
        // ReSharper disable once Unity.UnknownResource
        var settingCheckboxItem = Resources.Load<GameObject>("Gui/Prefabs/Modal/Setting/SettingCheckboxItem");
        var smallToggleNoFrame = settingCheckboxItem.transform.Find("SmallToggleNoFrame");
        var checkBox = Object.Instantiate(smallToggleNoFrame, character.transform);
        var checkBoxRect = checkBox.GetComponent<RectTransform>();

        checkBox.name = "DefaultHeroToggle";
        checkBox.gameObject.SetActive(true);
        checkBox.Find("Background").gameObject.AddComponent<GuiTooltip>();

        checkBoxRect.anchoredPosition = new Vector2(160, 40);

        return checkBox;
    }

    internal static void Disable(RectTransform charactersTable)
    {
        for (var i = 0; i < charactersTable.childCount; i++)
        {
            var character = charactersTable.GetChild(i);
            var checkBoxToggle = character.GetComponentInChildren<Toggle>();

            if (checkBoxToggle)
            {
                checkBoxToggle.gameObject.SetActive(false);
            }
        }
    }

    internal sealed class FunctorRespec : Functor
    {
        internal static bool IsRespecing { get; private set; }
        internal static string OldHeroName { get; private set; }

        public override IEnumerator Execute(FunctorParametersDescription functorParameters,
            FunctorExecutionContext context)
        {
            var gameLocationScreenExploration = Gui.GuiService.GetScreen<GameLocationScreenExploration>();
            var gameLocationScreenExplorationVisible =
                gameLocationScreenExploration && gameLocationScreenExploration.Visible;

            if (Global.IsMultiplayer || !gameLocationScreenExplorationVisible)
            {
                Gui.GuiService.ShowMessage(
                    MessageModal.Severity.Informative1,
                    "RestActivity/&RestActivityRespecTitle", "Message/&RespecMultiplayerAbortDescription",
                    "Message/&MessageOkTitle", string.Empty,
                    null, null);

                yield break;
            }

            IsRespecing = true;

            var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();
            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
            var oldHero = functorParameters.RestingHero;
            var newHero = characterBuildingService.CreateNewCharacter().HeroCharacter;

            OldHeroName = oldHero.Name;

            guiConsoleScreen.Hide(true);
            gameLocationScreenExploration.Hide(true);

            yield return StartRespec(newHero);

            if (IsRespecing)
            {
                FinalizeRespec(oldHero, newHero);
            }

            guiConsoleScreen.Show();
            gameLocationScreenExploration.Show();
        }

        private static IEnumerator StartRespec(RulesetCharacterHero hero)
        {
            var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
            var restModalScreen = Gui.GuiService.GetScreen<RestModal>();

            restModalScreen.KeepCurrentState = true;
            restModalScreen.Hide(true);
            characterCreationScreen.OriginScreen = restModalScreen;
            characterCreationScreen.CurrentHero = hero;
            characterCreationScreen.Show();

            while (characterCreationScreen.isActiveAndEnabled)
            {
                yield return null;
            }

            IsRespecing = !hero.TryGetHeroBuildingData(out _);
        }

        private static void FinalizeRespec([NotNull] RulesetCharacter oldHero,
            [NotNull] RulesetCharacter newHero)
        {
            var guid = oldHero.Guid;
            var tags = oldHero.Tags;
            var experience = oldHero.GetAttribute(AttributeDefinitions.Experience);
            var gameCampaignCharacters = Gui.GameCampaign.Party.CharactersList;
            var gameLocationCharacterService =
                ServiceRepository.GetService<IGameLocationCharacterService>() as GameLocationCharacterManager;
            var worldLocationEntityFactoryService =
                ServiceRepository.GetService<IWorldLocationEntityFactoryService>();

            if (gameLocationCharacterService != null)
            {
                var gameLocationCharacter =
                    gameLocationCharacterService.PartyCharacters.Find(x => x.RulesetCharacter == oldHero);

                newHero.guid = guid;
                newHero.Tags.AddRange(tags);
                newHero.Attributes[AttributeDefinitions.Experience] = experience;

                CopyInventoryOver(oldHero, gameLocationCharacter.LocationPosition);

                gameCampaignCharacters.Find(x => x.RulesetCharacter == oldHero).RulesetCharacter = newHero;

                UpdateRestPanelUi(gameCampaignCharacters);

                gameLocationCharacter.SetRuleset(newHero);

                if (worldLocationEntityFactoryService.TryFindWorldCharacter(gameLocationCharacter,
                        out var worldLocationCharacter))
                {
                    worldLocationCharacter.GraphicsCharacter.RulesetCharacter = newHero;
                }

                gameLocationCharacterService.dirtyParty = true;
                gameLocationCharacterService.RefreshAllCharacters();
            }

            Gui.GuiService.ShowMessage(
                MessageModal.Severity.Informative1,
                "RestActivity/&RestActivityRespecTitle", "Message/&RespecSuccessfulDescription",
                "Message/&MessageOkTitle", string.Empty,
                null, null);

            IsRespecing = false;
        }

        private static void CopyInventoryOver([NotNull] RulesetCharacter oldHero,
            int3 position)
        {
            var inventoryCommandService = ServiceRepository.GetService<IInventoryCommandService>();
            var personalSlots = oldHero.CharacterInventory.PersonalContainer.InventorySlots;
            var slotsByName = oldHero.CharacterInventory.InventorySlotsByName;

            foreach (var equipedItem in personalSlots.Select(i => i.EquipedItem).Where(i => i != null))
            {
                equipedItem.AttunedToCharacter = string.Empty;
                inventoryCommandService.CreateItemAtPosition(equipedItem, position);
            }

            foreach (var equipedItem in slotsByName.Select(s => s.Value.EquipedItem).Where(i => i != null))
            {
                equipedItem.AttunedToCharacter = string.Empty;
                inventoryCommandService.CreateItemAtPosition(equipedItem, position);
            }
        }

        private static void UpdateRestPanelUi(IReadOnlyList<GameCampaignCharacter> gameCampaignCharacters)
        {
            var restModalScreen = Gui.GuiService.GetScreen<RestModal>();
            var restAfterPanel = restModalScreen.restAfterPanel;
            var characterPlatesTable =
                restAfterPanel.characterPlatesTable;

            for (var index = 0; index < characterPlatesTable.childCount; ++index)
            {
                var child = characterPlatesTable.GetChild(index);
                var component = child.GetComponent<CharacterPlateGame>();

                component.Unbind();

                if (index < gameCampaignCharacters.Count)
                {
                    component.Bind(gameCampaignCharacters[index].RulesetCharacter,
                        TooltipDefinitions.AnchorMode.BOTTOM_CENTER);
                    component.Refresh();
                }

                child.gameObject.SetActive(index < gameCampaignCharacters.Count);
            }
        }
    }
}
