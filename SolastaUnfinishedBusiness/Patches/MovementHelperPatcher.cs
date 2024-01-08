using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class MovementHelperPatcher
{
    //PATCH: supports tweak the movement grid
    [HarmonyPatch(typeof(MovementHelper), nameof(MovementHelper.DrawMovementGrid))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class DrawMovementGrid_Patch
    {
        [UsedImplicitly]
        public static void Prefix(MovementHelper __instance)
        {
            foreach (var lineRenderer in __instance.movementGridRenderers)
            {
                lineRenderer.startWidth = 0.05f * Main.Settings.MovementGridWidthModifier / 100.0f;
                lineRenderer.endWidth = 0.05f * Main.Settings.MovementGridWidthModifier / 100.0f;
            }
        }
    }

    [HarmonyPatch(typeof(MovementHelper), nameof(MovementHelper.DrawMovementOutlines))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class DrawMovementOutlines_Patch
    {
        [UsedImplicitly]
        public static void Prefix(MovementHelper __instance)
        {
            __instance.movementOutlineWidth = 0.10f * Main.Settings.OutlineGridWidthModifier / 100.0f;
            __instance.movementOutlineSpeed = 0.15f * Main.Settings.OutlineGridWidthSpeed / 100.0f;
        }
    }
}
