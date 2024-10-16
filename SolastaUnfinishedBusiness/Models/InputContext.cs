using System;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Models;

public static class InputContext
{
    private const int ModCommandBaseline = 424200;

    internal static void Load()
    {
        // register commands by name
        const string Max = "Max";

        var inputManager = ServiceRepository.GetService<IInputService>() as InputManager;
        var commandByName = inputManager!.commandByName;

        commandByName.Remove(Max);

        foreach (InputCommandsExtra id in Enum.GetValues(typeof(InputCommandsExtra)))
        {
            commandByName.Add(id.ToString(), (InputCommands.Id)id);
        }

        commandByName.Add(Max, (InputCommands.Id)commandByName.Count);

        // register default commands
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.ToggleHud, (int)KeyCode.H);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.CharacterExport, (int)KeyCode.X);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.DebugOverlay, (int)KeyCode.O);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.TeleportParty, (int)KeyCode.T);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.RejoinParty, (int)KeyCode.R);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.SpawnEncounter, (int)KeyCode.P);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.VttCamera, (int)KeyCode.Backspace);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.FormationSet1, -1);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.FormationSet2, -1);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.FormationSet3, -1);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.FormationSet4, -1);
        inputManager.RegisterCommand((InputCommands.Id)InputCommandsExtra.FormationSet5, -1);
    }

    // properly registers extended commands to use the InputModManager instance instead of the vanilla one
    internal static void RegisterCommand(
        InputManager __instance, InputCommands.Id command,
        int primaryKeyCode, int primaryModifier1 = -1, int primaryModifier2 = -1,
        int secondaryKeyCode = -1, int secondaryModifier1 = -1, int secondaryModifier2 = -1)
    {
        var input = string.Empty;
        var commandMapping = new InputCommands.CommandMapping();
        var isExtendedCommand = (int)command > ModCommandBaseline;
        var property = isExtendedCommand
            ? typeof(SettingsContext.IInputModSettingsService).GetProperty(((InputCommandsExtra)command)
                .ToString())
            : typeof(IInputSettingsService).GetProperty(command.ToString());
        object finalInstance = isExtendedCommand ? SettingsContext.InputModManagerInstance : __instance;

        if (property != null)
        {
            input = (string)Convert.ChangeType(property.GetValue(finalInstance, null), typeof(string));
        }

        if (string.IsNullOrEmpty(input))
        {
            commandMapping.PrimaryKeyCode = primaryKeyCode;
            commandMapping.PrimaryModifier1 = primaryModifier1;
            commandMapping.PrimaryModifier2 = primaryModifier2;
            commandMapping.SecondaryKeyCode = secondaryKeyCode;
            commandMapping.SecondaryModifier1 = secondaryModifier1;
            commandMapping.SecondaryModifier2 = secondaryModifier2;
            property?.SetValue(finalInstance, commandMapping.DumpToString(), null);
        }
        else
        {
            commandMapping.ReadFromString(input);
        }

        __instance.commandsMap[(int)command] = commandMapping;
    }

    internal static void HandleInput(GameLocationBaseScreen gameLocationBaseScreen, InputCommands.Id command)
    {
        var isSinglePlayer = !Global.IsMultiplayer;

        switch ((InputCommandsExtra)command)
        {
            case InputCommandsExtra.CharacterExport:
                CharacterExportContext.ExportInspectedCharacter();
                return;
            case InputCommandsExtra.ToggleHud:
                GameUiContext.GameHud.ShowAll(gameLocationBaseScreen);
                return;
            case InputCommandsExtra.DebugOverlay:
                ServiceRepository.GetService<IDebugOverlayService>()?.ToggleActivation();
                return;
            case InputCommandsExtra.TeleportParty:
                GameUiContext.Teleporter.ConfirmTeleportParty(GameUiContext.Teleporter.GetEncounterPosition);
                return;
            case InputCommandsExtra.RejoinParty:
                GameUiContext.Teleporter.ConfirmTeleportParty(GameUiContext.Teleporter.GetLeaderPosition);
                return;
            case InputCommandsExtra.VttCamera:
                GameUiContext.ToggleVttCamera();
                return;
            case InputCommandsExtra.SpawnEncounter
                when EncountersSpawnContext.EncounterCharacters.Count > 0:
                EncountersSpawnContext.ConfirmStageEncounter();
                return;
            case InputCommandsExtra.FormationSet1 when isSinglePlayer:
                GameUiContext.SetFormationGrid(0);
                return;
            case InputCommandsExtra.FormationSet2 when isSinglePlayer:
                GameUiContext.SetFormationGrid(1);
                return;
            case InputCommandsExtra.FormationSet3 when isSinglePlayer:
                GameUiContext.SetFormationGrid(2);
                return;
            case InputCommandsExtra.FormationSet4 when isSinglePlayer:
                GameUiContext.SetFormationGrid(3);
                return;
            case InputCommandsExtra.FormationSet5 when isSinglePlayer:
                GameUiContext.SetFormationGrid(4);
                return;
            default:
                return;
        }
    }

    internal static string GetCommandName(InputCommands.Id command)
    {
        return (int)command > ModCommandBaseline
            ? ((InputCommandsExtra)command).ToString()
            : command.ToString();
    }

    private enum InputCommandsExtra
    {
        CharacterExport = ModCommandBaseline + 1,
        DebugOverlay,
        RejoinParty,
        SpawnEncounter,
        TeleportParty,
        ToggleHud,
        VttCamera,
        FormationSet1 = ModCommandBaseline + 11,
        FormationSet2,
        FormationSet3,
        FormationSet4,
        FormationSet5
    }
}
