// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.Infrastructure;
using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace ModKit;

public static partial class UI
{
    private const int SliderTop = 3;
    private const int SliderBottom = -7;

    private static readonly HashSet<Type> WidthTypes = new()
    {
        Width(0).GetType(), MinWidth(0).GetType(), MaxWidth(0).GetType(), AutoWidth().GetType()
    };

    private static GUILayoutOption[] AddDefaults(this GUILayoutOption[] options, params GUILayoutOption[] desired)
    {
        if (options.Any(option => WidthTypes.Contains(option.GetType())))
        {
            return options;
        }

        options = desired.Length > 0 ? options.Concat(desired).ToArray() : options.Append(AutoWidth()).ToArray();

        return options;
    }

    // Labels

    public static void Label(string title, params GUILayoutOption[] options)
    {
        GL.Label(title, options.AddDefaults());
    }

    // Text Fields

    public static string TextField(ref string text, string name = null, params GUILayoutOption[] options)
    {
        if (name != null) { GUI.SetNextControlName(name); }

        text = GL.TextField(text, options.AddDefaults());
        return text;
    }

    public static float FloatTextField(ref float value, string name = null, params GUILayoutOption[] options)
    {
        var text = $"{value}";
        TextField(ref text, name, options);
        if (float.TryParse(text, out var val))
        {
            value = val;
        }

        return value;
    }

    // Action Text Fields

    public static void ActionTextField(ref string text,
        string name,
        Action<string> action,
        Action enterAction,
        params GUILayoutOption[] options
    )
    {
        if (name != null)
        {
            GUI.SetNextControlName(name);
        }

        var newText = GL.TextField(text, options.AddDefaults());
        if (newText != text)
        {
            text = newText;
            action?.Invoke(text);
        }

        if (name != null && enterAction != null && UserHasHitReturn && FocusedControlName == name)
        {
            enterAction();
        }
    }

    // Action Buttons

    public static void ActionButton(string title, Action action, params GUILayoutOption[] options)
    {
        if (GL.Button(title, options.AddDefaults())) { action(); }
    }

    // Sliders
    public static bool Slider(string title, ref float value, float min, float max, float defaultValue = 1.0f,
        int decimals = 0, string units = "", params GUILayoutOption[] options)
    {
        value = Math.Max(min, Math.Min(max, value)); // clamp it
        var newValue = value;
        using (HorizontalScope(options))
        {
            using (VerticalScope(Width(300)))
            {
                Space((SliderTop - 1).Point());
                Label(title.Cyan(), Width(300));
                Space(SliderBottom.Point());
            }

            Space(25);
            using (VerticalScope(Width(200)))
            {
                Space((SliderTop + 4).Point());
                newValue = (float)Math.Round(GL.HorizontalSlider(value, min, max, Width(200)), decimals);
                Space(SliderBottom.Point());
            }

            Space(25);
            using (VerticalScope(Width(75)))
            {
                Space((SliderTop + 2).Point());
                FloatTextField(ref newValue, null, Width(75));
                Space(SliderBottom.Point());
            }

            if (units.Length > 0)
            {
                Label($"{units}".Orange().Bold(), Width(25 + GUI.skin.label.CalcSize(new GUIContent(units)).x));
            }

            Space(25);
            using (VerticalScope(AutoWidth()))
            {
                Space((SliderTop - 0).Point());
                ActionButton("Reset", () => { newValue = defaultValue; }, AutoWidth());
                Space(SliderBottom.Point());
            }
        }

        var changed = value != newValue;
        value = Math.Min(max, Math.Max(min, newValue));
        value = newValue;
        return changed;
    }

    public static bool Slider(string title, ref int value, int min, int max, int defaultValue = 1,
        string units = "", params GUILayoutOption[] options)
    {
        float fvalue = value;
        var changed = Slider(title, ref fvalue, min, max, defaultValue, 0, units, options);
        value = (int)fvalue;
        return changed;
    }
}
