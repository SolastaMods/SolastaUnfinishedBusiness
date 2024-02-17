using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using UnityEngine;
using static RuleDefinitions;

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

    //PATCH: UseHeightOneCylinderEffect
    [HarmonyPatch(typeof(GameLocationTargetingManager), nameof(GameLocationTargetingManager.BuildAABB))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BuildAABB_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationTargetingManager __instance)
        {
            if (!Main.Settings.UseHeightOneCylinderEffect)
            {
                return;
            }

            if (__instance.shapeType != MetricsDefinitions.GeometricShapeType.Cube)
            {
                return;
            }

            if (__instance.geometricParameter2 <= 0)
            {
                return;
            }

            var edgeSize = __instance.geometricParameter;
            var height = __instance.geometricParameter2;

            Vector3 vector = new();

            if (__instance.hasMagneticTargeting || __instance.rangeType == RangeType.Self)
            {
                if (edgeSize % 2.0 == 0.0)
                {
                    vector = new Vector3(0.5f, 0f, 0.5f);
                }

                if (height % 2.0 == 0.0)
                {
                    vector.y = 0.5f;
                }
            }
            else
            {
                vector = new Vector3(0.0f, (float)((0.5 * height) - 0.5), 0.0f);

                if (edgeSize % 2.0 == 0.0)
                {
                    vector += new Vector3(0.5f, 0.0f, 0.5f);
                }
            }

            __instance.bounds = new Bounds(__instance.origin + vector, new Vector3(edgeSize, height, edgeSize));
        }
    }

    //PATCH: UseHeightOneCylinderEffect
    [HarmonyPatch(typeof(GameLocationTargetingManager), nameof(GameLocationTargetingManager.DoesShapeContainPoint))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class DoesShapeContainPoint_Patch
    {
        [UsedImplicitly]
        public static bool MyCubeContainsPoint_Regular(
            Vector3 cubeOrigin,
            float edgeSize,
            int targetSize,
            bool hasMagneticTargeting,
            Vector3 point,
            float height)
        {
            var result =
                GeometryUtils.CubeContainsPoint_Regular(cubeOrigin, edgeSize, targetSize, hasMagneticTargeting, point);

            if (!Main.Settings.UseHeightOneCylinderEffect)
            {
                return result;
            }

            if (height == 0)
            {
                return result;
            }

            // Code from CubeContainsPoint_Regular modified with height
            var vector3 = new Vector3();

            if (hasMagneticTargeting)
            {
                if (edgeSize % 2.0 == 0.0)
                {
                    vector3 = new Vector3(0.5f, 0f, 0.5f);
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

            var vector32 = point - cubeOrigin - vector3;

            result =
                Mathf.Abs(vector32.x) <= (double)0.5f * edgeSize
                && Mathf.Abs(vector32.y) <= (double)0.5f * height
                && Mathf.Abs(vector32.z) <= (double)0.5f * edgeSize;

            return result;
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var geometricParameter2Field =
                typeof(GameLocationTargetingManager).GetField("geometricParameter2",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            var cubeContainsPointRegularMethod = typeof(GeometryUtils).GetMethod("CubeContainsPoint_Regular");
            var myCubeContainsPointRegularMethod =
                typeof(DoesShapeContainPoint_Patch).GetMethod("MyCubeContainsPoint_Regular");

            return instructions.ReplaceCalls(cubeContainsPointRegularMethod,
                "CursorLocationGeometricShape.DoesShapeContainPoint",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, geometricParameter2Field),
                new CodeInstruction(OpCodes.Call, myCubeContainsPointRegularMethod));
        }

        [HarmonyPatch(typeof(CursorLocationGeometricShape), nameof(CursorLocationGeometricShape.RefreshHover))]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        [UsedImplicitly]
        public static class RefreshHover_Patch
        {
            [UsedImplicitly]
            public static void Postfix(CursorLocationGeometricShape __instance)
            {
                __instance.affectedCharacterColor = Main.Settings.EnableHighContrastTargetingAoe ? Color.yellow : new Color(0.110f, 0.311f, 0.287f, 1.000f);
            }
        }
    }
}
