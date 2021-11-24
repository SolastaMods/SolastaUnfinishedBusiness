using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Functors
{
    public class FunctorRespec : Functor
    {
        private const int RESPEC_STATE_NORESPEC = 0;
        private const int RESPEC_STATE_RESPECING = 1;
        private const int RESPEC_STATE_ABORTED = 2;

        private static int RespecState { get; set; }

        internal static bool IsRespecing => RespecState == RESPEC_STATE_RESPECING;

        internal static void AbortRespec() => RespecState = RESPEC_STATE_ABORTED;

        internal static void StartRespec() => RespecState = RESPEC_STATE_RESPECING;

        internal static void StopRespec() => RespecState = RESPEC_STATE_NORESPEC;

        private static readonly List<RulesetItemSpellbook> rulesetItemSpellbooks = new List<RulesetItemSpellbook>();

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

            var gameLocationscreenExplorationVisible = gameLocationScreenExploration?.Visible;

            StartRespec();

            guiConsoleScreen.Hide(true);
            gameCampaignScreen.Hide(true);

            gameLocationScreenExploration?.Hide(true);

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

            if (gameLocationscreenExplorationVisible == true)
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

            while (characterCreationScreen.isActiveAndEnabled) yield return null;
        }

        internal static void FinalizeRespec(RulesetCharacterHero oldHero, RulesetCharacterHero newHero)
        {
            var experience = oldHero.GetAttribute(AttributeDefinitions.Experience);
            var gameCampaignCharacters = Gui.GameCampaign.Party.CharactersList;
            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            oldHero.Unregister();
            oldHero.ResetForOutgame();

            CopyInventoryOver(oldHero, newHero);

            newHero.Register(true);
            newHero.Attributes[AttributeDefinitions.Experience] = experience;

            gameCampaignCharacters.Find(x => x.RulesetCharacter == oldHero).RulesetCharacter = newHero;

            UpdateRestPanelUi(gameCampaignCharacters);

            if (gameLocationCharacterService != null)
            {
                var gameLocationCharacter = gameLocationCharacterService.PartyCharacters.Find(x => x.RulesetCharacter == oldHero);
                var worldLocationEntityFactoryService = ServiceRepository.GetService<IWorldLocationEntityFactoryService>();

                gameLocationCharacter.SetRuleset(newHero);

                if (worldLocationEntityFactoryService.TryFindWorldCharacter(gameLocationCharacter, out WorldLocationCharacter worldLocationCharacter))
                {
                    worldLocationCharacter.GraphicsCharacter.RulesetCharacter = newHero;
                }

                AccessTools.Field(gameLocationCharacterService.GetType(), "dirtyParty").SetValue(gameLocationCharacterService, true);
            }
        }

        internal static void CopyInventoryOver(RulesetCharacterHero oldHero, RulesetCharacterHero newHero)
        {
            foreach (var inventorySlot in oldHero.CharacterInventory.PersonalContainer.InventorySlots)
            {
                if (inventorySlot.EquipedItem != null)
                {
                    var equipedItem = inventorySlot.EquipedItem;

                    equipedItem.AttunedToCharacter = string.Empty;
                    oldHero.CharacterInventory.DropItem(equipedItem);
                    newHero.GrantItem(equipedItem.ItemDefinition, equipedItem.ItemDefinition.ForceEquip, equipedItem.StackCount);
                }
            }

            foreach (var inventorySlot in oldHero.CharacterInventory.InventorySlotsByName)
            {
                if (inventorySlot.Value.EquipedItem != null)
                {
                    var equipedItem = inventorySlot.Value.EquipedItem;

                    equipedItem.AttunedToCharacter = string.Empty;
                    oldHero.CharacterInventory.DropItem(inventorySlot.Value.EquipedItem);
                    newHero.GrantItem(equipedItem.ItemDefinition, equipedItem.ItemDefinition.ForceEquip, equipedItem.StackCount);
                }
            }
        }

        internal static void UpdateRestPanelUi(List<GameCampaignCharacter> gameCampaignCharacters)
        {
            var restModalScreen = Gui.GuiService.GetScreen<RestModal>();
            var restAfterPanel = AccessTools.Field(restModalScreen.GetType(), "restAfterPanel").GetValue(restModalScreen) as RestAfterPanel;
            var characterPlatesTable = AccessTools.Field(restAfterPanel.GetType(), "characterPlatesTable").GetValue(restAfterPanel) as RectTransform;

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
