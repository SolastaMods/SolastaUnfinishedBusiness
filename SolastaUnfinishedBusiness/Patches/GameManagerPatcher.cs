using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class GameManagerPatcher
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class BindPostDatabase_Patch
    {
        public static void Postfix()
        {
            //PATCH: loads all mod contexts
            BootContext.Startup();
        }
    }
}
