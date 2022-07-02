using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using TA;

namespace SolastaCommunityExpansion.Models;

public static class RespecContext
{
    private const RestActivityDefinition.ActivityCondition ActivityConditionDisabled =
        (RestActivityDefinition.ActivityCondition)(-1001);

    private const string LevelDownName = "LevelDown";
    private const string RespecName = "Respec";

    private static RestActivityDefinition RestActivityLevelDown { get; } = RestActivityDefinitionBuilder
        .Create(LevelDownName, "fdb4d86eaef942d1a22dbf1fb5a7299f")
        .SetGuiPresentation("MainMenu/&ExportPdfTitle", "MainMenu/&ExportPdfDescription")
        .SetRestData(
            RestDefinitions.RestStage.AfterRest, RuleDefinitions.RestType.LongRest,
            RestActivityDefinition.ActivityCondition.None, LevelDownName, string.Empty)
        .AddToDB();

    private static RestActivityDefinition RestActivityRespec { get; } = RestActivityDefinitionBuilder
        .Create(RespecName, "40824029eb224fb581f0d4e5989b6735")
        .SetGuiPresentation("RestActivity/&ZSRespecTitle", "RestActivity/&ZSRespecDescription")
        .SetRestData(
            RestDefinitions.RestStage.AfterRest, RuleDefinitions.RestType.LongRest,
            RestActivityDefinition.ActivityCondition.None, RespecName, string.Empty)
        .AddToDB();

    internal static void Load()
    {
        _ = RestActivityLevelDown;
        _ = RestActivityRespec;

        ServiceRepository.GetService<IFunctorService>()
            .RegisterFunctor(LevelDownName, new LevelDownContext.FunctorLevelDown());
        ServiceRepository.GetService<IFunctorService>().RegisterFunctor(RespecName, new FunctorRespec());

        Switch();
    }

    internal static void Switch()
    {
        if (Main.Settings.EnableRespec)
        {
            RestActivityLevelDown.condition = RestActivityDefinition.ActivityCondition.None;
            RestActivityRespec.condition = RestActivityDefinition.ActivityCondition.None;
        }
        else
        {
            RestActivityLevelDown.condition = ActivityConditionDisabled;
            RestActivityRespec.condition = ActivityConditionDisabled;
        }
    }

    public class FunctorRespec : Functor
    {
        internal static bool IsRespecing { get; set; }

        public override IEnumerator Execute(FunctorParametersDescription functorParameters,
            FunctorExecutionContext context)
        {
            var gameLocationScreenExploration = Gui.GuiService.GetScreen<GameLocationScreenExploration>();
            var gameLocationscreenExplorationVisible =
                gameLocationScreenExploration && gameLocationScreenExploration.Visible;

            if (Global.IsMultiplayer || !gameLocationscreenExplorationVisible)
            {
                Gui.GuiService.ShowMessage(
                    MessageModal.Severity.Informative1,
                    "RestActivity/&ZSRespecTitle", "Message/&RespecMultiplayerAbortDescription",
                    "Message/&MessageOkTitle", string.Empty,
                    null, null);

                yield break;
            }

            IsRespecing = true;

            var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();
            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
            var oldHero = functorParameters.RestingHero;
            var newHero = characterBuildingService.CreateNewCharacter().HeroCharacter;

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

            IsRespecing = !hero.TryGetHeroBuildingData(out var _);
        }

        private static void FinalizeRespec(RulesetCharacterHero oldHero, RulesetCharacterHero newHero)
        {
            var guid = oldHero.Guid;
            var tags = oldHero.Tags;
            var experience = oldHero.GetAttribute(AttributeDefinitions.Experience);
            var gameCampaignCharacters = Gui.GameCampaign.Party.CharactersList;
            var gameLocationCharacterService =
                ServiceRepository.GetService<IGameLocationCharacterService>() as GameLocationCharacterManager;
            var worldLocationEntityFactoryService =
                ServiceRepository.GetService<IWorldLocationEntityFactoryService>();
            var gameLocationCharacter =
                gameLocationCharacterService.PartyCharacters.Find(x => x.RulesetCharacter == oldHero);

            newHero.guid = guid;
            newHero.Tags.AddRange(tags);
            newHero.Attributes[AttributeDefinitions.Experience] = experience;

            CopyInventoryOver(oldHero, newHero, gameLocationCharacter.LocationPosition);

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

            IsRespecing = false;
        }

        private static void CopyInventoryOver(RulesetCharacterHero oldHero, RulesetCharacterHero newHero,
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

        private static void UpdateRestPanelUi(List<GameCampaignCharacter> gameCampaignCharacters)
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
