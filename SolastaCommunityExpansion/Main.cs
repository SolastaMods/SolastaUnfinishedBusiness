using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityModManagerNet;
using ModKit;
using HarmonyLib;

namespace SolastaCommunityExpansion
{
    public class Main
    {
        public static bool Loaded = false;
        public static bool Enabled = false;
        public static readonly string MOD_FOLDER = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [Conditional("DEBUG")]
        internal static void Log(string msg) => Logger.Log(msg);
        internal static void Error(Exception ex) => Logger?.Error(ex.ToString());
        internal static void Error(string msg) => Logger?.Error(msg);
        internal static void Warning(string msg) => Logger?.Warning(msg);
        internal static UnityModManager.ModEntry ModEntry;
        internal static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
        internal static ModManager<Core, Settings> Mod;
        internal static MenuManager Menu;
        internal static Settings Settings { get { return Mod.Settings; } }

        internal static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var harmony = new Harmony("SolastaCommunityExpansionPreLoad");
                var methodGameManagerBindPostDatabase = typeof(GameManager).GetMethod("BindPostDatabase");
                var methodMainFinishLoading = typeof(Main).GetMethod("FinishLoading");

                ModEntry = modEntry;
                Logger = modEntry.Logger;
                Mod = new ModManager<Core, Settings>();
                Menu = new MenuManager();

                Menu.Enable(ModEntry, assembly);
                harmony.Patch(methodGameManagerBindPostDatabase, postfix: new HarmonyMethod(methodMainFinishLoading));
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

        internal static bool IsSafeToLoad() => true;

        public static void FinishLoading()
        {
            Main.Loaded = true;

            if (IsSafeToLoad())
            {
                Mod.Enable(ModEntry, Assembly.GetExecutingAssembly());
                Translations.Load(MOD_FOLDER);

                Models.AdditionalNamesContext.Load();
                Models.AsiAndFeatContext.Load();
                Models.InitialChoicesContext.Load();
                Models.ItemCraftingContext.Load();
                Models.GameUiContext.Load();
                Models.FeatsContext.Load();
                Models.SubclassesContext.Load();
                Models.FlexibleBackgroundsContext.Load();
                Models.FlexibleRacesContext.Load();
                Models.VisionContext.Load();
                Models.PickPocketContext.Load();
                Models.EpicArrayContext.Load();

                Main.Enabled = true;
            }
        }
    }
}
