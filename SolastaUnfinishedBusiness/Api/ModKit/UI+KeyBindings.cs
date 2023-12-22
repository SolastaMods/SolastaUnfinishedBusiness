using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    private static string _selectedIdentifier;
    private static KeyBind _oldValue;

    [UsedImplicitly]
    public static KeyBind EditKeyBind(string identifier, bool showHint = true, bool allowModifierOnly = false,
        params GUILayoutOption[] options)
    {
        if (Event.current.type == EventType.Layout)
        {
            KeyBindings.OnGUI();
        }

        var keyBind = KeyBindings.GetBinding(identifier);
        var isEditing = identifier == _selectedIdentifier;
        var isEditingOther = _selectedIdentifier != null && identifier != _selectedIdentifier && _oldValue != null;
        var label = keyBind.IsEmpty ? isEditing ? "Cancel" : "Bind" : keyBind.ToString().Orange().Bold();
        showHint = showHint && isEditing;
        var conflicts = keyBind.Conflicts();
        using (VerticalScope(options))
        {
            Space((float)3.Point());
            if (GL.Button(label, HotkeyStyle, AutoWidth()))
            {
                if (isEditing || isEditingOther)
                {
                    KeyBindings.SetBinding(_selectedIdentifier, _oldValue);
                    if (isEditing)
                    {
                        _selectedIdentifier = null;
                        _oldValue = null;
                        return KeyBindings.GetBinding(identifier);
                    }
                }

                _selectedIdentifier = identifier;
                _oldValue = keyBind;
                keyBind = new KeyBind(identifier);
                KeyBindings.SetBinding(identifier, keyBind);
            }

            var enumerable = conflicts as string[] ?? conflicts.ToArray();
            if (enumerable.Length != 0)
            {
                Label("conflicts".Orange().Bold() + "\n" + string.Join("\n", enumerable));
            }

            if (showHint)
            {
                var hint = "";
                if (keyBind.IsEmpty)
                {
                    hint = _oldValue == null ? "set key binding".Green() : "press key".Green();
                }

                Label(hint);
            }
        }

        if (!isEditing || !keyBind.IsEmpty || Event.current == null)
        {
            return keyBind;
        }

        var isCtrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        var isAltDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        var isCmdDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        var isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var keyCode = Event.current.keyCode;
        //Logger.Log($"    {keyCode.ToString()} ctrl:{isCtrlDown} alt:{isAltDown} cmd: {isCmdDown} shift: {isShiftDown}");
        if (keyCode is KeyCode.Escape or KeyCode.Backspace)
        {
            _selectedIdentifier = null;
            _oldValue = null;
            //Logger.Log("   unbound");
            return KeyBindings.GetBinding(identifier);
        }

        switch (Event.current.isKey)
        {
            case true when !keyCode.IsModifier():
                keyBind = new KeyBind(identifier, keyCode, isCtrlDown, isAltDown, isCmdDown, isShiftDown);
                // ReSharper disable once InvocationIsSkipped
                Main.Log($"    currentEvent isKey - bind: {keyBind}");
                KeyBindings.SetBinding(identifier, keyBind);
                _selectedIdentifier = null;
                _oldValue = null;
                Input.ResetInputAxes();
                return keyBind;
            // Allow raw modifier keys as key binds
            case true when keyCode.IsModifier() && allowModifierOnly:
                keyBind = new KeyBind(identifier, keyCode);
                // ReSharper disable once InvocationIsSkipped
                Main.Log($"    currentEvent isKey - bind: {keyBind}");
                KeyBindings.SetBinding(identifier, keyBind);
                _selectedIdentifier = null;
                _oldValue = null;
                Input.ResetInputAxes();
                return keyBind;
        }

        foreach (var mouseButton in AllowedMouseButtons.Where(Input.GetKey))
        {
            keyBind = new KeyBind(identifier, mouseButton, isCtrlDown, isAltDown, isCmdDown, isShiftDown);
            KeyBindings.SetBinding(identifier, keyBind);
            _selectedIdentifier = null;
            _oldValue = null;
            Input.ResetInputAxes();
            return keyBind;
        }

        return keyBind;
    }

    [UsedImplicitly]
    public static void KeyBindPicker(string identifier, string title, float indent = 0, float titleWidth = 0)
    {
        using (HorizontalScope())
        {
            Space(indent);
            Label(title.Bold(), titleWidth == 0 ? ExpandWidth(false) : Width(titleWidth));
            Space((float)25);
            EditKeyBind(identifier);
        }
    }

    [UsedImplicitly]
    public static void ModifierPicker(string identifier, string title, float titleWidth = 0)
    {
        using (HorizontalScope())
        {
            Label(title.Bold(), titleWidth == 0 ? ExpandWidth(false) : Width(titleWidth));
            Space((float)25);
            EditKeyBind(identifier, true, true);
        }
    }

    [UsedImplicitly]
    // One stop shopping for making an instant button that you want to let a player bind to a key in game
    public static void BindableActionButton(string title, params GUILayoutOption[] options)
    {
        if (options.Length == 0) { options = new[] { GL.Width(300) }; }

        var action = KeyBindings.GetAction(title);
        if (GL.Button(title, options)) { action(); }

        EditKeyBind(title, true, false, Width((float)200));
    }

    [UsedImplicitly]
    // Action button designed to live in a collection with a BindableActionButton
    public static void NonBindableActionButton(string title, Action action, params GUILayoutOption[] options)
    {
        if (options.Length == 0) { options = new[] { GL.Width(300) }; }

        if (GL.Button(title, options)) { action(); }

        Space((float)204);
        if (Event.current.type == EventType.Layout)
        {
            KeyBindings.RegisterAction(title, action);
        }
    }
}
