using HarmonyLib;
using SolastaModApi.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolastaCommunityExpansion.Functors
{
    public class FunctorRespec : Functor
    {
        private const int RESPEC_STATE_NORESPEC = 0;
        private const int RESPEC_STATE_RESPECING = 1;
        private const int RESPEC_STATE_ABORTED = 2;

        private static int respecState = RESPEC_STATE_NORESPEC;
        private static RulesetCharacterHero newHero;

        public static bool IsRespecing => respecState == RESPEC_STATE_RESPECING;

        private static readonly List<RulesetItemSpellbook> rulesetItemSpellbooks = new List<RulesetItemSpellbook>();

        internal static void DropSpellbooksIfRequired(RulesetCharacterHero rulesetCharacterHero)
        {
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

        public static void AbortRespec()
        {
            respecState = RESPEC_STATE_ABORTED;
        }

        public override IEnumerator Execute(
          FunctorParametersDescription functorParameters,
          Functor.FunctorExecutionContext context)
        {
            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

            var gameCampaignScreen = Gui.GuiService.GetScreen<GameCampaignScreen>();
            var gameLocationScreenExploration = Gui.GuiService.GetScreen<GameLocationScreenExploration>();
            var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();

            var guiConsoleScreenVisible = guiConsoleScreen?.Visible;
            var gameCampaignScreenVisible = gameCampaignScreen?.Visible;
            var gameLocationscreenExplorationVisible = gameLocationScreenExploration?.Visible;

            respecState = RESPEC_STATE_RESPECING;

            gameCampaignScreen.Hide(true);
            gameLocationScreenExploration?.Hide(true);
            guiConsoleScreen.Hide(true);

            characterBuildingService.CreateNewCharacter();
            newHero = characterBuildingService.HeroCharacter;

            functorParameters.RestingHero?.CharacterInventory?.BrowseAllCarriedItems<RulesetItemSpellbook>(rulesetItemSpellbooks);
            DropSpellbooksIfRequired(functorParameters.RestingHero);

            yield return StartCharacterCreationWizard();

            if (respecState != RESPEC_STATE_ABORTED)
            {
                var gameCampaignCharacters = gameCampaignScreen.GameService.Game.GameCampaign.Party.CharactersList;
                var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
                var gameLocationCharacter = gameLocationCharacterService?.PartyCharacters.Find(x => x.RulesetCharacter.Guid == functorParameters.RestingHero.Guid);

                CopyInventoryOver(functorParameters.RestingHero, newHero);

                newHero.SetGuid(functorParameters.RestingHero.Guid);
                newHero.Attributes[AttributeDefinitions.Experience] = functorParameters.RestingHero.GetAttribute(AttributeDefinitions.Experience);

                gameCampaignCharacters.Find(x => x.RulesetCharacter == functorParameters.RestingHero).RulesetCharacter = newHero;
                gameLocationCharacter?.SetRuleset(newHero);

                if (gameLocationscreenExplorationVisible == true)
                {
                    var gameSerializationService = ServiceRepository.GetService<IGameSerializationService>();

                    gameSerializationService.QuickSaveGame();
                    gameSerializationService.QuickLoadGame();
                }
                else
                {
                    yield return UpdateUI();
                }
            }
            else
            {
                PickupSpellbooksIfRequired(functorParameters.RestingHero);
            }

            if (gameCampaignScreenVisible == true)
            {
                gameCampaignScreen.Show(true);
            }
            if (gameLocationscreenExplorationVisible == true)
            {
                gameLocationScreenExploration.Show(true);
            }
            if (guiConsoleScreenVisible == true)
            {
                guiConsoleScreen.Show(true);
            }

            newHero = null;
            respecState = RESPEC_STATE_NORESPEC;

            yield break;
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

            yield return null;
        }

        internal static void CopyInventoryOver(RulesetCharacter oldHero, RulesetCharacter newHero)
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

        internal static IEnumerator UpdateUI()
        {
            var restModalScreen = Gui.GuiService.GetScreen<RestModal>();
            var restAfterPanel = (RestAfterPanel)AccessTools.Field(restModalScreen.GetType(), "restAfterPanel").GetValue(restModalScreen);
            var characterPlatesTable = (RectTransform)AccessTools.Field(restAfterPanel.GetType(), "characterPlatesTable").GetValue(restAfterPanel);
            var heroes = restModalScreen.GameService.EnumerateHeroes();

            for (int index = 0; index < characterPlatesTable.childCount; ++index)
            {
                Transform child = characterPlatesTable.GetChild(index);
                CharacterPlateGame component = child.GetComponent<CharacterPlateGame>();
                if (index < heroes.Count)
                {
                    child.gameObject.SetActive(true);
                    component.Bind((RulesetCharacter)heroes[index], TooltipDefinitions.AnchorMode.BOTTOM_CENTER);
                    component.Refresh();
                }
                else
                {
                    component.Unbind();
                    child.gameObject.SetActive(false);
                }
            }

            yield return null;
        }
    }
}
