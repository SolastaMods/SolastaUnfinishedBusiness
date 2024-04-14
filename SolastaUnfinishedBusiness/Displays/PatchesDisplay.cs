#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SolastaUnfinishedBusiness.Displays;

internal static class PatchesDisplay
{
    private static Dictionary<string, string> _modIdsToColor = new();
    private static Dictionary<MethodBase, List<Patch>> _patches;
    private static bool _firstTime = true;

    internal static void DisplayPatches()
    {
        if (_firstTime)
        {
            RefreshListOfPatchOwners();
            RefreshPatchInfoOfAllMods();
            _firstTime = false;
        }

        try
        {
            var methodBases = _patches?.Keys
                .Distinct()
                .OrderBy(m => m.Name)
                .ToArray() ?? Array.Empty<MethodBase>();

            UI.Label($"Total: {methodBases.Length.ToString().Cyan()}".Orange());

            var index = 1;

            foreach (var method in methodBases)
            {
                if (method.DeclaringType == null)
                {
                    continue;
                }

                var typeStr = method.DeclaringType.FullName;
                var methodComponents = method.ToString().Split();
                var returnTypeStr = methodComponents[0];
                var methodName = methodComponents[1];

                UI.Label();

                using (new GUILayout.VerticalScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"{index++}", GUI.skin.box, UI.AutoWidth());
                        UI.Space(10f);
                        GUILayout.Label(
                            $"{returnTypeStr.Grey().Bold()} {methodName.Bold()}\t{typeStr.Grey().Italic()}");
                    }

                    var patches = EnabledPatchesForMethod(method);

                    UI.Space(15f);
                    using (new GUILayout.HorizontalScope())
                    {
                        UI.Space(50f);

                        using (new GUILayout.VerticalScope())
                        {
                            foreach (var patch in patches)
                            {
                                GUILayout.Label(patch.PatchMethod.Name, GUI.skin.label);
                            }
                        }

                        UI.Space(10f);
                        using (new GUILayout.VerticalScope())
                        {
                            foreach (var patch in patches)
                            {
                                GUILayout.Label(
                                    patch.owner.Color($"#{_modIdsToColor[patch.owner]}").Bold(),
                                    GUI.skin.label);
                            }
                        }

                        UI.Space(10f);
                        using (new GUILayout.VerticalScope())
                        {
                            foreach (var patch in patches)
                            {
                                GUILayout.Label(patch.priority.ToString(), GUI.skin.label);
                            }
                        }

                        UI.Space(10f);
                        using (new GUILayout.VerticalScope())
                        {
                            foreach (var patch in patches)
                            {
                                if (patch.PatchMethod.DeclaringType != null)
                                {
                                    GUILayout.Label(patch.PatchMethod.DeclaringType.DeclaringType?.Name ?? "---",
                                        GUI.skin.label);
                                }
                            }
                        }

                        UI.Space(10f);
                        using (new GUILayout.VerticalScope())
                        {
                            foreach (var patch in patches)
                            {
                                if (patch.PatchMethod.DeclaringType != null)
                                {
                                    GUILayout.TextArea(patch.PatchMethod.DeclaringType.Name, GUI.skin.textField);
                                }
                            }
                        }

                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }
        catch
        {
            _patches = null;
            _modIdsToColor = null;
        }

        UI.Label();
    }

    private static Patch[] EnabledPatchesForMethod([NotNull] MethodBase method)
    {
        return _patches.TryGetValue(method, out var result)
            ? [.. result.OrderBy(p => p.owner)]
            : [];
    }

    private static void RefreshListOfPatchOwners()
    {
        var hue = 0.0f;
        var owners = Harmony.GetAllPatchedMethods()
            .SelectMany(method =>
            {
                var patchInfo = Harmony.GetPatchInfo(method);

                return patchInfo.Prefixes.Concat(patchInfo.Transpilers).Concat(patchInfo.Postfixes);
            })
            .Select(patchInfo => patchInfo.owner)
            .Distinct()
            .OrderBy(owner => owner);

        foreach (var owner in owners)
        {
            if (_modIdsToColor.ContainsKey(owner))
            {
                continue;
            }

            var color = Random.ColorHSV(
                hue, hue,
                0.25f, .75f,
                0.75f, 1f
            );

            _modIdsToColor[owner] = ColorUtility.ToHtmlStringRGBA(color);
            hue = (hue + 0.1f) % 1.0f;
        }
    }

    private static void RefreshPatchInfoOfAllMods()
    {
        _patches = new Dictionary<MethodBase, List<Patch>>();

        foreach (var method in Harmony.GetAllPatchedMethods())
        {
            var patches = Harmony.GetPatchInfo(method);

            _patches.Add(method, patches.Prefixes
                .Concat(patches.Transpilers)
                .Concat(patches.Postfixes)
                .OrderByDescending(patch => patch.priority)
                .ToList());
        }
    }
}
#endif
