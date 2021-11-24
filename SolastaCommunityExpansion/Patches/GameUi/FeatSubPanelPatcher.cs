using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(FeatSubPanel), "RuntimeLoaded")]
    internal static class FeatSubPanell_RuntimeLoaded
    {
        internal static void Sort(List<FeatDefinition> relevantFeats)
        {
            relevantFeats?.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            System.Reflection.FieldInfo relevantFeatsField = typeof(FeatSubPanel).GetField("relevantFeats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            System.Reflection.MethodInfo sortMethod = typeof(FeatSubPanell_RuntimeLoaded).GetMethod("Sort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;

                if (instruction.opcode == OpCodes.Pop)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, relevantFeatsField);
                    yield return new CodeInstruction(OpCodes.Call, sortMethod);
                }
            }
        }
    }
}
