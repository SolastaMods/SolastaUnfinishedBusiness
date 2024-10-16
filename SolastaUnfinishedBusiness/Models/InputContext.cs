using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SolastaUnfinishedBusiness.Models;

public static class InputContext
{
    internal const int ModCommandBaseline = 424200;
    internal static SettingsContext.InputModManager InputModManagerInstance { get; } = new();

    internal static void Load()
    {
        // register commands by name
        const string Max = "Max";

        var inputManager = ServiceRepository.GetService<IInputService>() as InputManager;
        var commandByName = inputManager!.commandByName;

        commandByName.Remove(Max);

        foreach (SettingsContext.InputCommandsExtra id in Enum.GetValues(typeof(SettingsContext.InputCommandsExtra)))
        {
            commandByName.Add(id.ToString(), (InputCommands.Id)id);
        }

        commandByName.Add(Max, (InputCommands.Id)commandByName.Count);
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
            ? typeof(SettingsContext.IInputModSettingsService).GetProperty(((SettingsContext.InputCommandsExtra)command)
                .ToString())
            : typeof(IInputSettingsService).GetProperty(command.ToString());
        object finalInstance = isExtendedCommand ? InputModManagerInstance : __instance;

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

        switch ((SettingsContext.InputCommandsExtra)command)
        {
            case SettingsContext.InputCommandsExtra.CharacterExport:
                CharacterExportContext.ExportInspectedCharacter();
                return;
            case SettingsContext.InputCommandsExtra.ToggleHud:
                GameUiContext.GameHud.ShowAll(gameLocationBaseScreen);
                return;
            case SettingsContext.InputCommandsExtra.DebugOverlay:
                ServiceRepository.GetService<IDebugOverlayService>()?.ToggleActivation();
                return;
            case SettingsContext.InputCommandsExtra.TeleportParty:
                GameUiContext.Teleporter.ConfirmTeleportParty(GameUiContext.Teleporter.GetEncounterPosition);
                return;
            case SettingsContext.InputCommandsExtra.RejoinParty:
                GameUiContext.Teleporter.ConfirmTeleportParty(GameUiContext.Teleporter.GetLeaderPosition);
                return;
            case SettingsContext.InputCommandsExtra.VttCamera:
                GameUiContext.ToggleVttCamera();
                return;
            case SettingsContext.InputCommandsExtra.SpawnEncounter
                when EncountersSpawnContext.EncounterCharacters.Count > 0:
                EncountersSpawnContext.ConfirmStageEncounter();
                return;
            case SettingsContext.InputCommandsExtra.FormationSet1 when isSinglePlayer:
                GameUiContext.SetFormationGrid(0);
                return;
            case SettingsContext.InputCommandsExtra.FormationSet2 when isSinglePlayer:
                GameUiContext.SetFormationGrid(1);
                return;
            case SettingsContext.InputCommandsExtra.FormationSet3 when isSinglePlayer:
                GameUiContext.SetFormationGrid(2);
                return;
            case SettingsContext.InputCommandsExtra.FormationSet4 when isSinglePlayer:
                GameUiContext.SetFormationGrid(3);
                return;
            case SettingsContext.InputCommandsExtra.FormationSet5 when isSinglePlayer:
                GameUiContext.SetFormationGrid(4);
                return;
            default:
                return;
        }
    }

    internal static void AddKeybindingSettings(List<Setting> settingList)
    {
        const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.Public |
                                         BindingFlags.GetProperty | BindingFlags.SetProperty |
                                         BindingFlags.FlattenHierarchy;

        settingList.AddRange(
            typeof(SettingsContext.IInputModSettingsService)
                .GetProperties(BindingAttr)
                .Select(property => new
                {
                    property,
                    customAttributes =
                        (SettingTypeAttribute[])property.GetCustomAttributes(typeof(SettingTypeAttribute), true)
                })
                .Where(t => t.customAttributes.Length != 0)
                .Select(t => new Setting(InputModManagerInstance, t.property, t.customAttributes[0])));
    }

    internal static string GetCommandName(InputCommands.Id command)
    {
        return (int)command > ModCommandBaseline
            ? ((SettingsContext.InputCommandsExtra)command).ToString()
            : command.ToString();
    }
}
