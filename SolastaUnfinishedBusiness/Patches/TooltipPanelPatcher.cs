using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

public static class TooltipPanelPatcher
{
    [HarmonyPatch(typeof(TooltipPanel), "SetupFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SetupFeatures_Patch
    {
        public static void Prefix(ref TooltipDefinitions.Scope scope,
            GuiTooltipClassDefinition tooltipClassDefinition)
        {
            //PATCH: swaps holding ALT behavior for tooltips
            if (!Main.Settings.InvertAltBehaviorOnTooltips)
            {
                return;
            }

            var isCondition = tooltipClassDefinition.Name == GuiActiveCondition.tooltipClass;

            scope = scope switch
            {
                TooltipDefinitions.Scope.Simplified => isCondition
                    ? TooltipDefinitions.Scope.All //Make condition tooltips show all parts
                    : TooltipDefinitions.Scope.Detailed,
                TooltipDefinitions.Scope.Detailed => TooltipDefinitions.Scope.Simplified,
                _ => scope
            };
        }

        //TODO: review non working transpiler
        public static IEnumerable<CodeInstruction> Transpiler_DISABLED([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support All scope for tooltip
            var customMethod =
                new Func<TooltipDefinitions.Scope, TooltipDefinitions.Scope, bool>(Compare).Method;

            var codes = instructions.ToList();
            var found = false;

            for (var i = 2; i < codes.Count; i++)
            {
                var before = codes[i - 2];
                var prev = codes[i - 1];
                var code = codes[i];

                if (code.opcode == OpCodes.Ceq
                    && prev.opcode == OpCodes.Ldarg_1
                    && before.opcode == OpCodes.Ldfld
                    && $"{before.operand}".Contains("TooltipDefinitions+Scope"))
                {
                    found = true;
                    codes[i] = new CodeInstruction(OpCodes.Call, customMethod);
                    break;
                }
            }

            if (!found)
            {
                Main.Error("Couldn't patch TooltipPanel.SetupFeatures");
            }

            return codes;
        }

        private static bool Compare(TooltipDefinitions.Scope featureScope, TooltipDefinitions.Scope tipScope)
        {
            return tipScope == TooltipDefinitions.Scope.All || featureScope == tipScope;
        }
    }
}
