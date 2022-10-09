// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    // GUILayout wrappers and extensions so other modules can use UI.MethodName()
    private static GUILayoutOption ExpandWidth(bool v)
    {
        return GL.ExpandWidth(v);
    }

    internal static GUILayoutOption AutoWidth()
    {
        return GL.ExpandWidth(false);
    }

    internal static GUILayoutOption AutoHeight()
    {
        return GL.ExpandHeight(false);
    }

    internal static GUILayoutOption Width(float v)
    {
        return GL.Width(v);
    }

    internal static GUILayoutOption Height(float v)
    {
        return GL.Height(v);
    }

    private static GUILayoutOption MaxWidth(float v)
    {
        return GL.MaxWidth(v);
    }

    private static GUILayoutOption MinWidth(float v)
    {
        return GL.MinWidth(v);
    }

    internal static void Space(float size = 150f)
    {
        GL.Space(size);
    }

    private static void BeginHorizontal(params GUILayoutOption[] options)
    {
        GL.BeginHorizontal(options);
    }

    private static void EndHorizontal()
    {
        GL.EndHorizontal();
    }

    internal static GUILayout.HorizontalScope HorizontalScope(params GUILayoutOption[] options)
    {
        return new GUILayout.HorizontalScope(options);
    }

    internal static GUILayout.VerticalScope VerticalScope(params GUILayoutOption[] options)
    {
        return new GUILayout.VerticalScope(options);
    }
}
