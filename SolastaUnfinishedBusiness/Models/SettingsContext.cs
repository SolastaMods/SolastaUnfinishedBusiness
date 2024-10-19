using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using static TacticalAdventuresApplication;

namespace SolastaUnfinishedBusiness.Models;

public static class SettingsContext
{
    internal static GuiModManager GuiModManagerInstance { get; } = new();
    internal static InputModManager InputModManagerInstance { get; } = new();

    internal static Setting[] GetExtendedSettingList(IService serviceProvider, Setting[] __result, bool sortByPriority)
    {
        return serviceProvider switch
        {
            IGuiSettingsService =>
                AddSettings<IGuiModSettingsService>(__result, GuiModManagerInstance, sortByPriority),
            IInputSettingsService =>
                AddSettings<IInputModSettingsService>(__result, InputModManagerInstance, sortByPriority),
            _ => __result
        };
    }

    private static Setting[] AddSettings<T>(Setting[] settings, object provider, bool sortByPriority)
    {
        const BindingFlags BindingAttr = BindingFlags.Instance | BindingFlags.Public |
                                         BindingFlags.GetProperty | BindingFlags.SetProperty |
                                         BindingFlags.FlattenHierarchy;

        var settingList = settings.ToList();

        settingList.AddRange(
            typeof(T)
                .GetProperties(BindingAttr)
                .Select(property => new
                {
                    property,
                    customAttributes =
                        (SettingTypeAttribute[])property.GetCustomAttributes(typeof(SettingTypeAttribute), true)
                })
                .Where(t => t.customAttributes.Length != 0)
                .Select(t => new Setting(provider, t.property, t.customAttributes[0])));

        if (sortByPriority && settingList.Count >= 2)
        {
            settingList.Sort((left, right) =>
                left.SettingTypeAttribute.SortOrder.CompareTo(right.SettingTypeAttribute.SortOrder));
        }

        return [.. settingList];
    }

    #region Gui Settings

    public interface IGuiModSettingsService
    {
        [SettingTypeHeader("ModHeader", SortOrder = 1000)]
        [UsedImplicitly]
        bool ModHeader { get; set; }

        [SettingTypeToggle("EnableCharacterChecker", SortOrder = 1011, DisplayFooter = true)]
        [UsedImplicitly]
        bool EnableCharacterChecker { get; set; }

        [SettingTypeToggle("EnableCheatMenu", SortOrder = 1012, DisplayFooter = true)]
        [UsedImplicitly]
        bool EnableCheatMenu { get; set; }

        [SettingTypeToggle("EnableSaveByLocation", SortOrder = 1013, DisplayFooter = true)]
        [UsedImplicitly]
        bool EnableSaveByLocation { get; set; }

        [SettingTypeToggle("EnablePartyToggles", SortOrder = 1014, DisplayFooter = false)]
        [UsedImplicitly]
        bool EnablePartyToggles { get; set; }

        [SettingTypeToggle("InvertTooltipBehavior", SortOrder = 1021, DisplayFooter = true)]
        [UsedImplicitly]
        bool InvertTooltipBehavior { get; set; }

        [SettingTypeSlider("TooltipWidth", SortOrder = 1022, MinValue = 1f, MaxValue = 2f, Default = 1f, Format = "0.0",
            DisplayFooter = false)]
        [UsedImplicitly]
        float TooltipWidth { get; set; }
    }

    internal sealed class GuiModManager : IGuiModSettingsService
    {
        private bool _enableCharacterChecker = UserPreferences.GetValue<bool>("Settings/Gui/EnableCharacterChecker");
        private bool _enableCheatMenu = UserPreferences.GetValue<bool>("Settings/Gui/EnableCheatMenu");
        private bool _enablePartyToggles = UserPreferences.GetValue<bool>("Settings/Gui/EnablePartyToggles");
        private bool _enableSaveByLocation = UserPreferences.GetValue<bool>("Settings/Gui/EnableSaveByLocation");
        private bool _invertTooltipBehavior = UserPreferences.GetValue<bool>("Settings/Gui/InvertTooltipBehavior");
        private float _tooltipWidth = UserPreferences.GetValue<float>("Settings/Gui/TooltipWidth");

        public bool ModHeader { get; set; }

        public bool InvertTooltipBehavior
        {
            get => _invertTooltipBehavior;
            set
            {
                _invertTooltipBehavior = value;
                UserPreferences.SetValue("Settings/Gui/InvertTooltipBehavior", _invertTooltipBehavior);
            }
        }

        public float TooltipWidth
        {
            get => _tooltipWidth;
            set
            {
                _tooltipWidth = value;
                UserPreferences.SetValue("Settings/Gui/TooltipWidth", _tooltipWidth);
            }
        }

        public bool EnableSaveByLocation
        {
            get => _enableSaveByLocation;
            set
            {
                _enableSaveByLocation = value;
                UserPreferences.SetValue("Settings/Gui/EnableSaveByLocation", _enableSaveByLocation);
            }
        }

        public bool EnablePartyToggles
        {
            get => _enablePartyToggles;
            set
            {
                _enablePartyToggles = value;
                UserPreferences.SetValue("Settings/Gui/EnablePartyToggles", _enablePartyToggles);
            }
        }

        public bool EnableCharacterChecker
        {
            get => _enableCharacterChecker;
            set
            {
                _enableCharacterChecker = value;
                UserPreferences.SetValue("Settings/Gui/EnableCharacterChecker", _enableCharacterChecker);
            }
        }

