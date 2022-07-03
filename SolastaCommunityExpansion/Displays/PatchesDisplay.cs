using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using ModKit;
using SolastaCommunityExpansion.Api.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Displays;

internal static class PatchesDisplay
{
    private static Dictionary<string, string> _modIdsToColor;
    private static string _modID;
    private static Dictionary<MethodBase, List<Patch>> _patches;
    private static readonly Dictionary<MethodBase, List<Patch>> _disabled = new();
    private static GUIStyle _buttonStyle;
    private static bool firstTime = true;
    private static string searchText = "";

    internal static void DisplayPatches()
    {
        if (_buttonStyle == null)
        {
            _buttonStyle = new GUIStyle(GUI.skin.button) {alignment = TextAnchor.MiddleLeft};
        }

        if (firstTime)
        {
            RefreshListOfPatchOwners();
            RefreshPatchInfoOfAllMods();
            firstTime = false;
        }

        try
        {
            var selectedPatchName = "All".Bold();

            using (new GUILayout.HorizontalScope())
            {
                // modId <=> color
                using (new GUILayout.VerticalScope())
                {
                    if (GUILayout.Button("Refresh list of patch owners", _buttonStyle, UI.Width(200)))
                    {
                        RefreshListOfPatchOwners();
                    }
                }

                // mod selection
                if (_modIdsToColor != null)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        if (GUILayout.Button("All".Bold(), _buttonStyle))
                        {
                            _patches = null;
                            _modID = null;
                            RefreshPatchInfoOfAllMods();
                        }

                        foreach (var pair in _modIdsToColor)
                        {
                            if (GUILayout.Button(pair.Key.Color($"#{pair.Value}").Bold(), _buttonStyle))
                            {
                                _patches = null;
                                _modID = pair.Key;
                                RefreshPatchInfoOfSelected();
                            }
                        }
                    }

                    // info selection
                    using (new GUILayout.VerticalScope())
                    {
                        selectedPatchName = string.IsNullOrEmpty(_modID)
                            ? "All".Bold()
                            : _modID.Color($"#{_modIdsToColor[_modID]}").Bold();
                        if (GUILayout.Button($"Refresh Patch Info ({selectedPatchName})", _buttonStyle,
                                UI.Width(200)))
                        {
                            RefreshPatchInfoOfAllMods();
                        }

                        if (GUILayout.Button($"Potential Conflicts for ({selectedPatchName})", _buttonStyle,
                                UI.Width(200)))
                        {
                            RefreshPatchInfoOfPotentialConflict();
                        }
                    }
                }

                GUILayout.FlexibleSpace();
            }

