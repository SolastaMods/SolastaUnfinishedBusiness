using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityModManagerNet;
using ModKit;

namespace SolastaCommunityExpansion
{
    public class Main
    {
        public static bool Enabled = false;
        public static readonly string MOD_FOLDER = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [Conditional("DEBUG")]
        internal static void Log(string msg) => Logger.Log(msg);
        internal static void Error(Exception ex) => Logger?.Error(ex.ToString());
        internal static void Error(string msg) => Logger?.Error(msg);
        internal static void Warning(string msg) => Logger?.Warning(msg);
        internal static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
        internal static ModManager<Core, Settings> Mod;
        internal static MenuManager Menu;
        internal static Settings Settings { get { return Mod.Settings; } }
        internal static Type CurrentCursorType = null;

        internal static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                Logger = modEntry.Logger;
                Mod = new ModManager<Core, Settings>();
                Mod.Enable(modEntry, assembly);

                Menu = new MenuManager();
                Menu.Enable(modEntry, assembly);

                modEntry.OnShowGUI = (UnityModManager.ModEntry _) =>
                {
                    var service = ServiceRepository.GetService<ICursorService>();

                    if (service?.CurrentCursor != null)
                    {
                        CurrentCursorType = service.CurrentCursor.GetType();
                        service.DeactivateCursor();
                    }
                };

                modEntry.OnHideGUI = (UnityModManager.ModEntry _) =>
                {
                    var service = ServiceRepository.GetService<ICursorService>();

                    if (service?.CurrentCursor != null && CurrentCursorType != null)
                    {
                        service.ActivateCursor(CurrentCursorType);
                        CurrentCursorType = null;
                    }
                };

                Translations.Load(MOD_FOLDER);
            }
            catch (Exception ex)
            {
                Error(ex);
                throw;
            }

            return true;
        }

        internal static bool IsModHelpersLoaded()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.Contains("SolastaModHelpers"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
