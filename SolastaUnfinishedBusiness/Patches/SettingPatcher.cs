using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SettingPatcher
{
    //PATCH: extend vanilla keybinding settings with mod ones
    [HarmonyPatch(typeof(Setting), nameof(Setting.GetSettings),
        typeof(IService), typeof(bool))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetSetting_1_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ref Setting[] __result, IService serviceProvider, bool sortByPriority = false)
        {
            __result = SettingsContext.GetExtendedSettingList(serviceProvider, __result, sortByPriority);
        }
    }

    //PATCH: extend vanilla keybinding settings with mod ones
    [HarmonyPatch(typeof(Setting), nameof(Setting.GetSettings),
        typeof(object), typeof(Type), typeof(bool), typeof(bool))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetSetting_2_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ref Setting[] __result, object serviceProvider, bool sortByPriority = false)
        {
            __result = SettingsContext.GetExtendedSettingList((IService)serviceProvider, __result, sortByPriority);
        }
    }
}
