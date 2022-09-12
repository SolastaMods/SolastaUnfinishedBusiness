using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Utils;
using TA;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.GadgetBlueprints;

namespace SolastaUnfinishedBusiness.Models;

internal static class GameUiContext
{
    private const int ExitsWithGizmos = 2;

    private static readonly GadgetBlueprint[] GadgetExits =
    {
        VirtualExit, VirtualExitMultiple, Exit, ExitMultiple, TeleporterIndividual, TeleporterParty
    };

    private static bool EnableDebugCamera { get; set; }

    // Converts continuous ratio into series of stepped values
    internal static float GetSteppedHealthRatio(float ratio)
    {
        return ratio switch
        {
            // Green
            >= 1f => 1f,
            // Green
            >= 0.5f => 0.75f,
            // Orange
            >= 0.25f => 0.5f,
            // Red
            > 0f => 0.25f,
            _ => ratio
        };
    }

    internal static bool IsGadgetExit(GadgetBlueprint gadgetBlueprint, bool onlyWithGizmos = false)
    {
        return Array.IndexOf(GadgetExits, gadgetBlueprint) >= (onlyWithGizmos ? ExitsWithGizmos : 0);
    }

    //

    private static void RemoveBugVisualModels()
    {
        if (!Main.Settings.RemoveBugVisualModels)
        {
            return;
        }

        // Spiderlings, fire spider, kindred spirit spider, BadlandsSpider(normal, conjured and wildshaped versions)
        const string ASSET_REFERENCE_SPIDER_1 = "362fc51df586d254ab182ef854396f82";
        //CrimsonSpiderling, PhaseSpider, SpectralSpider, CrimsonSpider, deep spider(normal, conjured and wildshaped versions)
        const string ASSET_REFERENCE_SPIDER_2 = "40b5fe532a9a0814097acdb16c74e967";
        // spider queen
        const string ASSET_REFERENCE_SPIDER_3 = "8fc96b2a8c5fcc243b124d31c63df5d9";
        //Giant_Beetle, Small_Beetle, Redeemer_Zealot, Redeemer_Pilgrim
        const string ASSET_REFERENCE_BEETLE = "04dfcec8c8afb8642a80c1116de218d4";
        //Young_Remorhaz, Remorhaz
        const string ASSET_REFERENCE_REMORHAZ = "ded896e0c4ef46144904375ecadb1bb1";

        var brownBear = DatabaseHelper.MonsterDefinitions.BrownBear;
        var bearPrefab = new AssetReference("cc36634f504fa7049a4499a91749d7d5");

        var wolf = DatabaseHelper.MonsterDefinitions.Wolf;
        var wolfPrefab = new AssetReference("6e02c9bcfb5122042a533e7732182b1d");

        var ape = DatabaseHelper.MonsterDefinitions.Ape_MonsterDefinition;
        var apePrefab = new AssetReference("8f4589a9a294b444785fab045256a713");

        var dbMonsterDefinition = DatabaseRepository.GetDatabase<MonsterDefinition>();

        // check every monster for targeted prefab guid references
        foreach (var monster in dbMonsterDefinition)
        {
            // get monster asset reference for prefab guid comparison
            var value = monster.MonsterPresentation.malePrefabReference;

            switch (value.AssetGUID)
            {
                // swap bears for spiders
                case ASSET_REFERENCE_SPIDER_1:
                case ASSET_REFERENCE_SPIDER_2:
                case ASSET_REFERENCE_SPIDER_3:
                    monster.MonsterPresentation.malePrefabReference = bearPrefab;
                    monster.MonsterPresentation.femalePrefabReference = bearPrefab;
                    monster.GuiPresentation.spriteReference = brownBear.GuiPresentation.SpriteReference;
                    monster.bestiarySpriteReference = brownBear.BestiarySpriteReference;
                    monster.MonsterPresentation.monsterPresentationDefinitions = brownBear.MonsterPresentation
                        .MonsterPresentationDefinitions;
                    break;
                // swap apes for remorhaz
                case ASSET_REFERENCE_REMORHAZ:
                    monster.MonsterPresentation.malePrefabReference = apePrefab;
                    monster.MonsterPresentation.femalePrefabReference = apePrefab;
                    monster.GuiPresentation.spriteReference = ape.GuiPresentation.SpriteReference;
                    monster.bestiarySpriteReference = ape.BestiarySpriteReference;
                    monster.MonsterPresentation.monsterPresentationDefinitions = ape.MonsterPresentation
                        .MonsterPresentationDefinitions;
                    break;
                // swap wolves for beetles
                case ASSET_REFERENCE_BEETLE:
                    monster.MonsterPresentation.malePrefabReference = wolfPrefab;
                    monster.MonsterPresentation.femalePrefabReference = wolfPrefab;
                    monster.GuiPresentation.spriteReference = wolf.GuiPresentation.SpriteReference;
                    monster.bestiarySpriteReference = wolf.BestiarySpriteReference;
                    monster.MonsterPresentation.monsterPresentationDefinitions = wolf.MonsterPresentation
                        .MonsterPresentationDefinitions;

                    // changing beetle scale to suit replacement model
                    monster.MonsterPresentation.maleModelScale = 0.655f;
                    monster.MonsterPresentation.femaleModelScale = 0.655f;
                    break;
            }
        }
    }

