using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Diagnostic
{
    internal static class EffectFormControl
    {
        public static bool Sanitize { get; set; }
    }

    [HarmonyPatch(typeof(EffectForm), "DamageForm", MethodType.Getter)]
    internal static class EffectFormPatch_Damage
    {
        public static void Postfix(EffectForm __instance, ref DamageForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Damage)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "HealingForm", MethodType.Getter)]
    internal static class EffectFormPatch_Healing
    {
        public static void Postfix(EffectForm __instance, ref HealingForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Healing)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ConditionForm", MethodType.Getter)]
    internal static class EffectFormPatch_Condition
    {
        public static void Postfix(EffectForm __instance, ref ConditionForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Condition)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "LightSourceForm", MethodType.Getter)]
    internal static class EffectFormPatch_LightSource
    {
        public static void Postfix(EffectForm __instance, ref LightSourceForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.LightSource)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "SummonForm", MethodType.Getter)]
    internal static class EffectFormPatch_Summon
    {
        public static void Postfix(EffectForm __instance, ref SummonForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Summon)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "CounterForm", MethodType.Getter)]
    internal static class EffectFormPatch_Counter
    {
        public static void Postfix(EffectForm __instance, ref CounterForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Counter)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "TemporaryHitPointsForm", MethodType.Getter)]
    internal static class EffectFormPatch_TemporaryHitPoints
    {
        public static void Postfix(EffectForm __instance, ref TemporaryHitPointsForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.TemporaryHitPoints)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "MotionForm", MethodType.Getter)]
    internal static class EffectFormPatch_Motion
    {
        public static void Postfix(EffectForm __instance, ref MotionForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Motion)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "SpellSlotsForm", MethodType.Getter)]
    internal static class EffectFormPatch_SpellSlots
    {
        public static void Postfix(EffectForm __instance, ref SpellSlotsForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.SpellSlots)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "DivinationForm", MethodType.Getter)]
    internal static class EffectFormPatch_Divination
    {
        public static void Postfix(EffectForm __instance, ref DivinationForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Divination)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ItemPropertyForm", MethodType.Getter)]
    internal static class EffectFormPatch_ItemProperty
    {
        public static void Postfix(EffectForm __instance, ref ItemPropertyForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.ItemProperty)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "AlterationForm", MethodType.Getter)]
    internal static class EffectFormPatch_Alteration
    {
        public static void Postfix(EffectForm __instance, ref AlterationForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Alteration)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "TopologyForm", MethodType.Getter)]
    internal static class EffectFormPatch_Topology
    {
        public static void Postfix(EffectForm __instance, ref TopologyForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Topology)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ReviveForm", MethodType.Getter)]
    internal static class EffectFormPatch_Revive
    {
        public static void Postfix(EffectForm __instance, ref ReviveForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Revive)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "KillForm", MethodType.Getter)]
    internal static class EffectFormPatch_Kill
    {
        public static void Postfix(EffectForm __instance, ref KillForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.Kill)
            {
                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(EffectForm), "ShapeChangeForm", MethodType.Getter)]
    internal static class EffectFormPatch_ShapeChange
    {
        public static void Postfix(EffectForm __instance, ref ShapeChangeForm __result)
        {
            if (EffectFormControl.Sanitize && __instance.FormType != EffectForm.EffectFormType.ShapeChange)
            {
                __result = null;
            }
        }
    }
}
