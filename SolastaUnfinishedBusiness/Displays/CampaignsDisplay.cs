using System.Diagnostics;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Displays;

internal static class CampaignsDisplay
{
    private static bool _selectedForSwap;
    private static int _selectedX, _selectedY;
    private static readonly string[] SetNames = ["1", "2", "3", "4", "5"];

    internal static void DisplayGameUi()
    {
        #region Campaign

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Locations"));
        UI.Label();

        var toggle = Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView;
        if (UI.Toggle(Gui.Localize("ModUi/&AltOnlyHighlightItemsInPartyFieldOfView"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView = toggle;
        }

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

        toggle = Main.Settings.EnableLogDialoguesToConsole;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableLogDialoguesToConsole"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableLogDialoguesToConsole = toggle;
        }

        toggle = Main.Settings.EnableHeroWithBestProficiencyToRollChoice;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableHeroWithBestProficiencyToRollChoice"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableHeroWithBestProficiencyToRollChoice = toggle;
        }

        toggle = Main.Settings.EnableAlternateVotingSystem;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableAlternateVotingSystem"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableAlternateVotingSystem = toggle;

            if (!toggle)
            {
                Main.Settings.EnableSumD20OnAlternateVotingSystem = false;
            }
        }

        if (Main.Settings.EnableAlternateVotingSystem)
        {
            toggle = Main.Settings.EnableSumD20OnAlternateVotingSystem;
            if (UI.Toggle(Gui.Localize("ModUi/&EnableSumD20OnAlternateVotingSystem"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableSumD20OnAlternateVotingSystem = toggle;
            }
        }

        UI.Label();

        toggle = Main.Settings.EnableStatsOnHeroTooltip;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableStatsOnHeroTooltip"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableStatsOnHeroTooltip = toggle;
        }

        toggle = Main.Settings.EnableActionSwitching;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableActionSwitching"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableActionSwitching = toggle;
        }

        toggle = Main.Settings.EnableCustomPortraits;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCustomPortraits"), ref toggle))
        {
            Main.Settings.EnableCustomPortraits = toggle;
        }

