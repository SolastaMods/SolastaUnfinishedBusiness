using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUiLocation
{
    [HarmonyPatch(typeof(GameLocationScreenExploration), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationScreenExploration_HandleInput
    {
        internal static void Postfix(
            GameLocationScreenExploration __instance,
            InputCommands.Id command,
            PartyControlPanel ___partyControlPanel,
            CharacterControlPanelExploration ___characterControlPanelExploration,
            TimeAndNavigationPanel ___timeAndNavigationPanel)
        {
            if (Main.Settings.EnableHotkeysToToggleHud)
            {
                switch (command)
                {
                    case Settings.CTRL_C:
                        if (___characterControlPanelExploration.Visible)
                        {
                            ___characterControlPanelExploration.Hide();
                        }
                        else
                        {
                            var gameLocationSelectionService = ServiceRepository.GetService<IGameLocationSelectionService>();

                            if (gameLocationSelectionService.SelectedCharacters.Count > 0)
                            {
                                ___characterControlPanelExploration.Bind(gameLocationSelectionService.SelectedCharacters[0], __instance.ActionTooltipDock);
                                ___characterControlPanelExploration.Show();
                            }
                        }
                        break;

                    case Settings.CTRL_L:
                        var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();

                        if (guiConsoleScreen.Visible)
                        {
                            guiConsoleScreen.Hide();
                        }
                        else
                        {
                            guiConsoleScreen.Show();
                        }
                        break;

                    case Settings.CTRL_M:
                        if (___timeAndNavigationPanel.Visible)
                        {
                            ___timeAndNavigationPanel.Hide();
                        }
                        else
                        {
                            ___timeAndNavigationPanel.Show();
                        }
                        break;

                    case Settings.CTRL_P:
                        if (___partyControlPanel.Visible)
                        {
                            ___partyControlPanel.Hide();
                        }
                        else
                        {
                            ___partyControlPanel.Show();
                        }
                        break;
                }
            }
        }
    }
}
