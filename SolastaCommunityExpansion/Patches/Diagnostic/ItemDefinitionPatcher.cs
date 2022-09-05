#if DEBUG
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Diagnostic;

//PATCH: These patches are for item usage diagnostics
internal static class ItemDefinitionPatcher
{
    [HarmonyPatch(typeof(ItemDefinition), "ArmorDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ArmorDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref ArmorDescription __result)
        {
            VerifyUsage(__instance, __instance.IsArmor, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "WeaponDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class WeaponDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref WeaponDescription __result)
        {
            VerifyUsage(__instance, __instance.IsWeapon, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "AmmunitionDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AmmunitionDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref AmmunitionDescription __result)
        {
            VerifyUsage(__instance, __instance.IsAmmunition, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "UsableDeviceDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UsableDeviceDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref UsableDeviceDescription __result)
        {
            VerifyUsage(__instance, __instance.IsUsableDevice, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "ToolDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ToolDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref ToolDescription __result)
        {
            VerifyUsage(__instance, __instance.IsTool, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "StarterPackDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class StarterPackDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref StarterPackDescription __result)
        {
            VerifyUsage(__instance, __instance.IsStarterPack, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "ContainerItemDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ContainerItemDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref ContainerItemDescription __result)
        {
            VerifyUsage(__instance, __instance.IsContainerItem, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "LightSourceItemDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class LightSourceItemDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref LightSourceItemDescription __result)
        {
            VerifyUsage(__instance, __instance.IsLightSourceItem, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "FocusItemDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FocusItemDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref FocusItemDescription __result)
        {
            VerifyUsage(__instance, __instance.IsFocusItem, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "WealthPileDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class WealthPileDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref WealthPileDescription __result)
        {
            VerifyUsage(__instance, __instance.IsWealthPile, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "SpellbookDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellbookDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref SpellbookDescription __result)
        {
            VerifyUsage(__instance, __instance.IsSpellbook, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "FoodDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FoodDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref FoodDescription __result)
        {
            VerifyUsage(__instance, __instance.IsFood, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "FactionRelicDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FactionRelicDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref FactionRelicDescription __result)
        {
            VerifyUsage(__instance, __instance.IsFactionRelic, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "DocumentDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class DocumentDescription_Patch
    {
        public static void Postfix(ItemDefinition __instance, ref DocumentDescription __result)
        {
            VerifyUsage(__instance, __instance.IsDocument, ref __result);
        }
    }
}
#endif