    internal static void Load()
    {
        var inputService = ServiceRepository.GetService<IInputService>();

        // Dungeon Maker
        inputService.RegisterCommand(InputCommands.Id.EditorRotate, (int)KeyCode.R, (int)KeyCode.LeftShift);

        // HUD
        inputService.RegisterCommand(Hotkeys.CtrlShiftC, (int)KeyCode.C, (int)KeyCode.LeftShift,
            (int)KeyCode.LeftControl);
        inputService.RegisterCommand(Hotkeys.CtrlShiftL, (int)KeyCode.L, (int)KeyCode.LeftShift,
            (int)KeyCode.LeftControl);
        inputService.RegisterCommand(Hotkeys.CtrlShiftM, (int)KeyCode.M, (int)KeyCode.LeftShift,
            (int)KeyCode.LeftControl);
        inputService.RegisterCommand(Hotkeys.CtrlShiftP, (int)KeyCode.P, (int)KeyCode.LeftShift,
            (int)KeyCode.LeftControl);
        inputService.RegisterCommand(Hotkeys.CtrlShiftH, (int)KeyCode.H, (int)KeyCode.LeftShift,
            (int)KeyCode.LeftControl);

        // Debug Overlay
        inputService.RegisterCommand(Hotkeys.CtrlShiftD, (int)KeyCode.D, (int)KeyCode.LeftShift,
            (int)KeyCode.LeftControl);

        // Export Character
        inputService.RegisterCommand(Hotkeys.CtrlShiftE, (int)KeyCode.E, (int)KeyCode.LeftShift,
            (int)KeyCode.LeftControl);

        // Spawn Encounter
        inputService.RegisterCommand(Hotkeys.CtrlShiftS, (int)KeyCode.S, (int)KeyCode.LeftShift,
            (int)KeyCode.LeftControl);

        // Teleport
        inputService.RegisterCommand(Hotkeys.CtrlShiftT, (int)KeyCode.T, (int)KeyCode.LeftShift,
            (int)KeyCode.LeftControl);

        // Zoom Camera
        inputService.RegisterCommand(Hotkeys.CtrlShiftZ, (int)KeyCode.Z, (int)KeyCode.LeftShift,
            (int)KeyCode.LeftControl);

        // Replace Visual Models
        RemoveBugVisualModels();
    }

