using System;
using System.Diagnostics;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class ToolsDisplay
{
    internal const float DefaultFastTimeModifier = 1.5f;

    private static string ExportFileName { get; set; } =
        ServiceRepository.GetService<INetworkingService>().GetUserName();

    internal static void DisplayTools()
    {
        DisplayGeneral();
        DisplayAdventure();
        DisplaySettings();
        UI.Label();
    }

    private static void DisplayGeneral()
    {
        UI.Label();
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&Update"), () => UpdateContext.UpdateMod(),
                UI.Width(200f));
            UI.ActionButton(Gui.Localize("ModUi/&Rollback"), UpdateContext.DisplayRollbackMessage,
                UI.Width(200f));
            UI.ActionButton(Gui.Localize("ModUi/&Changelog"), UpdateContext.OpenChangeLog,
                UI.Width(200f));
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Format("ModUi/&Donate", "Github"), UpdateContext.OpenDonateGithub,
                UI.Width(200f));
            UI.ActionButton(Gui.Format("ModUi/&Donate", "Patreon"), UpdateContext.OpenDonatePatreon,
                UI.Width(200f));
            UI.ActionButton(Gui.Format("ModUi/&Donate", "PayPal"), UpdateContext.OpenDonatePayPal,
                UI.Width(200f));
        }

        UI.Label();

        var toggle = Main.Settings.DisableUnofficialTranslations;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableUnofficialTranslations"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableUnofficialTranslations = toggle;
            Main.Settings.FixAsianLanguagesTextWrap = !toggle;
        }

        if (!Main.Settings.DisableUnofficialTranslations)
        {
            toggle = Main.Settings.FixAsianLanguagesTextWrap;
            if (UI.Toggle(Gui.Localize("ModUi/&FixAsianLanguagesTextWrap"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.FixAsianLanguagesTextWrap = toggle;
            }
        }

#if false
        toggle = Main.Settings.EnableBetaContent;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableBetaContent"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableBetaContent = toggle;
        }
#endif

        UI.Label();

        toggle = Main.Settings.EnablePcgRandom;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePcgRandom"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePcgRandom = toggle;
        }

        toggle = Main.Settings.EnableSaveByLocation;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSaveByLocation"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSaveByLocation = toggle;
        }

        UI.Label();

        toggle = Main.Settings.EnableTogglesToOverwriteDefaultTestParty;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableTogglesToOverwriteDefaultTestParty"), ref toggle))
        {
            Main.Settings.EnableTogglesToOverwriteDefaultTestParty = toggle;
        }

        toggle = Main.Settings.EnableCharacterChecker;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCharacterChecker"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableCharacterChecker = toggle;
        }

        UI.Label();

        toggle = Main.Settings.EnableCheatMenu;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCheatMenu"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableCheatMenu = toggle;
        }

        toggle = Main.Settings.EnableHotkeyDebugOverlay;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableHotkeyDebugOverlay"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableHotkeyDebugOverlay = toggle;
        }
    }

    private static void DisplayAdventure()
    {
        UI.Label();

        var toggle = Main.Settings.NoExperienceOnLevelUp;
        if (UI.Toggle(Gui.Localize("ModUi/&NoExperienceOnLevelUp"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.NoExperienceOnLevelUp = toggle;
        }

        toggle = Main.Settings.OverrideMinMaxLevel;
        if (UI.Toggle(Gui.Localize("ModUi/&OverrideMinMaxLevel"), ref toggle))
        {
            Main.Settings.OverrideMinMaxLevel = toggle;
        }

        UI.Label();

        var intValue = Main.Settings.MultiplyTheExperienceGainedBy;
        if (UI.Slider(Gui.Localize("ModUi/&MultiplyTheExperienceGainedBy"), ref intValue, 0, 200, 100, string.Empty,
                UI.Width(100f)))
        {
            Main.Settings.MultiplyTheExperienceGainedBy = intValue;
            ToolsContext.SwitchEncounterPercentageChance();
        }

        UI.Label();

        intValue = Main.Settings.OverridePartySize;
        if (UI.Slider(Gui.Localize("ModUi/&OverridePartySize"), ref intValue,
                ToolsContext.MinPartySize, ToolsContext.MaxPartySize,
                ToolsContext.GamePartySize, string.Empty, UI.AutoWidth()))
        {
            Main.Settings.OverridePartySize = intValue;

            while (Main.Settings.DefaultPartyHeroes.Count > intValue)
            {
                Main.Settings.DefaultPartyHeroes.RemoveAt(Main.Settings.DefaultPartyHeroes.Count - 1);
            }
        }

        if (Main.Settings.OverridePartySize > ToolsContext.GamePartySize)
        {
            UI.Label();

            toggle = Main.Settings.AllowAllPlayersOnNarrativeSequences;
            if (UI.Toggle(Gui.Localize("ModUi/&AllowAllPlayersOnNarrativeSequences"), ref toggle))
            {
                Main.Settings.AllowAllPlayersOnNarrativeSequences = toggle;
            }
        }

        UI.Label();

        var floatValue = Main.Settings.FasterTimeModifier;
        if (UI.Slider(Gui.Localize("ModUi/&FasterTimeModifier"), ref floatValue,
                DefaultFastTimeModifier, 10f, DefaultFastTimeModifier, 1, string.Empty, UI.AutoWidth()))
        {
            Main.Settings.FasterTimeModifier = floatValue;
        }

        UI.Label();

        intValue = Main.Settings.EncounterPercentageChance;
        if (UI.Slider(Gui.Localize("ModUi/&EncounterPercentageChance"), ref intValue, 0, 100, 5, string.Empty,
                UI.AutoWidth()))
        {
            Main.Settings.EncounterPercentageChance = intValue;
        }

        if (Gui.GameCampaign == null)
        {
            return;
        }

        var gameCampaign = Gui.GameCampaign;

        if (gameCampaign == null)
        {
            return;
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Localize("ModUi/&IncreaseGameTimeBy"), UI.Width(300f));
            UI.ActionButton("1 hour", () => gameCampaign.UpdateTime(60 * 60), UI.Width(100f));
            UI.ActionButton("6 hours", () => gameCampaign.UpdateTime(60 * 60 * 6), UI.Width(100f));
            UI.ActionButton("12 hours", () => gameCampaign.UpdateTime(60 * 60 * 12), UI.Width(100f));
            UI.ActionButton("24 hours", () => gameCampaign.UpdateTime(60 * 60 * 24), UI.Width(100f));
        }
    }

    private static void DisplaySettings()
    {
        UI.Label();
        UI.Label(Gui.Localize("ModUi/&SettingsHelp"));
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&SettingsExport"), () =>
            {
                Main.SaveSettings(ExportFileName);
            }, UI.Width(144f));

            UI.ActionButton(Gui.Localize("ModUi/&SettingsRemove"), () =>
            {
                Main.RemoveSettings(ExportFileName);
            }, UI.Width(144f));

            var text = ExportFileName;

            UI.ActionTextField(ref text, String.Empty, s => { ExportFileName = s; }, null, UI.Width(144f));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&SettingsRefresh"), Main.LoadSettingFilenames, UI.Width(144f));
            UI.ActionButton(Gui.Localize("ModUi/&SettingsOpenFolder"), () =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Main.SettingsFolder, UseShellExecute = true, Verb = "open"
                });
            }, UI.Width(292f));
        }

        UI.Label();

        if (Main.SettingsFiles.Length == 0)
        {
            return;
        }

        UI.Label(Gui.Localize("ModUi/&SettingsLoad"));
        UI.Label();

        var intValue = -1;
        if (UI.SelectionGrid(ref intValue, Main.SettingsFiles, Main.SettingsFiles.Length, 4, UI.Width(440f)))
        {
            Main.LoadSettings(Main.SettingsFiles[intValue]);
        }
    }
}
