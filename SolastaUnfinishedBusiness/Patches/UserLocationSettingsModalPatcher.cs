using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Models.DmProEditorContext;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UserLocationSettingsModalPatcher
{
    [HarmonyPatch(typeof(UserLocationSettingsModal), nameof(UserLocationSettingsModal.RuntimeLoaded))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RuntimeLoaded_Patch
    {
        [UsedImplicitly]
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
