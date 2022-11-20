using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class MultiplayerSetupPanelPatcher
{
    //PATCH: set flag that prevents hero auto assignment under MP (DEFAULT_PARTY)
    [HarmonyPatch(typeof(MultiplayerSetupPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnBeginShow_Patch
    {
        internal static void Prefix()
        {
            Global.IsSettingUpMultiplayer = true;
        }
    }
}
