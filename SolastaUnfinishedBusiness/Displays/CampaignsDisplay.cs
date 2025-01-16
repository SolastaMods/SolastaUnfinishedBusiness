using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Displays;

internal static class CampaignsDisplay
{
    internal const float DefaultFastTimeModifier = 1.5f;
    private static bool _selectedForSwap;
    private static int _selectedX, _selectedY;
    private static readonly string[] SetNames = ["1", "2", "3", "4", "5"];

    internal static void DisplayGameUi()
    {
        #region Campaign

        UI.Label();
        UI.Label();

        var toggle = Main.Settings.NoExperienceOnLevelUp;
        if (UI.Toggle(Gui.Localize("ModUi/&NoExperienceOnLevelUp"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.NoExperienceOnLevelUp = toggle;
        }

        toggle = Main.Settings.OverrideMinMaxLevel;
        if (UI.Toggle(Gui.Localize("ModUi/&OverrideMinMaxLevel"), ref toggle))
        {
            Main.Settings.OverrideMinMaxLevel = toggle;
        }

        UI.Label();

        var floatValue = Main.Settings.FasterTimeModifier;
        if (UI.Slider(Gui.Localize("ModUi/&FasterTimeModifier"), ref floatValue,
                DefaultFastTimeModifier, 10f, DefaultFastTimeModifier, 1, string.Empty, UI.AutoWidth()))
        {
            Main.Settings.FasterTimeModifier = floatValue;
        }

        var intValue = Main.Settings.MultiplyTheExperienceGainedBy;
        if (UI.Slider(Gui.Localize("ModUi/&MultiplyTheExperienceGainedBy"), ref intValue, 0, 200, 100, string.Empty,
                UI.Width(100f)))
        {
            Main.Settings.MultiplyTheExperienceGainedBy = intValue;
        }

        intValue = Main.Settings.OverridePartySize;
        if (UI.Slider(Gui.Localize("ModUi/&OverridePartySize"), ref intValue,
                ToolsContext.MinPartySize, ToolsContext.MaxPartySize,
                ToolsContext.GamePartySize, string.Empty, UI.AutoWidth()))
        {
            Main.Settings.OverridePartySize = intValue;

            while (Main.Settings.DefaultPartyHeroes.Count > intValue)
            {
                Main.Settings.DefaultPartyHeroes.RemoveAt(Main.Settings.DefaultPartyHeroes.Count - 1);
            }
        }

        if (Main.Settings.OverridePartySize > ToolsContext.GamePartySize)
        {
            toggle = Main.Settings.AllowAllPlayersOnNarrativeSequences;
            if (UI.Toggle(Gui.Localize("ModUi/&AllowAllPlayersOnNarrativeSequences"), ref toggle))
            {
                Main.Settings.AllowAllPlayersOnNarrativeSequences = toggle;
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

        toggle = Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView;
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

        toggle = Main.Settings.EnableSpeech;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSpeech"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSpeech = toggle;
        }

        if (Main.Settings.EnableSpeech)
        {
            UI.Label();

            using (UI.HorizontalScope())
            {
                UI.ActionButton(
                    Gui.Localize("ModUi/&RefreshVoice"),
                    SpeechContext.RefreshAvailableVoices, UI.Width(227f));
                UI.ActionButton(
                    SpeechContext.VoicesDownloader.Shared.GetButtonLabel(),
                    SpeechContext.VoicesDownloader.Shared.DownloadVoices, UI.Width(227f));
            }

            UI.Label();

            toggle = Main.Settings.EnableSpeechOnNpcs;
            if (UI.Toggle(Gui.Localize("ModUi/&EnableSpeechOnNpcs"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableSpeechOnNpcs = toggle;
            }

            toggle = Main.Settings.ForceModSpeechOnNpcs;
            if (UI.Toggle(Gui.Localize("ModUi/&ForceModSpeechOnNpcs"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.ForceModSpeechOnNpcs = toggle;
            }

            UI.Label();
            UI.Label(Gui.Localize("ModUi/&EnableSpeechActorHelp"));
            UI.Label();

            intValue = Main.Settings.SpeechChoice;
            if (UI.SelectionGrid(
                    ref intValue, SpeechContext.Choices, SpeechContext.Choices.Length, SpeechContext.MaxHeroes + 1,
                    UI.Width(800f)))
            {
                Main.Settings.SpeechChoice = intValue;
            }

            UI.Label();
            UI.Label(Gui.Localize("ModUi/&EnableSpeechVoiceHelp"));
            UI.Label();

            (var voice, floatValue) = Main.Settings.SpeechVoices[Main.Settings.SpeechChoice];

            intValue = Array.IndexOf(SpeechContext.VoiceNames, voice);

            if (UI.Slider(Gui.Localize("ModUi/&SpeechScale"), ref floatValue,
                    0.5f, 2f, 0.8f, 1, string.Empty, UI.AutoWidth()))
            {
                voice = SpeechContext.VoiceNames[intValue];
                Main.Settings.SpeechVoices[Main.Settings.SpeechChoice] = (voice, floatValue);
            }

            floatValue = Main.Settings.SpeechVolume;
            if (UI.Slider(Gui.Localize("ModUi/&SpeechVolume"), ref floatValue,
                    0.0f, 1.2f, 1, 1, string.Empty, UI.AutoWidth()))
            {
                Main.Settings.SpeechVolume = floatValue;
            }

            UI.Label();

            if (UI.SelectionGrid(
                    ref intValue, SpeechContext.VoiceNames, SpeechContext.VoiceNames.Length, 3, UI.Width(800f)))
            {
                voice = SpeechContext.VoiceNames[intValue];
                Main.Settings.SpeechVoices[Main.Settings.SpeechChoice] = (voice, floatValue);
                SpeechContext.SpeakQuote();
                SpeechContext.UpdateAvailableVoices();
            }

            UI.Label();
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

        toggle = Main.Settings.AllowMoreRealStateOnRestPanel;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowMoreRealStateOnRestPanel"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowMoreRealStateOnRestPanel = toggle;
        }

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

        toggle = Main.Settings.EnableExtendedProficienciesPanelDisplay;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableExtendedProficienciesPanelDisplay"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableExtendedProficienciesPanelDisplay = toggle;
        }

        UI.Label();
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

        var color = CampaignsContext.HighContrastColorStrings[Main.Settings.HighContrastTargetingAoeSelectedColor];
        var title = Gui.Localize("ModUi/&HighContrastTargetingAoeColor").Replace("$$$$$$", color);

        UI.ActionButton(title, () =>
        {
            Main.Settings.HighContrastTargetingAoeSelectedColor =
                (Main.Settings.HighContrastTargetingAoeSelectedColor + 1) % CampaignsContext.HighContrastColors.Length;
        }, UI.Width(300f));

        color = CampaignsContext.HighContrastColorStrings[Main.Settings.HighContrastTargetingSingleSelectedColor];
        title = Gui.Localize("ModUi/&HighContrastTargetingSingleColor").Replace("$$$$$$", color);

        UI.ActionButton(title, () =>
        {
            Main.Settings.HighContrastTargetingSingleSelectedColor =
                (Main.Settings.HighContrastTargetingSingleSelectedColor + 1) %
                CampaignsContext.HighContrastColors.Length;
        }, UI.Width(300f));

        color = CampaignsContext.GridColorStrings[Main.Settings.GridSelectedColor];
        title = Gui.Localize("ModUi/&GridSelectedColor").Replace("$$$$$$", color);

        UI.ActionButton(title, () =>
        {
            Main.Settings.GridSelectedColor =
                (Main.Settings.GridSelectedColor + 1) % CampaignsContext.GridColors.Length;
            CampaignsContext.UpdateMovementGrid();
        }, UI.Width(300f));

        intValue = Main.Settings.MovementGridWidthModifier;
        if (UI.Slider(Gui.Localize("ModUi/&MovementGridWidthModifier"), ref intValue, 0, 200, 100, string.Empty,
                UI.Width(100f)))
        {
            Main.Settings.MovementGridWidthModifier = intValue;
            CampaignsContext.UpdateMovementGrid();
        }

        intValue = Main.Settings.OutlineGridWidthModifier;
        if (UI.Slider(Gui.Localize("ModUi/&OutlineGridWidthModifier"), ref intValue, 0, 200, 100, string.Empty,
                UI.Width(100f)))
        {
            Main.Settings.OutlineGridWidthModifier = intValue;
            CampaignsContext.UpdateMovementGrid();
        }

        intValue = Main.Settings.OutlineGridWidthSpeed;
        if (UI.Slider(Gui.Localize("ModUi/&OutlineGridWidthSpeed"), ref intValue, 0, 200, 100, string.Empty,
                UI.Width(100f)))
        {
            Main.Settings.OutlineGridWidthSpeed = intValue;
            CampaignsContext.UpdateMovementGrid();
        }

        #endregion

        #region Factions

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&FactionRelations"));
        UI.Label();

        var flip = true;
        var gameCampaign = Gui.GameCampaign;
        var gameFactionService = ServiceRepository.GetService<IGameFactionService>();

        // NOTE: don't use gameCampaign?. which bypasses Unity object lifetime check
        if (gameCampaign)
        {
            if (gameFactionService != null)
            {
                foreach (var faction in gameFactionService.RegisteredFactions.Where(faction =>
                             !faction.BuiltIn &&
                             !faction.GuiPresentation.Hidden))
                {
                    title = faction.FormatTitle();
                    title = flip ? title.Khaki() : title.White();

                    intValue = gameFactionService.FactionRelations[faction.Name];

                    if (UI.Slider("                              " + title, ref intValue, faction.MinRelationCap,
                            faction.MaxRelationCap, 0, "", UI.AutoWidth()))
                    {
                        SetFactionRelation(faction.Name, intValue);
                    }

                    flip = !flip;
                }
            }
            else
            {
                UI.Label(Gui.Localize("ModUi/&FactionHelp"));
            }
        }
        else
        {
            UI.Label(Gui.Localize("ModUi/&FactionHelp"));
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

        #region Merchants

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Merchants"));
        UI.Label();

        toggle = Main.Settings.ScaleMerchantPricesCorrectly;
        if (UI.Toggle(Gui.Localize("ModUi/&ScaleMerchantPricesCorrectly"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ScaleMerchantPricesCorrectly = toggle;
        }

        toggle = Main.Settings.StockGorimStoreWithAllNonMagicalClothing;
        if (UI.Toggle(Gui.Localize("ModUi/&StockGorimStoreWithAllNonMagicalClothing"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.StockGorimStoreWithAllNonMagicalClothing = toggle;
        }

        toggle = Main.Settings.StockGorimStoreWithAllNonMagicalInstruments;
        if (UI.Toggle(Gui.Localize("ModUi/&StockGorimStoreWithAllNonMagicalInstruments"), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.StockGorimStoreWithAllNonMagicalInstruments = toggle;
        }

        toggle = Main.Settings.StockHugoStoreWithAdditionalFoci;
        if (UI.Toggle(Gui.Localize("ModUi/&StockHugoStoreWithAdditionalFoci"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.StockHugoStoreWithAdditionalFoci = toggle;
            Main.Settings.EnableAdditionalFociInDungeonMaker = toggle;
            ItemCraftingMerchantContext.SwitchFociItems();
        }

        if (Main.Settings.StockHugoStoreWithAdditionalFoci)
        {
            toggle = Main.Settings.EnableAdditionalFociInDungeonMaker;
            if (UI.Toggle(Gui.Localize("ModUi/&EnableAdditionalItemsInDungeonMaker"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableAdditionalFociInDungeonMaker = toggle;
                ItemCraftingMerchantContext.SwitchFociItemsDungeonMaker();
            }
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&RestockHelp"));
        UI.Label();

        toggle = Main.Settings.RestockAntiquarians;
        if (UI.Toggle(Gui.Localize("ModUi/&RestockAntiquarians"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RestockAntiquarians = toggle;
            ItemCraftingMerchantContext.SwitchRestockAntiquarian();
        }

        toggle = Main.Settings.RestockArcaneum;
        if (UI.Toggle(Gui.Localize("ModUi/&RestockArcaneum"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RestockArcaneum = toggle;
            ItemCraftingMerchantContext.SwitchRestockArcaneum();
        }

        toggle = Main.Settings.RestockCircleOfDanantar;
        if (UI.Toggle(Gui.Localize("ModUi/&RestockCircleOfDanantar"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RestockCircleOfDanantar = toggle;
            ItemCraftingMerchantContext.SwitchRestockCircleOfDanantar();
        }

        toggle = Main.Settings.RestockTowerOfKnowledge;
        // ReSharper disable once InvertIf
        if (UI.Toggle(Gui.Localize("ModUi/&RestockTowerOfKnowledge"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RestockTowerOfKnowledge = toggle;
            ItemCraftingMerchantContext.SwitchRestockTowerOfKnowledge();
        }

        #endregion

        UI.Label();
    }

    private static void SetFactionRelation(string name, int value)
    {
        var service = ServiceRepository.GetService<IGameFactionService>();

        service?.ExecuteFactionOperation(name, FactionDefinition.FactionOperation.Increase,
            value - service.FactionRelations[name], "",
            null /* this string and monster doesn't matter if we're using "SetValue" */);
    }

    private static void DisplayFormationGrid()
    {
        var selectedSet = Main.Settings.FormationGridSelectedSet;

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&FormationResetAllSets"), () =>
                {
                    _selectedForSwap = false;
                    CampaignsContext.ResetAllFormationGrids();
                },
                UI.Width(110f));

            if (UI.SelectionGrid(ref selectedSet, SetNames, SetNames.Length, SetNames.Length, UI.Width(165f)))
            {
                _selectedForSwap = false;
                Main.Settings.FormationGridSelectedSet = selectedSet;
                CampaignsContext.FillDefinitionFromFormationGrid();
            }

            UI.Label(Gui.Localize("ModUi/&FormationHelp1"));
        }

        UI.Label();

        for (var y = 0; y < CampaignsContext.GridSize; y++)
        {
            using (UI.HorizontalScope())
            {
                // first line
                if (y == 0)
                {
                    UI.ActionButton(Gui.Localize("ModUi/&FormationResetThisSet"), () =>
                        {
                            _selectedForSwap = false;
                            CampaignsContext.ResetFormationGrid(Main.Settings.FormationGridSelectedSet);
                        },
                        UI.Width(110f));
                }
                else
                {
                    UI.Label("", UI.Width(110f));
                }

                for (var x = 0; x < CampaignsContext.GridSize; x++)
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

                            CampaignsContext.FillDefinitionFromFormationGrid();

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
