using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUiLevelUp
{
    [HarmonyPatch(typeof(FeatSubPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatSubPanel_Bind
    {
        internal static void Prefix(List<FeatDefinition> ___relevantFeats, RectTransform ___table, GameObject ___itemPrefab)
        {
            var dbFeatDefinition = DatabaseRepository.GetDatabase<FeatDefinition>();

            ___relevantFeats.SetRange(dbFeatDefinition.Where(x => !x.GuiPresentation.Hidden));

            while (___table.childCount < ___relevantFeats.Count)
            {
                Gui.GetPrefabFromPool(___itemPrefab, ___table);
            }

            while (___table.childCount > ___relevantFeats.Count)
            {
                Gui.ReleaseInstanceToPool(___table.GetChild(___table.childCount - 1).gameObject);
            }
        }
    }

    [HarmonyPatch(typeof(FeatSubPanel), "RuntimeLoaded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatSubPanel_RuntimeLoaded
    {
        internal static void Sort(List<FeatDefinition> relevantFeats)
        {
            relevantFeats?.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;

                if (Main.Settings.EnableSortingFeats && instruction.opcode == OpCodes.Pop)
                {
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
                    var relevantFeatsField = typeof(FeatSubPanel).GetField("relevantFeats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var sortMethod = typeof(FeatSubPanel_RuntimeLoaded).GetMethod("Sort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, relevantFeatsField);
                    yield return new CodeInstruction(OpCodes.Call, sortMethod);
                }
            }
        }
    }
}
