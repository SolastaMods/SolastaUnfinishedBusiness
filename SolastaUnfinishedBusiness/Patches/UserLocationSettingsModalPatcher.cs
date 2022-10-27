using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaUnfinishedBusiness.Models.DmProEditorContext;

namespace SolastaUnfinishedBusiness.Patches;

public static class UserLocationSettingsModalPatcher
{
    [HarmonyPatch(typeof(UserLocationSettingsModal), "RuntimeLoaded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class RuntimeLoaded_Patch
    {
        public static void Postfix(UserLocationSettingsModal __instance)
        {
            //PATCH: adds custom dungeons sizes (DMP)
            if (!Main.Settings.EnableDungeonMakerModdedContent)
            {
                return;
            }

            AddCustomDungeonSizes(__instance);
        }
    }
}
