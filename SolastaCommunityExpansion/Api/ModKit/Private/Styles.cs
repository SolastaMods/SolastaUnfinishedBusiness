using UnityEngine;
using UnityModManagerNet;

namespace ModKit;

public static partial class UI
{
    private static Texture2D _fillTexture;

    private static readonly Color FillColor = new(1f, 1f, 1f, 0.65f);

    private static GUIStyle _toggleStyle;

    private static GUIStyle _divStyle;

    private static GUIStyle ToggleStyle =>
        _toggleStyle ??= new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleLeft};

    private static int Point(this int x)
    {
        return UnityModManager.UI.Scale(x);
    }

    private static void Div(Color color, float indent = 0, float height = 0, float width = 0)
    {
        if (_fillTexture == null)
        {
            _fillTexture = new Texture2D(1, 1);
        }

        _divStyle = new GUIStyle {fixedHeight = 1};
        _fillTexture.SetPixel(0, 0, color);
        _fillTexture.Apply();
        _divStyle.normal.background = _fillTexture;
        if (_divStyle.margin == null)
        {
            _divStyle.margin = new RectOffset((int)indent, 0, 4, 4);
        }
        else
        {
            _divStyle.margin.left = (int)indent + 3;
        }

        if (width > 0)
        {
            _divStyle.fixedWidth = width;
        }
        else
        {
            _divStyle.fixedWidth = 0;
        }

        Space(2f * height / 3f);
        GUILayout.Box(GUIContent.none, _divStyle);
        Space(height / 3f);
    }
}
