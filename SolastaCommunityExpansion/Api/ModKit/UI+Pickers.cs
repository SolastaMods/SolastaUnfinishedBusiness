// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Linq;
using SolastaCommunityExpansion.Api.Infrastructure;
using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace ModKit;

public static partial class UI
{
    public static bool SelectionGrid(
        ref int selected,
        string[] texts,
        int xCols,
        int maxColsIfNarrow = 4,
        params GUILayoutOption[] options)
    {
        if (xCols <= 0)
        {
            xCols = texts.Count();
        }

        if (IsNarrow)
        {
            xCols = Math.Min(maxColsIfNarrow, xCols);
        }

        var sel = selected;
        var titles = texts.Select((a, i) => i == sel ? a.Orange().Bold() : a);

        if (xCols <= 0)
        {
            xCols = texts.Count();
        }

        selected = GL.SelectionGrid(selected, titles.ToArray(), xCols, options);

        return sel != selected;
    }
}
