using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class GameUiDisplay
{
    #region Formation Helpers

    private static bool _selectedForSwap;
    private static int _selectedX, _selectedY;
    private static readonly string[] SetNames = { "1", "2", "3", "4", "5" };

    private static void DisplayFormationGrid()
    {
        UI.Label();

        var selectedSet = Main.Settings.FormationGridSelectedSet;

        if (UI.SelectionGrid(ref selectedSet, SetNames, SetNames.Length, SetNames.Length, UI.Width(165)))
        {
            _selectedForSwap = false;
            Main.Settings.FormationGridSelectedSet = selectedSet;
            GameUiContext.FillDefinitionFromFormationGrid();
        }

        UI.Label();

        for (var y = 0; y < GameUiContext.GridSize; y++)
        {
            using (UI.HorizontalScope())
            {
                for (var x = 0; x < GameUiContext.GridSize; x++)
                {
                    string label;

                    if (Main.Settings.FormationGridSets[selectedSet][y][x] == 1)
                    {
                        if (_selectedForSwap && _selectedX == x && _selectedY == y)
                        {
                            label = "<b><color=red>@</color></b>";
                        }
                        else
                        {
                            label = "<b><color=#1E81B0>@</color></b>";
                        }
                    }
                    else
                    {
                        if (_selectedForSwap && _selectedX == x && _selectedY == y)
                        {
                            label = "<b><color=red>..</color></b>";
                        }
                        else
                        {
                            label = "..";
                        }
                    }

                    UI.ActionButton(label, () =>
                    {
                        if (_selectedForSwap)
                        {
                            (Main.Settings.FormationGridSets[selectedSet][y][x],
                                Main.Settings.FormationGridSets[selectedSet][_selectedY][_selectedX]) = (
                                Main.Settings.FormationGridSets[selectedSet][_selectedY][_selectedX],
                                Main.Settings.FormationGridSets[selectedSet][y][x]);

                            GameUiContext.FillDefinitionFromFormationGrid();

                            _selectedForSwap = false;
                        }
                        else
                        {
                            _selectedX = x;
                            _selectedY = y;
                            _selectedForSwap = true;
                        }
                    }, UI.Width(30));
                }
            }
        }
    }

    #endregion

    internal static void DisplayGameUi()
    {
        #region Campaign

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&CampaignsAndLocations"));
        UI.Label();

        var toggle = Main.Settings.DontFollowCharacterInBattle;
        if (UI.Toggle(Gui.Localize("ModUi/&DontFollowCharacterInBattle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DontFollowCharacterInBattle = toggle;
        }

        if (Main.Settings.DontFollowCharacterInBattle)
        {
            var intValue = Main.Settings.DontFollowMargin;
            if (UI.Slider(Gui.Localize("ModUi/&DontFollowMargin"), ref intValue, 0, 20,
                    1, "%", UI.AutoWidth()))
            {
                Main.Settings.DontFollowMargin = intValue;
            }
        }

        UI.Label();

        toggle = Main.Settings.EnableStatsOnHeroTooltip;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableStatsOnHeroTooltip"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableStatsOnHeroTooltip = toggle;
        }

        toggle = Main.Settings.EnableAdditionalBackstoryDisplay;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableAdditionalBackstoryDisplay"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableAdditionalBackstoryDisplay = toggle;
        }

        toggle = Main.Settings.EnableLogDialoguesToConsole;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableLogDialoguesToConsole"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableLogDialoguesToConsole = toggle;
        }

        UI.Label();

        toggle = Main.Settings.EnableAdditionalIconsOnLevelMap;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableAdditionalIconsOnLevelMap"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableAdditionalIconsOnLevelMap = toggle;

            if (toggle)
            {
                Main.Settings.MarkInvisibleTeleportersOnLevelMap = false;
            }
        }

        if (Main.Settings.EnableAdditionalIconsOnLevelMap)
        {
            toggle = Main.Settings.MarkInvisibleTeleportersOnLevelMap;
            if (UI.Toggle(Gui.Localize("ModUi/&MarkInvisibleTeleportersOnLevelMap"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.MarkInvisibleTeleportersOnLevelMap = toggle;
            }
        }

        toggle = Main.Settings.HideExitsAndTeleportersGizmosIfNotDiscovered;
        if (UI.Toggle(Gui.Localize("ModUi/&HideExitAndTeleporterGizmosIfNotDiscovered"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.HideExitsAndTeleportersGizmosIfNotDiscovered = toggle;
        }

        UI.Label();

        toggle = Main.Settings.AllowMoreRealStateOnRestPanel;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowMoreRealStateOnRestPanel"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowMoreRealStateOnRestPanel = toggle;
        }

        UI.Label();

        toggle = Main.Settings.AddWildshapeSwapAttackToggle;
        if (UI.Toggle(Gui.Localize("ModUi/&AddWildshapeSwapAttackToggle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddWildshapeSwapAttackToggle = toggle;
        }

        toggle = Main.Settings.AddMonkKiPointsToggle;
        if (UI.Toggle(Gui.Localize("ModUi/&AddMonkKiPointsToggle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddMonkKiPointsToggle = toggle;
        }

        toggle = Main.Settings.AddPaladinSmiteToggle;
        if (UI.Toggle(Gui.Localize("ModUi/&AddPaladinSmiteToggle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddPaladinSmiteToggle = toggle;
        }

        #endregion

        #region Formation

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Formation"));
        UI.Label();

        if (Global.IsMultiplayer)
        {
            UI.Label(Gui.Localize("ModUi/&FormationError"));
        }
        else
        {
            UI.Label(Gui.Localize("ModUi/&FormationHelp"));

            DisplayFormationGrid();
        }

        #endregion

        #region Input

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Input"));
        UI.Label();

        toggle = Main.Settings.InvertAltBehaviorOnTooltips;
        if (UI.Toggle(Gui.Localize("ModUi/&InvertAltBehaviorOnTooltips"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.InvertAltBehaviorOnTooltips = toggle;
        }

        toggle = Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView;
        if (UI.Toggle(Gui.Localize("ModUi/&AltOnlyHighlightItemsInPartyFieldOfView"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView = toggle;
        }

        UI.Label();

        toggle = Main.Settings.EnableHotkeyToggleHud;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableHotkeyToggleHud"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableHotkeyToggleHud = toggle;
        }

        toggle = Main.Settings.EnableCharacterExport;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCharacterExport"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableCharacterExport = toggle;
        }

        toggle = Main.Settings.EnableTeleportParty;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableTeleportParty"), ref toggle))
        {
            Main.Settings.EnableTeleportParty = toggle;
        }

        toggle = Main.Settings.EnableRejoinParty;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRejoinParty"), ref toggle))
        {
            Main.Settings.EnableRejoinParty = toggle;
        }

        toggle = Main.Settings.EnableCancelEditOnRightMouseClick;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCancelEditOnRightMouseClick"), ref toggle))
        {
            Main.Settings.EnableCancelEditOnRightMouseClick = toggle;
        }

        #endregion

        #region Item

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&InventoryAndItems"));
        UI.Label();

        toggle = Main.Settings.DisableAutoEquip;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableAutoEquip"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableAutoEquip = toggle;
        }

        toggle = Main.Settings.EnableInventoryFilteringAndSorting;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableInventoryFilteringAndSorting"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableInventoryFilteringAndSorting = toggle;
            Main.Settings.EnableInventoryTaintNonProficientItemsRed = toggle;
            InventoryManagementContext.RefreshControlsVisibility();
        }

        if (Main.Settings.EnableInventoryFilteringAndSorting)
        {
            toggle = Main.Settings.EnableInventoryTaintNonProficientItemsRed;
            if (UI.Toggle(Gui.Localize("ModUi/&EnableInventoryTaintNonProficientItemsRed"), ref toggle,
                    UI.AutoWidth()))
            {
                Main.Settings.EnableInventoryTaintNonProficientItemsRed = toggle;
            }
        }

        toggle = Main.Settings.EnableInvisibleCrownOfTheMagister;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableInvisibleCrownOfTheMagister"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableInvisibleCrownOfTheMagister = toggle;
            GameUiContext.SwitchCrownOfTheMagister();
        }

        #endregion

        #region Monster

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Monsters"));
        UI.Label();

        toggle = Main.Settings.HideMonsterHitPoints;
        if (UI.Toggle(Gui.Localize("ModUi/&HideMonsterHitPoints"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.HideMonsterHitPoints = toggle;
        }

        toggle = Main.Settings.RemoveBugVisualModels;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveBugVisualModels"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveBugVisualModels = toggle;
        }

        #endregion

        #region Spell

#if false
        // ModUi/&Spells=<color=#F0DAA0>Spells:</color>
        // ModUi/&MaxSpellLevelsPerLine=<color=white>Max levels per line on Spell Panel</color>
        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&Spells"));
        UI.Label("");

        intValue = Main.Settings.MaxSpellLevelsPerLine;
        if (UI.Slider(Gui.Localize("ModUi/&MaxSpellLevelsPerLine"), ref intValue, 3, 7, 5, "", UI.AutoWidth()))
        {
            Main.Settings.MaxSpellLevelsPerLine = intValue;
        }
#endif

        #endregion

        UI.Label();
    }
}
