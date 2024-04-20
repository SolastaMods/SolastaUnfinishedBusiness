using System;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static class GUIHelper
{
    private static Texture2D _fillTexture;
    private static readonly Color FillColor = new(1f, 1f, 1f, 0.65f);

    internal static bool AdjusterButton(ref int value, string text, int min = int.MinValue, int max = int.MaxValue)
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

    internal static void TextField(ref string value, Action onChanged, GUIStyle style = null,
        params GUILayoutOption[] options)
    {
        var old = value;

        value = GUILayout.TextField(value, style ?? GUI.skin.textField, options);

        if (value == old)
        {
            return;
        }

        onChanged();
    }

    internal static void SelectionGrid(ref int selected, string[] texts, int xCount, Action onChanged,
        GUIStyle style = null, params GUILayoutOption[] options)
    {
        var old = selected;
        selected = GUILayout.SelectionGrid(selected, texts, xCount, style ?? GUI.skin.button, options);
        if (selected != old)
        {
            onChanged?.Invoke();
        }
    }

    internal static void Div(float indent = 0, float height = 25, float width = 0)
    {
        if (!_fillTexture)
        {
            _fillTexture = new Texture2D(1, 1);
        }

        var divStyle = new GUIStyle { fixedHeight = 1 };
        _fillTexture.SetPixel(0, 0, FillColor);
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
}
