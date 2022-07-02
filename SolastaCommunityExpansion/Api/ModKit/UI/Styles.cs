using UnityEngine;
using UnityModManagerNet;

namespace ModKit;

public static partial class UI
{
    private static Texture2D fillTexture;

    private static readonly Color fillColor = new(1f, 1f, 1f, 0.65f);

    private static GUIStyle _textBoxStyle;

    private static GUIStyle _toggleStyle;

    private static GUIStyle _divStyle;

    private static GUIStyle TextBoxStyle
    {
        get
        {
            _textBoxStyle ??= new GUIStyle(GUI.skin.box) {richText = true};

            _textBoxStyle.fontSize = 14.Point();
            _textBoxStyle.fixedHeight = 19.Point();
            _textBoxStyle.margin = new RectOffset(2.Point(), 2.Point(), 1.Point(), 2.Point());
            _textBoxStyle.padding = new RectOffset(2.Point(), 2.Point(), 0.Point(), 0);
            _textBoxStyle.contentOffset = new Vector2(0, 2.Point());
#pragma warning disable CS0618 // Type or member is obsolete
            _textBoxStyle.clipOffset = new Vector2(0, 2.Point());
#pragma warning restore CS0618 // Type or member is obsolete

            return _textBoxStyle;
        }
    }

    private static GUIStyle ToggleStyle =>
        _toggleStyle ??= new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleLeft};

    private static int Point(this int x)
    {
        return UnityModManager.UI.Scale(x);
    }

    private static void Div(Color color, float indent = 0, float height = 0, float width = 0)
    {
        if (fillTexture == null)
        {
            fillTexture = new Texture2D(1, 1);
        }

        _divStyle = new GUIStyle {fixedHeight = 1};
        fillTexture.SetPixel(0, 0, color);
        fillTexture.Apply();
        _divStyle.normal.background = fillTexture;
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
