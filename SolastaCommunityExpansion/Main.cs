using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using HarmonyLib;
using ModKit;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;
using UnityModManagerNet;
using Debug = UnityEngine.Debug;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SolastaCommunityExpansion;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class Main
{
    internal static bool IsDebugBuild = Debug.isDebugBuild;

    internal static string MOD_FOLDER { get; } =
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    internal static bool Enabled { get; set; }

    // need to be public for MC sidecar
    public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

    internal static ModManager<Core, Settings> Mod { get; private set; }
    internal static MenuManager Menu { get; private set; }

    // need to be public for MC sidecar
    public static Settings Settings => Mod.Settings;

    // need to be public for MC sidecar
    [Conditional("DEBUG")]
    public static void Log(string msg, bool console = false)
    {
        Logger.Log(msg);

        if (console)
        {
            var game = Gui.Game;
            if (game != null)
            {
                game.GameConsole?.LogSimpleLine(msg);
            }
        }
    }

    // need to be public for MC sidecar
    public static void Error(Exception ex)
    {
        Logger?.Error(ex.ToString());
    }

    // need to be public for MC sidecar
    public static void Error(string msg)
    {
        Logger?.Error(msg);
    }

    // need to be public for MC sidecar
    public static void Warning(string msg)
    {
        Logger?.Warning(msg);
    }

    internal static bool Load(UnityModManager.ModEntry modEntry)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();

            Logger = modEntry.Logger;
            Mod = new ModManager<Core, Settings>();
            Mod.Enable(modEntry, assembly);
            modEntry.OnShowGUI = OnShowGui;

            Menu = new MenuManager();
            Menu.Enable(modEntry, assembly);

            LoadSidecars(assembly.GetName().Name);
        }
        catch (Exception ex)
        {
            Error(ex);
            throw;
        }

        return true;
    }

    internal static void OnShowGui(UnityModManager.ModEntry modEntry)
    {
        if (Settings.EnableHeroesControlledByComputer)
        {
            PlayerControllerContext.RefreshGuiState();
        }
    }

    internal static void LoadSidecars(string currentAssemblyName)
    {
        foreach (var path in Directory.EnumerateFiles(MOD_FOLDER, "Solasta*.dll"))
        {
            var filename = Path.GetFileName(path);

            if (filename.StartsWith(currentAssemblyName))
            {
                continue;
            }

#pragma warning disable S3885 // "Assembly.Load" should be used
            var sidecarAssembly = Assembly.LoadFile(path);
            var harmony = new Harmony(sidecarAssembly.GetName().Name);
#pragma warning restore S3885 // "Assembly.Load" should be used

            harmony.PatchAll(sidecarAssembly);
        }
    }
}
