// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit.Utility;
using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    // UI for picking items from a collection
    [UsedImplicitly]
    public static void Toolbar(ref int value, string[] texts, params GUILayoutOption[] options)
    {
        value = GL.Toolbar(value, texts, options);
    }

    [UsedImplicitly]
    public static void Toolbar(ref int value, string[] texts, GUIStyle style, params GUILayoutOption[] options)
    {
        value = GL.Toolbar(value, texts, style, options);
    }

    [UsedImplicitly]
    public static bool SelectionGrid(ref int selected, string[] texts, int xCols, params GUILayoutOption[] options)
    {
        if (xCols <= 0)
        {
            xCols = texts.Length;
        }

        if (IsNarrow)
        {
            xCols = Math.Min(4, xCols);
        }

        var sel = selected;
        var titles = texts.Select((a, i) => i == sel ? a.Orange().Bold() : a);
        if (xCols <= 0)
        {
            xCols = texts.Length;
        }

        selected = GL.SelectionGrid(selected, titles.ToArray(), xCols, options);
        return sel != selected;
    }

    [UsedImplicitly]
    public static bool SelectionGrid(string title, ref int selected, string[] texts, int xCols,
        params GUILayoutOption[] options)
    {
        using (HorizontalScope())
        {
            Label(title.Cyan(), Width(300f));
            Space(25f);
            return SelectionGrid(ref selected, texts, xCols, options);
        }
    }

    [UsedImplicitly]
    public static bool SelectionGrid(ref int selected, string[] texts, int xCols, GUIStyle style,
        params GUILayoutOption[] options)
    {
        if (xCols <= 0)
        {
            xCols = texts.Length;
        }

        if (IsNarrow)
        {
            xCols = Math.Min(5, xCols);
        }

        var sel = selected;
        var titles = texts.Select((a, i) => i == sel ? a.Orange().Bold() : a);
        if (xCols <= 0)
        {
            xCols = texts.Length;
        }

        selected = GL.SelectionGrid(selected, titles.ToArray(), xCols, style, options);
        return sel != selected;
    }

    [UsedImplicitly]
    public static bool SelectionGrid<T>(ref int selected, T[] items, int xCols, params GUILayoutOption[] options)
    {
        if (xCols <= 0)
        {
            xCols = items.Length;
        }

        if (IsNarrow)
        {
            xCols = Math.Min(4, xCols);
        }

        var sel = selected;
        var titles = items.Select((a, i) => i == sel ? $"{a}".Orange().Bold() : $"{a}");
        if (xCols <= 0)
        {
            xCols = items.Length;
        }

        selected = GL.SelectionGrid(selected, titles.ToArray(), xCols, options);
        return sel != selected;
    }

    [UsedImplicitly]
    public static bool SelectionGrid<T>(ref int selected, T[] items, int xCols, GUIStyle style,
        params GUILayoutOption[] options)
    {
        if (xCols <= 0)
        {
            xCols = items.Length;
        }

        if (IsNarrow)
        {
            xCols = Math.Min(4, xCols);
        }

        var sel = selected;
        var titles = items.Select((a, i) => i == sel ? $"{a}".Orange().Bold() : $"{a}");
        if (xCols <= 0)
        {
            xCols = items.Length;
        }

        selected = GL.SelectionGrid(selected, titles.ToArray(), xCols, style, options);
        return sel != selected;
    }

    [UsedImplicitly]
    public static bool SelectionGrid(ref int selected, [NotNull] string[] texts, int xCols, int maxColsIfNarrow = 4,
        params GUILayoutOption[] options)
    {
        if (xCols <= 0)
        {
            xCols = texts.Length;
        }

        if (IsNarrow)
        {
            xCols = Math.Min(maxColsIfNarrow, xCols);
        }

        var sel = selected;
        var titles = texts.Select((a, i) =>
            i == sel ? a.Orange().Bold() : a);

        if (xCols <= 0)
        {
            xCols = texts.Length;
        }

        selected = GL.SelectionGrid(selected, titles.ToArray(), xCols, options);

        return sel != selected;
    }

    [UsedImplicitly]
    public static void ActionSelectionGrid(ref int selected, string[] texts, int xCols, Action<int> action,
        params GUILayoutOption[] options)
    {
        var sel = selected;
        var titles = texts.Select((a, i) => i == sel ? a.Orange().Bold() : a);
        if (xCols <= 0)
        {
            xCols = texts.Length;
        }

        // don't reuse sel to avoid warnings on lambda sel variable capture above
        var sel2 = GL.SelectionGrid(selected, titles.ToArray(), xCols, options);
        if (selected == sel2)
        {
            return;
        }

        selected = sel2;
        action(selected);
    }

    [UsedImplicitly]
    public static void ActionSelectionGrid(ref int selected, string[] texts, int xCols, Action<int> action,
        GUIStyle style, params GUILayoutOption[] options)
    {
        var sel = selected;
        var titles = texts.Select((a, i) => i == sel ? a.Orange().Bold() : a);
        if (xCols <= 0)
        {
            xCols = texts.Length;
        }

        // don't reuse sel to avoid warnings on lambda sel variable capture above
        var sel2 = GL.SelectionGrid(selected, titles.ToArray(), xCols, style, options);
        if (selected == sel2)
        {
            return;
        }

        selected = sel2;
        action(selected);
    }

    // EnumGrids

    [UsedImplicitly]
    public static void EnumGrid<TEnum>(Func<TEnum> get, Action<TEnum> set, int xCols, params GUILayoutOption[] options)
        where TEnum : struct
    {
        var value = get();
        var names = Enum.GetNames(typeof(TEnum));
        var index = Array.IndexOf(names, value.ToString());
        if (!SelectionGrid(ref index, names, xCols, options))
        {
            return;
        }

        if (Enum.TryParse(names[index], out TEnum newValue))
        {
            set(newValue);
        }
    }

    [UsedImplicitly]
    public static bool EnumGrid<TEnum>(ref TEnum value, int xCols, Func<string, TEnum, string> titleFormatter = null,
        GUIStyle style = null, params GUILayoutOption[] options) where TEnum : struct
    {
        options = options.AddDefaults();
        var names = Enum.GetNames(typeof(TEnum));
        var formattedNames = names;
        var nameToEnum = value.NameToValueDictionary();
        if (titleFormatter != null)
        {
            formattedNames = names.Select(n => titleFormatter(n, nameToEnum[n])).ToArray();
        }

        var index = Array.IndexOf(names, value.ToString());

        if (style == null
                ? !SelectionGrid(ref index, formattedNames, xCols, options)
                : !SelectionGrid(ref index, formattedNames, xCols, style, options))
        {
            return false;
        }

        if (!Enum.TryParse(names[index], out TEnum newValue))
        {
            return false;
        }

        value = newValue;

        return true;
    }

    [UsedImplicitly]
    public static bool EnumGrid<TEnum>(ref TEnum value, int xCols, Func<string, TEnum, string> titleFormatter = null,
        params GUILayoutOption[] options) where TEnum : struct
    {
        return EnumGrid(ref value, xCols, titleFormatter, null, options);
    }

    [UsedImplicitly]
    public static bool EnumGrid<TEnum>(ref TEnum value, int xCols, params GUILayoutOption[] options)
        where TEnum : struct
    {
        return EnumGrid(ref value, xCols, null, options);
    }

    [UsedImplicitly]
    public static bool EnumGrid<TEnum>(ref TEnum value, params GUILayoutOption[] options) where TEnum : struct
    {
        return EnumGrid(ref value, 0, null, options);
    }

    [UsedImplicitly]
    public static bool EnumGrid<TEnum>(string title, ref TEnum value, int xCols, params GUILayoutOption[] options)
        where TEnum : struct
    {
        bool changed;

        using (HorizontalScope())
        {
            Label(title.Cyan(), Width(300f));
            Space(25f);
            changed = EnumGrid(ref value, xCols, null, options);
        }

        return changed;
    }

    [UsedImplicitly]
    public static bool EnumGrid<TEnum>(string title, ref TEnum value, params GUILayoutOption[] options)
        where TEnum : struct
    {
        bool changed;

        using (HorizontalScope())
        {
            Label(title.Cyan(), Width(300f));
            Space(25f);
            changed = EnumGrid(ref value, 0, null, options);
        }

        return changed;
    }

    [UsedImplicitly]
    public static bool EnumGrid<TEnum>(string title, ref TEnum value, int xCols, GUIStyle style = null,
        params GUILayoutOption[] options) where TEnum : struct
    {
        bool changed;

        using (HorizontalScope())
        {
            Label(title.Cyan(), Width(300f));
            Space(25f);
            changed = EnumGrid(ref value, xCols, null, style, options);
        }

        return changed;
    }

    [UsedImplicitly]
    public static bool EnumGrid<TEnum>(string title, ref TEnum value, int xCols,
        Func<string, TEnum, string> titleFormatter = null, params GUILayoutOption[] options) where TEnum : struct
    {
        bool changed;

        using (HorizontalScope())
        {
            Label(title.Cyan(), Width(300f));
            Space(25f);
            changed = EnumGrid(ref value, xCols, titleFormatter, options);
        }

        return changed;
    }

    [UsedImplicitly]
    public static bool EnumGrid<TEnum>(string title, ref TEnum value, int xCols,
        Func<string, TEnum, string> titleFormatter = null, GUIStyle style = null, params GUILayoutOption[] options)
        where TEnum : struct
    {
        bool changed;

        using (HorizontalScope())
        {
            Label(title.Cyan(), Width(300f));
            Space(25f);
            changed = EnumGrid(ref value, xCols, titleFormatter, style, options);
        }

        return changed;
    }

    [UsedImplicitly]
    public static bool EnumGrid<TEnum>(string title, Func<TEnum> get, Action<TEnum> set,
        params GUILayoutOption[] options) where TEnum : struct
    {
        bool changed;

        using (HorizontalScope())
        {
            Label(title.Cyan(), Width(300f));
            Space(25f);
            var value = get();
            changed = EnumGrid(ref value, 0, null, options);
            if (changed)
            {
                set(value);
            }
        }

        return changed;
    }

    // EnumerablePicker

    [UsedImplicitly]
    public static void EnumerablePicker<T>(
        string title,
        ref int selected,
        IEnumerable<T> range,
        int xCols,
        Func<T, string> titleFormatter = null,
        params GUILayoutOption[] options)
    {
        titleFormatter ??= a => $"{a}";

        var enumerable = range as T[] ?? range.ToArray();
        if (selected > enumerable.Length)
        {
            selected = 0;
        }

        var sel = selected;
        var titles = enumerable.Select((a, i) => i == sel ? titleFormatter(a).Orange().Bold() : titleFormatter(a));
        if (xCols > enumerable.Length)
        {
            xCols = enumerable.Length;
        }

        if (xCols <= 0)
        {
            xCols = enumerable.Length;
        }

        Label(title, AutoWidth());
        Space(25f);
        selected = GL.SelectionGrid(selected, titles.ToArray(), xCols, options);
    }

    [UsedImplicitly]
    public static NamedFunc<T> TypePicker<T>(string title, ref int selectedIndex, NamedFunc<T>[] items) where T : class
    {
        var sel = selectedIndex;
        var titles = items.Select((item, i) => i == sel ? item.Name.Orange().Bold() : item.Name).ToArray();
        if (title?.Length > 0) { Label(title); }

        selectedIndex = GL.SelectionGrid(selectedIndex, titles, 6);
        return items[selectedIndex];
    }

    // GridPicker

    [UsedImplicitly]
    private static bool GridPicker<T>(ref T selected,
        List<T> items,
        string unselectedTitle,
        Func<T, string> titleFunc,
        ref string searchText,
        int xCols,
        GUIStyle style,
        params GUILayoutOption[] options
    ) where T : class
    {
        options = options.AddDefaults();
        style ??= GUI.skin.button;

        var changed = false;
        if (searchText != null)
        {
            ActionTextField(
                ref searchText,
                "itemSearchText",
                _ => { changed = true; },
                () => { },
                xCols == 1 ? options : [Width(300f)]);
            if (searchText?.Length > 0)
            {
                var searchStr = searchText.ToLower();
#pragma warning disable CA1862
                items = items.Where(i => titleFunc(i).ToLower().Contains(searchStr)).ToList();
#pragma warning restore CA1862
            }
        }

        var selectedItemIndex = items.IndexOf(selected);

        if (items.Count > 0)
        {
            var titles = items.Select(titleFunc);
            var hasUnselectedTitle = unselectedTitle != null;
            if (hasUnselectedTitle)
            {
                titles = titles.Prepend(unselectedTitle);
                selectedItemIndex += 1;
            }

            var adjustedIndex = Math.Max(0, selectedItemIndex);
            if (adjustedIndex != selectedItemIndex)
            {
                selectedItemIndex = adjustedIndex;
                changed = true;
            }

            ActionSelectionGrid(
                ref selectedItemIndex,
                titles.ToArray(),
                xCols,
                _ => { changed = true; },
                style,
                options);
            if (hasUnselectedTitle)
            {
                selectedItemIndex -= 1;
            }

            selected = selectedItemIndex >= 0 ? items[selectedItemIndex] : null;
            //if (changed) Mod.Log($"sel index: {selectedItemIndex} sel: {selected}");
        }
        else
        {
            Label("No Items".Grey(), options);
        }

        return changed;
    }

    [UsedImplicitly]
    public static bool GridPicker<T>(ref T selected, List<T> items,
        string unselectedTitle,
        Func<T, string> titleFunc,
        ref string searchText,
        int xCols,
        params GUILayoutOption[] options
    ) where T : class
    {
        return GridPicker(ref selected, items, unselectedTitle, titleFunc, ref searchText, xCols, ButtonStyle,
            options);
    }

    [UsedImplicitly]
    public static bool GridPicker<T>(ref T selected, List<T> items,
        string unselectedTitle,
        Func<T, string> titleFunc,
        ref string searchText,
        params GUILayoutOption[] options
    ) where T : class
    {
        return GridPicker(ref selected, items, unselectedTitle, titleFunc, ref searchText, 6, ButtonStyle, options);
    }

    [UsedImplicitly]
    // VPicker
    public static bool VPicker<T>(
        string title,
        ref T selected, List<T> items,
        string unselectedTitle,
        Func<T, string> titleFunc,
        ref string searchText,
        Action extras,
        GUIStyle style,
        params GUILayoutOption[] options
    ) where T : class
    {
        // style ??= GUI.skin.button;

        if (title != null)
        {
            Label(title, options);
        }

        extras?.Invoke();
        Div();

        return GridPicker(ref selected, items, unselectedTitle, titleFunc, ref searchText, 1, options);
    }

    [UsedImplicitly]
    public static bool VPicker<T>(
        string title,
        ref T selected, List<T> items,
        string unselectedTitle,
        Func<T, string> titleFunc,
        ref string searchText,
        Action extras,
        params GUILayoutOption[] options
    ) where T : class
    {
        return VPicker(title, ref selected, items, unselectedTitle, titleFunc, ref searchText, extras, ButtonStyle,
            options);
    }

    [UsedImplicitly]
    public static bool VPicker<T>(
        string title,
        ref T selected, List<T> items,
        string unselectedTitle,
        Func<T, string> titleFunc,
        ref string searchText,
        params GUILayoutOption[] options
    ) where T : class
    {
        return VPicker(title, ref selected, items, unselectedTitle, titleFunc, ref searchText, () => { }, ButtonStyle,
            options);
    }
}
