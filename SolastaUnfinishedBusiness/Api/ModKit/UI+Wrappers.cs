// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using JetBrains.Annotations;
using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    // GUILayout wrappers and extensions so other modules can use UI.MethodName()
    [UsedImplicitly]
    public static GUILayoutOption ExpandWidth(bool v)
    {
        return GL.ExpandWidth(v);
    }

    [UsedImplicitly]
    public static GUILayoutOption ExpandHeight(bool v)
    {
        return GL.ExpandHeight(v);
    }

    [UsedImplicitly]
    public static GUILayoutOption AutoWidth()
    {
        return GL.ExpandWidth(false);
    }

    [UsedImplicitly]
    public static GUILayoutOption AutoHeight()
    {
        return GL.ExpandHeight(false);
    }

    [UsedImplicitly]
    public static GUILayoutOption Width(float v)
    {
        return GL.Width(v);
    }

    [UsedImplicitly]
    public static GUILayoutOption Width(this int v)
    {
        return GL.Width(v);
    }

    [UsedImplicitly]
    public static GUILayoutOption[] Width(float min, float max)
    {
        return [GL.MinWidth(min), GL.MaxWidth(max)];
    }

    [UsedImplicitly]
    public static GUILayoutOption[] Height(float min, float max)
    {
        return [GL.MinHeight(min), GL.MaxHeight(max)];
    }

    [UsedImplicitly]
    public static GUILayoutOption Height(float v)
    {
        return GL.Height(v);
    }

    [UsedImplicitly]
    public static GUILayoutOption MaxWidth(float v)
    {
        return GL.MaxWidth(v);
    }

    [UsedImplicitly]
    public static GUILayoutOption MaxHeight(float v)
    {
        return GL.MaxHeight(v);
    }

    [UsedImplicitly]
    public static GUILayoutOption MinWidth(float v)
    {
        return GL.MinWidth(v);
    }

    [UsedImplicitly]
    public static GUILayoutOption MinHeight(float v)
    {
        return GL.MinHeight(v);
    }

    [UsedImplicitly]
    public static void Space(float size = 150f)
    {
        GL.Space(size);
    }

    [UsedImplicitly]
    public static void Space(this int size)
    {
        GL.Space(size);
    }

    [UsedImplicitly]
    public static void Indent(int indent, float size = 75f)
    {
        GL.Space(indent * size);
    }

    [UsedImplicitly]
    public static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
    {
        GL.BeginHorizontal(style, options);
    }

    [UsedImplicitly]
    public static void BeginHorizontal(params GUILayoutOption[] options)
    {
        GL.BeginHorizontal(options);
    }

    [UsedImplicitly]
    public static void EndHorizontal()
    {
        GL.EndHorizontal();
    }

    [UsedImplicitly]
    // ReSharper disable once UseSymbolAlias
    public static GUILayout.AreaScope AreaScope(Rect screenRect)
    {
        // ReSharper disable once UseSymbolAlias
        return new GUILayout.AreaScope(screenRect);
    }

    [UsedImplicitly]
    // ReSharper disable once UseSymbolAlias
    public static GUILayout.AreaScope AreaScope(Rect screenRect, string text)
    {
        // ReSharper disable once UseSymbolAlias
        return new GUILayout.AreaScope(screenRect, text);
    }

    [UsedImplicitly]
    // ReSharper disable once UseSymbolAlias
    public static GUILayout.HorizontalScope HorizontalScope(params GUILayoutOption[] options)
    {
        // ReSharper disable once UseSymbolAlias
        return new GUILayout.HorizontalScope(options);
    }

    [UsedImplicitly]
    // ReSharper disable once UseSymbolAlias
    public static GUILayout.HorizontalScope HorizontalScope(float width)
    {
        // ReSharper disable once UseSymbolAlias
        return new GUILayout.HorizontalScope(Width(width));
    }

    [UsedImplicitly]
    // ReSharper disable once UseSymbolAlias
    public static GUILayout.HorizontalScope HorizontalScope(GUIStyle style, params GUILayoutOption[] options)
    {
        // ReSharper disable once UseSymbolAlias
        return new GUILayout.HorizontalScope(style, options);
    }

    [UsedImplicitly]
    // ReSharper disable once UseSymbolAlias
    public static GUILayout.HorizontalScope HorizontalScope(GUIStyle style, float width)
    {
        // ReSharper disable once UseSymbolAlias
        return new GUILayout.HorizontalScope(style, Width(width));
    }

    [UsedImplicitly]
    // ReSharper disable once UseSymbolAlias
    public static GUILayout.VerticalScope VerticalScope(params GUILayoutOption[] options)
    {
        // ReSharper disable once UseSymbolAlias
        return new GUILayout.VerticalScope(options);
    }

    [UsedImplicitly]
    // ReSharper disable once UseSymbolAlias
    public static GUILayout.VerticalScope VerticalScope(GUIStyle style, params GUILayoutOption[] options)
    {
        // ReSharper disable once UseSymbolAlias
        return new GUILayout.VerticalScope(style, options);
    }

    [UsedImplicitly]
    // ReSharper disable once UseSymbolAlias
    public static GUILayout.ScrollViewScope ScrollViewScope(Vector2 scrollPosition, params GUILayoutOption[] options)
    {
        // ReSharper disable once UseSymbolAlias
        return new GUILayout.ScrollViewScope(scrollPosition, options);
    }

    [UsedImplicitly]
    // ReSharper disable once UseSymbolAlias
    public static GUILayout.ScrollViewScope ScrollViewScope(
        Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
    {
        // ReSharper disable once UseSymbolAlias
        return new GUILayout.ScrollViewScope(scrollPosition, style, options);
    }

    [UsedImplicitly]
    public static void BeginVertical(params GUILayoutOption[] options)
    {
        GL.BeginVertical(options);
    }

    [UsedImplicitly]
    public static void BeginVertical(GUIStyle style, params GUILayoutOption[] options)
    {
        GL.BeginVertical(style, options);
    }

    [UsedImplicitly]
    public static void EndVertical()
    {
        GL.EndVertical();
    }
}
