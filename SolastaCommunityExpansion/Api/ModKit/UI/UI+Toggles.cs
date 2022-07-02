// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace ModKit;

public enum ToggleState
{
    Off = 0,
    On = 1,
    None = 2
}

public static partial class UI
{
    public const string onMark = "<color=green><b>✔</b></color>";
    public const string offMark = "<color=#A0A0A0E0>✖</color>";

    public static bool IsOn(this ToggleState state)
    {
        return state == ToggleState.On;
    }

    public static bool IsOff(this ToggleState state)
    {
        return state == ToggleState.Off;
    }

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
        params GUILayoutOption[] options
    )
    {
        options = options.AddDefaults();
        var changed = false;
        if (width == 0 && !disclosureStyle)
        {
            width = toggleStyle.CalcSize(new GUIContent(title.bold())).x +
                    GUI.skin.box.CalcSize(Private.UI.CheckOn).x + 10;
        }

        options = options.AddItem(width == 0 ? AutoWidth() : Width(width)).ToArray();
        if (!disclosureStyle)
        {
            title = value ? title.bold() : title.color(RGBA.medgrey).bold();
            if (!Private.UI.CheckBox(title, value, isEmpty, toggleStyle, options))
            {
                return changed;
            }

            value = !value;
            changed = true;
        }
        else
        {
            if (!Private.UI.DisclosureToggle(title, value, isEmpty, options))
            {
                return changed;
            }

            value = !value;
            changed = true;
        }

        return changed;
    }

    public static void ToggleButton(ref ToggleState toggle, string title, GUIStyle style = null,
        params GUILayoutOption[] options)
    {
        var isOn = toggle.IsOn();
        var isEmpty = toggle == ToggleState.None;
        if (TogglePrivate(title, ref isOn, isEmpty, true, 0, options))
        {
            toggle = toggle.Flip();
        }
    }

    public static bool Toggle(string title, ref bool value, params GUILayoutOption[] options)
    {
        options = options.AddDefaults();
        var changed = false;
        if (!Private.UI.CheckBox(title, value, false, toggleStyle, options))
        {
            return changed;
        }

        value = !value;
        changed = true;

        return changed;
    }

    public static bool DisclosureToggle(string title, ref bool value, float width = 175, params Action[] actions)
    {
        var changed = TogglePrivate(title, ref value, false, true, width);
        If(value, actions);
        return changed;
    }
}
