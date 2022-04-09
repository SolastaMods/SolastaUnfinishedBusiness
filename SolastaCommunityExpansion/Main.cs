using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using HarmonyLib;
using ModKit;
using UnityModManagerNet;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SolastaCommunityExpansion
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class Main
    {
        private const string CeFilename = "SolastaCommunityExpansion.dll";
        private const string McFilename = "SolastaMulticlass.dll";
        internal static bool IsDebugBuild => UnityEngine.Debug.isDebugBuild;
        internal static bool Enabled { get; set; }
        internal static string MOD_FOLDER { get; private set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // need to be public for MC sidecar
        [Conditional("DEBUG")]
        public static void Log(string msg)
        {
            Logger.Log(msg);
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

        // need to be public for MC sidecar
        public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        internal static ModManager<Core, Settings> Mod { get; private set; }
        internal static MenuManager Menu { get; private set; }

        // need to be public for MC sidecar
        public static Settings Settings => Mod.Settings;

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

                // load sidecars
                foreach (var path in Directory.EnumerateFiles(MOD_FOLDER, "Solasta*.dll"))
                {
                    var filename = Path.GetFileName(path);

                    if (filename == CeFilename || (filename == McFilename && !Main.Settings.EnableMulticlass))
                    {
                        continue;
                    }

#pragma warning disable S3885 // "Assembly.Load" should be used
                    var customCodeAssembly = Assembly.LoadFile(path);
                    var harmony = new Harmony(customCodeAssembly.GetName().Name);
#pragma warning restore S3885 // "Assembly.Load" should be used
                    harmony.PatchAll(customCodeAssembly);
                }
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
                Models.PlayerControllerContext.RefreshGuiState();
            }
        }
    }
}
