// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace ModKit;

public static partial class UI
{
    // GUILayout wrappers and extensions so other modules can use UI.MethodName()
    public static GUILayoutOption ExpandWidth(bool v)
    {
        return GL.ExpandWidth(v);
    }

    public static GUILayoutOption ExpandHeight(bool v)
    {
        return GL.ExpandHeight(v);
    }

    public static GUILayoutOption AutoWidth()
    {
        return GL.ExpandWidth(false);
    }

    public static GUILayoutOption AutoHeight()
    {
        return GL.ExpandHeight(false);
    }

    public static GUILayoutOption Width(float v)
    {
        return GL.Width(v);
    }

    public static GUILayoutOption Height(float v)
    {
        return GL.Height(v);
    }

    public static GUILayoutOption MaxWidth(float v)
    {
        return GL.MaxWidth(v);
    }

    public static GUILayoutOption MaxHeight(float v)
    {
        return GL.MaxHeight(v);
    }

    public static GUILayoutOption MinWidth(float v)
    {
        return GL.MinWidth(v);
    }

    public static GUILayoutOption MinHeight(float v)
    {
        return GL.MinHeight(v);
    }

    public static void Space(float size = 150f)
    {
        GL.Space(size);
    }

    public static void BeginHorizontal(params GUILayoutOption[] options)
    {
        GL.BeginHorizontal(options);
    }

    public static void EndHorizontal()
    {
        GL.EndHorizontal();
    }

    public static GUILayout.HorizontalScope HorizontalScope(params GUILayoutOption[] options)
    {
        return new GUILayout.HorizontalScope(options);
    }

    public static GUILayout.VerticalScope VerticalScope(params GUILayoutOption[] options)
    {
        return new GUILayout.VerticalScope(options);
    }

    public static void BeginVertical(params GUILayoutOption[] options)
    {
        GL.BeginHorizontal(options);
    }

    public static void EndVertical()
    {
        GL.BeginHorizontal();
    }
}
