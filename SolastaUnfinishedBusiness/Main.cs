using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using UnityModManagerNet;
using Debug = UnityEngine.Debug;

namespace SolastaUnfinishedBusiness;

internal static class Main
{
    internal static readonly bool IsDebugBuild = Debug.isDebugBuild;

    internal static readonly string ModFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    private static ModManager<Core, Settings> Mod { get; set; }

    internal static bool Enabled { get; private set; }

    internal static Action Enable { get; private set; }

    private static UnityModManager.ModEntry.ModLogger Logger { get; set; }

    internal static Settings Settings => Mod.Settings;

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
        Logger.Error(ex.ToString());
    }

    internal static void Error(string msg)
    {
        Logger.Error(msg);
    }

    internal static void Info(string msg)
    {
        Logger.Log(msg);
    }

    [UsedImplicitly]
    public static bool Load([NotNull] UnityModManager.ModEntry modEntry)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();

            Logger = modEntry.Logger;
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
                Enabled = true;
                new MenuManager().Enable(modEntry, assembly);
                Logger.Log("enabled.");
            };

            TranslatorContext.LoadCustomLanguages();
            TranslatorContext.LoadCustomTerms();
        }
        catch (Exception ex)
        {
            Error(ex);
            throw;
        }

        return true;
    }
}
