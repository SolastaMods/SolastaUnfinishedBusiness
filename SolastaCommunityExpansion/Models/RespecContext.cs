using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine;
using static SolastaModApi.DatabaseHelper.RestActivityDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class RespecContext
    {
        internal const RestActivityDefinition.ActivityCondition ActivityConditionCanRespec = (RestActivityDefinition.ActivityCondition)(-1001);

        public class RestActivityRespecBuilder : RestActivityDefinitionBuilder
        {
            private const string RespecName = "ZSRespec";
            private const string RespecGuid = "40824029eb224fb581f0d4e5989b6735";

            protected RestActivityRespecBuilder(string name, string guid) : base(LevelUp, name, guid)
            {
                Definition.GuiPresentation.Title = "RestActivity/&ZSRespecTitle";
                Definition.GuiPresentation.Description = "RestActivity/&ZSRespecDescription";
                Definition.SetCondition(ActivityConditionCanRespec);
                Definition.SetFunctor(RespecName);
                ServiceRepository.GetService<IFunctorService>().RegisterFunctor(RespecName, new FunctorRespec());
            }

            private static RestActivityDefinition CreateAndAddToDB(string name, string guid)
            {
                return new RestActivityRespecBuilder(name, guid).AddToDB();
            }

            public static readonly RestActivityDefinition RestActivityRespec
                = CreateAndAddToDB(RespecName, RespecGuid);
        }

        internal static void Load()
        {
            _ = RestActivityRespecBuilder.RestActivityRespec;
        }

        public class FunctorRespec : Functor
        {
            private const int RESPEC_STATE_NORESPEC = 0;
            private const int RESPEC_STATE_RESPECING = 1;
            private const int RESPEC_STATE_ABORTED = 2;

            private static int RespecState { get; set; }

            internal static bool IsRespecing => RespecState == RESPEC_STATE_RESPECING;

            internal static void AbortRespec()
            {
                RespecState = RESPEC_STATE_ABORTED;
            }

            internal static void StartRespec()
            {
                RespecState = RESPEC_STATE_RESPECING;
            }

            internal static void StopRespec()
            {
                RespecState = RESPEC_STATE_NORESPEC;
            }

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
                var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();
                var gameCampaignScreen = Gui.GuiService.GetScreen<GameCampaignScreen>();

                var gameLocationScreenExploration = Gui.GuiService.GetScreen<GameLocationScreenExploration>();

                var guiConsoleScreenVisible = guiConsoleScreen.Visible;
                var gameCampaignScreenVisible = gameCampaignScreen.Visible;

                // NOTE: don't use gameLocationScreenExploration?. which bypasses Unity object lifetime check
                var gameLocationscreenExplorationVisible = gameLocationScreenExploration && gameLocationScreenExploration.Visible;

                StartRespec();

                guiConsoleScreen.Hide(true);
                gameCampaignScreen.Hide(true);

                // NOTE: don't use gameLocationScreenExploration?. which bypasses Unity object lifetime check
                if (gameLocationScreenExploration)
                {
                    gameLocationScreenExploration.Hide(true);
                }

                var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

                characterBuildingService.CreateNewCharacter();

                var oldHero = functorParameters.RestingHero;
                var newHero = characterBuildingService.HeroCharacter;

                DropSpellbooksIfRequired(oldHero);

                yield return StartCharacterCreationWizard();

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

                StopRespec();
            }

            internal static IEnumerator StartCharacterCreationWizard()
            {
                var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
                var restModalScreen = Gui.GuiService.GetScreen<RestModal>();

                restModalScreen.KeepCurrentstate = true;
                restModalScreen.Hide(true);
                characterCreationScreen.OriginScreen = restModalScreen;
                characterCreationScreen.Show();

                while (characterCreationScreen.isActiveAndEnabled)
                {
                    yield return null;
                }
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
