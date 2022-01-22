using ModKit;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class HotKeysDisplay
    {
        internal static void DisplayHotkeys()
        {
            bool toggle;

            #region Hotkeys
            UI.Label("");

            toggle = Main.Settings.EnableHotkeysToToggleHud;
            if (UI.Toggle("Enable the " + "ctrl-shift-(C)".cyan() + "ontrol Panel, " + "ctrl-shift-(L)".cyan() + "og, " + "ctrl-shift-(M)".cyan() + "ap, " + "ctrl-shift-(P)".cyan() + "arty and " + "ctrl-shift-(H)".cyan() + "ud hotkeys to toggle UI panels visibility", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeysToToggleHud = toggle;
            }

            toggle = Main.Settings.EnableCharacterExport;
            if (UI.Toggle("Enable the " + "ctrl-shift-(E)".cyan() + "xport character hotkey " + "[on character inspection screen only]".yellow().italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCharacterExport = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableHotkeyDebugOverlay;
            if (UI.Toggle("Enable the " + "ctrl-shift-(D)".cyan() + "ebug overlay hotkey", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyDebugOverlay = toggle;
            }

            toggle = Main.Settings.EnableHotkeyDebugOverlay;
            if (UI.Toggle("Enable the " + "ctrl-shift-(Z)".cyan() + "oom camera hotkey " + "[useful when taking dungeon screenshots for publishing]".yellow().italic(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHotkeyDebugOverlay = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableTeleportParty;
            if (UI.Toggle("Enable the " + "ctrl-shift-(T)".cyan() + "eleport party hotkey " + "[You might break quests or maps if you teleport to an undiscovered place]".italic().yellow(), ref toggle))
            {
                Main.Settings.EnableTeleportParty = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView;
            if (UI.Toggle("ALT".cyan() + " key only highlight gadgets in party field of view " + "[only in custom dungeons]".italic().yellow(), ref toggle, UI.AutoWidth()))
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
