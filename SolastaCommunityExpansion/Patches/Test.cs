using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(CursorLocationGeometricShape), "UpdateGeometricShape")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CursorLocationGeometricShape_UpdateGeometricShape
    {
        internal static int? Height { get; set; }

        public static void Prefix(int ___targetParameter2)
        {
            Height = ___targetParameter2;
            Main.Log($"CursorLocationGeometricShape_UpdateGeometricShape: tp2={Height}");
        }

        public static void Postfix()
        {
            Height = null;
        }
    }

    [HarmonyPatch(typeof(GeometricShape), "UpdateCubePosition_Regular")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GeometricShape_UpdateCubePosition_Regular
    {
        public static void Postfix(MeshRenderer ___cubeRenderer, Vector3 origin, float edgeSize, bool adaptToGroundLevel)
        {
            var height = CursorLocationGeometricShape_UpdateGeometricShape.Height;

            if ((height ?? 0) == 0)
            {
                return;
            }

            Main.Log($"GeometricShape_UpdateCubePosition_Regular - setting height={height.Value}");

            Vector3 vector3 = new Vector3();

            if (!adaptToGroundLevel)
            {
                if ((double)edgeSize % 2.0 == 0.0)
                    vector3 = new Vector3(0.5f, 0.5f, 0.5f);
                if (height.Value % 2.0 == 0.0)
                    vector3.y = 0.5f;
            }
            else
            {
                vector3.y = (float)(0.5 * (double)height - 0.5);
                if ((double)edgeSize % 2.0 == 0.0)
                    vector3 += new Vector3(0.5f, 0.0f, 0.5f);
            }

            var transform = ___cubeRenderer.transform;
            transform.SetPositionAndRotation(origin + vector3, Quaternion.identity);
            transform.localScale = new Vector3(edgeSize, 0.5f * height.Value, edgeSize);

            var p = transform.position;
            var s = transform.localScale;
            Main.Log($"SquareCylinder: ({p.x},{p.y},{p.z}), ({s.x},{s.y},{s.z})");
        }
    }

    [HarmonyPatch(typeof(GeometricShape), "UpdateCylinderPosition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GeometricShape_UpdateCylinderPosition
    {
        public static void Postfix(MeshRenderer ___cylinderRenderer)
        {
            var transform = ___cylinderRenderer.transform;

            var p = transform.position;
            var s = transform.localScale;
            Main.Log($"Cylinder: ({p.x},{p.y},{p.z}), ({s.x},{s.y},{s.z})");
        }
    }
}
