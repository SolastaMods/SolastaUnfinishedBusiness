// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit;

public enum ToggleState
{
    Off = 0,
    On = 1,
    None = 2
}

internal static partial class UI
{
    [UsedImplicitly]
    public static bool IsOn(this ToggleState state)
    {
        return state == ToggleState.On;
    }

    [UsedImplicitly]
    public static bool IsOff(this ToggleState state)
    {
        return state == ToggleState.Off;
    }

    [UsedImplicitly]
    public static ToggleState Flip(this ToggleState state)
    {
        return state switch
        {
            ToggleState.Off => ToggleState.On,
            ToggleState.On => ToggleState.Off,
            ToggleState.None => ToggleState.None,
            _ => ToggleState.None
        };
    }

    private static bool TogglePrivate(
        string title,
        ref bool value,
        bool isEmpty,
        bool disclosureStyle = false,
        float width = 0,
        params GUILayoutOption[] options)
    {
        options = options.AddDefaults();

        if (width == 0 && !disclosureStyle)
        {
            width = ToggleStyle.CalcSize(new GUIContent(title.Bold())).x +
                    GUI.skin.box.CalcSize(CheckOn).x + 10;
        }

        options = options.AddItem(width == 0 ? AutoWidth() : Width(width)).ToArray();

        if (!disclosureStyle)
        {
            title = value ? title.Bold() : title.MedGrey().Bold();
            if (!CheckBox(title, value, isEmpty, ToggleStyle, options))
            {
                return false;
            }
        }
        else
        {
            if (!DisclosureToggle(title, value, isEmpty, options))
            {
                return false;
            }
        }

        value = !value;

        return true;
    }

    [UsedImplicitly]
    public static void ToggleButton(ref ToggleState toggle, string title, params GUILayoutOption[] options)
    {
        var isOn = toggle.IsOn();
        var isEmpty = toggle == ToggleState.None;

        if (TogglePrivate(title, ref isOn, isEmpty, true, 0, options))
        {
            toggle = toggle.Flip();
        }
    }

    [UsedImplicitly]
    public static bool Toggle(string title, ref bool value, params GUILayoutOption[] options)
    {
        options = options.AddDefaults();

        if (!CheckBox(title, value, false, ToggleStyle, options))
        {
            return false;
        }

        value = !value;

        return true;
    }

    [UsedImplicitly]
    public static bool DisclosureToggle(string title, ref bool value, float width = 175, params Action[] actions)
    {
        var changed = TogglePrivate(title, ref value, false, true, width);

        If(value, actions);

        return changed;
    }

    [UsedImplicitly]
    public static void ToggleButton(ref ToggleState toggle, string title, Action<ToggleState> applyToChildren,
        params GUILayoutOption[] options)
    {
        var isOn = toggle.IsOn();
        var isEmpty = toggle == ToggleState.None;
        var state = toggle;
        if (TogglePrivate("", ref isOn, isEmpty, true, 0, options))
        {
            state = state.Flip();
        }

        Space((float)-10);
        if (state == ToggleState.None)
        {
            Space(35f);
        }
        else
        {
            var deepTitle = state switch
            {
                ToggleState.On => "≪",
                ToggleState.Off => "≫",
                _ => ""
            };
            ActionButton(deepTitle, () =>
            {
                state = state.Flip();
                applyToChildren(state);
            }, ToggleStyle, Width(35f));
        }

        Label(title, ToggleStyle);
        toggle = state;
    }

    [UsedImplicitly]
    public static bool Toggle(string title, ref bool value, string on, string off, float width = 0,
        GUIStyle stateStyle = null, GUIStyle labelStyle = null, params GUILayoutOption[] options)
    {
        stateStyle ??= GUI.skin.box;

        labelStyle ??= GUI.skin.box;

        if (width == 0)
        {
            width = ToggleStyle.CalcSize(new GUIContent(title.Bold())).x +
                    GUI.skin.box.CalcSize(Utility.Private.UI.CheckOn).x +
                    10;
        }

        options = options.AddItem(width == 0 ? AutoWidth() : Width(width)).ToArray();
        title = value ? title.Bold() : title.MedGrey().Bold();

        if (!Utility.Private.UI.Toggle(title, value, on, off, stateStyle, labelStyle, options))
        {
            return false;
        }

        value = !value;

        return true;
    }
#if false
    public static bool Toggle(string title, ref bool value, params GUILayoutOption[] options)
    {
        options = options.AddDefaults();
        var changed = false;
        if (Private.UI.CheckBox(title, value, false, toggleStyle, options)) { value = !value; changed = true; }
        return changed;
    }
#endif

    [UsedImplicitly]
    public static bool ActionToggle(
        string title,
        Func<bool> get,
        Action<bool> set,
        float width = 0,
        params GUILayoutOption[] options)
    {
        var value = get();
        if (TogglePrivate(title, ref value, false, false, width, options))
        {
            set(value);
        }

        return value;
    }

    [UsedImplicitly]
    public static bool ActionToggle(
        string title,
        Func<bool> get,
        Action<bool> set,
        Func<bool> isEmpty,
        float width = 0,
        params GUILayoutOption[] options)
    {
        var value = get();
        var empty = isEmpty();
        if (!TogglePrivate(title, ref value, empty, false, width, options))
        {
            return value;
        }

        if (!empty)
        {
            set(value);
        }

        return value;
    }

    [UsedImplicitly]
    public static bool ToggleCallback(
        string title,
        ref bool value,
        Action<bool> callback,
        float width = 0,
        params GUILayoutOption[] options)
    {
        var result = TogglePrivate(title, ref value, false, false, width, options);
        if (result)
        {
            callback(value);
        }

        return result;
    }

    [UsedImplicitly]
    public static bool BitFieldToggle(
        string title,
        ref int bitfield,
        int offset,
        float width = 0,
        params GUILayoutOption[] options)
    {
        var bit = ((1 << offset) & bitfield) != 0;
        var newBit = bit;
        TogglePrivate(title, ref newBit, false, false, width, options);
        if (bit != newBit) { bitfield ^= 1 << offset; }

        return bit != newBit;
    }
#if false
    public static bool DisclosureToggle(string title, ref bool value, float width = 175, params Action[] actions)
    {
        var changed = TogglePrivate(title, ref value, false, true, width);
        If(value, actions);
        return changed;
    }

#endif

    [UsedImplicitly]
    public static bool DisclosureToggle(string title, ref bool value, params Action[] actions)
    {
        var changed = TogglePrivate(title, ref value, false, true, 175);
        If(value, actions);
        return changed;
    }

    [UsedImplicitly]
    public static bool DisclosureBitFieldToggle(string title, ref int bitfield, int offset, bool exclusive = true,
        float width = 175, params Action[] actions)
    {
        var bit = ((1 << offset) & bitfield) != 0;
        var newBit = bit;
        TogglePrivate(title, ref newBit, false, true, width);
        if (bit != newBit)
        {
            if (exclusive)
            {
                bitfield = newBit ? 1 << offset : 0;
            }
            else
            {
                bitfield ^= 1 << offset;
            }
        }

        If(newBit, actions);
        return bit != newBit;
    }
}
