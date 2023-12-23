using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.DataViewer;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Displays;

internal static class BlueprintDisplay
{
    private static IEnumerable<BaseDefinition> _allBlueprints;

    // blueprint info
    private static Type[] _bpTypes;
    private static string[] _bpTypeNames;
    private static IEnumerable<BaseDefinition> _filteredBPs;

    // tree view
    private static readonly ReflectionTreeView TreeView = new();
    private static int _bpTypeIndex;

    private static Vector2 _bpsScrollPosition;

    // search
    private static string _selectionSearchText;

    private static Dictionary<string, FieldInfo> _bpFields;
    private static Dictionary<string, PropertyInfo> _bpProperties;
    private static string[] _bpChildNames;

    private static int _searchIndex;
    private static bool _searchReversed;
    private static string _searchText;

    // search selection
    private static ToggleState _searchExpanded;

    private static readonly GUIStyle ButtonStyle = new(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };

    internal static IEnumerable<BaseDefinition> GetBlueprints()
    {
        if (_allBlueprints != null)
        {
            return _allBlueprints;
        }

        if (BlueprintLoader.Shared.LoadInProgress())
        {
            return _allBlueprints;
        }

        BlueprintLoader.Shared.Load(bps => _allBlueprints = bps);

        return _allBlueprints;
    }

    private static void RefreshBpSearchData()
    {
        _filteredBPs = GetBlueprints();

        if (_filteredBPs != null)
        {
            TreeView.SetRoot(_filteredBPs);
        }
        else
        {
            TreeView.Clear();
        }

        _bpFields = Node.GetFields(_bpTypes[_bpTypeIndex])
            .OrderBy(info => info.Name)
            .ToDictionary(info => info.Name);
        _bpProperties = Node.GetProperties(_bpTypes[_bpTypeIndex])
            .OrderBy(info => info.Name)
            .ToDictionary(info => info.Name);
        _bpChildNames = _bpFields.Keys.Concat(_bpProperties.Keys)
            .OrderBy(key => key)
            .ToArray();
        _searchIndex = Array.IndexOf(_bpChildNames, "name");
    }

    private static void RefreshTypeNames()
    {
        _bpTypes = new Type[] { null }
            .Concat(GetBlueprints()
                .Select(bp => bp.GetType())
                .Distinct()
                .OrderBy(type => type.Name))
            .ToArray();

        if (!string.IsNullOrEmpty(_selectionSearchText))
        {
            _bpTypes = _bpTypes
                .Where(type => type == null || type.Name.Matches(_selectionSearchText))
                .ToArray();
        }

        _bpTypeNames = _bpTypes.Select(type => type?.Name).ToArray();
        _bpTypeNames[0] = "All";
        _bpTypes[0] = typeof(BaseDefinition);
        _bpTypeIndex = 0;
    }

    private static void UpdateSearchResults()
    {
        if (string.IsNullOrEmpty(_searchText))
        {
            TreeView.SetRoot(_filteredBPs);
        }
        else
        {
            var searchText = _searchText.ToLower();

            if (_bpFields.TryGetValue(_bpChildNames[_searchIndex], out var f))
            {
                TreeView.SetRoot(_filteredBPs.Where(bp =>
                {
                    try
                    {
#pragma warning disable CA1862
                        return (f.GetValue(bp)?.ToString().ToLower().Contains(searchText) ?? false) !=
#pragma warning restore CA1862
                               _searchReversed;
                    }
                    catch
                    {
                        return _searchReversed;
                    }
                }));
            }
            else if (_bpProperties.TryGetValue(_bpChildNames[_searchIndex], out var p))
            {
                TreeView.SetRoot(_filteredBPs.Where(bp =>
                {
                    try
                    {
#pragma warning disable CA1862
                        return (p.GetValue(bp)?.ToString().ToLower().Contains(searchText) ?? false) !=
#pragma warning restore CA1862
                               _searchReversed;
                    }
                    catch
                    {
                        return _searchReversed;
                    }
                }));
            }
        }
    }

