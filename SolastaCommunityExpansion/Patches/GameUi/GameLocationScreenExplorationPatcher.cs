using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    class GameLocationScreenExplorationPatcher
    {
        [HarmonyPatch(typeof(GameLocationScreenExploration), "HandleInput")]
        internal static class GameLocationScreenExploration_HandleInput
        {
            internal static bool Prefix(
                GameLocationScreenExploration __instance,
                InputCommands.Id command, 
                ref bool __result,
                PartyControlPanel ___partyControlPanel,
                CharacterControlPanelExploration ___characterControlPanelExploration,
                TimeAndNavigationPanel ___timeAndNavigationPanel)
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
                        __result = true;
                        return false;

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
                        __result = true;
                        return false;

                    case Settings.CTRL_M:
                        if (___timeAndNavigationPanel.Visible)
                        {
                            ___timeAndNavigationPanel.Hide();
                        }
                        else
                        {
                            ___timeAndNavigationPanel.Show();
                        }
                        __result = true;
                        return false;

                    case Settings.CTRL_P:
                        if (___partyControlPanel.Visible)
                        {
                            ___partyControlPanel.Hide();
                        }
                        else
                        {
                            ___partyControlPanel.Show();
                        }
                        __result = true;
                        return false;

                    default:
                        return true;
                }
            }
        }
    }
}