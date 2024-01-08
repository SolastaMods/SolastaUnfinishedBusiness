using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

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
        // this is vanilla code
        [UsedImplicitly]
        public static bool Prefix(MovementHelper __instance, bool isHighlighted)
        {
            // BEGIN PATCH
            var width = 0.05f * Main.Settings.MovementGridWidthModifier / 100.0f;
            // END PATCH

            // BEGIN PATCH
            // var color = new Color(1f, 1f, 1f, isHighlighted ? 1f : 0.2f);
            var color = GameUiContext.GetGridColor(isHighlighted);
            // END PATCH

            var height = new Vector3(0.0f, __instance.movementGridShadowHeightOffset, 0.0f);

            for (var index = 0; index < __instance.gridXLines.Count; ++index)
            {
                var (int1, int2) = __instance.gridXLines[index];
                var position1 = (Vector3)int1 + new Vector3(
                    __instance.movementOutlineOffset, __instance.movementGridHeightOffset, 0.0f);
                var position2 = (Vector3)int2 + new Vector3(
                    1f - __instance.movementOutlineOffset, __instance.movementGridHeightOffset, 0.0f);
                var movementGridRenderer = __instance.movementGridRenderers[index];

                movementGridRenderer.positionCount = 2;
                movementGridRenderer.startColor = color;
                movementGridRenderer.endColor = color;

                // BEGIN PATCH
                movementGridRenderer.startWidth = width;
                movementGridRenderer.endWidth = width;
                // END PATCH

                movementGridRenderer.SetPosition(0, position1);
                movementGridRenderer.SetPosition(1, position2);
                movementGridRenderer.material.SetVector(__instance.hoveredPositionId, new Vector4(
                    __instance.HoveredPosition.x, __instance.HoveredPosition.y, __instance.HoveredPosition.z));

                if (!__instance.enableMovementGridShadows)
                {
                    continue;
                }

                var gridShadowRenderer = __instance.movementGridShadowRenderers[index];

                gridShadowRenderer.positionCount = 2;
                gridShadowRenderer.startColor = color;
                gridShadowRenderer.endColor = color;

                // BEGIN PATCH
                gridShadowRenderer.startWidth = width;
                gridShadowRenderer.endWidth = width;
                // END PATCH

                gridShadowRenderer.SetPosition(0, position1 + height);
                gridShadowRenderer.SetPosition(1, position2 + height);
                gridShadowRenderer.material.SetVector(__instance.hoveredPositionId, new Vector4(
                    __instance.HoveredPosition.x, __instance.HoveredPosition.y, __instance.HoveredPosition.z));
            }

            for (var index = 0; index < __instance.gridZLines.Count; ++index)
            {
                var (int1, int2) = __instance.gridZLines[index];
                var position3 = (Vector3)int1 + new Vector3(
                    0.0f, __instance.movementGridHeightOffset, __instance.movementOutlineOffset);
                var position4 = (Vector3)int2 + new Vector3(
                    0.0f, __instance.movementGridHeightOffset, 1f - __instance.movementOutlineOffset);
                var movementGridRenderer = __instance.movementGridRenderers[index + __instance.gridXLines.Count];

                movementGridRenderer.positionCount = 2;
                movementGridRenderer.startColor = color;
                movementGridRenderer.endColor = color;

                // BEGIN PATCH
                movementGridRenderer.startWidth = width;
                movementGridRenderer.endWidth = width;
                // END PATCH

                movementGridRenderer.SetPosition(0, position3);
                movementGridRenderer.SetPosition(1, position4);
                movementGridRenderer.material.SetVector(__instance.hoveredPositionId, new Vector4(
                    __instance.HoveredPosition.x, __instance.HoveredPosition.y, __instance.HoveredPosition.z));

                if (!__instance.enableMovementGridShadows)
                {
                    continue;
                }

                var gridShadowRenderer = __instance.movementGridShadowRenderers[index + __instance.gridXLines.Count];

                gridShadowRenderer.positionCount = 2;
                gridShadowRenderer.startColor = color;
                gridShadowRenderer.endColor = color;

                // BEGIN PATCH
                gridShadowRenderer.startWidth = width;
                gridShadowRenderer.endWidth = width;
                // END PATCH

                gridShadowRenderer.SetPosition(0, position3 + height);
                gridShadowRenderer.SetPosition(1, position4 + height);
                gridShadowRenderer.material.SetVector(__instance.hoveredPositionId, new Vector4(
                    __instance.HoveredPosition.x, __instance.HoveredPosition.y, __instance.HoveredPosition.z));
            }

            return false;
        }
    }

    //PATCH: supports tweak the outline grid
    [HarmonyPatch(typeof(MovementHelper), nameof(MovementHelper.DrawMovementOutlines))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class DrawMovementOutlines_Patch
    {
        // this is vanilla code
        [UsedImplicitly]
        public static bool Prefix(MovementHelper __instance, bool isHighlighted)
        {
            // BEGIN PATCH
            var movementGridWidth = 0.05f * Main.Settings.MovementGridWidthModifier / 100.0f;
            // END PATCH

            __instance.movementOutlineWidth = 0.10f * Main.Settings.OutlineGridWidthModifier / 100.0f;
            __instance.movementOutlineSpeed = 0.15f * Main.Settings.OutlineGridWidthSpeed / 100.0f;

            // BEGIN PATCH
            // var color = new Color(1f, 1f, 1f, isHighlighted ? 1f : 0.2f);
            var color = GameUiContext.GetGridColor(isHighlighted);
            // END PATCH

            for (var index = 0; index < __instance.movementOutlines.Count; ++index)
            {
                var movementOutline = __instance.movementOutlines[index];

                if (movementOutline.Empty())
                {
                    break;
                }

                // BEGIN PATCH
                // divide movementOutlineBaseHeight by 1.5 so outline grid aligns with movement one
                var vector3 = new Vector3(0.0f, __instance.movementOutlineBaseHeight / 1.5f, 0.0f);
                // END PATCH

                var outlineIndex = index * __instance.movementOutlineCount;
                var movementOutlineRenderer = __instance.movementOutlineRenderers[outlineIndex];

                movementOutlineRenderer.positionCount = movementOutline.Count;
                movementOutlineRenderer.startColor = color;
                movementOutlineRenderer.endColor = color;

                // BEGIN PATCH
                // force the lines that are an extension of the movement grid to same width as it
                movementOutlineRenderer.startWidth = movementGridWidth;
                movementOutlineRenderer.endWidth = movementGridWidth;
                // END PATCH

                movementOutlineRenderer.material.SetVector(__instance.hoveredPositionId,
                    new Vector4(
                        __instance.HoveredPosition.x, __instance.HoveredPosition.y, __instance.HoveredPosition.z));

                for (var i = 0; i < movementOutline.Count; ++i)
                {
                    movementOutlineRenderer.SetPosition(i, movementOutline[i] + vector3);
                }

                var outlineZFailRenderer = __instance.movementOutlineZFailRenderers[outlineIndex];

                outlineZFailRenderer.positionCount = movementOutline.Count;
                outlineZFailRenderer.startColor = color;
                outlineZFailRenderer.endColor = color;
                outlineZFailRenderer.startWidth = __instance.movementOutlineWidth;
                outlineZFailRenderer.endWidth = __instance.movementOutlineWidth;
                outlineZFailRenderer.material.SetVector(
                    __instance.hoveredPositionId,
                    new Vector4(
                        __instance.HoveredPosition.x, __instance.HoveredPosition.y, __instance.HoveredPosition.z));

                for (var i = 0; i < movementOutline.Count; ++i)
                {
                    outlineZFailRenderer.SetPosition(i, movementOutline[i] + vector3);
                }

                for (var i = 1; i < __instance.movementOutlineCount; ++i)
                {
                    ++outlineIndex;

                    var t = (float)(((i / (double)(__instance.movementOutlineCount - 1)) +
                                     (__instance.movementOutlineSpeed * (double)Time.timeSinceLevelLoad)) % 1.0);

                    vector3 = new Vector3(0.0f,
                        Mathf.Lerp(
                            __instance.movementOutlineBaseHeight,
                            __instance.movementOutlineBaseHeight + __instance.movementOutlineHeight, t), 0.0f);

                    var finalColor = color;

                    finalColor.a *= 1f - t;

                    movementOutlineRenderer = __instance.movementOutlineRenderers[outlineIndex];

                    movementOutlineRenderer.positionCount = movementOutline.Count;
                    movementOutlineRenderer.startColor = finalColor;
                    movementOutlineRenderer.endColor = finalColor;
                    movementOutlineRenderer.startWidth = __instance.movementOutlineWidth;
                    movementOutlineRenderer.endWidth = __instance.movementOutlineWidth;
                    movementOutlineRenderer.material.SetVector(
                        __instance.hoveredPositionId,
                        new Vector4(
                            __instance.HoveredPosition.x, __instance.HoveredPosition.y, __instance.HoveredPosition.z));

                    for (var j = 0; j < movementOutline.Count; ++j)
                    {
                        movementOutlineRenderer.SetPosition(j, movementOutline[j] + vector3);
                    }

                    outlineZFailRenderer = __instance.movementOutlineZFailRenderers[outlineIndex];

                    outlineZFailRenderer.positionCount = movementOutline.Count;
                    outlineZFailRenderer.startColor = finalColor;
                    outlineZFailRenderer.endColor = finalColor;
                    outlineZFailRenderer.startWidth = __instance.movementOutlineWidth;
                    outlineZFailRenderer.endWidth = __instance.movementOutlineWidth;
                    outlineZFailRenderer.material.SetVector(
                        __instance.hoveredPositionId,
                        new Vector4(
                            __instance.HoveredPosition.x, __instance.HoveredPosition.y, __instance.HoveredPosition.z));

                    for (var j = 0; j < movementOutline.Count; ++j)
                    {
                        outlineZFailRenderer.SetPosition(j, movementOutline[j] + vector3);
                    }
                }
            }

            return false;
        }
    }
}
