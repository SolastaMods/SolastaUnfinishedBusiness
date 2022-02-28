using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.SquareCylinder
{
    [HarmonyPatch(typeof(CursorLocationGeometricShape), "UpdateGeometricShape")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CursorLocationGeometricShape_UpdateGeometricShape
    {
        public static void MyUpdateCubePosition_Regular(
            GeometricShape __instance,
            Vector3 origin,
            float edgeSize,
            bool adaptToGroundLevel,
            bool isValid,
            int height)
        {
            __instance.UpdateCubePosition_Regular(origin, edgeSize, adaptToGroundLevel, isValid);

            if (!Main.Settings.EnableTargetTypeSquareCylinder)
            {
#if DEBUG
                var t = __instance.GetField<GeometricShape, MeshRenderer>("cubeRenderer").transform;
                var p1 = t.position;
                var s1 = t.localScale;
                Main.Log($"Cube: origin=({origin.x}, {origin.y}, {origin.z}) position=({p1.x},{p1.y},{p1.z}), scale=({s1.x},{s1.y},{s1.z})");
#endif
                return;
            }

            if (height == 0)
            {
                return;
            }

            var vector3 = new Vector3();

            if (!adaptToGroundLevel)
            {
                if ((double)edgeSize % 2.0 == 0.0)
                    vector3 = new Vector3(0.5f, 0.0f, 0.5f);

                if (height % 2.0 == 0.0)
                    vector3.y = 0.5f;
            }
            else
            {
                vector3.y = (float)(0.5 * (double)height - 0.5);

                if ((double)edgeSize % 2.0 == 0.0)
                    vector3 += new Vector3(0.5f, 0.0f, 0.5f);
            }

            var transform = __instance.GetField<GeometricShape, MeshRenderer>("cubeRenderer").transform;
            transform.SetPositionAndRotation(origin + vector3, Quaternion.identity);
            transform.localScale = new Vector3(edgeSize, height, edgeSize);

#if DEBUG
            var p = transform.position;
            var s = transform.localScale;
            Main.Log($"SquareCylinder: origin=({origin.x}, {origin.y}, {origin.z}) position=({p.x},{p.y},{p.z}), scale=({s.x},{s.y},{s.z})");
#endif
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var targetParameter2Field = typeof(CursorLocationGeometricShape).GetField("targetParameter2", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var updateCubePosition_RegularMethod = typeof(GeometricShape).GetMethod("UpdateCubePosition_Regular");
            var myUpdateCubePosition_RegularMethod = typeof(CursorLocationGeometricShape_UpdateGeometricShape).GetMethod("MyUpdateCubePosition_Regular");

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(updateCubePosition_RegularMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                    yield return new CodeInstruction(OpCodes.Ldfld, targetParameter2Field);
                    yield return new CodeInstruction(OpCodes.Call, myUpdateCubePosition_RegularMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

#if DEBUG
    // For comparison - can be removed when working
    [HarmonyPatch(typeof(GeometricShape), "UpdateCylinderPosition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GeometricShape_UpdateCylinderPosition
    {
        public static void Postfix(MeshRenderer ___cylinderRenderer, Vector3 origin)
        {
            var transform = ___cylinderRenderer.transform;
            var p = transform.position;
            var s = transform.localScale;
            Main.Log($"Cylinder: origin=({origin.x}, {origin.y}, {origin.z}) position=({p.x},{p.y},{p.z}), scale=({s.x},{s.y},{s.z})");
        }
    }
#endif

    [HarmonyPatch(typeof(GameLocationTargetingManager), "BuildAABB")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationTargetingManager_BuildAABB
    {
        public static void Postfix(
            MetricsDefinitions.GeometricShapeType ___shapeType, Vector3 ___origin, ref Bounds ___bounds,
            float ___geometricParameter, float ___geometricParameter2, bool ___hasMagneticTargeting, RuleDefinitions.RangeType ___rangeType)
        {
            if (!Main.Settings.EnableTargetTypeSquareCylinder)
            {
#if DEBUG
                var min1 = ___bounds.min;
                var max1 = ___bounds.max;
                Main.Log($"BuildAAAB {___shapeType} min({min1.x}, {min1.y}, {min1.z}), max({max1.x}, {max1.y}, {max1.z})");
#endif
                return;
            }

            if (___shapeType != MetricsDefinitions.GeometricShapeType.Cube)
            {
                return;
            }

            if (___geometricParameter2 <= 0)
            {
                return;
            }

            var edgeSize = ___geometricParameter;
            var height = ___geometricParameter2;

            Vector3 vector = new();

            if (___hasMagneticTargeting || ___rangeType == RuleDefinitions.RangeType.Self)
            {
                if ((double)edgeSize % 2.0 == 0.0)
                    vector = new Vector3(0.5f, 0f, 0.5f);

                if (height % 2.0 == 0.0)
                    vector.y = 0.5f;
            }
            else
            {
                vector = new Vector3(0.0f, (float)(0.5 * (double)height - 0.5), 0.0f);

                if ((double)edgeSize % 2.0 == 0.0)
                    vector += new Vector3(0.5f, 0.0f, 0.5f);
            }

            ___bounds = new Bounds(___origin + vector, new Vector3(edgeSize, height, edgeSize));
#if DEBUG
            var min = ___bounds.min;
            var max = ___bounds.max;
            Main.Log($"BuildAAAB min({min.x}, {min.y}, {min.z}), max({max.x}, {max.y}, {max.z})");
#endif
        }
    }

    [HarmonyPatch(typeof(GameLocationTargetingManager), "DoesShapeContainPoint")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationTargetingManager_DoesShapeContainPoint
    {
        public static bool MyCubeContainsPoint_Regular(
            Vector3 cubeOrigin,
            float edgeSize,
            bool hasMagneticTargeting,
            Vector3 point,
            float height)
        {
            var __result = GeometryUtils.CubeContainsPoint_Regular(cubeOrigin, edgeSize, hasMagneticTargeting, point);

            if (!Main.Settings.EnableTargetTypeSquareCylinder)
            {
                Main.Log($"GeometryUtils_CubeContainsPoint_Regular (off): edge={edgeSize}, origin=({cubeOrigin.x}, {cubeOrigin.y}, {cubeOrigin.z}), point=({point.x}, {point.y}, {point.z}), result={__result}");
                return __result;
            }

            if (height  == 0)
            {
                return __result;
            }

            // Code from CubeContainsPoint_Regular modified with height
            var vector3 = new Vector3();

            if (hasMagneticTargeting)
            {
                if ((double)edgeSize % 2.0 == 0.0)
                    vector3 = new Vector3(0.5f, 0f, 0.5f);

                if (height % 2.0 == 0.0)
                    vector3.y = 0.5f;
            }
            else
            {
                vector3.y = (float)(0.5 * (double)height - 0.5);

                if ((double)edgeSize % 2.0 == 0.0)
                    vector3 += new Vector3(0.5f, 0.0f, 0.5f);
            }

            var vector3_2 = point - cubeOrigin - vector3;

            __result =
                (double)Mathf.Abs(vector3_2.x) <= (double)0.5f * edgeSize
                && (double)Mathf.Abs(vector3_2.y) <= (double)0.5f * height
                && (double)Mathf.Abs(vector3_2.z) <= (double)0.5f * edgeSize;

            Main.Log($"GeometryUtils_CubeContainsPoint_Regular (on): edge={edgeSize}, height={height}, origin=({cubeOrigin.x}, {cubeOrigin.y}, {cubeOrigin.z}), point=({point.x}, {point.y}, {point.z}), result={__result}");

            return __result;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var geometricParameter2Field = typeof(GameLocationTargetingManager).GetField("geometricParameter2", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var cubeContainsPoint_RegularMethod = typeof(GeometryUtils).GetMethod("CubeContainsPoint_Regular");
            var myCubeContainsPoint_RegularMethod = typeof(GameLocationTargetingManager_DoesShapeContainPoint).GetMethod("MyCubeContainsPoint_Regular");

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(cubeContainsPoint_RegularMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                    yield return new CodeInstruction(OpCodes.Ldfld, geometricParameter2Field);
                    yield return new CodeInstruction(OpCodes.Call, myCubeContainsPoint_RegularMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

#if DEBUG
    // For comparison - can be removed when working
    [HarmonyPatch(typeof(GeometryUtils), "CylinderContainsPoint")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GeometryUtils_CylinderContainsPoint
    {
        public static void Postfix(
            Vector3 cylinderOrigin, /*Vector3 cylinderDirection,*/ float cylinderLength, float cylinderDiameter, Vector3 point, ref bool __result)
        {
            Main.Log($"GeometryUtils_CylinderContainsPoint: diameter={cylinderDiameter}, height/length={cylinderLength}, origin=({cylinderOrigin.x}, {cylinderOrigin.y}, {cylinderOrigin.z}), point=({point.x}, {point.y}, {point.z}), result={__result}");
        }
    }
#endif
}
