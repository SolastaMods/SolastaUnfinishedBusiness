// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal enum ToggleState
{
    Off = 0,
    On = 1,
    None = 2
}

internal static partial class UI
{
    internal static bool IsOn(this ToggleState state)
    {
        return state == ToggleState.On;
    }

    internal static bool IsOff(this ToggleState state)
    {
        return state == ToggleState.Off;
    }

    private static ToggleState Flip(this ToggleState state)
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

            value = !value;
        }
        else
        {
            if (!DisclosureToggle(title, value, isEmpty, options))
            {
                return false;
            }

            value = !value;
        }

        return true;
    }

    internal static void ToggleButton(ref ToggleState toggle, string title, GUIStyle style = null,
        params GUILayoutOption[] options)
    {
        var isOn = toggle.IsOn();
        var isEmpty = toggle == ToggleState.None;

        if (TogglePrivate(title, ref isOn, isEmpty, true, 0, options))
        {
            toggle = toggle.Flip();
        }
    }

    internal static bool Toggle(string title, ref bool value, params GUILayoutOption[] options)
    {
        options = options.AddDefaults();

        if (!CheckBox(title, value, false, ToggleStyle, options))
        {
            return false;
        }

        value = !value;

        return true;
    }

    internal static bool DisclosureToggle(string title, ref bool value, float width = 175, params Action[] actions)
    {
        var changed = TogglePrivate(title, ref value, false, true, width);

        If(value, actions);
        return changed;
    }
}
