using ModKit;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class HotKeysDisplay
    {
        internal static void DisplayHotkeys()
        {
            bool toggle;

            string hk(char key)
            {
                return "ctrl-shift-(".cyan() + key + ")".cyan();
            }

            #region Hotkeys
            UI.Label("");

            toggle = Main.Settings.EnableHotkeyToggleIndividualHud;
            if (UI.Toggle($"Enable {hk('C')}ontrol Panel, {hk('L')}og, {hk('M')}ap and {hk('P')}arty to toggle each UI panels visibility", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyToggleIndividualHud = toggle;
            }

            toggle = Main.Settings.EnableHotkeyToggleHud;
            if (UI.Toggle($"Enable {hk('H')}ud to toggle all UI panels visibility", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyToggleHud = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableCharacterExport;
            if (UI.Toggle($"Enable {hk('E')}xport character " + "[on character inspection screen only]".yellow().italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCharacterExport = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableHotkeyDebugOverlay;
            if (UI.Toggle($"Enable {hk('D')}ebug overlay", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyDebugOverlay = toggle;
            }

            toggle = Main.Settings.EnableHotkeyZoomCamera;
            if (UI.Toggle($"Enable {hk('Z')}oom camera " + "[useful when taking dungeon screenshots for publishing]".yellow().italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyZoomCamera = toggle;
            }

            toggle = Main.Settings.EnableTeleportParty;
            if (UI.Toggle($"Enable {hk('T')}eleport party " + "[you might break quests or maps if you teleport to an undiscovered place]".yellow().italic(), ref toggle))
            {
                Main.Settings.EnableTeleportParty = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView;
            if (UI.Toggle("ALT".cyan() + " key only highlight gadgets in party field of view " + "[only in custom dungeons]".yellow().italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView = toggle;
            }

            toggle = Main.Settings.InvertAltBehaviorOnTooltips;
            if (UI.Toggle("Invert " + "ALT".cyan() + " key behavior on tooltips", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.InvertAltBehaviorOnTooltips = toggle;
            }
            #endregion

            UI.Label("");
        }
    }
}
