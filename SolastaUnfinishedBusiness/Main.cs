using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Utils;
using UnityModManagerNet;
using Debug = UnityEngine.Debug;

namespace SolastaUnfinishedBusiness;

internal static class Main
{
    internal static readonly bool IsDebugBuild = Debug.isDebugBuild;
    
    internal static Action Enable { get; private set; }

    internal static bool Enabled { get; private set; }
    
    internal static string ModFolder { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    
    internal static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

    internal static Settings Settings { get; private set; }

    [Conditional("DEBUG")]
    internal static void Log(string msg, bool console = false)
    {
        Logger.Log(msg);

        if (!console)
        {
            return;
        }

        var game = Gui.Game;

        if (game != null)
        {
            game.GameConsole?.LogSimpleLine(msg);
        }
    }

    internal static void Error(Exception ex)
    {
        Logger?.Error(ex.ToString());
    }

    internal static void Error(string msg)
    {
        Logger?.Error(msg);
    }

    public static bool Load([NotNull] UnityModManager.ModEntry modEntry)
    {
        try
        {
            var mod = new ModManager<Core, Settings>();
            var assembly = Assembly.GetExecutingAssembly();

            Logger = modEntry.Logger;
            Settings = mod.Settings;
            mod.Enable(modEntry, assembly);

            modEntry.OnShowGUI = _ =>
            {
                if (Settings.EnableHeroesControlledByComputer)
                {
                    PlayerControllerContext.RefreshGuiState();
                }
            };

            Enable = () =>
            {
                (new MenuManager()).Enable(modEntry, assembly);
                Logger.Log("enabled.");
                Enabled = true;
            };
        }
        catch (Exception ex)
        {
            Error(ex);
            throw;
        }

        return true;
    }
}
