using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CursorLocationGeometricShapePatcher
{
    //PATCH: UseHeightOneCylinderEffect
    [HarmonyPatch(typeof(CursorLocationGeometricShape), nameof(CursorLocationGeometricShape.UpdateGeometricShape))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpdateGeometricShape_Patch
    {
        [UsedImplicitly]
        public static void MyUpdateCubePosition_Regular(
            [NotNull] GeometricShape __instance,
            Vector3 origin,
            float edgeSize,
            int targetSize,
            bool adaptToGroundLevel,
            bool isValid,
            int height)
        {
            __instance.UpdateCubePosition_Regular(origin, edgeSize, targetSize, adaptToGroundLevel, isValid);

            if (!Main.Settings.UseHeightOneCylinderEffect)
            {
                return;
            }

            if (height == 0)
            {
                return;
            }

            var vector3 = new Vector3();

            if (!adaptToGroundLevel)
            {
                if (edgeSize % 2.0 == 0.0)
                {
                    vector3 = new Vector3(0.5f, 0.0f, 0.5f);
                }

                if (height % 2.0 == 0.0)
                {
                    vector3.y = 0.5f;
                }
            }
            else
            {
                vector3.y = (float)((0.5 * height) - 0.5);

                if (edgeSize % 2.0 == 0.0)
                {
                    vector3 += new Vector3(0.5f, 0.0f, 0.5f);
                }
            }

            var transform = __instance.cubeRenderer.transform;

            transform.SetPositionAndRotation(origin + vector3, Quaternion.identity);
            transform.localScale = new Vector3(edgeSize, height, edgeSize);
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var targetParameter2Field =
                typeof(CursorLocationGeometricShape).GetField("targetParameter2",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            var updateCubePositionRegularMethod = typeof(GeometricShape).GetMethod("UpdateCubePosition_Regular");
            var myUpdateCubePositionRegularMethod =
                typeof(UpdateGeometricShape_Patch).GetMethod("MyUpdateCubePosition_Regular");

            return instructions.ReplaceCalls(updateCubePositionRegularMethod,
                "CursorLocationGeometricShape.UpdateGeometricShape",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, targetParameter2Field),
                new CodeInstruction(OpCodes.Call, myUpdateCubePositionRegularMethod));
        }
    }
}
