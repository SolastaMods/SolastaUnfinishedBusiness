using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolastaModApi.Infrastructure;
using TA;
using TMPro;
using UnityEngine;
using static SolastaModApi.DatabaseHelper.GadgetBlueprints;

namespace SolastaCommunityExpansion.Models
{
    internal static class GameUiContext
    {
        internal static readonly GadgetBlueprint[] GadgetBlueprintsWithGizmos = new GadgetBlueprint[]
        {
            Exit,
            ExitMultiple,
            TeleporterIndividual,
            TeleporterParty,
        };

        internal static void Load()
        {
            var inputService = ServiceRepository.GetService<IInputService>();

            inputService.RegisterCommand(Hotkeys.CTRL_SHIFT_C, (int)KeyCode.C, (int)KeyCode.LeftShift, (int)KeyCode.LeftControl, -1, -1, -1);
            inputService.RegisterCommand(Hotkeys.CTRL_SHIFT_L, (int)KeyCode.L, (int)KeyCode.LeftShift, (int)KeyCode.LeftControl, -1, -1, -1);
            inputService.RegisterCommand(Hotkeys.CTRL_SHIFT_M, (int)KeyCode.M, (int)KeyCode.LeftShift, (int)KeyCode.LeftControl, -1, -1, -1);
            inputService.RegisterCommand(Hotkeys.CTRL_SHIFT_P, (int)KeyCode.P, (int)KeyCode.LeftShift, (int)KeyCode.LeftControl, -1, -1, -1);
            inputService.RegisterCommand(Hotkeys.CTRL_SHIFT_H, (int)KeyCode.H, (int)KeyCode.LeftShift, (int)KeyCode.LeftControl, -1, -1, -1);
            inputService.RegisterCommand(Hotkeys.CTRL_SHIFT_D, (int)KeyCode.D, (int)KeyCode.LeftShift, (int)KeyCode.LeftControl, -1, -1, -1);
            inputService.RegisterCommand(Hotkeys.CTRL_SHIFT_T, (int)KeyCode.T, (int)KeyCode.LeftShift, (int)KeyCode.LeftControl, -1, -1, -1);

            // Export Character
            inputService.RegisterCommand(Hotkeys.CTRL_SHIFT_E, (int)KeyCode.E, (int)KeyCode.LeftShift, (int)KeyCode.LeftControl, -1, -1, -1);

            // Spawn Encounter
            inputService.RegisterCommand(Hotkeys.CTRL_SHIFT_S, (int)KeyCode.S, (int)KeyCode.LeftShift, (int)KeyCode.LeftControl, -1, -1, -1);
        }

        internal static void HandleInput(GameLocationBaseScreen gameLocationBaseScreen, InputCommands.Id command)
        {
            if (Main.Settings.EnableHotkeysToToggleHud)
            {
                switch (command)
                {
                    case Hotkeys.CTRL_SHIFT_C:
                        GameHud.ShowCharacterControlPanel(gameLocationBaseScreen);
                        return;

                    case Hotkeys.CTRL_SHIFT_L:
                        GameHud.TogglePanelVisibility(Gui.GuiService.GetScreen<GuiConsoleScreen>());
                        return;

                    case Hotkeys.CTRL_SHIFT_M:
                        GameHud.TogglePanelVisibility(GetTimeAndNavigationPanel());
                        return;

                    case Hotkeys.CTRL_SHIFT_P:
                        GameHud.TogglePanelVisibility(GetInitiativeOrPartyPanel());
                        return;

                    case Hotkeys.CTRL_SHIFT_H:
                        GameHud.ShowAll(gameLocationBaseScreen, GetInitiativeOrPartyPanel(), GetTimeAndNavigationPanel());
                        return;
                }
            }

            if (Main.Settings.EnableDebugOverlay && command == Hotkeys.CTRL_SHIFT_D)
            {
                ServiceRepository.GetService<IDebugOverlayService>()?.ToggleActivation();
            }
            else if (Main.Settings.EnableTeleportParty && command == Hotkeys.CTRL_SHIFT_T)
            {
                Teleporter.ConfirmTeleportParty();
            }
            else if (EncountersSpawnContext.EncounterCharacters.Count > 0 && command == Hotkeys.CTRL_SHIFT_S)
            {
                EncountersSpawnContext.ConfirmStageEncounter();
            }

            GuiPanel GetInitiativeOrPartyPanel()
            {
                if (gameLocationBaseScreen is GameLocationScreenExploration gameLocationScreenExploration)
                {
                    return gameLocationScreenExploration.GetField<GameLocationScreenExploration, PartyControlPanel>("partyControlPanel");
                }
                else if (gameLocationBaseScreen is GameLocationScreenBattle gameLocationScreenBattle)
                {
                    return gameLocationScreenBattle.GetField<GameLocationScreenBattle, BattleInitiativeTable>("initiativeTable");
                }
                else
                {
                    return null;
                }
            }

            TimeAndNavigationPanel GetTimeAndNavigationPanel()
            {
                if (gameLocationBaseScreen is GameLocationScreenExploration gameLocationScreenExploration)
                {
                    return gameLocationScreenExploration.GetField<GameLocationScreenExploration, TimeAndNavigationPanel>("timeAndNavigationPanel");
                }
                else if (gameLocationBaseScreen is GameLocationScreenBattle gameLocationScreenBattle)
                {
                    return gameLocationScreenBattle.GetField<GameLocationScreenBattle, TimeAndNavigationPanel>("timeAndNavigationPanel");
                }
                else
                {
                    return null;
                }
            }
        }

