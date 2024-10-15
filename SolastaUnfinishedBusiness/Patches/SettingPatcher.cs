using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SettingPatcher
{
    private static Setting[] GetExtendedSettingList(IService serviceProvider, Setting[] __result, bool sortByPriority)
    {
        if (serviceProvider is not IInputService)
        {
            return __result;
        }

        var settingList = __result.ToList();

        InputContext.AddKeybindingSettings(settingList);

        if (sortByPriority && settingList.Count >= 2)
        {
            settingList.Sort((left, right) =>
                left.SettingTypeAttribute.SortOrder.CompareTo(right.SettingTypeAttribute.SortOrder));
        }

        return [..settingList];
    }

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
            __result = GetExtendedSettingList(serviceProvider, __result, sortByPriority);
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
            __result = GetExtendedSettingList((IService)serviceProvider, __result, sortByPriority);
        }
    }
}
