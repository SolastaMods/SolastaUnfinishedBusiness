using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.SquareCylinder;

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

        if (!Main.Settings.UseHeightOneCylinderEffect)
        {
#if DEBUG
            var t = __instance.cubeRenderer.transform;
            var p1 = t.position;
            var s1 = t.localScale;
            Main.Log(
                $"Cube: origin=({origin.x}, {origin.y}, {origin.z}) position=({p1.x},{p1.y},{p1.z}), scale=({s1.x},{s1.y},{s1.z})");
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

#if DEBUG
        var p = transform.position;
        var s = transform.localScale;
        Main.Log(
            $"SquareCylinder: origin=({origin.x}, {origin.y}, {origin.z}) position=({p.x},{p.y},{p.z}), scale=({s.x},{s.y},{s.z})");
#endif
    }

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var targetParameter2Field =
            typeof(CursorLocationGeometricShape).GetField("targetParameter2",
                BindingFlags.Instance | BindingFlags.NonPublic);
        var updateCubePosition_RegularMethod = typeof(GeometricShape).GetMethod("UpdateCubePosition_Regular");
        var myUpdateCubePosition_RegularMethod =
            typeof(CursorLocationGeometricShape_UpdateGeometricShape).GetMethod("MyUpdateCubePosition_Regular");

        foreach (var instruction in instructions)
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
    public static void Postfix(GeometricShape __instance, Vector3 origin)
    {
        var transform = __instance.cylinderRenderer.transform;
        var p = transform.position;
        var s = transform.localScale;
        Main.Log(
            $"Cylinder: origin=({origin.x}, {origin.y}, {origin.z}) position=({p.x},{p.y},{p.z}), scale=({s.x},{s.y},{s.z})");
    }
}
#endif

[HarmonyPatch(typeof(GameLocationTargetingManager), "BuildAABB")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationTargetingManager_BuildAABB
{
    public static void Postfix(GameLocationTargetingManager __instance)
    {
        if (!Main.Settings.UseHeightOneCylinderEffect)
        {
#if DEBUG
            var min1 = __instance.bounds.min;
            var max1 = __instance.bounds.max;
            Main.Log(
                $"BuildAAAB {__instance.shapeType} min({min1.x}, {min1.y}, {min1.z}), max({max1.x}, {max1.y}, {max1.z})");
#endif
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

        if (__instance.hasMagneticTargeting || __instance.rangeType == RuleDefinitions.RangeType.Self)
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
#if DEBUG
        var min = __instance.bounds.min;
        var max = __instance.bounds.max;
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

        if (!Main.Settings.UseHeightOneCylinderEffect)
        {
            Main.Log(
                $"GeometryUtils_CubeContainsPoint_Regular (off): edge={edgeSize}, origin=({cubeOrigin.x}, {cubeOrigin.y}, {cubeOrigin.z}), point=({point.x}, {point.y}, {point.z}), result={__result}");
            return __result;
        }

        if (height == 0)
        {
            return __result;
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

        var vector3_2 = point - cubeOrigin - vector3;

        __result =
            Mathf.Abs(vector3_2.x) <= (double)0.5f * edgeSize
            && Mathf.Abs(vector3_2.y) <= (double)0.5f * height
            && Mathf.Abs(vector3_2.z) <= (double)0.5f * edgeSize;

        Main.Log(
            $"GeometryUtils_CubeContainsPoint_Regular (on): edge={edgeSize}, height={height}, origin=({cubeOrigin.x}, {cubeOrigin.y}, {cubeOrigin.z}), point=({point.x}, {point.y}, {point.z}), result={__result}");

        return __result;
    }

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var geometricParameter2Field =
            typeof(GameLocationTargetingManager).GetField("geometricParameter2",
                BindingFlags.Instance | BindingFlags.NonPublic);
        var cubeContainsPoint_RegularMethod = typeof(GeometryUtils).GetMethod("CubeContainsPoint_Regular");
        var myCubeContainsPoint_RegularMethod =
            typeof(GameLocationTargetingManager_DoesShapeContainPoint).GetMethod("MyCubeContainsPoint_Regular");

        foreach (var instruction in instructions)
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
        Vector3 cylinderOrigin, /*Vector3 cylinderDirection,*/ float cylinderLength, float cylinderDiameter,
        Vector3 point, ref bool __result)
    {
        Main.Log(
            $"GeometryUtils_CylinderContainsPoint: diameter={cylinderDiameter}, height/length={cylinderLength}, origin=({cylinderOrigin.x}, {cylinderOrigin.y}, {cylinderOrigin.z}), point=({point.x}, {point.y}, {point.z}), result={__result}");
    }
}
#endif
