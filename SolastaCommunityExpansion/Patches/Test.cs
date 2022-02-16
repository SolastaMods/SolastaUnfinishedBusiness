using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(CursorLocationGeometricShape), "UpdateGeometricShape")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CursorLocationGeometricShape_UpdateGeometricShape
    {
        internal static int? TargetParameter2 { get;set; }

        public static void Prefix(int ___targetParameter2)
        {
            TargetParameter2 = ___targetParameter2;
        }

        public static void Postfix()
        {
            TargetParameter2 = null;
        }
    }    
    
    [HarmonyPatch(typeof(GeometricShape), "UpdateCubePosition_Regular")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GeometricShape_UpdateCubePosition_Regular
    {
        public static void Postfix(MeshRenderer ___cubeRenderer)
        {
            if (CursorLocationGeometricShape_UpdateGeometricShape.TargetParameter2.HasValue)
            {
                Main.Log($"GeometricShape_UpdateCubePosition_Regular - setting z value: {CursorLocationGeometricShape_UpdateGeometricShape.TargetParameter2.Value}");
                var ls = ___cubeRenderer.transform.localScale;
                ___cubeRenderer.transform.localScale = new Vector3(ls.x, ls.y, CursorLocationGeometricShape_UpdateGeometricShape.TargetParameter2.Value);
            }
        }
    }
}
