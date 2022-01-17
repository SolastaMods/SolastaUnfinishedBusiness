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
                case Hotkeys.CTRL_C:
                    ShowCharacterControlPanelBattle();
                    break;

                case Hotkeys.CTRL_L:
                    ShowGuiConsoleScreen();
                    break;

                case Hotkeys.CTRL_M:
                    ShowTimeAndNavigationPanel();
                    break;

                case Hotkeys.CTRL_P:
                    ShowPartyControlPanel();
                    break;

                case Hotkeys.CTRL_H:
                    ShowAll();
                    break;
            }

            void ShowAll()
            {
                var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();
                var anyVisible = ___characterControlPanelBattle.Visible || guiConsoleScreen.Visible || ___timeAndNavigationPanel.Visible || ___initiativeTable.Visible;

                ShowCharacterControlPanelBattle(anyVisible);
                ShowGuiConsoleScreen(anyVisible);
                ShowTimeAndNavigationPanel(anyVisible);
                ShowPartyControlPanel(anyVisible);
            }

            void ShowCharacterControlPanelBattle(bool forceHide = false)
            {
                if (___characterControlPanelBattle.Visible || forceHide)
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

            void ShowGuiConsoleScreen(bool forceHide = false)
            {
                var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();

                if (guiConsoleScreen.Visible || forceHide)
                {
                    guiConsoleScreen.Hide();
                }
                else
                {
                    guiConsoleScreen.Show();
                }
            }

            void ShowTimeAndNavigationPanel(bool forceHide = false)
            {
                if (___timeAndNavigationPanel.Visible || forceHide)
                {
                    ___timeAndNavigationPanel.Hide();
                }
                else
                {
                    ___timeAndNavigationPanel.Show();
                }
            }

            void ShowPartyControlPanel(bool forceHide = false)
            {
                if (___initiativeTable.Visible || forceHide)
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
