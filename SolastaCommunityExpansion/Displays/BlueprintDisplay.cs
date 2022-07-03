using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModKit;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Api.ModKit;
using SolastaCommunityExpansion.DataViewer;
using UnityEngine;

namespace SolastaCommunityExpansion.Displays;

public static class BlueprintDisplay
{
    private static IEnumerable<BaseDefinition> _allBlueprints;

    // blueprint info
    private static Type[] _bpTypes;
    private static string[] _bpTypeNames;
    private static IEnumerable<BaseDefinition> _filteredBPs;

    // tree view
    private static readonly ReflectionTreeView _treeView = new();
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

    private static readonly GUIStyle _buttonStyle = new(GUI.skin.button) {alignment = TextAnchor.MiddleLeft};

    private static IEnumerable<BaseDefinition> GetBlueprints()
    {
        if (_allBlueprints == null)
        {
            if (BlueprintLoader.Shared.LoadInProgress())
            {
                return _allBlueprints;
            }

            BlueprintLoader.Shared.Load(bps => _allBlueprints = bps);
        }

        return _allBlueprints;
    }

    private static void RefreshBPSearchData()
    {
        _filteredBPs = GetBlueprints();

        if (_filteredBPs != null)
        {
            _treeView.SetRoot(_filteredBPs);
        }
        else
        {
            _treeView.Clear();
        }

        _bpFields = Node.GetFields(_bpTypes[_bpTypeIndex]).OrderBy(info => info.Name)
            .ToDictionary(info => info.Name);
        _bpProperties = Node.GetProperties(_bpTypes[_bpTypeIndex]).OrderBy(info => info.Name)
            .ToDictionary(info => info.Name);
        _bpChildNames = _bpFields.Keys.Concat(_bpProperties.Keys).OrderBy(key => key).ToArray();
        _searchIndex = Array.IndexOf(_bpChildNames, "name");
    }

    public static void RefreshTypeNames()
    {
        _bpTypes = new Type[] {null}
            .Concat(GetBlueprints().Select(bp => bp.GetType()).Distinct().OrderBy(type => type.Name)).ToArray();

        if (!string.IsNullOrEmpty(_selectionSearchText))
        {
            _bpTypes = _bpTypes
                .Where(type => type == null || StringExtensions.Matches(type.Name, _selectionSearchText)).ToArray();
        }

        _bpTypeNames = _bpTypes.Select(type => type?.Name).ToArray();
        _bpTypeNames[0] = "All";
        _bpTypes[0] = typeof(BaseDefinition);
        _bpTypeIndex = 0;
    }

    public static void UpdateSearchResults()
    {
        if (string.IsNullOrEmpty(_searchText))
        {
            _treeView.SetRoot(_filteredBPs);
        }
        else
        {
            var searchText = _searchText.ToLower();

            if (_bpFields.TryGetValue(_bpChildNames[_searchIndex], out var f))
            {
                _treeView.SetRoot(_filteredBPs.Where(bp =>
                {
                    try
                    {
                        return (f.GetValue(bp)?.ToString()?.ToLower().Contains(searchText) ?? false) !=
                               _searchReversed;
                    }
                    catch
                    {
                        return _searchReversed;
                    }
                }).ToList());
            }
            else if (_bpProperties.TryGetValue(_bpChildNames[_searchIndex], out var p))
            {
                _treeView.SetRoot(_filteredBPs.Where(bp =>
                {
                    try
                    {
                        return (p.GetValue(bp)?.ToString()?.ToLower().Contains(searchText) ?? false) !=
                               _searchReversed;
                    }
                    catch
                    {
                        return _searchReversed;
                    }
                }).ToList());
            }
        }
    }

    public static void DisplayBlueprints()
    {
        if (!Main.Enabled)
        {
            return;
        }

        try
        {
            // refresh blueprint types
            if (_bpTypeNames == null)
            {
                if (GetBlueprints() == null)
                {
                    GUILayout.Label("Blueprints".Orange().Bold() + " loading: " +
                                    BlueprintLoader.Shared.Progress.ToString("P2").Cyan().Bold());
                    return;
                }

                RefreshTypeNames();
                RefreshBPSearchData();
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
                        GUILayout.Label($"{_bpTypeNames[_bpTypeIndex]}".Cyan(), GUILayout.Width(300));
                        GUILayout.Space(10);
                        GUIHelper.TextField(ref _selectionSearchText, () => blueprintListIsDirty = true, null,
                            GUILayout.MinWidth(150));
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
                        RefreshBPSearchData();
                        _filteredBPs = _bpTypeIndex == 0
                            ? GetBlueprints()
                            : GetBlueprints().Where(item => item.GetType() == _bpTypes[_bpTypeIndex]).ToList();
                        _treeView.SetRoot(_filteredBPs);
                    }, _buttonStyle, GUILayout.Width(450));
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

                            // slelection - button
                            using (new GUILayout.HorizontalScope())
                            {
                                UI.ToggleButton(ref _searchExpanded, $"Search: {_bpChildNames[_searchIndex]}",
                                    _buttonStyle, GUILayout.ExpandWidth(false));

                                // _searchText input
                                GUILayout.Space(10);
                                GUIHelper.TextField(ref _searchText, () => isDirty = true, null,
                                    GUILayout.Width(450));
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
                        // selection
                        GUIHelper.Div();
                        const float availableWidth = 960f - 550;
                        var xCols = (int)Math.Ceiling(availableWidth / 300);
                        GUIHelper.SelectionGrid(ref _searchIndex, _bpChildNames, xCols, () => isDirty = true,
                            _buttonStyle, GUILayout.Width(availableWidth));
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
                        _treeView.OnGUI();
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
