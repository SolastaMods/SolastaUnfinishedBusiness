using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness;

internal static class Main
{
    internal static readonly string ModFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    private static ModManager<Core, Settings> Mod { get; set; }
    private static UnityModManager.ModEntry ModEntry { get; set; }

    internal static bool Enabled { get; private set; }

    internal static Action Enable { get; private set; }

    internal static string SettingsFolder => Path.Combine(ModFolder, "Settings");
    internal static string[] SettingsFiles { get; private set; }
    internal static string SettingsFilename { get; private set; } = String.Empty;
    internal static Settings Settings => Mod.Settings;

    [Conditional("DEBUG")]
    internal static void Log(string msg, bool console = false)
    {
        ModEntry.Logger.Log(msg);

        if (!console)
        {
            return;
        }

        var game = Gui.Game;

        if (!game)
        {
            return;
        }

        game.GameConsole?.LogSimpleLine(msg);
    }

    internal static void Error(Exception ex)
    {
        ModEntry.Logger.Error(ex.ToString());
    }

    internal static void Error(string msg)
    {
        ModEntry.Logger.Error(msg);
    }

    internal static void Info(string msg)
    {
        ModEntry.Logger.Log(msg);
    }

    [UsedImplicitly]
    internal static bool Load([NotNull] UnityModManager.ModEntry modEntry)
    {
        ModEntry = modEntry;

        var now = DateTime.Now;
        var assembly = Assembly.GetExecutingAssembly();

        var runtimeVersion = typeof(UnityModManager)
            .GetTypeInfo()
            .Assembly
            .GetCustomAttribute<AssemblyFileVersionAttribute>();

        var unityModManagerVersion = runtimeVersion.Version.Split('.');

        if (unityModManagerVersion.Length > 2 &&
            int.TryParse(unityModManagerVersion[1], NumberStyles.Integer, CultureInfo.CurrentCulture, out var minor) &&
            int.TryParse(unityModManagerVersion[2], NumberStyles.Integer, CultureInfo.CurrentCulture, out var rev) &&
            ((minor == 27 && rev > 10) || minor > 27))
        {
            Info($"Unity mod manager version {runtimeVersion.Version} is not compatible with UB. aborting.");

            return false;
        }

        try
        {
            Mod = new ModManager<Core, Settings>();
            Mod.Enable(modEntry, assembly);

            modEntry.OnShowGUI = _ =>
            {
                if (Settings.EnableHeroesControlledByComputer)
                {
                    PlayerControllerContext.RefreshGuiState();
                }
            };

            Enable = () =>
            {
                var finished = DateTime.Now;

                new MenuManager().Enable(modEntry, assembly);
                LoadSettingFilenames();
                ModEntry.Logger.Log($"enabled in {finished - now:T}.");

                Enabled = true;
            };

            TranslatorContext.EarlyLoad();
        }
        catch (Exception ex)
        {
            Error(ex);
            throw;
        }

        return true;
    }

    internal static void LoadSettingFilenames()
    {
        Directory.CreateDirectory(SettingsFolder);

        SettingsFiles = Directory.GetFiles(SettingsFolder)
            .Where(x => x.EndsWith(".xml"))
            .Select(Path.GetFileNameWithoutExtension)
            .ToArray();
    }

    private static bool ValidateFilename(ref string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            return false;
        }

        filename = string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        filename = Path.GetFileName(filename) + ".xml";

        return true;
    }

    internal static void SaveSettings(string filename)
    {
        if (!ValidateFilename(ref filename))
        {
            return;
        }

        SettingsFilename = Path.Combine(SettingsFolder, filename);
        UnityModManager.ModSettings.Save(Settings, ModEntry);
        SettingsFilename = String.Empty;

        LoadSettingFilenames();
    }

    internal static void LoadSettings(string filename)
    {
        SettingsFilename = Path.Combine(SettingsFolder, $"{filename}.xml");
        Mod.Settings = UnityModManager.ModSettings.Load<Settings>(ModEntry);
        SettingsFilename = String.Empty;
    }

    internal static void RemoveSettings(string filename)
    {
        if (!ValidateFilename(ref filename))
        {
            return;
        }

        filename = Path.Combine(SettingsFolder, filename);

        if (File.Exists(filename))
        {
            File.Delete(filename);
        }

        LoadSettingFilenames();
    }
}
