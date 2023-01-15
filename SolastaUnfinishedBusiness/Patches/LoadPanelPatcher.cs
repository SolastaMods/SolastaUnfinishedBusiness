using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Models.SaveByLocationContext;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class LoadPanelPatcher
{
    [HarmonyPatch(typeof(LoadPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] LoadPanel __instance)
        {
            //PATCH: EnableSaveByLocation
            if (Main.Settings.EnableSaveByLocation)
            {
                return LoadPanelOnBeginShowSaveByLocationBehavior(__instance);
            }

            if (Dropdown != null)
            {
                Dropdown.SetActive(false);
            }

            return true;
        }
    }
}
