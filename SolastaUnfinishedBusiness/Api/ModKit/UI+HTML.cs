// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    private static GUIStyle _linkStyle;

    public static bool LinkButton(string title, string url, Action action = null, params GUILayoutOption[] options)
    {
        if (options.Length == 0) { options = new[] { AutoWidth() }; }

        if (_linkStyle == null)
        {
            _linkStyle = new GUIStyle(GUI.skin.toggle)
            {
                wordWrap = false, //linkStyle.normal.background = RarityTexture;
                // Match selection color which works nicely for both light and dark skins
                padding = new RectOffset(-3.Point(), 0, 0, 0)
            };
#pragma warning disable CS0618 // Type or member is obsolete
            _linkStyle.clipOffset = new Vector2(3.Point(), 0);
#pragma warning restore CS0618 // Type or member is obsolete
            _linkStyle.normal.textColor = new Color(0f, 0.75f, 1f);
            _linkStyle.stretchWidth = false;
        }

        bool result;
        Rect rect;
        using (HorizontalScope())
        {
            Space((float)4.Point());
            result = GL.Button(title, _linkStyle, options);
            rect = GUILayoutUtility.GetLastRect();
        }

        Div(_linkStyle.normal.textColor, 0, 0, rect.width + 4.Point());
        if (!result)
        {
            return false;
        }

        Application.OpenURL(url);
        action?.Invoke();

        return true;
    }
}
