using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine.InputSystem;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GuiGamepadSelectorPatcher
{
    //Is it better to use marker component instead of modifying name?
    private const string GlobalMarker = "|@Global";

    private static bool IsGlobal(this GuiGamepadSelector selector)
    {
        return selector.name.Contains(GlobalMarker);
    }

    public static void MarkGlobal(this GuiGamepadSelector selector)
    {
        if (selector.IsGlobal())
        {
            return;
        }

        selector.name += GlobalMarker;
    }

    private static void TryShowBindingForGlobal(this GuiGamepadSelector selector)
    {
        if (selector.IsGlobal())
        {
            selector.bindingImage.gameObject.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(GuiGamepadSelector), nameof(GuiGamepadSelector.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiGamepadSelector __instance)
        {
            __instance.TryShowBindingForGlobal();
        }
    }

    [HarmonyPatch(typeof(GuiGamepadSelector), nameof(GuiGamepadSelector.OnEnable))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnEnable_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiGamepadSelector __instance)
        {
            __instance.TryShowBindingForGlobal();
        }
    }

    [HarmonyPatch(typeof(GuiGamepadSelector), nameof(GuiGamepadSelector.OnSelect))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnSelect_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiGamepadSelector __instance)
        {
            __instance.TryShowBindingForGlobal();
        }
    }

    [HarmonyPatch(typeof(GuiGamepadSelector), nameof(GuiGamepadSelector.OnDeselect))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnDeselect_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiGamepadSelector __instance)
        {
            __instance.TryShowBindingForGlobal();
        }
    }

    [HarmonyPatch(typeof(GuiGamepadSelector), nameof(GuiGamepadSelector.PressPerformed))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class PressPerformed_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] GuiGamepadSelector __instance, InputAction.CallbackContext context)
        {
            //PATCH: completely replace method to allow reacting to gamepad input even if not focused
            
            if (!__instance.IsInteractable())
            {
                return false;
            }

            if (!(Gui.InputService.CurrentSelectedGameObject == __instance.gameObject) && !__instance.IsGlobal())
            {
                return false;
            }

            if (__instance.ItemsNumber <= 0)
            {
                return false;
            }

            if (context.ReadValue<float>() < 0.0)
            {
                __instance.currentSelection = (__instance.currentSelection + (__instance.ItemsNumber - 1)) %
                                              __instance.ItemsNumber;
                __instance.RefreshCurrent();
                __instance.SelectionChanged?.Invoke();
            }
            else if (context.ReadValue<float>() > 0.0)
            {
                __instance.currentSelection = (__instance.currentSelection + 1) % __instance.ItemsNumber;
                __instance.RefreshCurrent();
                __instance.SelectionChanged?.Invoke();
            }

            ServiceRepository.GetService<IAudioService>()?.PostGuiEvent(__instance.onChangeValueEvent);
            __instance.bindingModifier.StartAnimation(true);

            return false;
        }
    }
}
