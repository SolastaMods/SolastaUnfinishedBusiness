#if DEBUG
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.DataMiner;
using static EffectForm.EffectFormType;

namespace SolastaCommunityExpansion.Patches.Diagnostic;

//PATCH: These patches are for efefct form usage diagnostics
internal static class EffectFormPatcher
{
    [HarmonyPatch(typeof(EffectForm), "DamageForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Damage_Patch
    {
        public static void Postfix(EffectForm __instance, ref DamageForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Damage, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "HealingForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Healing_Patch
    {
        public static void Postfix(EffectForm __instance, ref HealingForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Healing, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ConditionForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Condition_Patch
    {
        public static void Postfix(EffectForm __instance, ref ConditionForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Condition, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "LightSourceForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class LightSource_Patch
    {
        public static void Postfix(EffectForm __instance, ref LightSourceForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, LightSource, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "SummonForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Summon_Patch
    {
        public static void Postfix(EffectForm __instance, ref SummonForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Summon, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "CounterForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Counter_Patch
    {
        public static void Postfix(EffectForm __instance, ref CounterForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Counter, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "TemporaryHitPointsForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TemporaryHitPoints_Patch
    {
        public static void Postfix(EffectForm __instance, ref TemporaryHitPointsForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, TemporaryHitPoints, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "MotionForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Motion_Patch
    {
        public static void Postfix(EffectForm __instance, ref MotionForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Motion, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "SpellSlotsForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellSlots_Patch
    {
        public static void Postfix(EffectForm __instance, ref SpellSlotsForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, SpellSlots, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "DivinationForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Divination_Patch
    {
        public static void Postfix(EffectForm __instance, ref DivinationForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Divination, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ItemPropertyForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemProperty_Patch
    {
        public static void Postfix(EffectForm __instance, ref ItemPropertyForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, ItemProperty, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "AlterationForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Alteration_Patch
    {
        public static void Postfix(EffectForm __instance, ref AlterationForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Alteration, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "TopologyForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Topology_Patch
    {
        public static void Postfix(EffectForm __instance, ref TopologyForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Topology, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ReviveForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Revive_Patch
    {
        public static void Postfix(EffectForm __instance, ref ReviveForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Revive, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "KillForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Kill_Patch
    {
        public static void Postfix(EffectForm __instance, ref KillForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, Kill, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ShapeChangeForm", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ShapeChange_Patch
    {
        public static void Postfix(EffectForm __instance, ref ShapeChangeForm __result)
        {
            EffectFormVerification.VerifyUsage(__instance, ShapeChange, ref __result);
        }
    }
}
#endif
