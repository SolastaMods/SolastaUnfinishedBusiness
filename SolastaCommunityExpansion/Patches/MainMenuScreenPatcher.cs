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

            FlexibleRacesContext.SwitchFlexibleRaces();
            InitialChoicesContext.RefreshTotalFeatsGrantedFistLevel();

            FeatsContext.Load();
            PowersContext.Load();
            SpellsContext.Load();

            GuiWrapperContext.Recache();

            Main.LateEnabled = true;
        }
    }
}
