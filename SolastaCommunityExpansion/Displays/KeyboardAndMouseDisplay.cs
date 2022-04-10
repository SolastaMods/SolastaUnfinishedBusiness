using ModKit;

namespace SolastaCommunityExpansion.Displays
{
    internal static class KeyboardAndMouseDisplay
    {
        private static bool SelectAll { get; set; } =
            Main.Settings.EnableCancelEditOnRightMouseClick &&
            Main.Settings.EnableHotkeyToggleIndividualHud &&
            Main.Settings.EnableHotkeyToggleHud &&
            Main.Settings.EnableCharacterExport &&
            Main.Settings.EnableHotkeyDebugOverlay &&
            Main.Settings.EnableHotkeyZoomCamera &&
            Main.Settings.EnableTeleportParty &&
            Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView &&
            Main.Settings.InvertAltBehaviorOnTooltips &&
            Main.Settings.EnableCtrlClickBypassMetamagicPanel &&
            Main.Settings.EnableCtrlClickOnlySwapsMainHand;

        private static void UpdateSettings(bool flag)
        {
            Main.Settings.EnableCancelEditOnRightMouseClick = flag;
            Main.Settings.EnableHotkeyToggleIndividualHud = flag;
            Main.Settings.EnableHotkeyToggleHud = flag;
            Main.Settings.EnableCharacterExport = flag;
            Main.Settings.EnableHotkeyDebugOverlay = flag;
            Main.Settings.EnableHotkeyZoomCamera = flag;
            Main.Settings.EnableTeleportParty = flag;
            Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView = flag;
            Main.Settings.InvertAltBehaviorOnTooltips = flag;
            Main.Settings.EnableCtrlClickBypassMetamagicPanel = flag;
            Main.Settings.EnableCtrlClickOnlySwapsMainHand = flag;
        }

        internal static void DisplayKeyboardAndMouse()
        {
            bool toggle;

            static string hk(char key)
            {
                return "ctrl-shift-(".cyan() + key + ")".cyan();
            }

            #region Hotkeys
            UI.Label("");
            UI.Label("General:".yellow());
            UI.Label("");

            toggle = SelectAll;
            if (UI.Toggle($"Select all", ref toggle, UI.AutoWidth()))
            {
                SelectAll = toggle;
                UpdateSettings(SelectAll);
            }

            UI.Label("");

            toggle = Main.Settings.EnableCancelEditOnRightMouseClick;
            if (UI.Toggle($"Enable cancel action with right-mouse click", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCancelEditOnRightMouseClick = toggle;
                SelectAll = false;
            }

            UI.Label("");

            toggle = Main.Settings.EnableHotkeyToggleIndividualHud;
            if (UI.Toggle($"Enable {hk('C')}ontrol Panel, {hk('L')}og, {hk('M')}ap and {hk('P')}arty to toggle each UI panels visibility", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyToggleIndividualHud = toggle;
                SelectAll = false;
            }

            toggle = Main.Settings.EnableHotkeyToggleHud;
            if (UI.Toggle($"Enable {hk('H')}ud to toggle all UI panels visibility", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyToggleHud = toggle;
                SelectAll = false;
            }

            UI.Label("");

            toggle = Main.Settings.EnableCharacterExport;
            if (UI.Toggle($"Enable {hk('E')}xport character " + "[on character inspection screen only]".yellow().italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCharacterExport = toggle;
                SelectAll = false;
            }

            UI.Label("");

            toggle = Main.Settings.EnableHotkeyDebugOverlay;
            if (UI.Toggle($"Enable {hk('D')}ebug overlay", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyDebugOverlay = toggle;
                SelectAll = false;
            }

            toggle = Main.Settings.EnableHotkeyZoomCamera;
            if (UI.Toggle($"Enable {hk('Z')}oom camera " + "[useful when taking dungeon screenshots for publishing]".yellow().italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyZoomCamera = toggle;
                SelectAll = false;
            }

            toggle = Main.Settings.EnableTeleportParty;
            if (UI.Toggle($"Enable {hk('T')}eleport party " + "[you might break quests or maps if you teleport to an undiscovered place]".yellow().italic(), ref toggle))
            {
                Main.Settings.EnableTeleportParty = toggle;
                SelectAll = false;
            }

            UI.Label("");

            toggle = Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView;
            if (UI.Toggle("ALT".cyan() + " key only highlight gadgets in party field of view " + "[only in custom dungeons]".yellow().italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView = toggle;
                SelectAll = false;
            }

            toggle = Main.Settings.InvertAltBehaviorOnTooltips;
            if (UI.Toggle("Invert " + "ALT".cyan() + " key behavior on tooltips", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.InvertAltBehaviorOnTooltips = toggle;
                SelectAll = false;
            }

            UI.Label("");

            toggle = Main.Settings.EnableCtrlClickBypassMetamagicPanel;
            if (UI.Toggle("Enable " + "CTRL".cyan() + " click on spells to auto ignore " + "Sorcerer".orange() + " metamagic panel", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCtrlClickBypassMetamagicPanel = toggle;
                SelectAll = false;
            }

            toggle = Main.Settings.EnableCtrlClickOnlySwapsMainHand;
            if (UI.Toggle("Enable " + "CTRL".cyan() + " click to keep off hand items when swapping wielded configurations", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCtrlClickOnlySwapsMainHand = toggle;
                SelectAll = false;
            }
            #endregion

            UI.Label("");
            UI.Label("Multiclass:".yellow());
            UI.Label("");

            UI.Label(". Press the " + "up".cyan() + " arrow to toggle the character panel selector visibility");
            UI.Label(". Press the " + "down".cyan() + " arrow to browse other classes details");
            UI.Label(". " + "SHIFT".cyan() + " click on a spell consumes a long rest slot instead of the default short rest one");

            UI.Label("");
        }
    }
}
