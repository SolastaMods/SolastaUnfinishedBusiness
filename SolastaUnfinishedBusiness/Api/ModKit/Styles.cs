using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    private static Texture2D _fillTexture;
    private static GUIStyle _fillStyle;

    private static GUIStyle _buttonStyle;

    private static GUIStyle _largeStyle;
    private static GUIStyle _textBoxStyle;

    private static GUIStyle _toggleStyle;
    private static GUIStyle _divStyle;

    private static Texture2D _rarityTexture;
    private static GUIStyle _rarityStyle;
    private static GUIStyle _rarityButtonStyle;

    private static Texture2D _submenuTexture;
    private static GUIStyle _submenuButtonStyle;
    private static Color FillColor { get; } = new(1f, 1f, 1f, 0.65f);

    [UsedImplicitly]
    public static GUIStyle ButtonStyle =>
        _buttonStyle ??= new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };

    [UsedImplicitly]
    public static GUIStyle LargeStyle
    {
        get
        {
            _largeStyle ??= new GUIStyle(GUI.skin.box) { richText = true };

            _largeStyle.fixedHeight = 24.Point();
            //_largeStyle.contentOffset = new Vector2(0, -6.point());
            _largeStyle.padding = new RectOffset(0, 0, -3.Point(), 0);
#pragma warning disable CS0618 // Type or member is obsolete
            _largeStyle.clipOffset = new Vector2(0, 3.Point());
#pragma warning restore CS0618 // Type or member is obsolete
            _largeStyle.fontSize = 21.Point();
            _largeStyle.fontStyle = FontStyle.Bold;
            _largeStyle.normal.background = GUI.skin.label.normal.background;

            return _largeStyle;
        }
    }

    [UsedImplicitly]
    public static GUIStyle TextBoxStyle
    {
        get
        {
            _textBoxStyle ??= new GUIStyle(GUI.skin.box) { richText = true };

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

    [UsedImplicitly]
    public static GUIStyle ToggleStyle =>
        _toggleStyle ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };

    [UsedImplicitly]
    public static Texture2D RarityTexture
    {
        get
        {
            if (!_rarityTexture)
            {
                _rarityTexture = new Texture2D(1, 1);
            }

            _rarityTexture.SetPixel(0, 0, RichText.Rgba.Black.Color());
            _rarityTexture.Apply();
            return _rarityTexture;
        }
    }

    [UsedImplicitly]
    public static GUIStyle RarityStyle =>
        _rarityStyle ??= new GUIStyle(GUI.skin.button) { normal = { background = RarityTexture } };

    [UsedImplicitly]
    public static GUIStyle RarityButtonStyle =>
        _rarityButtonStyle ??= new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleLeft, normal = { background = RarityTexture }
        };

    [UsedImplicitly]
    private static Texture2D SubmenuTexture
    {
        get
        {
            if (!_submenuTexture)
            {
                _submenuTexture = new Texture2D(1, 1);
                _submenuTexture.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f, 0.65f));
            }

            _submenuTexture.Apply();
            return _submenuTexture;
        }
    }

    [UsedImplicitly]
    private static GUIStyle SubmenuButtonStyle
    {
        get
        {
            if (_submenuButtonStyle != null)
            {
                return _submenuButtonStyle;
            }

            _submenuButtonStyle = new GUIStyle(GUI.skin.button);
#if false
                {
                    alignment = TextAnchor.MiddleLeft
                };

#endif
            _submenuButtonStyle.normal.background = SubmenuTexture;

            return _submenuButtonStyle;
        }
    }

    [UsedImplicitly]
    private static int Point(this int x)
    {
        return UnityModManager.UI.Scale(x);
    }

    [UsedImplicitly]
    private static GUIStyle FillStyle(Color color)
    {
        if (!_fillTexture)
        {
            _fillTexture = new Texture2D(1, 1);
        }

        _fillStyle ??= new GUIStyle();

        _fillTexture.SetPixel(0, 0, color);
        _fillTexture.Apply();
        _fillStyle.normal.background = _fillTexture;
        return _fillStyle;
    }

    [UsedImplicitly]
    private static void Div(Color color, float indent = 0, float height = 0, float width = 0)
    {
        if (!_fillTexture)
        {
            _fillTexture = new Texture2D(1, 1);
        }

        //if (divStyle == null) {
        _divStyle = new GUIStyle { fixedHeight = 1 };
        //}
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
