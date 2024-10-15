using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static TacticalAdventuresApplication;

namespace SolastaUnfinishedBusiness.Models;

public static class InputContext
{
    private const int ModCommandBaseline = 424200;
    internal static InputModManager InputModManagerInstance { get; } = new();

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
            ? typeof(IInputModSettingsService).GetProperty(((InputCommandsExtra)command).ToString())
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
            case InputCommandsExtra.SpawnEncounter when EncountersSpawnContext.EncounterCharacters.Count > 0:
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

    internal static void AddKeybindingSettings(List<Setting> settingList)
    {
        const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.Public |
                                         BindingFlags.GetProperty | BindingFlags.SetProperty |
                                         BindingFlags.FlattenHierarchy;

        settingList.AddRange(
            typeof(IInputModSettingsService)
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
        return (int)command > ModCommandBaseline ? ((InputCommandsExtra)command).ToString() : command.ToString();
    }

    #region Binding Helpers

    private interface IInputModSettingsService
    {
        [SettingTypeHeader("ModHeader", SortOrder = 1000)]
        bool ModHeader { get; set; }

        [SettingTypeKeyMapping("DebugOverlay", DisplayFooter = true, SortOrder = 1001)]
        string DebugOverlay { get; set; }

        [SettingTypeKeyMapping("CharacterExport", DisplayFooter = true, SortOrder = 1002)]
        string CharacterExport { get; set; }

        [SettingTypeKeyMapping("SpawnEncounter", DisplayFooter = false, SortOrder = 1003)]
        string SpawnEncounter { get; set; }

        [SettingTypeKeyMapping("RejoinParty", DisplayFooter = true, SortOrder = 1011)]
        string RejoinParty { get; set; }

        [SettingTypeKeyMapping("TeleportParty", DisplayFooter = false, SortOrder = 1012)]
        string TeleportParty { get; set; }

        [SettingTypeKeyMapping("ToggleHud", DisplayFooter = true, SortOrder = 1021)]
        string ToggleHud { get; set; }

        [SettingTypeKeyMapping("VttCamera", DisplayFooter = false, SortOrder = 1022)]
        string VttCamera { get; set; }

        [SettingTypeKeyMapping("FormationSet1", DisplayFooter = true, SortOrder = 1031)]
        string FormationSet1 { get; set; }

        [SettingTypeKeyMapping("FormationSet2", DisplayFooter = true, SortOrder = 1032)]
        string FormationSet2 { get; set; }

        [SettingTypeKeyMapping("FormationSet3", DisplayFooter = true, SortOrder = 1033)]
        string FormationSet3 { get; set; }

        [SettingTypeKeyMapping("FormationSet4", DisplayFooter = true, SortOrder = 1034)]
        string FormationSet4 { get; set; }

        [SettingTypeKeyMapping("FormationSet5", DisplayFooter = false, SortOrder = 1035)]
        string FormationSet5 { get; set; }
    }

    internal sealed class InputModManager : IInputModSettingsService
    {
        private string _characterExport = string.Empty;

        private string _debugOverlay = string.Empty;

        private string _formationSet1 = string.Empty;

        private string _formationSet2 = string.Empty;

        private string _formationSet3 = string.Empty;

        private string _formationSet4 = string.Empty;

        private string _formationSet5 = string.Empty;

        private string _rejoinParty = string.Empty;

        private string _spawnEncounter = string.Empty;

        private string _teleportParty = string.Empty;

        private string _toggleHud = string.Empty;

        private string _vttCamera = string.Empty;

        public bool ModHeader { get; set; }

        public string CharacterExport
        {
            get => _characterExport;
            set
            {
                _characterExport = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/CharacterExport", _characterExport);
            }
        }

        public string DebugOverlay
        {
            get => _debugOverlay;
            set
            {
                _debugOverlay = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/DebugOverlay", _debugOverlay);
            }
        }

        public string RejoinParty
        {
            get => _rejoinParty;
            set
            {
                _rejoinParty = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/RejoinParty", _rejoinParty);
            }
        }

        public string SpawnEncounter
        {
            get => _spawnEncounter;
            set
            {
                _spawnEncounter = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/SpawnEncounter", _spawnEncounter);
            }
        }

        public string TeleportParty
        {
            get => _teleportParty;
            set
            {
                _teleportParty = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/TeleportParty", _teleportParty);
            }
        }

        public string ToggleHud
        {
            get => _toggleHud;
            set
            {
                _toggleHud = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/ToggleHud", _toggleHud);
            }
        }

        public string VttCamera
        {
            get => _vttCamera;
            set
            {
                _vttCamera = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/VttCamera", _vttCamera);
            }
        }

        public string FormationSet1
        {
            get => _formationSet1;
            set
            {
                _formationSet1 = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/FormationSet1", _formationSet1);
            }
        }

        public string FormationSet2
        {
            get => _formationSet2;
            set
            {
                _formationSet2 = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/FormationSet2", _formationSet2);
            }
        }

        public string FormationSet3
        {
            get => _formationSet3;
            set
            {
                _formationSet3 = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/FormationSet3", _formationSet3);
            }
        }

        public string FormationSet4
        {
            get => _formationSet4;
            set
            {
                _formationSet4 = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/FormationSet4", _formationSet4);
            }
        }

        public string FormationSet5
        {
            get => _formationSet5;
            set
            {
                _formationSet5 = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/FormationSet5", _formationSet5);
            }
        }

        internal void ReadSettings()
        {
            CharacterExport =
                UserPreferences.GetValue("Settings/Keyboard/CharacterExport", _characterExport);
            DebugOverlay =
                UserPreferences.GetValue("Settings/Keyboard/DebugOverlay", _debugOverlay);
            FormationSet1 =
                UserPreferences.GetValue("Settings/Keyboard/FormationSet1", _formationSet1);
            FormationSet2 =
                UserPreferences.GetValue("Settings/Keyboard/FormationSet2", _formationSet2);
            FormationSet3 =
                UserPreferences.GetValue("Settings/Keyboard/FormationSet3", _formationSet3);
            FormationSet4 =
                UserPreferences.GetValue("Settings/Keyboard/FormationSet4", _formationSet4);
            FormationSet5 =
                UserPreferences.GetValue("Settings/Keyboard/FormationSet5", _formationSet5);
            RejoinParty =
                UserPreferences.GetValue("Settings/Keyboard/RejoinParty", _rejoinParty);
            SpawnEncounter =
                UserPreferences.GetValue("Settings/Keyboard/SpawnEncounter", _spawnEncounter);
            TeleportParty =
                UserPreferences.GetValue("Settings/Keyboard/TeleportParty", _teleportParty);
            ToggleHud =
                UserPreferences.GetValue("Settings/Keyboard/ToggleHud", _toggleHud);
            VttCamera =
                UserPreferences.GetValue("Settings/Keyboard/VttCamera", _vttCamera);
        }

        internal static void RegisterDefaultCommands(InputManager inputManager)
        {
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

        internal void ResetDefaults()
        {
            _characterExport = string.Empty;
            _debugOverlay = string.Empty;
            _formationSet1 = string.Empty;
            _formationSet2 = string.Empty;
            _formationSet3 = string.Empty;
            _formationSet4 = string.Empty;
            _formationSet5 = string.Empty;
            _rejoinParty = string.Empty;
            _spawnEncounter = string.Empty;
            _teleportParty = string.Empty;
            _toggleHud = string.Empty;
            _vttCamera = string.Empty;
        }
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

    #endregion
}