        internal static class GameHud
        {
            internal static void ShowAll(GameLocationBaseScreen gameLocationBaseScreen, GuiPanel initiativeOrPartyPanel, TimeAndNavigationPanel timeAndNavigationPanel)
            {
                var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();
                var anyVisible = guiConsoleScreen.Visible || gameLocationBaseScreen.CharacterControlPanel.Visible || initiativeOrPartyPanel.Visible || timeAndNavigationPanel.Visible;

                ShowCharacterControlPanel(gameLocationBaseScreen, anyVisible);
                TogglePanelVisibility(guiConsoleScreen, anyVisible);
                TogglePanelVisibility(initiativeOrPartyPanel);
                TogglePanelVisibility(timeAndNavigationPanel, anyVisible);
            }

            internal static void ShowCharacterControlPanel(GameLocationBaseScreen gameLocationBaseScreen, bool forceHide = false)
            {
                var characterControlPanel = gameLocationBaseScreen.CharacterControlPanel;

                if (characterControlPanel.Visible || forceHide)
                {
                    characterControlPanel.Hide();
                    characterControlPanel.Unbind();
                }
                else
                {
                    var gameLocationSelectionService = ServiceRepository.GetService<IGameLocationSelectionService>();

                    if (gameLocationSelectionService.SelectedCharacters.Count > 0)
                    {
                        characterControlPanel.Bind(gameLocationSelectionService.SelectedCharacters[0], gameLocationBaseScreen.ActionTooltipDock);
                        characterControlPanel.Show();
                    }
                }
            }

            internal static void TogglePanelVisibility(GuiPanel guiPanel, bool forceHide = false)
            {
                if (guiPanel == null)
                {
                    return;
                }

                if (guiPanel.Visible || forceHide)
                {
                    guiPanel.Hide();
                }
                else
                {
                    guiPanel.Show();
                }
            }
        }

        internal static class Teleporter
        {
            internal static void ConfirmTeleportParty()
            {
                var position = GetEncounterPosition();

                Gui.GuiService.ShowMessage(
                    MessageModal.Severity.Attention2,
                    "Message/&TeleportPartyTitle",
                    Gui.Format("Message/&TeleportPartyDescription", position.x.ToString(), position.x.ToString()),
                    "Message/&MessageYesTitle", "Message/&MessageNoTitle",
                    new MessageModal.MessageValidatedHandler(() => { TeleportParty(position); }),
                    null);
            }

            private static int3 GetEncounterPosition()
            {
                var gameLocationService = ServiceRepository.GetService<IGameLocationService>();

                int x = (int)gameLocationService.GameLocation.LastCameraPosition.x;
                int z = (int)gameLocationService.GameLocation.LastCameraPosition.z;

                return new int3(x, 0, z);
            }

            private static void TeleportParty(int3 position)
            {
                var gameLocationActionService = ServiceRepository.GetService<IGameLocationActionService>();
                var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
                var gameLocationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
                var formationPositions = new List<int3>();
                var partyAndGuests = new List<GameLocationCharacter>();
                var positions = new List<int3>();

                for (var iy = 0; iy < 4; iy++)
                {
                    for (var ix = 0; ix < 2; ix++)
                    {
                        formationPositions.Add(new int3(ix, 0, iy));
                    }
                }

                partyAndGuests.AddRange(gameLocationCharacterService.PartyCharacters);
                partyAndGuests.AddRange(gameLocationCharacterService.GuestCharacters);

                gameLocationPositioningService.ComputeFormationPlacementPositions(partyAndGuests, position, LocationDefinitions.Orientation.North, formationPositions, CellHelpers.PlacementMode.Station, positions, new List<RulesetActor.SizeParameters>(), 25);

                for (var index = 0; index < positions.Count; index++)
                {
                    partyAndGuests[index].LocationPosition = positions[index];

                    // rotates the characters in position to force the game to redrawn them
                    gameLocationActionService.MoveCharacter(partyAndGuests[index], positions[(index + 1) % positions.Count], LocationDefinitions.Orientation.North, 0, ActionDefinitions.MoveStance.Walk);
                }
            }
        }

        internal static class RemoveInvalidFilenameChars
        {
            private static readonly HashSet<char> InvalidFilenameChars = new HashSet<char>(Path.GetInvalidFileNameChars());

            public static bool Invoke(TMP_InputField textField)
            {
                if (textField != null)
                {
                    // Solasta original code disallows invalid filename chars and an additional list of illegal chars.
                    // We're disallowing invalid filename chars only.
                    // We're trimming whitespace from start only as per original method.
                    // This allows the users to create a name with spaces inside, but also allows trailing space.
                    textField.text = new string(
                        textField.text
                            .Where(n => !InvalidFilenameChars.Contains(n))
                            .ToArray()).TrimStart();

                    return false;
                }

                return true;
            }
        }
    }
}
