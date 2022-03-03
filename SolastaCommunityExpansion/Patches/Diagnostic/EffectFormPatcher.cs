using System;
using System.IO;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Diagnostics;
using static EffectForm.EffectFormType;

namespace SolastaCommunityExpansion.Patches.Diagnostic
{
#if DEBUG
    // Only add these 16 patches EffectForm for debug builds for diagnostic purposes.
    internal static class EffectFormVerification
    {
        [Flags]
        public enum Verification
        {
            None,
            ReturnNull = 1,
            Log = 2,
            Throw = 4
        }

        public static Verification Mode { get; set; } = Verification.None;

        private const string LogName = "EffectForm.txt";

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

        public static void VerifyUsage<T>(EffectForm form, EffectForm.EffectFormType type, ref T __result) where T : class
        {
            if (Mode == Verification.None)
            {
                return;
            }

            if (form.FormType == type)
            {
                return;
            }

            var msg = $"EffectForm with type {form.FormType} is being used as type {type}.";

            if (Mode.HasFlag(Verification.Log))
            {
                Main.Log(msg);

                var path = Path.Combine(DiagnosticsContext.DiagnosticsFolder, LogName);
                File.AppendAllLines(path, new string[] {
                        $"{Environment.NewLine}",
                        $"------------------------------------------------------------------------------------",
                        msg
                    });
                File.AppendAllText(path, Environment.StackTrace);
            }

            if (Mode.HasFlag(Verification.ReturnNull))
            {
                __result = null;
            }

            if (Mode.HasFlag(Verification.Throw))
            {
                throw new SolastaModApiException(msg);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "DamageForm", MethodType.Getter)]
        internal static class EffectFormPatch_Damage
        {
            public static void Postfix(EffectForm __instance, ref DamageForm __result)
            {
                VerifyUsage(__instance, Damage, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "HealingForm", MethodType.Getter)]
        internal static class EffectFormPatch_Healing
        {
            public static void Postfix(EffectForm __instance, ref HealingForm __result)
            {
                VerifyUsage(__instance, Healing, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "ConditionForm", MethodType.Getter)]
        internal static class EffectFormPatch_Condition
        {
            public static void Postfix(EffectForm __instance, ref ConditionForm __result)
            {
                VerifyUsage(__instance, Condition, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "LightSourceForm", MethodType.Getter)]
        internal static class EffectFormPatch_LightSource
        {
            public static void Postfix(EffectForm __instance, ref LightSourceForm __result)
            {
                VerifyUsage(__instance, LightSource, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "SummonForm", MethodType.Getter)]
        internal static class EffectFormPatch_Summon
        {
            public static void Postfix(EffectForm __instance, ref SummonForm __result)
            {
                VerifyUsage(__instance, Summon, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "CounterForm", MethodType.Getter)]
        internal static class EffectFormPatch_Counter
        {
            public static void Postfix(EffectForm __instance, ref CounterForm __result)
            {
                VerifyUsage(__instance, Counter, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "TemporaryHitPointsForm", MethodType.Getter)]
        internal static class EffectFormPatch_TemporaryHitPoints
        {
            public static void Postfix(EffectForm __instance, ref TemporaryHitPointsForm __result)
            {
                VerifyUsage(__instance, TemporaryHitPoints, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "MotionForm", MethodType.Getter)]
        internal static class EffectFormPatch_Motion
        {
            public static void Postfix(EffectForm __instance, ref MotionForm __result)
            {
                VerifyUsage(__instance, Motion, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "SpellSlotsForm", MethodType.Getter)]
        internal static class EffectFormPatch_SpellSlots
        {
            public static void Postfix(EffectForm __instance, ref SpellSlotsForm __result)
            {
                VerifyUsage(__instance, SpellSlots, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "DivinationForm", MethodType.Getter)]
        internal static class EffectFormPatch_Divination
        {
            public static void Postfix(EffectForm __instance, ref DivinationForm __result)
            {
                VerifyUsage(__instance, Divination, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "ItemPropertyForm", MethodType.Getter)]
        internal static class EffectFormPatch_ItemProperty
        {
            public static void Postfix(EffectForm __instance, ref ItemPropertyForm __result)
            {
                VerifyUsage(__instance, ItemProperty, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "AlterationForm", MethodType.Getter)]
        internal static class EffectFormPatch_Alteration
        {
            public static void Postfix(EffectForm __instance, ref AlterationForm __result)
            {
                VerifyUsage(__instance, Alteration, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "TopologyForm", MethodType.Getter)]
        internal static class EffectFormPatch_Topology
        {
            public static void Postfix(EffectForm __instance, ref TopologyForm __result)
            {
                VerifyUsage(__instance, Topology, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "ReviveForm", MethodType.Getter)]
        internal static class EffectFormPatch_Revive
        {
            public static void Postfix(EffectForm __instance, ref ReviveForm __result)
            {
                VerifyUsage(__instance, Revive, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "KillForm", MethodType.Getter)]
        internal static class EffectFormPatch_Kill
        {
            public static void Postfix(EffectForm __instance, ref KillForm __result)
            {
                VerifyUsage(__instance, Kill, ref __result);
            }
        }

        [HarmonyPatch(typeof(EffectForm), "ShapeChangeForm", MethodType.Getter)]
        internal static class EffectFormPatch_ShapeChange
        {
            public static void Postfix(EffectForm __instance, ref ShapeChangeForm __result)
            {
                VerifyUsage(__instance, ShapeChange, ref __result);
            }
        }
    }
#endif
}
