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
            Main.Settings.EnableCtrlClickBypassAttackReactionPanel &
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
            Main.Settings.EnableCtrlClickBypassAttackReactionPanel = flag;
            Main.Settings.EnableIgnoreCtrlClickOnCriticalHit = flag;
            Main.Settings.EnableCtrlClickOnlySwapsMainHand = flag;
        }

        internal static void DisplayKeyboardAndMouse()
        {
            bool toggle;

            #region Hotkeys

            UI.Label("");
            UI.Label(Gui.Format("ModUi/&General"));
            UI.Label("");

            toggle = SelectAll;
            if (UI.Toggle(Gui.Format("ModUi/&SelectAll"), ref toggle, UI.AutoWidth()))
            {
                SelectAll = toggle;
                UpdateSettings(SelectAll);
            }

            UI.Label("");

            toggle = Main.Settings.EnableCancelEditOnRightMouseClick;
            if (UI.Toggle(Gui.Format("ModUi/&EnableCancelEditOnRightMouseClick"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCancelEditOnRightMouseClick = toggle;
                SelectAll = false;
            }

            UI.Label("");

            toggle = Main.Settings.EnableHotkeyToggleIndividualHud;
            if (UI.Toggle(Gui.Format("ModUi/&EnableHotkeyToggleIndividualHud"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyToggleIndividualHud = toggle;
                SelectAll = false;
            }

            toggle = Main.Settings.EnableHotkeyToggleHud;
            if (UI.Toggle(Gui.Format("ModUi/&EnableHotkeyToggleHud"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyToggleHud = toggle;
                SelectAll = false;
            }

            UI.Label("");

            toggle = Main.Settings.EnableCharacterExport;
            if (UI.Toggle(Gui.Format("ModUi/&EnableCharacterExport"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCharacterExport = toggle;
                SelectAll = false;
            }

            UI.Label("");

            toggle = Main.Settings.EnableHotkeyDebugOverlay;
            if (UI.Toggle(Gui.Format("ModUi/&EnableHotkeyDebugOverlay"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyDebugOverlay = toggle;
                SelectAll = false;
            }

            toggle = Main.Settings.EnableHotkeyZoomCamera;
            if (UI.Toggle(Gui.Format("ModUi/&EnableHotkeyZoomCamera"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyZoomCamera = toggle;
                SelectAll = false;
            }

            toggle = Main.Settings.EnableTeleportParty;
            if (UI.Toggle(Gui.Format("ModUi/&EnableTeleportParty"), ref toggle))
            {
                Main.Settings.EnableTeleportParty = toggle;
                SelectAll = false;
            }

            UI.Label("");

            toggle = Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView;
            if (UI.Toggle(Gui.Format("ModUi/&AltOnlyHighlightItemsInPartyFieldOfView"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView = toggle;
                SelectAll = false;
            }

            toggle = Main.Settings.InvertAltBehaviorOnTooltips;
            if (UI.Toggle(Gui.Format("ModUi/&InvertAltBehaviorOnTooltips"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.InvertAltBehaviorOnTooltips = toggle;
                SelectAll = false;
            }

            UI.Label("");

            toggle = Main.Settings.EnableCtrlClickBypassMetamagicPanel;
            if (UI.Toggle(Gui.Format("ModUi/&EnableCtrlClickBypassMetamagicPanel"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCtrlClickBypassMetamagicPanel = toggle;
                SelectAll = false;
            }

            toggle = Main.Settings.EnableCtrlClickBypassAttackReactionPanel;
            if (UI.Toggle(Gui.Format("ModUi/&EnableCtrlClickBypassAttackReactionPanel"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCtrlClickBypassAttackReactionPanel = toggle;
                Main.Settings.EnableIgnoreCtrlClickOnCriticalHit = toggle;
                SelectAll = false;
            }

            if (Main.Settings.EnableCtrlClickBypassAttackReactionPanel)
            {
                toggle = Main.Settings.EnableIgnoreCtrlClickOnCriticalHit;
                if (UI.Toggle(Gui.Format("ModUi/&EnableIgnoreCtrlClickOnCriticalHit"), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableIgnoreCtrlClickOnCriticalHit = toggle;
                }
            }

            toggle = Main.Settings.EnableCtrlClickOnlySwapsMainHand;
            if (UI.Toggle(Gui.Format("ModUi/&EnableCtrlClickOnlySwapsMainHand"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCtrlClickOnlySwapsMainHand = toggle;
                SelectAll = false;
            }

            #endregion

            UI.Label("");
            UI.Label(Gui.Format("ModUi/&MulticlassKeyHelp"));
            UI.Label("");
        }
    }
}
