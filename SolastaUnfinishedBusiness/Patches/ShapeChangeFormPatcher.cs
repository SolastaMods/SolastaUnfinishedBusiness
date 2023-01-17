using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ShapeChangeFormPatcher
{
    [HarmonyPatch(typeof(ShapeSelectionPanel), nameof(ShapeSelectionPanel.Show))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Show_Patch
    {
        private static List<ShapeOptionDescription> ShapeOptions(
            ShapeChangeForm shapeChangeForm,
            RulesetCharacter shifter)
        {
            if (shapeChangeForm.shapeChangeType != ShapeChangeForm.Type.ClassLevelListSelection)
            {
                return shapeChangeForm.ShapeOptions;
            }

            var shapeOptions = shifter.GetSubFeaturesByType<IChangeShapeOptions>()
                .Where(x => x.SpecialSubstituteCondition == shapeChangeForm.specialSubstituteCondition)
                .SelectMany(y => y.ShapeOptions)
                .ToList();

            return shapeOptions.Count == 0 ? shapeChangeForm.ShapeOptions : shapeOptions;
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for IChangeShapeOptions
            var shapeOptionsMethod = typeof(ShapeChangeForm).GetMethod("get_ShapeOptions");
            var myShapeOptionsMethod =
                new Func<ShapeChangeForm, RulesetCharacter, List<ShapeOptionDescription>>(ShapeOptions).Method;

            return instructions.ReplaceCalls(shapeOptionsMethod, "ShapeSelectionPanel.Show",
                new CodeInstruction(OpCodes.Ldarg_3), // shifter
                new CodeInstruction(OpCodes.Call, myShapeOptionsMethod));
        }
    }
}
