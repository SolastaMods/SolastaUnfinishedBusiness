using System;
using System.Diagnostics;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Displays;

internal static class ToolsDisplay
{
    private static string ExportFileName { get; set; } =
        ServiceRepository.GetService<INetworkingService>().GetUserName();

    internal static void DisplayGameplay()
    {
        DisplayGeneral();
        UI.Label();
        DisplayMultiplayer();
        UI.Label();
        DisplayOneDnd();
        UI.Label();
    }

    private static void DisplayGeneral()
    {
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton(Gui.Localize("ModUi/&Update"), () => UpdateContext.UpdateMod(),
                UI.Width(195f));
            UI.ActionButton(Gui.Localize("ModUi/&Rollback"), UpdateContext.DisplayRollbackMessage,
                UI.Width(195f));
            UI.ActionButton(Gui.Localize("ModUi/&Changelog"), UpdateContext.OpenChangeLog,
                UI.Width(195f));
        }

        UI.Label();

        var toggle = Main.Settings.EnablePcgRandom;
        if (UI.Toggle(Gui.Localize("ModUi/&EnablePcgRandom"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnablePcgRandom = toggle;
        }

        toggle = Main.Settings.EnableCustomPortraits;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCustomPortraits"), ref toggle))
        {
            Main.Settings.EnableCustomPortraits = toggle;
        }

        if (Main.Settings.EnableCustomPortraits)
        {
            UI.Label();

            UI.ActionButton(Gui.Localize("ModUi/&PortraitsOpenFolder"), () =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = PortraitsContext.PortraitsFolder, UseShellExecute = true, Verb = "open"
                });
            }, UI.Width(292f));

            UI.Label();
            UI.Label(Gui.Localize("ModUi/&EnableCustomPortraitsHelp"));
            UI.Label();
        }

        UI.Label();

        toggle = Main.Settings.DisableMultilineSpellOffering;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableMultilineSpellOffering"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableMultilineSpellOffering = toggle;
        }

        toggle = Main.Settings.DisableUnofficialTranslations;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableUnofficialTranslations"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableUnofficialTranslations = toggle;
        }

        if (!Gui.GameCampaign)
        {
            return;
        }

        UI.Label();

        var gameCampaign = Gui.GameCampaign;

        if (!gameCampaign)
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

    private static void DisplayMultiplayer()
    {
        var toggle = Main.Settings.DisplayMultiplayerToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&Multiplayer"), ref toggle, 200))
        {
            Main.Settings.DisplayMultiplayerToggle = toggle;
        }

        if (!Main.Settings.DisplayMultiplayerToggle)
        {
            return;
        }

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

        if (Main.SettingsFiles.Length == 0)
        {
            return;
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&SettingsLoad"));
        UI.Label();

        var intValue = -1;
        if (UI.SelectionGrid(ref intValue, Main.SettingsFiles, Main.SettingsFiles.Length, 4, UI.Width(440f)))
        {
            Main.LoadSettings(Main.SettingsFiles[intValue]);
        }
    }

    private static void DisplayOneDnd()
    {
        var toggle = Main.Settings.DisplayOneDndToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&OneDnd"), ref toggle, 200))
        {
            Main.Settings.DisplayOneDndToggle = toggle;
        }

        if (!Main.Settings.DisplayOneDndToggle)
        {
            return;
        }

        UI.Label();

        toggle = Main.Settings.OneDndHealingPotionBonusAction;
        if (UI.Toggle(Gui.Localize("ModUi/&OneDndHealingPotionBonusAction"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.OneDndHealingPotionBonusAction = toggle;
            SrdAndHouseRulesContext.SwitchOneDndHealingPotionBonusAction();
        }

        toggle = Main.Settings.EnableOneDndHealingSpellsBuf;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableOneDndHealingSpellsBuf"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOneDndHealingSpellsBuf = toggle;
            SrdAndHouseRulesContext.SwitchOneDndHealingSpellsBuf();
        }

        toggle = Main.Settings.EnableWizardToLearnSchoolAtLevel3;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableWizardToLearnSchoolAtLevel3"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableWizardToLearnSchoolAtLevel3 = toggle;
            SrdAndHouseRulesContext.SwitchOneDndWizardSchoolOfMagicLearningLevel();
        }

        toggle = Main.Settings.SwapEvocationPotentCantripAndSculptSpell;
        if (!UI.Toggle(Gui.Localize("ModUi/&SwapEvocationPotentCantripAndSculptSpell"), ref toggle, UI.AutoWidth()))
        {
            return;
        }

        Main.Settings.SwapEvocationPotentCantripAndSculptSpell = toggle;
        WizardEvocation.SwapEvocationPotentCantripAndSculptSpell();
    }
}
