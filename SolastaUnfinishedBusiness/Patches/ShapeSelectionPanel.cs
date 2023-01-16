using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ShapeSelectionPanelPatcher
{
    [HarmonyPatch(typeof(ShapeSelectionPanel), nameof(ShapeSelectionPanel.OnEndShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnEndShow_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ShapeSelectionPanel __instance)
        {
            //PATCH: shrink the shape selection panel if too many shapes offered
            var rect = __instance.GetComponent<RectTransform>();
            var shapesCount = __instance.shapeOptionTable.childCount;

            rect.localScale = shapesCount > 10 ? new Vector3(0.9f, 0.9f, 0.9f) : Vector3.one;
        }
    }
}
