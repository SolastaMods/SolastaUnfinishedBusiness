using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ModSettingsPatcher
{
    //PATCH: supports exports / imports mod settings
    [HarmonyPatch(typeof(UnityModManager.ModSettings), nameof(UnityModManager.ModSettings.GetPath))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetPath_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref string __result)
        {
            if (Main.SettingsFilename == String.Empty)
            {
                return true;
            }

            __result = Main.SettingsFilename;

            return false;
        }
    }
}