        if (Main.Settings.EnableCustomPortraits)
        {
            UI.Label();

            UI.ActionButton(Gui.Localize("ModUi/&PortraitsOpenFolder"), () =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = PortraitsContext.PortraitsFolder, UseShellExecute = true, Verb = "open"
                });
            }, UI.Width(292f));

            UI.Label();

            UI.Label(Gui.Localize("ModUi/&EnableCustomPortraitsHelp"));

            UI.Label();
        }

        UI.Label();

        toggle = Main.Settings.AllowMoreRealStateOnRestPanel;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowMoreRealStateOnRestPanel"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowMoreRealStateOnRestPanel = toggle;
        }

        toggle = Main.Settings.EnableAdditionalBackstoryDisplay;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableAdditionalBackstoryDisplay"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableAdditionalBackstoryDisplay = toggle;
        }

        toggle = Main.Settings.EnableExtendedProficienciesPanelDisplay;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableExtendedProficienciesPanelDisplay"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableExtendedProficienciesPanelDisplay = toggle;
        }

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

        toggle = Main.Settings.ShowButtonWithControlledMonsterInfo;
        if (UI.Toggle(Gui.Localize("ModUi/&ShowButtonWithControlledMonsterInfo"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ShowButtonWithControlledMonsterInfo = toggle;
            if (!toggle)
            {
                CustomCharacterStatsPanel.MaybeInstance?.Unbind();
            }
        }

        #endregion

        #region Combat

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Combat"));
        UI.Label();

        int intValue;

        toggle = Main.Settings.DontFollowCharacterInBattle;
        if (UI.Toggle(Gui.Localize("ModUi/&DontFollowCharacterInBattle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DontFollowCharacterInBattle = toggle;
        }

        if (Main.Settings.DontFollowCharacterInBattle)
        {
            intValue = Main.Settings.DontFollowMargin;
            if (UI.Slider(Gui.Localize("ModUi/&DontFollowMargin"), ref intValue, 0, 20,
                    1, "%", UI.AutoWidth()))
            {
                Main.Settings.DontFollowMargin = intValue;
            }

            toggle = Main.Settings.EnableElevationCameraToStayAtPosition;
            if (UI.Toggle(Gui.Localize("ModUi/&EnableElevationCameraToStayAtPosition"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableElevationCameraToStayAtPosition = toggle;
            }

            toggle = Main.Settings.NeverMoveCameraOnEnemyTurn;
            if (UI.Toggle(Gui.Localize("ModUi/&NeverMoveCameraOnEnemyTurn"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.NeverMoveCameraOnEnemyTurn = toggle;
            }

            UI.Label();
        }

        toggle = Main.Settings.EnableCancelEditOnRightMouseClick;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCancelEditOnRightMouseClick"), ref toggle))
        {
            Main.Settings.EnableCancelEditOnRightMouseClick = toggle;
        }

        toggle = Main.Settings.EnableDistanceOnTooltip;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableTooltipDistance"), ref toggle))
        {
            Main.Settings.EnableDistanceOnTooltip = toggle;
        }

        toggle = Main.Settings.ShowMotionFormPreview;
        if (UI.Toggle(Gui.Localize("ModUi/&ShowMotionFormPreview"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ShowMotionFormPreview = toggle;
        }

        UI.Label();

        var color = GameUiContext.HighContrastColorStrings[Main.Settings.HighContrastTargetingAoeSelectedColor];
        var title = Gui.Localize("ModUi/&HighContrastTargetingAoeColor").Replace("$$$$$$", color);

        UI.ActionButton(title, () =>
        {
            Main.Settings.HighContrastTargetingAoeSelectedColor =
                (Main.Settings.HighContrastTargetingAoeSelectedColor + 1) % GameUiContext.HighContrastColors.Length;
        }, UI.Width(300f));

        color = GameUiContext.HighContrastColorStrings[Main.Settings.HighContrastTargetingSingleSelectedColor];
        title = Gui.Localize("ModUi/&HighContrastTargetingSingleColor").Replace("$$$$$$", color);

        UI.ActionButton(title, () =>
        {
            Main.Settings.HighContrastTargetingSingleSelectedColor =
                (Main.Settings.HighContrastTargetingSingleSelectedColor + 1) % GameUiContext.HighContrastColors.Length;
        }, UI.Width(300f));

        color = GameUiContext.GridColorStrings[Main.Settings.GridSelectedColor];
        title = Gui.Localize("ModUi/&GridSelectedColor").Replace("$$$$$$", color);

        UI.ActionButton(title, () =>
        {
            Main.Settings.GridSelectedColor = (Main.Settings.GridSelectedColor + 1) % GameUiContext.GridColors.Length;
            GameUiContext.UpdateMovementGrid();
        }, UI.Width(300f));

        intValue = Main.Settings.MovementGridWidthModifier;
        if (UI.Slider(Gui.Localize("ModUi/&MovementGridWidthModifier"), ref intValue, 0, 200, 100, string.Empty,
                UI.Width(100f)))
        {
            Main.Settings.MovementGridWidthModifier = intValue;
            GameUiContext.UpdateMovementGrid();
        }

        intValue = Main.Settings.OutlineGridWidthModifier;
        if (UI.Slider(Gui.Localize("ModUi/&OutlineGridWidthModifier"), ref intValue, 0, 200, 100, string.Empty,
                UI.Width(100f)))
        {
            Main.Settings.OutlineGridWidthModifier = intValue;
            GameUiContext.UpdateMovementGrid();
        }

        intValue = Main.Settings.OutlineGridWidthSpeed;
        if (UI.Slider(Gui.Localize("ModUi/&OutlineGridWidthSpeed"), ref intValue, 0, 200, 100, string.Empty,
                UI.Width(100f)))
        {
            Main.Settings.OutlineGridWidthSpeed = intValue;
            GameUiContext.UpdateMovementGrid();
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
            DisplayFormationGrid();
        }

        #endregion

        #region Item

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&InventoryAndItems"));
        UI.Label();

        toggle = Main.Settings.AddCustomIconsToOfficialItems;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&AddCustomIconsToOfficialItems")), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddCustomIconsToOfficialItems = toggle;
        }

        toggle = Main.Settings.AddNewWeaponsAndRecipesToShops;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&AddNewWeaponsAndRecipesToShops")), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddNewWeaponsAndRecipesToShops = toggle;
        }

        toggle = Main.Settings.DisableAutoEquip;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableAutoEquip"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableAutoEquip = toggle;
        }

        toggle = Main.Settings.EnableInventoryFilteringAndSorting;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableInventoryFilteringAndSorting"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableInventoryFilteringAndSorting = toggle;
            InventoryManagementContext.RefreshControlsVisibility();
        }

        UI.Label();

        toggle = Main.Settings.EnableInventoryTaintNonProficientItemsRed;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableInventoryTaintNonProficientItemsRed"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableInventoryTaintNonProficientItemsRed = toggle;
        }

        toggle = Main.Settings.EnableInventoryTintKnownRecipesRed;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableInventoryTintKnownRecipesRed"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableInventoryTintKnownRecipesRed = toggle;
        }

        UI.Label();

        toggle = Main.Settings.ShowCraftingRecipeInDetailedTooltips;
        if (UI.Toggle(Gui.Localize("ModUi/&ShowCraftingRecipeInDetailedTooltips"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ShowCraftingRecipeInDetailedTooltips = toggle;
        }

        toggle = Main.Settings.ShowCraftedItemOnRecipeIcon;
        if (UI.Toggle(Gui.Localize("ModUi/&ShowCraftedItemOnRecipeIcon"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ShowCraftedItemOnRecipeIcon = toggle;
        }

        if (Main.Settings.ShowCraftedItemOnRecipeIcon)
        {
            toggle = Main.Settings.SwapCraftedItemAndRecipeIcons;
            if (UI.Toggle(Gui.Localize("ModUi/&SwapCraftedItemAndRecipeIcons"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.SwapCraftedItemAndRecipeIcons = toggle;
            }
        }

        UI.Label();

        toggle = Main.Settings.AddPickPocketableLoot;
        if (UI.Toggle(Gui.Localize("ModUi/&AddPickPocketableLoot"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddPickPocketableLoot = toggle;
            if (toggle)
            {
                PickPocketContext.Load();
            }
        }

        UI.Label();

        intValue = Main.Settings.SetBeltOfDwarvenKindBeardChances;
        if (UI.Slider(Gui.Localize("ModUi/&SetBeltOfDwarvenKindBeardChances"), ref intValue,
                0, 100, 50, "%", UI.Width(500f)))
        {
            Main.Settings.SetBeltOfDwarvenKindBeardChances = intValue;
            ItemCraftingMerchantContext.SwitchSetBeltOfDwarvenKindBeardChances();
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Localize("ModUi/&EmpressGarbAppearance"), UI.Width(325f));

            intValue = Main.Settings.EmpressGarbAppearanceIndex;
            // ReSharper disable once InvertIf
            if (UI.SelectionGrid(ref intValue, ItemCraftingMerchantContext.EmpressGarbAppearances,
                    ItemCraftingMerchantContext.EmpressGarbAppearances.Length, 2, UI.Width(440f)))
            {
                Main.Settings.EmpressGarbAppearanceIndex = intValue;
                GameUiContext.SwitchEmpressGarb();
            }
        }

        #endregion

        UI.Label();
    }


    private static void DisplayFormationGrid()
    {
        var selectedSet = Main.Settings.FormationGridSelectedSet;

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&FormationResetAllSets"), () =>
                {
                    _selectedForSwap = false;
                    GameUiContext.ResetAllFormationGrids();
                },
                UI.Width(110f));

            if (UI.SelectionGrid(ref selectedSet, SetNames, SetNames.Length, SetNames.Length, UI.Width(165f)))
            {
                _selectedForSwap = false;
                Main.Settings.FormationGridSelectedSet = selectedSet;
                GameUiContext.FillDefinitionFromFormationGrid();
            }

            UI.Label(Gui.Localize("ModUi/&FormationHelp1"));
        }

        UI.Label();

        for (var y = 0; y < GameUiContext.GridSize; y++)
        {
            using (UI.HorizontalScope())
            {
                // first line
                if (y == 0)
                {
                    UI.ActionButton(Gui.Localize("ModUi/&FormationResetThisSet"), () =>
                        {
                            _selectedForSwap = false;
                            GameUiContext.ResetFormationGrid(Main.Settings.FormationGridSelectedSet);
                        },
                        UI.Width(110f));
                }
                else
                {
                    UI.Label("", UI.Width(110f));
                }

                for (var x = 0; x < GameUiContext.GridSize; x++)
                {
                    var saveColor = GUI.color;
                    string label;

                    if (Main.Settings.FormationGridSets[selectedSet][y][x] == 1)
                    {
                        // yep 256 not 255 for a light contrast
                        GUI.color = new Color(0x1E / 256f, 0x81 / 256f, 0xB0 / 256f);
                        label = "@";
                    }
                    else
                    {
                        label = "..";
                    }

                    if (_selectedForSwap && _selectedX == x && _selectedY == y)
                    {
                        label = $"<b><color=red>{label}</color></b>";
                    }

                    UI.ActionButton(label, () =>
                    {
                        // ReSharper disable once InlineTemporaryVariable
                        // ReSharper disable once AccessToModifiedClosure
                        var localX = x;
                        // ReSharper disable once InlineTemporaryVariable
                        // ReSharper disable once AccessToModifiedClosure
                        var localY = y;

                        if (_selectedForSwap)
                        {
                            (Main.Settings.FormationGridSets[selectedSet][localY][localX],
                                Main.Settings.FormationGridSets[selectedSet][_selectedY][_selectedX]) = (
                                Main.Settings.FormationGridSets[selectedSet][_selectedY][_selectedX],
                                Main.Settings.FormationGridSets[selectedSet][localY][localX]);

                            GameUiContext.FillDefinitionFromFormationGrid();

                            _selectedForSwap = false;
                        }
                        else
                        {
                            _selectedX = localX;
                            _selectedY = localY;
                            _selectedForSwap = true;
                        }
                    }, UI.Width(30f));

                    GUI.color = saveColor;
                }

                // first line
                if (y <= 1)
                {
                    UI.Label(Gui.Localize("ModUi/&FormationHelp" + (y + 2)));
                }
            }
        }
    }
}
