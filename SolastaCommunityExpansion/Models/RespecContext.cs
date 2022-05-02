using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Models
{
    public static class RespecContext
    {
        private const RestActivityDefinition.ActivityCondition ActivityConditionDisabled = (RestActivityDefinition.ActivityCondition)(-1001);

        private const string LevelDownName = "LevelDown";
        private const string RespecName = "Respec";

        public static RestActivityDefinition RestActivityLevelDown { get; private set; } = RestActivityDefinitionBuilder
            .Create(LevelDownName, "fdb4d86eaef942d1a22dbf1fb5a7299f")
            .SetGuiPresentation("MainMenu/&ExportPdfTitle", "MainMenu/&ExportPdfDescription")
            .SetRestData(
                RestDefinitions.RestStage.AfterRest, RuleDefinitions.RestType.LongRest,
                RestActivityDefinition.ActivityCondition.None, LevelDownName, string.Empty)
            .AddToDB();

        public static RestActivityDefinition RestActivityRespec { get; private set; } = RestActivityDefinitionBuilder
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

            ServiceRepository.GetService<IFunctorService>().RegisterFunctor(RespecName, new FunctorRespec());

            Switch();
        }

        internal static void Switch()
        {
            if (Main.Settings.EnableRespec)
            {
                if (Main.Settings.EnableMulticlass)
                {
                    RestActivityLevelDown.SetCondition(RestActivityDefinition.ActivityCondition.None);
                }

                RestActivityRespec.SetCondition(RestActivityDefinition.ActivityCondition.None);
            }
            else
            {
                RestActivityLevelDown.SetCondition(ActivityConditionDisabled);
                RestActivityRespec.SetCondition(ActivityConditionDisabled);
            }
        }

        public class FunctorRespec : Functor
        {
            internal static bool IsRespecing { get; set; }

            private static readonly List<RulesetItemSpellbook> rulesetItemSpellbooks = new();

            internal static void DropSpellbooksIfRequired(RulesetCharacterHero rulesetCharacterHero)
            {
                rulesetCharacterHero.CharacterInventory.BrowseAllCarriedItems(rulesetItemSpellbooks);

                if (rulesetCharacterHero.ClassesHistory[rulesetCharacterHero.ClassesHistory.Count - 1].Name == "Wizard")
                {
                    foreach (var rulesetItemSpellbook in rulesetItemSpellbooks)
                    {
                        rulesetCharacterHero.LoseItem(rulesetItemSpellbook, false);
                    }
                }
            }

            internal static void PickupSpellbooksIfRequired(RulesetCharacterHero rulesetCharacterHero)
            {
                if (rulesetCharacterHero.ClassesHistory[rulesetCharacterHero.ClassesHistory.Count - 1].Name == "Wizard")
                {
                    foreach (var rulesetItemSpellbook in rulesetItemSpellbooks)
                    {
                        rulesetCharacterHero.GrantItem(rulesetItemSpellbook, false);
                    }
                }
            }

            public override IEnumerator Execute(FunctorParametersDescription functorParameters, FunctorExecutionContext context)
            {
                if (Global.IsMultiplayer)
                {
                    Gui.GuiService.ShowMessage(
                        MessageModal.Severity.Informative1,
                        "RestActivity/&ZSRespecTitle", "Message/&RespecMultiplayerAbortDescription",
                        "Message/&MessageOkTitle", string.Empty,
                        null, null);

                    yield break;             
                }

                var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();
                var gameCampaignScreen = Gui.GuiService.GetScreen<GameCampaignScreen>();

                var gameLocationScreenExploration = Gui.GuiService.GetScreen<GameLocationScreenExploration>();

                var guiConsoleScreenVisible = guiConsoleScreen.Visible;
                var gameCampaignScreenVisible = gameCampaignScreen.Visible;

                // NOTE: don't use gameLocationScreenExploration?. which bypasses Unity object lifetime check
                var gameLocationscreenExplorationVisible = gameLocationScreenExploration && gameLocationScreenExploration.Visible;

                guiConsoleScreen.Hide(true);
                gameCampaignScreen.Hide(true);

                // NOTE: don't use gameLocationScreenExploration?. which bypasses Unity object lifetime check
                if (gameLocationScreenExploration)
                {
                    gameLocationScreenExploration.Hide(true);
                }

                var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
                var oldHero = functorParameters.RestingHero;
                var newHero = characterBuildingService.CreateNewCharacter().HeroCharacter;

                IsRespecing = true;

                DropSpellbooksIfRequired(oldHero);

                yield return StartCharacterCreationWizard(newHero);

                if (IsRespecing)
                {
                    FinalizeRespec(oldHero, newHero);
                }
                else
                {
                    PickupSpellbooksIfRequired(oldHero);
                }

                guiConsoleScreen.Show(guiConsoleScreenVisible);
                gameCampaignScreen.Show(gameCampaignScreenVisible);

                // NOTE: don't use gameLocationScreenExploration?. which bypasses Unity object lifetime check
                if (gameLocationscreenExplorationVisible && gameLocationScreenExploration)
                {
                    gameLocationScreenExploration.Show(true);
                }
            }

            internal static IEnumerator StartCharacterCreationWizard(RulesetCharacterHero hero)
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

                // if there is hero building data still then respec was aborted
                IsRespecing = !hero.TryGetHeroBuildingData(out var _);
            }

            internal static void FinalizeRespec(RulesetCharacterHero oldHero, RulesetCharacterHero newHero)
            {
                var guid = oldHero.Guid;
                var tags = oldHero.Tags;
                var experience = oldHero.GetAttribute(AttributeDefinitions.Experience);
                var gameCampaignCharacters = Gui.GameCampaign.Party.CharactersList;
                var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

                newHero.SetGuid(guid);
                newHero.Tags.AddRange(tags);
                newHero.Attributes[AttributeDefinitions.Experience] = experience;

                CopyInventoryOver(oldHero, newHero);

                gameCampaignCharacters.Find(x => x.RulesetCharacter == oldHero).RulesetCharacter = newHero;

                UpdateRestPanelUi(gameCampaignCharacters);

                if (gameLocationCharacterService == null)
                {
                    return;
                }

                var gameLocationCharacter = gameLocationCharacterService.PartyCharacters.Find(x => x.RulesetCharacter == oldHero);
                var worldLocationEntityFactoryService = ServiceRepository.GetService<IWorldLocationEntityFactoryService>();

                gameLocationCharacter.SetRuleset(newHero);

                if (worldLocationEntityFactoryService.TryFindWorldCharacter(gameLocationCharacter, out WorldLocationCharacter worldLocationCharacter))
                {
                    worldLocationCharacter.GraphicsCharacter.RulesetCharacter = newHero;
                }

                gameLocationCharacterService.SetField("dirtyParty", true);

                IsRespecing = false;
            }

            internal static void CopyInventoryOver(RulesetCharacterHero oldHero, RulesetCharacterHero newHero)
            {
                var personalSlots = oldHero.CharacterInventory.PersonalContainer.InventorySlots;

                foreach (var equipedItem in personalSlots.Select(i => i.EquipedItem).Where(i => i != null))
                {
                    equipedItem.AttunedToCharacter = string.Empty;
                    oldHero.CharacterInventory.DropItem(equipedItem);
                    newHero.GrantItem(equipedItem.ItemDefinition, equipedItem.ItemDefinition.ForceEquip, equipedItem.StackCount);
                }

                var slotsByName = oldHero.CharacterInventory.InventorySlotsByName;

                foreach (var equipedItem in slotsByName.Select(s => s.Value.EquipedItem).Where(i => i != null))
                {
                    equipedItem.AttunedToCharacter = string.Empty;
                    oldHero.CharacterInventory.DropItem(equipedItem);
                    newHero.GrantItem(equipedItem.ItemDefinition, equipedItem.ItemDefinition.ForceEquip, equipedItem.StackCount);
                }
            }

            internal static void UpdateRestPanelUi(List<GameCampaignCharacter> gameCampaignCharacters)
            {
                var restModalScreen = Gui.GuiService.GetScreen<RestModal>();
                var restAfterPanel = restModalScreen.GetField<RestModal, RestAfterPanel>("restAfterPanel");
                var characterPlatesTable = restAfterPanel.GetField<RestAfterPanel, RectTransform>("characterPlatesTable");

                for (int index = 0; index < characterPlatesTable.childCount; ++index)
                {
                    Transform child = characterPlatesTable.GetChild(index);
                    CharacterPlateGame component = child.GetComponent<CharacterPlateGame>();

                    component.Unbind();

                    if (index < gameCampaignCharacters.Count)
                    {
                        component.Bind(gameCampaignCharacters[index].RulesetCharacter, TooltipDefinitions.AnchorMode.BOTTOM_CENTER);
                        component.Refresh();
                    }

                    child.gameObject.SetActive(index < gameCampaignCharacters.Count);
                }
            }
        }
    }
}
