#if DEBUG
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Diagnostics;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Diagnostic;

internal static class ItemDefinitionVerification
{
    [Flags]
    public enum Verification
    {
        None,
        ReturnNull = 1,
        Log = 2,
        Throw = 4
    }

    private const string LogName = "ItemDefinition.txt";

    public static Verification Mode { get; set; } = Verification.None;

    internal static void Load()
    {
        // Delete the log file to stop it growing out of control
        var path = Path.Combine(DiagnosticsContext.DiagnosticsFolder, LogName);

        try
        {
            File.Delete(path);
        }
        catch (Exception ex)
        {
            Main.Error(ex);
        }

        // Apply mode before any definitions are created
        Mode = Main.Settings.DebugLogVariantMisuse ? Verification.Log : Verification.None;
    }

    public static void VerifyUsage<T>(ItemDefinition definition, bool hasFlag, ref T __result) where T : class
    {
        if (Mode == Verification.None)
        {
            return;
        }

        // If the return val is null we can be sure it's either throwing an exception
        // or being checked for.
        if (hasFlag || __result == null)
        {
            return;
        }

        var msg =
            $"ItemDefinition {definition.Name}[{definition.GUID}] property {typeof(T)} does not have the matching flag set.";

        if (Mode.HasFlag(Verification.Log))
        {
            Main.Log(msg);

            var path = Path.Combine(DiagnosticsContext.DiagnosticsFolder, LogName);
            File.AppendAllLines(path,
                new[]
                {
                    $"{Environment.NewLine}",
                    "------------------------------------------------------------------------------------", msg
                });
            File.AppendAllText(path, Environment.StackTrace);

            if (Mode.HasFlag(Verification.ReturnNull))
            {
                __result = null;
            }

            if (Mode.HasFlag(Verification.Throw))
            {
                throw new SolastaCommunityExpansionException(msg);
            }
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "ArmorDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_ArmorDescription
    {
        public static void Postfix(ItemDefinition __instance, ref ArmorDescription __result)
        {
            VerifyUsage(__instance, __instance.IsArmor, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "WeaponDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_WeaponDescription
    {
        public static void Postfix(ItemDefinition __instance, ref WeaponDescription __result)
        {
            VerifyUsage(__instance, __instance.IsWeapon, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "AmmunitionDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_AmmunitionDescription
    {
        public static void Postfix(ItemDefinition __instance, ref AmmunitionDescription __result)
        {
            VerifyUsage(__instance, __instance.IsAmmunition, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "UsableDeviceDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_UsableDeviceDescription
    {
        public static void Postfix(ItemDefinition __instance, ref UsableDeviceDescription __result)
        {
            VerifyUsage(__instance, __instance.IsUsableDevice, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "ToolDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_ToolDescription
    {
        public static void Postfix(ItemDefinition __instance, ref ToolDescription __result)
        {
            VerifyUsage(__instance, __instance.IsTool, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "StarterPackDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_StarterPackDescription
    {
        public static void Postfix(ItemDefinition __instance, ref StarterPackDescription __result)
        {
            VerifyUsage(__instance, __instance.IsStarterPack, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "ContainerItemDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_ContainerItemDescription
    {
        public static void Postfix(ItemDefinition __instance, ref ContainerItemDescription __result)
        {
            VerifyUsage(__instance, __instance.IsContainerItem, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "LightSourceItemDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_LightSourceItemDescription
    {
        public static void Postfix(ItemDefinition __instance, ref LightSourceItemDescription __result)
        {
            VerifyUsage(__instance, __instance.IsLightSourceItem, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "FocusItemDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_FocusItemDescription
    {
        public static void Postfix(ItemDefinition __instance, ref FocusItemDescription __result)
        {
            VerifyUsage(__instance, __instance.IsFocusItem, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "WealthPileDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_WealthPileDescription
    {
        public static void Postfix(ItemDefinition __instance, ref WealthPileDescription __result)
        {
            VerifyUsage(__instance, __instance.IsWealthPile, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "SpellbookDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_SpellbookDescription
    {
        public static void Postfix(ItemDefinition __instance, ref SpellbookDescription __result)
        {
            VerifyUsage(__instance, __instance.IsSpellbook, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "FoodDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_FoodDescription
    {
        public static void Postfix(ItemDefinition __instance, ref FoodDescription __result)
        {
            VerifyUsage(__instance, __instance.IsFood, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "FactionRelicDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_FactionRelicDescription
    {
        public static void Postfix(ItemDefinition __instance, ref FactionRelicDescription __result)
        {
            VerifyUsage(__instance, __instance.IsFactionRelic, ref __result);
        }
    }

    [HarmonyPatch(typeof(ItemDefinition), "DocumentDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemDefinitionPatch_DocumentDescription
    {
        public static void Postfix(ItemDefinition __instance, ref DocumentDescription __result)
        {
            VerifyUsage(__instance, __instance.IsDocument, ref __result);
        }
    }
}
#endif