        public bool EnableCheatMenu
        {
            get => _enableCheatMenu;
            set
            {
                _enableCheatMenu = value;
                UserPreferences.SetValue("Settings/Gui/EnableCheatMenu", _enableCheatMenu);
            }
        }
    }

    #endregion

    #region Keyboard Settings

    internal interface IInputModSettingsService
    {
        [SettingTypeHeader("ModHeader", SortOrder = 1000)]
        [UsedImplicitly]
        bool ModHeader { get; set; }

        [SettingTypeKeyMapping("DebugOverlay", DisplayFooter = true, SortOrder = 1001)]
        [UsedImplicitly]
        string DebugOverlay { get; set; }

        [SettingTypeKeyMapping("CharacterExport", DisplayFooter = true, SortOrder = 1002)]
        [UsedImplicitly]
        string CharacterExport { get; set; }

        [SettingTypeKeyMapping("SpawnEncounter", DisplayFooter = false, SortOrder = 1003)]
        [UsedImplicitly]
        string SpawnEncounter { get; set; }

        [SettingTypeKeyMapping("RejoinParty", DisplayFooter = true, SortOrder = 1011)]
        [UsedImplicitly]
        string RejoinParty { get; set; }

        [SettingTypeKeyMapping("TeleportParty", DisplayFooter = false, SortOrder = 1012)]
        [UsedImplicitly]
        string TeleportParty { get; set; }

        [SettingTypeKeyMapping("ToggleHud", DisplayFooter = true, SortOrder = 1021)]
        [UsedImplicitly]
        string ToggleHud { get; set; }

        [SettingTypeKeyMapping("VttCamera", DisplayFooter = false, SortOrder = 1022)]
        [UsedImplicitly]
        string VttCamera { get; set; }

        [SettingTypeKeyMapping("FormationSet1", DisplayFooter = true, SortOrder = 1031)]
        [UsedImplicitly]
        string FormationSet1 { get; set; }

        [SettingTypeKeyMapping("FormationSet2", DisplayFooter = true, SortOrder = 1032)]
        [UsedImplicitly]
        string FormationSet2 { get; set; }

        [SettingTypeKeyMapping("FormationSet3", DisplayFooter = true, SortOrder = 1033)]
        [UsedImplicitly]
        string FormationSet3 { get; set; }

        [SettingTypeKeyMapping("FormationSet4", DisplayFooter = true, SortOrder = 1034)]
        [UsedImplicitly]
        string FormationSet4 { get; set; }

        [SettingTypeKeyMapping("FormationSet5", DisplayFooter = false, SortOrder = 1035)]
        [UsedImplicitly]
        string FormationSet5 { get; set; }

        //
        // these should blend with vanilla settings
        //

        // order here matters as the sort routine will revert items in the collection if same sort order

        [SettingTypeKeyMapping("SelectCharacter6", DisplayFooter = true, SortOrder = 29)]
        string SelectCharacter6 { get; set; }

        [SettingTypeKeyMapping("SelectCharacter5", DisplayFooter = true, SortOrder = 29)]
        string SelectCharacter5 { get; set; }

        [SettingTypeKeyMapping("Hide", DisplayFooter = true, SortOrder = 22)]
        string Hide { get; set; }
    }

    internal sealed class InputModManager : IInputModSettingsService
    {
        private string _characterExport = UserPreferences.GetValue("Settings/Keyboard/CharacterExport");

        private string _debugOverlay = UserPreferences.GetValue("Settings/Keyboard/DebugOverlay");

        private string _formationSet1 = UserPreferences.GetValue("Settings/Keyboard/FormationSet1");

        private string _formationSet2 = UserPreferences.GetValue("Settings/Keyboard/FormationSet2");

        private string _formationSet3 = UserPreferences.GetValue("Settings/Keyboard/FormationSet3");

        private string _formationSet4 = UserPreferences.GetValue("Settings/Keyboard/FormationSet4");

        private string _formationSet5 = UserPreferences.GetValue("Settings/Keyboard/FormationSet5");
        private string _hide = UserPreferences.GetValue("Settings/Keyboard/Hide");

        private string _rejoinParty = UserPreferences.GetValue("Settings/Keyboard/RejoinParty");
        private string _selectCharacter5 = UserPreferences.GetValue("Settings/Keyboard/SelectCharacter5");
        private string _selectCharacter6 = UserPreferences.GetValue("Settings/Keyboard/SelectCharacter6");

        private string _spawnEncounter = UserPreferences.GetValue("Settings/Keyboard/SpawnEncounter");

        private string _teleportParty = UserPreferences.GetValue("Settings/Keyboard/TeleportParty");

        private string _toggleHud = UserPreferences.GetValue("Settings/Keyboard/ToggleHud");

        private string _vttCamera = UserPreferences.GetValue("Settings/Keyboard/VttCamera");

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

        public string SelectCharacter5
        {
            get => _selectCharacter5;
            set
            {
                _selectCharacter5 = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/SelectCharacter5", _selectCharacter5);
            }
        }

        public string SelectCharacter6
        {
            get => _selectCharacter6;
            set
            {
                _selectCharacter6 = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/SelectCharacter6", _selectCharacter6);
            }
        }

        public string Hide
        {
            get => _hide;
            set
            {
                _hide = value;
                UserPreferences.SetValue<string>("Settings/Keyboard/Hide", _hide);
            }
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

    #endregion
}
