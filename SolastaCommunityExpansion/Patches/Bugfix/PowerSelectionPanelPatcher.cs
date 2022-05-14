using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    internal static class PowerSelectionPanelPatcher
    {
        // Fixes TA's code not checking for some power's activation time and directly loking it up in a dict where this activation time is absent.
        [HarmonyPatch(typeof(PowerSelectionPanel), "Bind")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class PowerSelectionPanel_Bind
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                var customMethod =
                    new Func<List<FeatureDefinition>, FeatureDefinitionPower, bool>(CustomCheck).Method;

                var bindIndex = codes.FindIndex(x =>
                    x.opcode == OpCodes.Callvirt && x.operand.ToString().Contains("Contains"));

                if (bindIndex > 0)
                {
                    codes[bindIndex] = new CodeInstruction(OpCodes.Call, customMethod);
                }

                return codes.AsEnumerable();
            }

            //Replaces 'overridenPowers.Contains(power)' check by adding check to see if this power's activation time is present in ActionDefinitions.CastingTimeToActionDefinition
            private static bool CustomCheck(List<FeatureDefinition> overridenPowers, FeatureDefinitionPower power)
            {
                return overridenPowers.Contains(power)
                       || !ActionDefinitions.CastingTimeToActionDefinition.ContainsKey(power.ActivationTime);
            }
        }
    }
}
