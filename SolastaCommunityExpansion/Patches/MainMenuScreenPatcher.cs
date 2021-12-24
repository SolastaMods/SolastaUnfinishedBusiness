using HarmonyLib;
using SolastaCommunityExpansion.Models;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(MainMenuScreen), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class MainMenuScreen_OnBeginShow
    {
        internal static void Postfix()
        {
            if (Main.LateEnabled)
            {
                return;
            }

            FightingStyleContext.Load(); // Fighting Styles should be loaded before feats in order to generate feats of new fighting styles
            FeatsContext.Load();
            FlexibleBackgroundsContext.Load();
            FlexibleRacesContext.Load();
            InitialChoicesContext.Load(); 

            Main.LateEnabled = true;
        }
    }
}
