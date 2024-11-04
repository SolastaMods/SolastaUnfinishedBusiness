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

        [SettingTypeToggle("UnlockAdditionalLoreFriendlyNames", SortOrder = 1031, DisplayFooter = false)]
        [UsedImplicitly]
        bool UnlockAdditionalLoreFriendlyNames { get; set; }

        [SettingTypeToggle("UnlockAllNpcFaces", SortOrder = 1041, DisplayFooter = true)]
        [UsedImplicitly]
        bool UnlockAllNpcFaces { get; set; }

        [SettingTypeToggle("UnlockUnmarkedSorcerers", SortOrder = 1042, DisplayFooter = true)]
        [UsedImplicitly]
        bool UnlockUnmarkedSorcerers { get; set; }

        [SettingTypeToggle("UnlockMarkAndTattoosForAllCharacters", SortOrder = 1043, DisplayFooter = false)]
        [UsedImplicitly]
        bool UnlockMarkAndTattoosForAllCharacters { get; set; }

        [SettingTypeToggle("UnlockEyeStyles", SortOrder = 1051, DisplayFooter = true)]
        [UsedImplicitly]
        bool UnlockEyeStyles { get; set; }

        [SettingTypeToggle("UnlockNewBrightEyeColors", SortOrder = 1052, DisplayFooter = true)]
        [UsedImplicitly]
        bool UnlockNewBrightEyeColors { get; set; }

        [SettingTypeToggle("UnlockGlowingEyeColors", SortOrder = 1053, DisplayFooter = false)]
        [UsedImplicitly]
        bool UnlockGlowingEyeColors { get; set; }

        [SettingTypeToggle("UnlockGlowingColorsForAllMarksAndTattoos", SortOrder = 1061, DisplayFooter = true)]
        [UsedImplicitly]
        bool UnlockGlowingColorsForAllMarksAndTattoos { get; set; }

        [SettingTypeToggle("UnlockSkinColors", SortOrder = 1062, DisplayFooter = false)]
        [UsedImplicitly]
        bool UnlockSkinColors { get; set; }

        [SettingTypeToggle("HideCrownOfMagister", SortOrder = 1071, DisplayFooter = true)]
        [UsedImplicitly]
        bool HideCrownOfMagister { get; set; }

        [SettingTypeToggle("HideHelmets", SortOrder = 1072, DisplayFooter = true)]
        [UsedImplicitly]
        bool HideHelmets { get; set; }

        [SettingTypeToggle("UnlockBeardlessDwarves", SortOrder = 1073, DisplayFooter = true)]
        [UsedImplicitly]
        bool UnlockBeardlessDwarves { get; set; }

        [SettingTypeDropList("EmpressGarbAppearance",
            SortOrder = 1081, DisplayFooter = false,
            Items =
            [
                "Normal", "Barbarian", "Druid", "ElvenChain", "SorcererOutfit", "StuddedLeather",
                "GreenMageArmor", "WizardOutfit", "ScavengerOutfit1", "ScavengerOutfit2", "BardArmor", "WarlockArmor"
            ])]
        [UsedImplicitly]
        string EmpressGarbAppearance { get; set; }
    }

    internal sealed class GuiModManager : IGuiModSettingsService
    {
        private string _empressGarbAppearance = UserPreferences.GetValue<string>("Settings/Gui/EmpressGarbAppearance");
        private bool _enableCharacterChecker = UserPreferences.GetValue<bool>("Settings/Gui/EnableCharacterChecker");
        private bool _enableCheatMenu = UserPreferences.GetValue<bool>("Settings/Gui/EnableCheatMenu");
        private bool _enablePartyToggles = UserPreferences.GetValue<bool>("Settings/Gui/EnablePartyToggles");
        private bool _enableSaveByLocation = UserPreferences.GetValue<bool>("Settings/Gui/EnableSaveByLocation");
        private bool _hideCrownOfMagister = UserPreferences.GetValue<bool>("Settings/Gui/HideCrownOfMagister");
        private bool _hideHelmets = UserPreferences.GetValue<bool>("Settings/Gui/HideHelmets");
        private bool _invertTooltipBehavior = UserPreferences.GetValue<bool>("Settings/Gui/InvertTooltipBehavior");
        private float _tooltipWidth = UserPreferences.GetValue<float>("Settings/Gui/TooltipWidth");

        private bool _unlockAdditionalLoreFriendlyNames =
            UserPreferences.GetValue<bool>("Settings/Gui/UnlockAdditionalLoreFriendlyNames");

        private bool _unlockAllNpcFaces = UserPreferences.GetValue<bool>("Settings/Gui/UnlockAllNpcFaces");
        private bool _unlockBeardlessDwarves = UserPreferences.GetValue<bool>("Settings/Gui/UnlockBeardlessDwarves");
        private bool _unlockEyeStyles = UserPreferences.GetValue<bool>("Settings/Gui/UnlockEyeStyles");

        private bool _unlockGlowingColorsForAllMarksAndTattoos =
            UserPreferences.GetValue<bool>("Settings/Gui/UnlockGlowingColorsForAllMarksAndTattoos");

        private bool _unlockGlowingEyeColors = UserPreferences.GetValue<bool>("Settings/Gui/UnlockGlowingEyeColors");

        private bool _unlockMarkAndTattoosForAllCharacters =
            UserPreferences.GetValue<bool>("Settings/Gui/UnlockMarkAndTattoosForAllCharacters");

        private bool _unlockNewBrightEyeColors =
            UserPreferences.GetValue<bool>("Settings/Gui/UnlockNewBrightEyeColors");

        private bool _unlockSkinColors = UserPreferences.GetValue<bool>("Settings/Gui/UnlockSkinColors");
        private bool _unlockUnmarkedSorcerers = UserPreferences.GetValue<bool>("Settings/Gui/UnlockUnmarkedSorcerers");

        public bool ModHeader { get; set; }

        public string EmpressGarbAppearance
        {
            get => _empressGarbAppearance ?? "Normal";
            set
            {
                _empressGarbAppearance = value;
                CampaignsContext.SwitchEmpressGarb();
                UserPreferences.SetValue("Settings/Gui/EmpressGarbAppearance", _empressGarbAppearance);
            }
        }

        public bool HideCrownOfMagister
        {
            get => _hideCrownOfMagister;
            set
            {
                _hideCrownOfMagister = value;
                CampaignsContext.SwitchCrownOfTheMagister();
                UserPreferences.SetValue("Settings/Gui/HideCrownOfMagister", _hideCrownOfMagister);
            }
        }

        public bool HideHelmets
        {
            get => _hideHelmets;
            set
            {
                _hideHelmets = value;
                UserPreferences.SetValue("Settings/Gui/HideHelmets", _hideHelmets);
            }
        }

        public bool UnlockBeardlessDwarves
        {
            get => _unlockBeardlessDwarves;
            set
            {
                _unlockBeardlessDwarves = value;
                UserPreferences.SetValue("Settings/Gui/UnlockBeardlessDwarves", _unlockBeardlessDwarves);
            }
        }

        public bool UnlockSkinColors
        {
            get => _unlockSkinColors;
            set
            {
                _unlockSkinColors = value;
                UserPreferences.SetValue("Settings/Gui/UnlockSkinColors", _unlockSkinColors);
            }
        }

        public bool UnlockGlowingColorsForAllMarksAndTattoos
        {
            get => _unlockGlowingColorsForAllMarksAndTattoos;
            set
            {
                _unlockGlowingColorsForAllMarksAndTattoos = value;
                UserPreferences.SetValue("Settings/Gui/UnlockGlowingColorsForAllMarksAndTattoos",
                    _unlockGlowingColorsForAllMarksAndTattoos);
            }
        }

        public bool UnlockGlowingEyeColors
        {
            get => _unlockGlowingEyeColors;
            set
            {
                _unlockGlowingEyeColors = value;
                UserPreferences.SetValue("Settings/Gui/UnlockGlowingEyeColors", _unlockGlowingEyeColors);
            }
        }

        public bool UnlockNewBrightEyeColors
        {
            get => _unlockNewBrightEyeColors;
            set
            {
                _unlockNewBrightEyeColors = value;
                UserPreferences.SetValue("Settings/Gui/UnlockNewBrightEyeColors", _unlockNewBrightEyeColors);
            }
        }

        public bool UnlockEyeStyles
        {
            get => _unlockEyeStyles;
            set
            {
                _unlockEyeStyles = value;
                UserPreferences.SetValue("Settings/Gui/UnlockEyeStyles", _unlockEyeStyles);
            }
        }

        public bool UnlockMarkAndTattoosForAllCharacters
        {
            get => _unlockMarkAndTattoosForAllCharacters;
            set
            {
                _unlockMarkAndTattoosForAllCharacters = value;
                UserPreferences.SetValue("Settings/Gui/UnlockMarkAndTattoosForAllCharacters",
                    _unlockMarkAndTattoosForAllCharacters);
            }
        }

        public bool UnlockUnmarkedSorcerers
        {
            get => _unlockUnmarkedSorcerers;
            set
            {
                _unlockUnmarkedSorcerers = value;
                UserPreferences.SetValue("Settings/Gui/UnlockUnmarkedSorcerers", _unlockUnmarkedSorcerers);
            }
        }

        public bool UnlockAllNpcFaces
        {
            get => _unlockAllNpcFaces;
            set
            {
                _unlockAllNpcFaces = value;
                UserPreferences.SetValue("Settings/Gui/UnlockAllNpcFaces", _unlockAllNpcFaces);
            }
        }

        public bool UnlockAdditionalLoreFriendlyNames
        {
            get => _unlockAdditionalLoreFriendlyNames;
            set
            {
                _unlockAdditionalLoreFriendlyNames = value;
                UserPreferences.SetValue("Settings/Gui/UnlockAdditionalLoreFriendlyNames",
                    _unlockAdditionalLoreFriendlyNames);
            }
        }

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
        [SettingTypeHeader("ModHeader", SortOrder = 500)]
        [UsedImplicitly]
        bool ModHeader { get; set; }

        [SettingTypeToggle("EnableCtrlClickDragToBypassQuestItemsOnDrop", SortOrder = 901, DisplayFooter = true)]
        [UsedImplicitly]
        bool EnableCtrlClickDragToBypassQuestItemsOnDrop { get; set; }

        [SettingTypeToggle("EnableShiftToSnapLineSpells", SortOrder = 902, DisplayFooter = false)]
        [UsedImplicitly]
        bool EnableShiftToSnapLineSpells { get; set; }

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

        [SettingTypeKeyMapping("EditorRotate", DisplayFooter = true, SortOrder = 1041)]
        [UsedImplicitly]
        string EditorRotate { get; set; }

        [SettingTypeKeyMapping("EditorKnudgeNorth", DisplayFooter = true, SortOrder = 1042)]
        [UsedImplicitly]
        string EditorKnudgeNorth { get; set; }

        [SettingTypeKeyMapping("EditorKnudgeEast", DisplayFooter = true, SortOrder = 1043)]
        [UsedImplicitly]
        string EditorKnudgeEast { get; set; }

        [SettingTypeKeyMapping("EditorKnudgeSouth", DisplayFooter = true, SortOrder = 1044)]
        [UsedImplicitly]
        string EditorKnudgeSouth { get; set; }

        [SettingTypeKeyMapping("EditorKnudgeWest", DisplayFooter = false, SortOrder = 1045)]
        [UsedImplicitly]
        string EditorKnudgeWest { get; set; }

        //
        // these should blend with vanilla settings
        //

        // order here matters as the sort routine will revert items in the collection if same sort order

        [SettingTypeKeyMapping("SelectCharacter6", DisplayFooter = true, SortOrder = 29)]
        [UsedImplicitly]
        string SelectCharacter6 { get; set; }

        [SettingTypeKeyMapping("SelectCharacter5", DisplayFooter = true, SortOrder = 29)]
        [UsedImplicitly]
        string SelectCharacter5 { get; set; }

        [SettingTypeKeyMapping("Hide", DisplayFooter = true, SortOrder = 22)]
        [UsedImplicitly]
        string Hide { get; set; }
    }

    internal sealed class InputModManager : IInputModSettingsService
    {
        private string _characterExport = UserPreferences.GetValue("Settings/Keyboard/CharacterExport");
        private string _debugOverlay = UserPreferences.GetValue("Settings/Keyboard/DebugOverlay");

        private string _editorKnudgeEast = UserPreferences.GetValue("Settings/Keyboard/EditorKnudgeEast");
        private string _editorKnudgeNorth = UserPreferences.GetValue("Settings/Keyboard/EditorKnudgeNorth");
        private string _editorKnudgeSouth = UserPreferences.GetValue("Settings/Keyboard/EditorKnudgeSouth");
        private string _editorKnudgeWest = UserPreferences.GetValue("Settings/Keyboard/EditorKnudgeWest");
        private string _editorRotate = UserPreferences.GetValue("Settings/Keyboard/EditorRotate");

        private bool _enableCtrlClickDragToBypassQuestItemsOnDrop =
            UserPreferences.GetValue<bool>("Settings/Keyboard/EnableCtrlClickDragToBypassQuestItemsOnDrop");

        private bool _enableShiftToSnapLineSpells =
            UserPreferences.GetValue<bool>("Settings/Keyboard/EnableShiftToSnapLineSpells");

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

        public string EditorRotate
        {
            get => _editorRotate;
            set
            {
                _editorRotate = value;
                SetVanillaCommand(InputCommands.Id.EditorRotate, value);
                UserPreferences.SetValue("Settings/Keyboard/EditorRotate", _editorRotate);
            }
        }

        public string EditorKnudgeNorth
        {
            get => _editorKnudgeNorth;
            set
            {
                _editorKnudgeNorth = value;
                SetVanillaCommand(InputCommands.Id.EditorKnudgeNorth, value);
                UserPreferences.SetValue("Settings/Keyboard/EditorKnudgeNorth", _editorKnudgeNorth);
            }
        }

        public string EditorKnudgeEast
        {
            get => _editorKnudgeEast;
            set
            {
                _editorKnudgeEast = value;
                SetVanillaCommand(InputCommands.Id.EditorKnudgeEast, value);
                UserPreferences.SetValue("Settings/Keyboard/EditorKnudgeEast", _editorKnudgeEast);
            }
        }

        public string EditorKnudgeSouth
        {
            get => _editorKnudgeSouth;
            set
            {
                _editorKnudgeSouth = value;
                SetVanillaCommand(InputCommands.Id.EditorKnudgeSouth, value);
                UserPreferences.SetValue("Settings/Keyboard/EditorKnudgeSouth", _editorKnudgeSouth);
            }
        }

        public string EditorKnudgeWest
        {
            get => _editorKnudgeWest;
            set
            {
                _editorKnudgeWest = value;
                SetVanillaCommand(InputCommands.Id.EditorKnudgeWest, value);
                UserPreferences.SetValue("Settings/Keyboard/EditorKnudgeWest", _editorKnudgeWest);
            }
        }

        public bool EnableCtrlClickDragToBypassQuestItemsOnDrop
        {
            get => _enableCtrlClickDragToBypassQuestItemsOnDrop;
            set
            {
                _enableCtrlClickDragToBypassQuestItemsOnDrop = value;
                UserPreferences.SetValue(
                    "Settings/Keyboard/EnableCtrlClickDragToBypassQuestItemsOnDrop",
                    _enableCtrlClickDragToBypassQuestItemsOnDrop);
            }
        }

        public bool EnableShiftToSnapLineSpells
        {
            get => _enableShiftToSnapLineSpells;
            set
            {
                _enableShiftToSnapLineSpells = value;
                UserPreferences.SetValue("Settings/Keyboard/EnableShiftToSnapLineSpells", _enableShiftToSnapLineSpells);
            }
        }

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

        private static void SetVanillaCommand(InputCommands.Id id, string command)
        {
            var commandMapping = new InputCommands.CommandMapping();
            var inputManager = ServiceRepository.GetService<IInputService>() as InputManager;

            commandMapping.ReadFromString(command);
            inputManager!.commandsMap[(int)id] = commandMapping;
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
            _enableShiftToSnapLineSpells = false;
            _enableCtrlClickDragToBypassQuestItemsOnDrop = false;
        }
    }

    #endregion
}