    internal static void DisplayBlueprints()
    {
        try
        {
            // refresh blueprint types
            if (_bpTypeNames == null)
            {
                if (GetBlueprints() == null)
                {
                    GUILayout.Label("Loading: " +
                                    BlueprintLoader.Shared.Progress.ToString("P2").Cyan().Bold());
                    return;
                }

                RefreshTypeNames();
                RefreshBpSearchData();
            }

            using (new GUILayout.HorizontalScope())
            {
                var isDirty = false;

                // Blueprint Picker
                using (new GUILayout.VerticalScope())
                {
                    // Header and Search Field
                    var blueprintListIsDirty = false;

                    GUIHelper.Div();

                    using (new GUILayout.HorizontalScope(GUILayout.Width(450)))
                    {
                        // Header and Search Field
                        GUILayout.Label($"{_bpTypeNames![_bpTypeIndex]}".Cyan(), GUILayout.Width(300));
                        GUILayout.Space(10);
                        GUIHelper.TextField(ref _selectionSearchText,
                            () => blueprintListIsDirty = true, null, GUILayout.MinWidth(150));
                    }

                    if (blueprintListIsDirty)
                    {
                        RefreshTypeNames();
                    }

                    GUIHelper.Div();

                    // Blueprint Picker List
                    using var scrollView = new GUILayout.ScrollViewScope(_bpsScrollPosition, GUILayout.Width(450));

                    _bpsScrollPosition = scrollView.scrollPosition;
                    GUIHelper.SelectionGrid(ref _bpTypeIndex, _bpTypeNames, 1, () =>
                    {
                        _searchText = null;
                        RefreshBpSearchData();
                        _filteredBPs = _bpTypeIndex == 0
                            ? GetBlueprints()
                            : GetBlueprints().Where(item => item.GetType() == _bpTypes[_bpTypeIndex]);
                        TreeView.SetRoot(_filteredBPs);
                    }, ButtonStyle, GUILayout.Width(450));
                }

                using (new GUILayout.VerticalScope(GUI.skin.box))
                {
                    // Data Search Bar
                    GUIHelper.Div();
                    if (_bpChildNames.Length > 0)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            // search bar
                            GUILayout.Space(10f);

                            // selection - button
                            using (new GUILayout.HorizontalScope())
                            {
                                UI.ToggleButton(ref _searchExpanded,
                                    $"Search: {_bpChildNames[_searchIndex]}",
                                    GUILayout.ExpandWidth(false));

                                // _searchText input
                                GUILayout.Space(10);
                                GUIHelper.TextField(ref _searchText,
                                    () => isDirty = true, null, GUILayout.Width(450));
                                GUILayout.Space(10f);

                                if (UI.Toggle("By Excluding", ref _searchReversed, GUILayout.ExpandWidth(false)))
                                {
                                    isDirty = true;
                                }

                                if (_searchExpanded.IsOn())
                                {
                                    GUILayout.Space(10f);
                                }
                            }
                        }
                    }

                    // Data Search Field Picker
                    if (_searchExpanded.IsOn())
                    {
                        const float AVAILABLE_WIDTH = 960f - 550;
                        var xCols = (int)Math.Ceiling(AVAILABLE_WIDTH / 300);

                        // selection
                        GUIHelper.Div();
                        GUIHelper.SelectionGrid(ref _searchIndex,
                            _bpChildNames, xCols, () => isDirty = true, ButtonStyle, GUILayout.Width(AVAILABLE_WIDTH));
                    }

                    // Do the search
                    if (isDirty)
                    {
                        UpdateSearchResults();
                    }

                    GUIHelper.Div();

                    // tree view
                    using (new GUILayout.VerticalScope())
                    {
                        TreeView.OnGUI();
                    }
                }
            }
        }
        catch
        {
            _bpTypeIndex = 0;
            throw;
        }
    }
}
