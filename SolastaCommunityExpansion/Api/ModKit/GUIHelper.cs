using System;
using UnityEngine;

namespace SolastaCommunityExpansion.Api.ModKit;

public static class GUIHelper
{
    private static Texture2D _fillTexture;
    private static readonly Color FillColor = new(1f, 1f, 1f, 0.65f);

    public static int AdjusterButton(int value, string text, int min = int.MinValue, int max = int.MaxValue)
    {
        AdjusterButton(ref value, text, min, max);

        return value;
    }

    public static bool AdjusterButton(ref int value, string text, int min = int.MinValue, int max = int.MaxValue)
    {
        var oldValue = value;

        GUILayout.Label(text, GUILayout.ExpandWidth(false));

        if (GUILayout.Button("-", GUILayout.ExpandWidth(false)) && value > min)
        {
            value--;
        }

        GUILayout.Label(value.ToString(), GUILayout.ExpandWidth(false));

        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)) && value < max)
        {
            value++;
        }

        return value != oldValue;
    }

    public static void TextField(ref string value, GUIStyle style = null, params GUILayoutOption[] options)
    {
        value = GUILayout.TextField(value, style ?? GUI.skin.textField, options);
    }

    public static void TextField(ref string value, Action onChanged, GUIStyle style = null,
        params GUILayoutOption[] options)
    {
        TextField(ref value, null, onChanged, style, options);
    }

    public static void TextField(ref string value, Action onClear, Action onChanged, GUIStyle style = null,
        params GUILayoutOption[] options)
    {
        var old = value;
        TextField(ref value, style, options);
        if (value != old)
        {
            if (onClear != null && string.IsNullOrEmpty(value))
            {
                onClear();
            }
            else
            {
                onChanged();
            }
        }
    }

    public static void SelectionGrid(ref int selected, string[] texts, int xCount, GUIStyle style = null,
        params GUILayoutOption[] options)
    {
        selected = GUILayout.SelectionGrid(selected, texts, xCount, style ?? GUI.skin.button, options);
    }

    public static void SelectionGrid(ref int selected, string[] texts, int xCount, Action onChanged,
        GUIStyle style = null, params GUILayoutOption[] options)
    {
        var old = selected;
        SelectionGrid(ref selected, texts, xCount, style, options);
        if (selected != old)
        {
            onChanged?.Invoke();
        }
    }

    private static void Div(Color color, float indent = 0, float height = 0, float width = 0)
    {
        if (_fillTexture == null)
        {
            _fillTexture = new Texture2D(1, 1);
        }

        var divStyle = new GUIStyle {fixedHeight = 1};
        _fillTexture.SetPixel(0, 0, color);
        _fillTexture.Apply();
        divStyle.normal.background = _fillTexture;
        divStyle.margin = new RectOffset((int)indent, 0, 4, 4);
        if (width > 0)
        {
            divStyle.fixedWidth = width;
        }
        else
        {
            divStyle.fixedWidth = 0;
        }

        GUILayout.Space(1f * height / 2f);
        GUILayout.Box(GUIContent.none, divStyle);
        GUILayout.Space(height / 2f);
    }

    public static void Div(float indent = 0, float height = 25, float width = 0)
    {
        Div(FillColor, indent, height, width);
    }
}
