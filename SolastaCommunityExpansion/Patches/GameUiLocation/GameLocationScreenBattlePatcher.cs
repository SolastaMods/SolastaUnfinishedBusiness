using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUiLocation
{
    [HarmonyPatch(typeof(GameLocationScreenBattle), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationScreenBattle_HandleInput
    {
        internal static void Postfix(
            GameLocationScreenBattle __instance,
            InputCommands.Id command,
            BattleInitiativeTable ___initiativeTable,
            CharacterControlPanelBattle ___characterControlPanelBattle,
            TimeAndNavigationPanel ___timeAndNavigationPanel)
        {
            if (!Main.Settings.EnableHotkeysToToggleHud)
            {
                return;
            }

            switch (command)
            {
                case GameUiContext.CTRL_C:
                    ShowCharacterControlPanelBattle();
                    break;

                case GameUiContext.CTRL_L:
                    ShowGuiConsoleScreen();
                    break;

                case GameUiContext.CTRL_M:
                    ShowTimeAndNavigationPanel();
                    break;

                case GameUiContext.CTRL_P:
                    ShowPartyControlPanel();
                    break;

                case GameUiContext.CTRL_H:
                    ShowAll();
                    break;
            }

            void ShowAll()
            {
                ShowCharacterControlPanelBattle();
                ShowGuiConsoleScreen();
                ShowTimeAndNavigationPanel();
                ShowPartyControlPanel();
            }

            void ShowCharacterControlPanelBattle()
            {
                if (___characterControlPanelBattle.Visible)
                {
                    ___characterControlPanelBattle.Hide();
                    ___characterControlPanelBattle.Unbind();
                }
                else
                {
                    var gameLocationSelectionService = ServiceRepository.GetService<IGameLocationSelectionService>();

                    if (gameLocationSelectionService.SelectedCharacters.Count > 0)
                    {
                        ___characterControlPanelBattle.Bind(gameLocationSelectionService.SelectedCharacters[0], __instance.ActionTooltipDock);
                        ___characterControlPanelBattle.Show();
                    }
                }
            }

            void ShowGuiConsoleScreen()
            {
                var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();

                if (guiConsoleScreen.Visible)
                {
                    guiConsoleScreen.Hide();
                }
                else
                {
                    guiConsoleScreen.Show();
                }
            }

            void ShowTimeAndNavigationPanel()
            {
                if (___timeAndNavigationPanel.Visible)
                {
                    ___timeAndNavigationPanel.Hide();
                }
                else
                {
                    ___timeAndNavigationPanel.Show();
                }
            }

            void ShowPartyControlPanel()
            {
                if (___initiativeTable.Visible)
                {
                    ___initiativeTable.Hide();
                }
                else
                {
                    ___initiativeTable.Show();
                }
            }
        }
    }
}
