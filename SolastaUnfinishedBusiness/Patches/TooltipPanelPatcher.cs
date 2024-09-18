using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class TooltipPanelPatcher
{
    [HarmonyPatch(typeof(TooltipPanel), nameof(TooltipPanel.SetupFeatures))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SetupFeatures_Patch
    {
        [UsedImplicitly]
        public static void Prefix(TooltipPanel __instance, ref TooltipDefinitions.Scope scope)
        {
            //PATCH: swaps holding ALT behavior for tooltips
            if (!Main.Settings.InvertAltBehaviorOnTooltips)
            {
                return;
            }

            scope = scope switch
            {
                TooltipDefinitions.Scope.Simplified => TooltipDefinitions.Scope.Detailed,
                TooltipDefinitions.Scope.Detailed => TooltipDefinitions.Scope.Simplified,
                _ => scope
            };
        }

        [UsedImplicitly]
        public static void Postfix(TooltipPanel __instance)
        {
            Tooltips.ModifyWidth<TooltipPanelWidthModifier, TooltipPanel>(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeature), nameof(TooltipFeature.Setup))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Setup_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeature __instance)
        {
            Tooltips.ModifyWidth(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureEffectsEnumerator), nameof(TooltipFeatureEffectsEnumerator.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureEffectsEnumerator_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureEffectsEnumerator __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureEffectsEnumWidthMod, TooltipFeatureEffectsEnumerator>(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureSubSpellsEnumerator), nameof(TooltipFeatureSubSpellsEnumerator.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureSubSpellsEnumerator_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureSubSpellsEnumerator __instance)
        {
            Tooltips.ModifyWidth<TooltipSubSpellEnumWidthModifier, TooltipFeatureSubSpellsEnumerator>(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureSpellParameters), nameof(TooltipFeatureSpellParameters.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureSpellParameters_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureSpellParameters __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureSpellParamsWidthModifier, TooltipFeatureSpellParameters>(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureBaseMagicParameters), nameof(TooltipFeatureBaseMagicParameters.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureBaseMagicParameters_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureBaseMagicParameters __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureBaseMagicParamsWidthModifier, TooltipFeatureBaseMagicParameters>(
                __instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureTagsEnumerator), nameof(TooltipFeatureTagsEnumerator.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureTagsEnumerator_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureTagsEnumerator __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureTagsEnumWidthModifier, TooltipFeatureTagsEnumerator>(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureSpellAdvancement), nameof(TooltipFeatureSpellAdvancement.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureSpellAdvancement_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureSpellAdvancement __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureSpellAdvancementWidthMod, TooltipFeatureSpellAdvancement>(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureDeviceParameters), nameof(TooltipFeatureDeviceParameters.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureDeviceParameters_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureDeviceParameters __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureDeviceParametersWidthMod, TooltipFeatureDeviceParameters>(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureItemPropertiesEnumerator), nameof(TooltipFeatureItemPropertiesEnumerator.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureItemPropertiesEnumerator_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureItemPropertiesEnumerator __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureItemPropertiesEnumWidthMod, TooltipFeatureItemPropertiesEnumerator>(
                __instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureDeviceFunctionsEnumerator),
        nameof(TooltipFeatureDeviceFunctionsEnumerator.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureDeviceFunctionsEnumerator_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureDeviceFunctionsEnumerator __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureDeviceFunctionsEnumWidthMod, TooltipFeatureDeviceFunctionsEnumerator>(
                __instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureItemStats), nameof(TooltipFeatureItemStats.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureItemStats_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureItemStats __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureItemStatsWidthMod, TooltipFeatureItemStats>(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureWeaponParameters), nameof(TooltipFeatureWeaponParameters.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureWeaponParameters_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureWeaponParameters __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureWeaponParametersWidthMod, TooltipFeatureWeaponParameters>(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureArmorParameters), nameof(TooltipFeatureArmorParameters.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureArmorParameters_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureArmorParameters __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureArmorParamsWidthMod, TooltipFeatureArmorParameters>(__instance);
        }
    }

    //TODO: move to separate file
    [HarmonyPatch(typeof(TooltipFeatureLightSourceParameters), nameof(TooltipFeatureLightSourceParameters.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TooltipFeatureLightSourceParameters_Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(TooltipFeatureLightSourceParameters __instance)
        {
            Tooltips.ModifyWidth<TooltipFeatureLightSourceParamsWidthMod, TooltipFeatureLightSourceParameters>(
                __instance);
        }
    }
}
