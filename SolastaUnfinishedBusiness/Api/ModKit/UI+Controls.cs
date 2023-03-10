// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
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

    public static void Label(string title = "", params GUILayoutOption[] options)
    {
        // var content = tooltip == null ? new GUIContent(title) : new GUIContent(title, tooltip);
        GL.Label(title, options.AddDefaults());
    }

    public static void Label(string title, GUIStyle style, params GUILayoutOption[] options)
    {
        // var content = tooltip == null ? new GUIContent(title) : new GUIContent(title, tooltip);
        //  if (options.Length == 0) { options = new GUILayoutOption[] { GL.Width(150f) }; }
        GL.Label(title, style, options.AddDefaults());
    }

    public static void Label(GUIContent content, params GUILayoutOption[] options)
    {
        // var content = tooltip == null ? new GUIContent(title) : new GUIContent(title, tooltip);
        //  if (options.Length == 0) { options = new GUILayoutOption[] { GL.Width(150f) }; }
        GL.Label(content, options);
    }

    public static void DescriptiveLabel(string title, string description, params GUILayoutOption[] options)
    {
        options = options.AddDefaults(Width(300));
        using (HorizontalScope())
        {
            Label(title, options);
            Space(25);
            Label(description);
        }
    }

    public static bool EditableLabel(ref string label, ref (string, string) editState, float minWidth, GUIStyle style,
        Func<string, string> formatter = null, params GUILayoutOption[] options)
    {
        var changed = false;
        if (editState.Item1 != label)
        {
            using (HorizontalScope(options.AddDefaults()))
            {
                Label(formatter(label), style, AutoWidth());
                Space(5);
                if (GL.Button("✎", GUI.skin.box, AutoWidth()))
                {
                    editState = (label, label);
                }
            }
        }
        else
        {
            GUI.SetNextControlName(label);
            using (HorizontalScope(options))
            {
                TextField(ref editState.Item2, null, MinWidth(minWidth), AutoWidth());
                Space(15);
                if (GL.Button("✖".Red(), GUI.skin.box, AutoWidth()))
                {
                    editState = (null, null);
                }

                if (!GL.Button("✔".Green(), GUI.skin.box, AutoWidth())
                    && (!UserHasHitReturn || FocusedControlName != label))
                {
                    return changed;
                }

                label = editState.Item2;
                changed = true;
                editState = (null, null);
            }
        }

        return changed;
    }

    public static bool EditableLabel(ref string label, ref (string, string) editState, float minWidth,
        Func<string, string> formatter = null, params GUILayoutOption[] options)
    {
        return EditableLabel(ref label, ref editState, minWidth, GUI.skin.label, formatter, options);
    }

    // Text Fields

    public static string TextField(ref string text, string name = null, params GUILayoutOption[] options)
    {
        if (name != null) { GUI.SetNextControlName(name); }

        text = GL.TextField(text, options.AddDefaults());
        return text;
    }

    public static int IntTextField(ref int value, string name = null, params GUILayoutOption[] options)
    {
        var text = $"{value}";
        TextField(ref text, name, options);
        _ = int.TryParse(text, out value);
        return value;
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

    public static void ActionTextField(ref string text,
        Action<string> action,
        params GUILayoutOption[] options)
    {
        ActionTextField(ref text, null, action, null, options);
    }

    public static void ActionIntTextField(
        ref int value,
        string name,
        Action<int> action,
        Action enterAction,
        int min = 0,
        int max = int.MaxValue,
        params GUILayoutOption[] options
    )
    {
        var changed = false;
        var hitEnter = false;
        var str = $"{value}";
        ActionTextField(ref str,
            name,
            text => { changed = true; },
            () => { hitEnter = true; },
            options);
        _ = int.TryParse(str, out value);
        value = Math.Min(max, Math.Max(value, min));
        if (changed) { action?.Invoke(value); }

        if (hitEnter && enterAction != null) { enterAction(); }
    }

    public static void ActionIntTextField(
        ref int value,
        string name,
        Action<int> action,
        Action enterAction,
        params GUILayoutOption[] options)
    {
        ActionIntTextField(ref value, name, action, enterAction, int.MinValue, int.MaxValue, options);
    }

    public static void ActionIntTextField(
        ref int value,
        Action<int> action,
        params GUILayoutOption[] options)
    {
        ActionIntTextField(ref value, null, action, null, int.MinValue, int.MaxValue, options);
    }

    // Buttons

    public static bool Button(string title, ref bool pressed, params GUILayoutOption[] options)
    {
        if (GL.Button(title, options.AddDefaults())) { pressed = true; }

        return pressed;
    }

    public static bool Button(string title, ref bool pressed, GUIStyle style, params GUILayoutOption[] options)
    {
        if (GL.Button(title, style, options.AddDefaults())) { pressed = true; }

        return pressed;
    }

    // Action Buttons

    public static void ActionButton(string title, Action action, params GUILayoutOption[] options)
    {
        if (GL.Button(title, options.AddDefaults())) { action?.Invoke(); }
    }

    public static void ActionButton(string title, Action action, GUIStyle style, params GUILayoutOption[] options)
    {
        if (GL.Button(title, style, options.AddDefaults())) { action?.Invoke(); }
    }

    public static void DangerousActionButton(string title, string warning, ref bool areYouSureState, Action action,
        params GUILayoutOption[] options)
    {
        using (HorizontalScope())
        {
            var areYouSure = areYouSureState;
            ActionButton(title, () => { areYouSure = !areYouSure; });
            if (areYouSureState)
            {
                Space(25);
                Label("Are you sure?".Yellow());
                Space(25);
                ActionButton("YES".Yellow().Bold(), action);
                Space(10);
                ActionButton("NO".Green(), () => areYouSure = false);
                Space(25);
                Label(warning.Orange());
            }

            areYouSureState = areYouSure;
        }
    }

    // Value Adjusters

    public static bool ValueAdjuster(ref int value, int increment = 1, int min = 0, int max = int.MaxValue)
    {
        var v = value;
        if (v > min)
        {
            ActionButton(" < ", () => { v = Math.Max(v - increment, min); }, TextBoxStyle, AutoWidth());
        }
        else
        {
            Space(-21);
            ActionButton("min ".Cyan(), () => { }, TextBoxStyle, AutoWidth());
        }

        Space(-8);
        var temp = false;
        Button($"{v}".Bold(), ref temp, TextBoxStyle, AutoWidth());
        Space(-8);
        if (v < max)
        {
            ActionButton(" > ", () => { v = Math.Min(v + increment, max); }, TextBoxStyle, AutoWidth());
        }
        else
        {
            ActionButton(" max".Cyan(), () => { }, TextBoxStyle, AutoWidth());
            Space(-27);
        }

        if (v == value)
        {
            return false;
        }

        value = v;
        return true;
    }

    public static bool ValueAdjuster(Func<int> get, Action<int> set, int increment = 1, int min = 0,
        int max = int.MaxValue)
    {
        var value = get();
        var changed = ValueAdjuster(ref value, increment, min, max);
        if (changed)
        {
            set(value);
        }

        return changed;
    }

    public static bool ValueAdjuster(string title, ref int value, int increment = 1, int min = 0,
        int max = int.MaxValue, params GUILayoutOption[] options)
    {
        var changed = false;
        using (HorizontalScope(options))
        {
            Label(title);
            changed = ValueAdjuster(ref value, increment, min, max);
        }

        return changed;
    }

    public static bool ValueAdjuster(string title, Func<int> get, Action<int> set, int increment = 1, int min = 0,
        int max = int.MaxValue)
    {
        var changed = false;
        using (HorizontalScope(Width(400)))
        {
            Label(title.Cyan(), Width(300));
            Space(15);
            var value = get();
            changed = ValueAdjuster(ref value, increment, min, max);
            if (changed)
            {
                set(value);
            }
        }

        return changed;
    }

    public static bool ValueAdjuster(string title, Func<int> get, Action<int> set, int increment = 1, int min = 0,
        int max = int.MaxValue, params GUILayoutOption[] options)
    {
        var changed = false;
        using (HorizontalScope())
        {
            Label(title.Cyan(), options);
            Space(15);
            var value = get();
            changed = ValueAdjuster(ref value, increment, min, max);
            if (changed)
            {
                set(value);
            }
        }

        return changed;
    }

    // Value Editors 

    public static bool ValueEditor(string title, Func<int> get, Action<int> set, ref int increment, int min = 0,
        int max = int.MaxValue, params GUILayoutOption[] options)
    {
        var changed = false;
        var value = get();
        var inc = increment;
        using (HorizontalScope(options))
        {
            Label(title.Cyan(), ExpandWidth(true));
            Space(25);
            var fieldWidth = GUI.skin.textField.CalcSize(new GUIContent(max.ToString())).x;
            if (ValueAdjuster(ref value, inc, min, max))
            {
                set(value);
                changed = true;
            }

            Space(50);
            ActionIntTextField(ref inc, title, v => { }, null, Width(fieldWidth + 25));
            increment = inc;
        }

        return changed;
    }

    // Sliders

    public static bool Slider(ref float value, float min, float max, float defaultValue = 1.0f, int decimals = 0,
        string units = "", params GUILayoutOption[] options)
    {
        value = Math.Max(min, Math.Min(max, value)); // clamp it
        var newValue = (float)Math.Round(GL.HorizontalSlider(value, min, max, Width(200)), decimals);
        using (HorizontalScope(options))
        {
            Space(25);
            FloatTextField(ref newValue, null, Width(75));
            if (units.Length > 0)
            {
                Label($"{units}", Width(25 + GUI.skin.label.CalcSize(new GUIContent(units)).x));
            }

            Space(25);
            ActionButton("Reset", () => { newValue = defaultValue; }, AutoWidth());
        }

        var changed = Math.Abs(value - newValue) > 0.00001;
        value = newValue;
        return changed;
    }

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
                Label($"{units}".Bold(), Width(25 + GUI.skin.label.CalcSize(new GUIContent(units)).x));
            }

            Space(25);
            using (VerticalScope(AutoWidth()))
            {
                Space((SliderTop - 0).Point());
                ActionButton("Reset", () => { newValue = defaultValue; }, AutoWidth());
                Space(SliderBottom.Point());
            }
        }

        var changed = Math.Abs(value - newValue) > 0.00001;
        value = Math.Min(max, Math.Max(min, newValue));
        value = newValue;
        return changed;
    }

    public static bool Slider(string title, Func<float> get, Action<float> set, float min, float max,
        float defaultValue = 1.0f, int decimals = 0, string units = "", params GUILayoutOption[] options)
    {
        var value = get();
        var changed = Slider(title, ref value, min, max, defaultValue, decimals, units, options);
        if (changed)
        {
            set(value);
        }

        return changed;
    }

    public static bool Slider(string title, ref int value, int min, int max, int defaultValue = 1, string units = "",
        params GUILayoutOption[] options)
    {
        float fvalue = value;
        var changed = Slider(title, ref fvalue, min, max, defaultValue, 0, units, options);
        value = (int)fvalue;
        return changed;
    }

    public static bool Slider(string title, Func<int> get, Action<int> set, int min, int max, int defaultValue = 1,
        string units = "", params GUILayoutOption[] options)
    {
        float fvalue = get();
        var changed = Slider(title, ref fvalue, min, max, defaultValue, 0, units, options);
        if (changed)
        {
            set((int)fvalue);
        }

        return changed;
    }

    public static bool Slider(ref int value, int min, int max, int defaultValue = 1, string units = "",
        params GUILayoutOption[] options)
    {
        float fvalue = value;
        var changed = Slider(ref fvalue, min, max, defaultValue, 0, units, options);
        value = (int)fvalue;
        return changed;
    }

    public static bool LogSlider(string title, ref float value, float min, float max, float defaultValue = 1.0f,
        int decimals = 0, string units = "", params GUILayoutOption[] options)
    {
        if (min < 0)
        {
            throw new Exception("LogSlider - min value: {min} must be >= 0");
        }

        BeginHorizontal(options);
        using (VerticalScope(Width(300)))
        {
            Space((SliderTop - 1).Point());
            Label(title.Cyan(), Width(300));
            Space(SliderBottom.Point());
        }

        Space(25);
        value = Math.Max(min, Math.Min(max, value)); // clamp it
        const int OFFSET = 1;
        var places = (int)Math.Max(0, Math.Min(15, decimals + 1.01 - Math.Log10(value + OFFSET)));
        var logMin = 100f * (float)Math.Log10(min + OFFSET);
        var logMax = 100f * (float)Math.Log10(max + OFFSET);
        var logValue = 100f * (float)Math.Log10(value + OFFSET);
        var logNewValue = logValue;
        using (VerticalScope(Width(200)))
        {
            Space((SliderTop + 4).Point());
            logNewValue = GL.HorizontalSlider(logValue, logMin, logMax, Width(200));
            Space(SliderBottom.Point());
        }

        var newValue = (float)Math.Round(Math.Pow(10, logNewValue / 100f) - OFFSET, places);
        Space(25);
        using (VerticalScope(Width(75)))
        {
            Space((SliderTop + 2).Point());
            FloatTextField(ref newValue, null, Width(75));
            Space(SliderBottom.Point());
        }

        if (units.Length > 0)
        {
            Label($"{units}".Bold(), Width(25 + GUI.skin.label.CalcSize(new GUIContent(units)).x));
        }

        Space(25);
        using (VerticalScope(AutoWidth()))
        {
            Space((SliderTop + 0).Point());
            ActionButton("Reset", () => { newValue = defaultValue; }, AutoWidth());
            Space(SliderBottom.Point());
        }

        EndHorizontal();
        var changed = Math.Abs(value - newValue) > 0.00001;
        value = Math.Min(max, Math.Max(min, newValue));
        return changed;
    }

    public static bool LogSlider(string title, Func<float> get, Action<float> set, float min, float max,
        float defaultValue = 1.0f, int decimals = 0, string units = "", params GUILayoutOption[] options)
    {
        var value = get();
        var changed = LogSlider(title, ref value, min, max, defaultValue, decimals, units, options);
        if (changed)
        {
            set(value);
        }

        return changed;
    }
}
