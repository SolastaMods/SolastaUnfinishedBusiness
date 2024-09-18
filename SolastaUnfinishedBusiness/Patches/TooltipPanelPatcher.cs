using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            var features = __instance.featuresTable.GetComponentsInChildren<TooltipFeature>();
            Main.Log2($"TT [{string.Join(", ", features.Select(f => f.GetType().Name))}]", true);

            Tooltips.ModifyWidth<TooltipPanelWidthModifier, TooltipPanel>(__instance);

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

}
