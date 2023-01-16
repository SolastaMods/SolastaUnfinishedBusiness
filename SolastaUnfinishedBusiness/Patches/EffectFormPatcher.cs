#if DEBUG
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static EffectForm.EffectFormType;
using static SolastaUnfinishedBusiness.DataMiner.EffectFormVerification;

namespace SolastaUnfinishedBusiness.Patches;

//PATCH: These patches are for effect form usage diagnostics
[UsedImplicitly] public static class EffectFormPatcher
{
    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.DamageForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Damage_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref DamageForm __result)
        {
            VerifyUsage(__instance, Damage, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.HealingForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Healing_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref HealingForm __result)
        {
            VerifyUsage(__instance, Healing, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.ConditionForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Condition_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref ConditionForm __result)
        {
            VerifyUsage(__instance, Condition, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.LightSourceForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class LightSource_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref LightSourceForm __result)
        {
            VerifyUsage(__instance, LightSource, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.SummonForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Summon_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref SummonForm __result)
        {
            VerifyUsage(__instance, Summon, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.CounterForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Counter_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref CounterForm __result)
        {
            VerifyUsage(__instance, Counter, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.TemporaryHitPointsForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class TemporaryHitPoints_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref TemporaryHitPointsForm __result)
        {
            VerifyUsage(__instance, TemporaryHitPoints, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.MotionForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Motion_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref MotionForm __result)
        {
            VerifyUsage(__instance, Motion, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.SpellSlotsForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class SpellSlots_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref SpellSlotsForm __result)
        {
            VerifyUsage(__instance, SpellSlots, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.DivinationForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Divination_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref DivinationForm __result)
        {
            VerifyUsage(__instance, Divination, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.ItemPropertyForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class ItemProperty_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref ItemPropertyForm __result)
        {
            VerifyUsage(__instance, ItemProperty, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.AlterationForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Alteration_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref AlterationForm __result)
        {
            VerifyUsage(__instance, Alteration, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.TopologyForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Topology_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref TopologyForm __result)
        {
            VerifyUsage(__instance, Topology, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.ReviveForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Revive_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref ReviveForm __result)
        {
            VerifyUsage(__instance, Revive, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.KillForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class Kill_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref KillForm __result)
        {
            VerifyUsage(__instance, Kill, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), nameof(EffectForm.ShapeChangeForm), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class ShapeChange_Getter_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectForm __instance, ref ShapeChangeForm __result)
        {
            VerifyUsage(__instance, ShapeChange, ref __result);
        }
    }
}
#endif
