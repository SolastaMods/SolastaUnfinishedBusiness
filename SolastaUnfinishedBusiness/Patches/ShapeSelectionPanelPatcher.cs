using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ShapeSelectionPanelPatcher
{
    //PATCH: allows shapes to be displayed regardless of the campaign max level
    [HarmonyPatch(typeof(ShapeSelectionPanel), nameof(ShapeSelectionPanel.Show))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Show_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            ShapeSelectionPanel __instance,
            BaseDefinition sourceDefinition,
            ShapeChangeForm shapeChangeForm,
            RulesetCharacter shifter,
            ShapeSelectionPanel.ShapeSelectionValidatedHandler shapeSelectionValidated,
            ShapeSelectionPanel.ShapeSelectionCancelledHandler shapeSelectionCancelled)
        {
            __instance.gameObject.SetActive(true);
            __instance.shapeOptions = shapeChangeForm.ShapeOptions;
            __instance.ShapeSelectionValidated = shapeSelectionValidated;
            __instance.ShapeSelectionCancelled = shapeSelectionCancelled;
            __instance.shapeOptionItems.Clear();
            __instance.title.Text = Gui.Localize(sourceDefinition.GuiPresentation.Title);

            while (__instance.shapeOptionTable.childCount < __instance.shapeOptions.Count)
            {
                Gui.GetPrefabFromPool(__instance.shapeOptionItemPrefab, __instance.shapeOptionTable)
                    .SetActive(false);
            }

            var index = 0;

            foreach (var shapeOption in __instance.shapeOptions)
            {
                var component = __instance.shapeOptionTable.GetChild(index).GetComponent<ShapeOptionItem>();
                var requiredLevel = shapeChangeForm.ShapeChangeType == ShapeChangeForm.Type.ClassLevelListSelection
                    ? shapeOption.RequiredLevel
                    : 0;

                component.Bind(
                    index, 0.3f, 0.1f,
                    shapeOption.SubstituteMonster,
                    shifter,
                    __instance.OnShapeSelected,
                    __instance.OnShapeDoubleClicked,
                    requiredLevel);

                component.Animate();

                __instance.shapeOptionItems.Add(component);
                component.gameObject.SetActive(true);

                ++index;
            }

            __instance.validateButton.interactable = false;
            __instance.selectedShape = -1;

            LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.shapeOptionTable);

            var x = __instance.shapeOptionTable.sizeDelta.x;

            __instance.RectTransform.sizeDelta = new Vector2(x + 40f,
                __instance.RectTransform.sizeDelta.y);
            __instance.modifierSize.StartSize = 0.0f;
            __instance.modifierSize.EndSize = x + 40;
            __instance.modifierSize.ResetModifier(true);
            __instance.modifierSize.Duration = 0.3f;
            __instance.gameObject.SetActive(false);

            if (Gui.GamepadActive)
            {
                __instance.OnShapeSelected(0);
            }

            __instance.Show();

            return false;
        }

        [UsedImplicitly]
        public static void Postfix(ShapeSelectionPanel __instance)
        {
            //PATCH: shrink the shape selection panel if too many shapes offered
            var rect = __instance.RectTransform;
            var shapesCount = __instance.shapeOptionTable.childCount;

            rect.localScale = shapesCount > 10 ? new Vector3(0.8f, 0.8f, 0.8f) : Vector3.one;
        }
    }
}
