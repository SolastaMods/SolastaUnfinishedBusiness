using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ModKit;
using UnityModManagerNet;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SolastaCommunityExpansion
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class Main
    {
        public static bool IsMulticlassInstalled { get; set; }
        internal static bool IsDebugBuild => UnityEngine.Debug.isDebugBuild;
        internal static bool Enabled { get; set; }

        internal static readonly string MOD_FOLDER = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [Conditional("DEBUG")]
        internal static void Log(string msg)
        {
            Logger.Log(msg);
        }

        internal static void Error(Exception ex)
        {
            Logger?.Error(ex.ToString());
        }

        internal static void Error(string msg)
        {
            Logger?.Error(msg);
        }

        internal static void Warning(string msg)
        {
            Logger?.Warning(msg);
        }

        internal static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
        internal static ModManager<Core, Settings> Mod { get; private set; }
        internal static MenuManager Menu { get; private set; }
        internal static Settings Settings => Mod.Settings;

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

                Translations.Load(MOD_FOLDER);
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
            if (Main.Settings.EnableHeroesControlledByComputer)
            {
                Models.PlayerControllerContext.RefreshGuiState();
            }
        }
    }
}
