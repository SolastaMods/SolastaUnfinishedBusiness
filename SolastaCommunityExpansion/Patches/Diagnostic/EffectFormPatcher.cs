using System;
using HarmonyLib;
using SolastaModApi.Diagnostics;
using static EffectForm.EffectFormType;

namespace SolastaCommunityExpansion.Patches.Diagnostic
{
#if DEBUG
    // Only add these 16 patches EffectForm for debug builds for diagnostic purposes.
    internal static class EffectFormControl
    {
        [Flags]
        public enum SanitizeMode
        {
            None,
            ReturnNull = 1,
            Log = 2,
            Throw = 4
        }

        // TODO: needs adding to a 'diagnostics' tab in setting UI
        public static SanitizeMode Mode { get; set; } = SanitizeMode.Log;

        public static void Sanitize<T>(EffectForm form, EffectForm.EffectFormType type, ref T __result) where T : class
        {
            if(Mode == SanitizeMode.None)
            {
                return;
            }

            if(form.FormType == type)
            {
                return;
            }

            if(Mode.HasFlag(SanitizeMode.Log))
            {
                Main.Log($"EffectForm with type {form.FormType} is being used as type {type}.");
            }

            if(Mode.HasFlag(SanitizeMode.ReturnNull))
            {
                __result = null;
            }

            if(Mode.HasFlag(SanitizeMode.Throw))
            {
                throw new SolastaModApiException($"EffectForm with type {form.FormType} is being used as type {type}.");
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "DamageForm", MethodType.Getter)]
    internal static class EffectFormPatch_Damage
    {
        public static void Postfix(EffectForm __instance, ref DamageForm __result)
        {
            EffectFormControl.Sanitize(__instance, Damage, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "HealingForm", MethodType.Getter)]
    internal static class EffectFormPatch_Healing
    {
        public static void Postfix(EffectForm __instance, ref HealingForm __result)
        {
            EffectFormControl.Sanitize(__instance, Healing, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ConditionForm", MethodType.Getter)]
    internal static class EffectFormPatch_Condition
    {
        public static void Postfix(EffectForm __instance, ref ConditionForm __result)
        {
            EffectFormControl.Sanitize(__instance, Condition, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "LightSourceForm", MethodType.Getter)]
    internal static class EffectFormPatch_LightSource
    {
        public static void Postfix(EffectForm __instance, ref LightSourceForm __result)
        {
            EffectFormControl.Sanitize(__instance, LightSource, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "SummonForm", MethodType.Getter)]
    internal static class EffectFormPatch_Summon
    {
        public static void Postfix(EffectForm __instance, ref SummonForm __result)
        {
            EffectFormControl.Sanitize(__instance, Summon, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "CounterForm", MethodType.Getter)]
    internal static class EffectFormPatch_Counter
    {
        public static void Postfix(EffectForm __instance, ref CounterForm __result)
        {
            EffectFormControl.Sanitize(__instance, Counter, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "TemporaryHitPointsForm", MethodType.Getter)]
    internal static class EffectFormPatch_TemporaryHitPoints
    {
        public static void Postfix(EffectForm __instance, ref TemporaryHitPointsForm __result)
        {
            EffectFormControl.Sanitize(__instance, TemporaryHitPoints, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "MotionForm", MethodType.Getter)]
    internal static class EffectFormPatch_Motion
    {
        public static void Postfix(EffectForm __instance, ref MotionForm __result)
        {
            EffectFormControl.Sanitize(__instance, Motion, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "SpellSlotsForm", MethodType.Getter)]
    internal static class EffectFormPatch_SpellSlots
    {
        public static void Postfix(EffectForm __instance, ref SpellSlotsForm __result)
        {
            EffectFormControl.Sanitize(__instance, SpellSlots, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "DivinationForm", MethodType.Getter)]
    internal static class EffectFormPatch_Divination
    {
        public static void Postfix(EffectForm __instance, ref DivinationForm __result)
        {
            EffectFormControl.Sanitize(__instance, Divination, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ItemPropertyForm", MethodType.Getter)]
    internal static class EffectFormPatch_ItemProperty
    {
        public static void Postfix(EffectForm __instance, ref ItemPropertyForm __result)
        {
            EffectFormControl.Sanitize(__instance, ItemProperty, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "AlterationForm", MethodType.Getter)]
    internal static class EffectFormPatch_Alteration
    {
        public static void Postfix(EffectForm __instance, ref AlterationForm __result)
        {
            EffectFormControl.Sanitize(__instance, Alteration, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "TopologyForm", MethodType.Getter)]
    internal static class EffectFormPatch_Topology
    {
        public static void Postfix(EffectForm __instance, ref TopologyForm __result)
        {
            EffectFormControl.Sanitize(__instance, Topology, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ReviveForm", MethodType.Getter)]
    internal static class EffectFormPatch_Revive
    {
        public static void Postfix(EffectForm __instance, ref ReviveForm __result)
        {
            EffectFormControl.Sanitize(__instance, Revive, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "KillForm", MethodType.Getter)]
    internal static class EffectFormPatch_Kill
    {
        public static void Postfix(EffectForm __instance, ref KillForm __result)
        {
            EffectFormControl.Sanitize(__instance, Kill, ref __result);
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ShapeChangeForm", MethodType.Getter)]
    internal static class EffectFormPatch_ShapeChange
    {
        public static void Postfix(EffectForm __instance, ref ShapeChangeForm __result)
        {
            EffectFormControl.Sanitize(__instance, ShapeChange, ref __result);
        }
    }
#endif
}
