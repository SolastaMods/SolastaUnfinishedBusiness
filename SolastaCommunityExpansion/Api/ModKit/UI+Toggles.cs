// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Infrastructure;
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
    public static bool IsOn(this ToggleState state)
    {
        return state == ToggleState.On;
    }

    public static bool IsOff(this ToggleState state)
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
                    GUI.skin.box.CalcSize(SolastaCommunityExpansion.Api.ModKit.Private.UI.CheckOn).x + 10;
        }

        options = options.AddItem(width == 0 ? AutoWidth() : Width(width)).ToArray();

        if (!disclosureStyle)
        {
            title = value ? title.Bold() : title.MedGrey().Bold();
            if (!SolastaCommunityExpansion.Api.ModKit.Private.UI.CheckBox(title, value, isEmpty, ToggleStyle, options))
            {
                return false;
            }

            value = !value;
        }
        else
        {
            if (!SolastaCommunityExpansion.Api.ModKit.Private.UI.DisclosureToggle(title, value, isEmpty, options))
            {
                return false;
            }

            value = !value;
        }

        return true;
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

        if (!SolastaCommunityExpansion.Api.ModKit.Private.UI.CheckBox(title, value, false, ToggleStyle, options))
        {
            return false;
        }

        value = !value;

        return true;
    }

    public static bool DisclosureToggle(string title, ref bool value, float width = 175, params Action[] actions)
    {
        var changed = TogglePrivate(title, ref value, false, true, width);

        If(value, actions);
        return changed;
    }
}
