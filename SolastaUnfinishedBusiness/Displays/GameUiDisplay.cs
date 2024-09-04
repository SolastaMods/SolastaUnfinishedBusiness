using System.Diagnostics;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class GameUiDisplay
{
    internal static void DisplayGameUi()
    {
        #region Campaign

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&CampaignsAndLocations"));
        UI.Label();

        var toggle = Main.Settings.EnableAdditionalIconsOnLevelMap;
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

        toggle = Main.Settings.EnableHeroWithBestProficiencyToRollChoice;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableHeroWithBestProficiencyToRollChoice"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableHeroWithBestProficiencyToRollChoice = toggle;
        }

        toggle = Main.Settings.EnableLogDialoguesToConsole;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableLogDialoguesToConsole"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableLogDialoguesToConsole = toggle;
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

        toggle = Main.Settings.AllowMoreRealStateOnRestPanel;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowMoreRealStateOnRestPanel"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowMoreRealStateOnRestPanel = toggle;
        }

        toggle = Main.Settings.AddPaladinSmiteToggle;
        if (UI.Toggle(Gui.Localize("ModUi/&AddPaladinSmiteToggle"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddPaladinSmiteToggle = toggle;
        }

        toggle = Main.Settings.EnableActionSwitching;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableActionSwitching"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableActionSwitching = toggle;
        }

        toggle = Main.Settings.EnableRespec;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRespec"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRespec = toggle;
            ToolsContext.SwitchRespec();
        }

        UI.Label();

        toggle = Main.Settings.EnableStatsOnHeroTooltip;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableStatsOnHeroTooltip"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableStatsOnHeroTooltip = toggle;
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

        toggle = Main.Settings.ShowChannelDivinityOnPortrait;
        if (UI.Toggle(Gui.Localize("ModUi/&ShowChannelDivinityOnPortrait"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ShowChannelDivinityOnPortrait = toggle;
        }

        UI.Label();

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

        toggle = Main.Settings.EnableDistanceOnTooltip;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableTooltipDistance"), ref toggle))
        {
            Main.Settings.EnableDistanceOnTooltip = toggle;
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

        UI.Label();

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

        toggle = Main.Settings.EnableHotkeySwapFormationSets;
        if (UI.Toggle(Gui.Localize("ModUi/&FormationHotkey"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableHotkeySwapFormationSets = toggle;
        }

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

        toggle = Main.Settings.EnableVttCamera;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableVttCamera"), ref toggle))
        {
            Main.Settings.EnableVttCamera = toggle;
        }

        UI.Label();

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

        toggle = Main.Settings.AddCustomIconsToOfficialItems;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&AddCustomIconsToOfficialItems")), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddCustomIconsToOfficialItems = toggle;
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

        toggle = Main.Settings.EnableInvisibleCrownOfTheMagister;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableInvisibleCrownOfTheMagister"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableInvisibleCrownOfTheMagister = toggle;
            GameUiContext.SwitchCrownOfTheMagister();
        }

        toggle = Main.Settings.DontDisplayHelmets;
        if (UI.Toggle(Gui.Localize("ModUi/&DontDisplayHelmets"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DontDisplayHelmets = toggle;
            ItemCraftingMerchantContext.SwitchSetBeltOfDwarvenKindBeardChances();
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

        UI.Label();
    }
}
