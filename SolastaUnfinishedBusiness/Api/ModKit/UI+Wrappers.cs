// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    // GUILayout wrappers and extensions so other modules can use UI.MethodName()
    internal static GUILayoutOption ExpandWidth(bool v)
    {
        return GL.ExpandWidth(v);
    }

    internal static GUILayoutOption ExpandHeight(bool v)
    {
        return GL.ExpandHeight(v);
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

    internal static GUILayoutOption MaxWidth(float v)
    {
        return GL.MaxWidth(v);
    }

    internal static GUILayoutOption MaxHeight(float v)
    {
        return GL.MaxHeight(v);
    }

    internal static GUILayoutOption MinWidth(float v)
    {
        return GL.MinWidth(v);
    }

    internal static GUILayoutOption MinHeight(float v)
    {
        return GL.MinHeight(v);
    }

    internal static void Space(float size = 150f)
    {
        GL.Space(size);
    }

    internal static void BeginHorizontal(params GUILayoutOption[] options)
    {
        GL.BeginHorizontal(options);
    }

    internal static void EndHorizontal()
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

    internal static void BeginVertical(params GUILayoutOption[] options)
    {
        GL.BeginHorizontal(options);
    }

    internal static void EndVertical()
    {
        GL.BeginHorizontal();
    }
}
