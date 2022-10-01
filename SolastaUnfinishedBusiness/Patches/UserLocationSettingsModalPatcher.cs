using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaUnfinishedBusiness.Models.DmProEditorContext;

namespace SolastaUnfinishedBusiness.Patches;

internal static class UserLocationSettingsModalPatcher
{
    [HarmonyPatch(typeof(UserLocationSettingsModal), "RuntimeLoaded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RuntimeLoaded_Patch
    {
        internal static void Postfix(UserLocationSettingsModal __instance)
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