    internal static void HandleInput(GameLocationBaseScreen gameLocationBaseScreen, InputCommands.Id command)
    {
        if (Main.Settings.EnableHotkeyToggleIndividualHud)
        {
            switch (command)
            {
                case Hotkeys.CtrlShiftC:
                    GameHud.ShowCharacterControlPanel(gameLocationBaseScreen);
                    return;

                case Hotkeys.CtrlShiftL:
                    GameHud.TogglePanelVisibility(Gui.GuiService.GetScreen<GuiConsoleScreen>());
                    return;

                case Hotkeys.CtrlShiftM:
                    GameHud.TogglePanelVisibility(GetTimeAndNavigationPanel());
                    return;

                case Hotkeys.CtrlShiftP:
                    GameHud.TogglePanelVisibility(GetInitiativeOrPartyPanel());
                    return;
            }
        }

        if (Main.Settings.EnableHotkeyToggleHud && command == Hotkeys.CtrlShiftH)
        {
            GameHud.ShowAll(gameLocationBaseScreen, GetInitiativeOrPartyPanel(), GetTimeAndNavigationPanel());
        }
        else if (Main.Settings.EnableHotkeyDebugOverlay && command == Hotkeys.CtrlShiftD)
        {
            ServiceRepository.GetService<IDebugOverlayService>()?.ToggleActivation();
        }
        else if (Main.Settings.EnableTeleportParty && command == Hotkeys.CtrlShiftT)
        {
            Teleporter.ConfirmTeleportParty();
        }
        else if (Main.Settings.EnableHotkeyZoomCamera && command == Hotkeys.CtrlShiftZ)
        {
            ToggleZoomCamera();
        }
        else if (EncountersSpawnContext.EncounterCharacters.Count > 0 && command == Hotkeys.CtrlShiftS)
        {
            EncountersSpawnContext.ConfirmStageEncounter();
        }

        void ToggleZoomCamera()
        {
            var cameraService = ServiceRepository.GetService<ICameraService>();

            if (cameraService == null)
            {
                return;
            }

            EnableDebugCamera = !EnableDebugCamera;
            cameraService.DebugCameraEnabled = EnableDebugCamera;
        }

        [SuppressMessage("Minor Code Smell", "IDE0066:Use switch expression", Justification = "Prefer switch here")]
        [CanBeNull]
        GuiPanel GetInitiativeOrPartyPanel()
        {
            return gameLocationBaseScreen switch
            {
                GameLocationScreenExploration gameLocationScreenExploration => gameLocationScreenExploration
                    .partyControlPanel,
                GameLocationScreenBattle gameLocationScreenBattle => gameLocationScreenBattle.initiativeTable,
                _ => null
            };
        }

        [SuppressMessage("Minor Code Smell", "IDE0066:Use switch expression", Justification = "Prefer switch here")]
        [CanBeNull]
        TimeAndNavigationPanel GetTimeAndNavigationPanel()
        {
            return gameLocationBaseScreen switch
            {
                GameLocationScreenExploration gameLocationScreenExploration => gameLocationScreenExploration
                    .timeAndNavigationPanel,
                GameLocationScreenBattle gameLocationScreenBattle => gameLocationScreenBattle.timeAndNavigationPanel,
                _ => null
            };
        }
    }

    internal static class GameHud
    {
        internal static void ShowAll([NotNull] GameLocationBaseScreen gameLocationBaseScreen,
            GuiPanel initiativeOrPartyPanel,
            TimeAndNavigationPanel timeAndNavigationPanel)
        {
            var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();
            var anyVisible = guiConsoleScreen.Visible || gameLocationBaseScreen.CharacterControlPanel.Visible ||
                             initiativeOrPartyPanel.Visible || timeAndNavigationPanel.Visible;

            ShowCharacterControlPanel(gameLocationBaseScreen, anyVisible);
            TogglePanelVisibility(guiConsoleScreen, anyVisible);
            TogglePanelVisibility(initiativeOrPartyPanel);
            TogglePanelVisibility(timeAndNavigationPanel, anyVisible);
        }

        internal static void ShowCharacterControlPanel([NotNull] GameLocationBaseScreen gameLocationBaseScreen,
            bool forceHide = false)
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

                if (gameLocationSelectionService.SelectedCharacters.Count <= 0)
                {
                    return;
                }

                characterControlPanel.Bind(gameLocationSelectionService.SelectedCharacters[0],
                    gameLocationBaseScreen.ActionTooltipDock);
                characterControlPanel.Show();
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

        public static void RefreshCharacterControlPanel()
        {
            if (Gui.CurrentLocationScreen != null && Gui.CurrentLocationScreen is GameLocationBaseScreen location)
            {
                location.CharacterControlPanel.RefreshNow();
            }
        }
    }

    private static class Teleporter
    {
        internal static void ConfirmTeleportParty()
        {
            var position = GetEncounterPosition();

            Gui.GuiService.ShowMessage(
                MessageModal.Severity.Attention2,
                "Message/&TeleportPartyTitle",
                Gui.Format("Message/&TeleportPartyDescription", position.x.ToString(), position.x.ToString()),
                "Message/&MessageYesTitle", "Message/&MessageNoTitle",
                () => TeleportParty(position),
                null);
        }

        private static int3 GetEncounterPosition()
        {
            var gameLocationService = ServiceRepository.GetService<IGameLocationService>();

            var x = (int)gameLocationService.GameLocation.LastCameraPosition.x;
            var z = (int)gameLocationService.GameLocation.LastCameraPosition.z;

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

            gameLocationPositioningService.ComputeFormationPlacementPositions(partyAndGuests, position,
                LocationDefinitions.Orientation.North, formationPositions, CellHelpers.PlacementMode.Station,
                positions, new List<RulesetActor.SizeParameters>(), 25);

            for (var index = 0; index < positions.Count; index++)
            {
                partyAndGuests[index].LocationPosition = positions[index];

                // rotates the characters in position to force the game to redrawn them
                gameLocationActionService.MoveCharacter(partyAndGuests[index],
                    positions[(index + 1) % positions.Count], LocationDefinitions.Orientation.North, 0,
                    ActionDefinitions.MoveStance.Walk);
            }
        }
    }
}