            UI.Space(25);
            var searchTextLower = searchText.ToLower();
            var methodBases = _patches?.Keys.Concat(_disabled.Keys).Distinct().OrderBy(m => m.Name).Where(m =>
                searchText.Length == 0
                || m.DeclaringType.FullName.ToLower().Contains(searchTextLower)
                || m.ToString().ToLower().Contains(searchTextLower)
            );
            if (_modIdsToColor != null && methodBases != null)
            {
                GUILayout.Space(10f);
                using (UI.HorizontalScope())
                {
                    GUILayout.Label($"Selected Patch Owner: {selectedPatchName}", UI.AutoWidth());
                    UI.Space(25);
                    UI.TextField(ref searchText, "Search", UI.Width(400));
                }

                UI.Space(25);
                UI.Label($"Patches Found: {methodBases.Count().ToString().Cyan()}".Orange());
                var index = 1;
                foreach (var method in methodBases)
                {
                    var typeStr = method.DeclaringType.FullName;
                    var methodComponents = method.ToString().Split();
                    var returnTypeStr = methodComponents[0];
                    var methodName = methodComponents[1];

                    UI.Label("");

                    using (new GUILayout.VerticalScope())
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label($"{index++}", GUI.skin.box, UI.AutoWidth());
                            UI.Space(10);
                            GUILayout.Label(
                                $"{returnTypeStr.Grey().Bold()} {methodName.Bold()}\t{typeStr.Grey().Italic()}");
                        }

                        var enabledPatches = EnabledPatchesForMethod(method);
                        var disabledPatches = DisabledPatchesForMethod(method);

                        // do some quick cleanup of disabled entries that have been re-enabled outside of here
                        var intersection = new HashSet<Patch>(disabledPatches);
                        intersection.IntersectWith(enabledPatches);
                        if (intersection.Count > 0)
                        {
                            foreach (var dupe in intersection)
                            {
                                disabledPatches.Remove(dupe);
                                _disabled[method] = disabledPatches;
                            }
                        }

                        var patches = enabledPatches.Concat(disabledPatches).OrderBy(p => p.owner).ToArray();
                        UI.Space(15);
                        using (new GUILayout.HorizontalScope())
                        {
                            UI.Space(50);
                            using (new GUILayout.VerticalScope())
                            {
                                foreach (var patch in patches)
                                {
                                    var enabled = enabledPatches.Contains(patch);
                                    if (Api.ModKit.Private.UI.CheckBox("", enabled, false))
                                    {
                                        EnablePatchForMethod(!enabled, patch, method);
                                    }
                                }
                            }

                            using (new GUILayout.VerticalScope())
                            {
                                foreach (var patch in patches)
                                {
                                    GUILayout.Label(patch.PatchMethod.Name, GUI.skin.label);
                                }
                            }

                            UI.Space(10);
                            using (new GUILayout.VerticalScope())
                            {
                                foreach (var patch in patches)
                                {
                                    GUILayout.Label(patch.owner.Color($"#{_modIdsToColor[patch.owner]}").Bold(),
                                        GUI.skin.label);
                                }
                            }

                            UI.Space(10);
                            using (new GUILayout.VerticalScope())
                            {
                                foreach (var patch in patches)
                                {
                                    GUILayout.Label(patch.priority.ToString(), GUI.skin.label);
                                }
                            }

                            UI.Space(10);
                            using (new GUILayout.VerticalScope())
                            {
                                foreach (var patch in patches)
                                {
                                    GUILayout.Label(patch.PatchMethod.DeclaringType.DeclaringType?.Name ?? "---",
                                        GUI.skin.label);
                                }
                            }

                            UI.Space(10);
                            using (new GUILayout.VerticalScope())
                            {
                                foreach (var patch in patches)
                                {
                                    GUILayout.TextArea(patch.PatchMethod.DeclaringType.Name, GUI.skin.textField);
                                }
                            }

                            GUILayout.FlexibleSpace();
                        }
                    }
                }
            }
        }
        catch
        {
            _patches = null;
            _modID = null;
            _modIdsToColor = null;
        }

        UI.Label("");
    }

    private static List<Patch> EnabledPatchesForMethod(MethodBase method)
    {
        return _patches.TryGetValue(method, out var result) ? result : new List<Patch>();
    }

    private static List<Patch> DisabledPatchesForMethod(MethodBase method)
    {
        return _disabled.TryGetValue(method, out var result) ? result : new List<Patch>();
    }

    private static void EnablePatchForMethod(bool enabled, Patch patch, MethodBase method)
    {
        var enabledPatches = EnabledPatchesForMethod(method);
        var disabledPatches = DisabledPatchesForMethod(method);
        if (enabled)
        {
            enabledPatches.Add(patch);
            disabledPatches.Remove(patch);
        }
        else
        {
            disabledPatches.Add(patch);
            enabledPatches.Remove(patch);
        }

        _patches[method] = enabledPatches;
        _disabled[method] = disabledPatches;
    }

    private static void RefreshListOfPatchOwners(bool reset = true)
    {
        if (reset || _modIdsToColor == null)
        {
            _modIdsToColor = new Dictionary<string, string>();
        }

        var patches = Harmony.GetAllPatchedMethods().SelectMany(method =>
        {
            var patchInfo = Harmony.GetPatchInfo(method);
            return patchInfo.Prefixes.Concat(patchInfo.Transpilers).Concat(patchInfo.Postfixes);
        });
        var owners = patches.Select(patchInfo => patchInfo.owner).Distinct().OrderBy(owner => owner);
        var hue = 0.0f;
        foreach (var owner in owners)
        {
            if (!_modIdsToColor.ContainsKey(owner))
            {
                var color = Random.ColorHSV(
                    hue, hue,
                    0.25f, .75f,
                    0.75f, 1f
                );
                _modIdsToColor[owner] = ColorUtility.ToHtmlStringRGBA(color);
                hue = (hue + 0.1f) % 1.0f;
            }
        }
    }

    private static void RefreshPatchInfoOfAllMods()
    {
        _patches = new Dictionary<MethodBase, List<Patch>>();
        foreach (var method in Harmony.GetAllPatchedMethods())
        {
            _patches.Add(method, GetSortedPatches(method).ToList());
        }
    }

    private static void RefreshPatchInfoOfSelected()
    {
        _patches = new Dictionary<MethodBase, List<Patch>>();
        foreach (var method in Harmony.GetAllPatchedMethods())
        {
            var patches =
                GetSortedPatches(method).Where(patch => patch.owner == _modID);
            if (patches.Any())
            {
                _patches.Add(method, patches.ToList());
            }
        }
    }

    private static void RefreshPatchInfoOfPotentialConflict()
    {
        _patches = new Dictionary<MethodBase, List<Patch>>();
        foreach (var method in Harmony.GetAllPatchedMethods())
        {
            var patches = GetSortedPatches(method);
            var owners = patches.Select(patch => patch.owner).Distinct().ToHashSet();
            if (owners.Count > 1 && (_modID == null || owners.Contains(_modID)))
            {
                _patches.Add(method, patches.ToList());
            }
        }
    }

    private static IEnumerable<Patch> GetSortedPatches(MethodBase method)
    {
        var patches = Harmony.GetPatchInfo(method);
        return patches.Prefixes.OrderByDescending(patch => patch.priority)
            .Concat(patches.Transpilers.OrderByDescending(patch => patch.priority))
            .Concat(patches.Postfixes.OrderByDescending(patch => patch.priority));
    }
}
