// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;
using GL = UnityEngine.GUILayout;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    private const float UmmWidth = 960f;
    internal static bool UserHasHitReturn = false;
    internal static string FocusedControlName = null;
    private static bool IsNarrow => UmmWidth < 1200;

    /*** UI Builders
     * 
     * This is a simple UI framework that simulates the style of SwiftUI.  
     * 
     * Usage - these are intended to be called from any OnGUI render path used in your mod
     * 
     * Elements will be defined like this
            UI.Section("Cheap Tricks", () =>
            {
                UI.HStack("Combat", 4,
                    () => { UI.ActionButton("Rest All", () => { CheatsCombat.RestAll(); }); },
                    () => { UI.ActionButton("Empowered", () => { CheatsCombat.Empowered(""); }); },
                    () => { UI.ActionButton("Full Buff Please", () => { CheatsCombat.RestAll(); }); },
                    () => { UI.ActionButton("Remove Death's Door", () => { CheatsCombat.Empowered(""); }); },
                    () => { UI.ActionButton("Kill All Enemies", () => { CheatsCombat.KillAll(); }); },
                    () => { UI.ActionButton("Summon Zoo", () => { CheatsCombat.SpawnInspectedEnemiesUnderCursor(""); }); }
                 );
                UI.Space(10);
                UI.HStack("Common", 4,
                    () => { UI.ActionButton("Change Weather", () => { CheatsCommon.ChangeWeather(""); }); },
                    () => { UI.ActionButton("Set Perception to 40", () => { CheatsCommon.StatPerception(); }); }
                 );
                UI.Space(10);
                UI.HStack("Unlocks", 4,
                    () => { UI.ActionButton("Give All Items", () => { CheatsUnlock.CreateAllItems(""); }); }
                 );
            });
    */

    private static void If(bool value, params Action[] actions)
    {
        if (!value)
        {
            return;
        }

        foreach (var action in actions)
        {
            action();
        }
    }

    internal static void HStack(string title = null, int stride = 0, params Action[] actions)
    {
        var length = actions.Length;
        if (stride < 1) { stride = length; }

        if (IsNarrow)
        {
            stride = Math.Min(3, stride);
        }

        for (var ii = 0; ii < actions.Length; ii += stride)
        {
            var hasTitle = title != null;

            BeginHorizontal();

            if (hasTitle)
            {
                if (ii == 0)
                {
                    Label(title.Bold(), Width(150f));
                }
                else
                {
                    Space(153);
                }
            }

            var filteredActions = actions.Skip(ii).Take(stride);

            foreach (var action in filteredActions)
            {
                action();
            }

            EndHorizontal();
        }
    }

    internal static void TabBar(ref int selected, Action header = null, params NamedAction[] actions)
    {
        if (selected >= actions.Length)
        {
            selected = 0;
        }

        var sel = selected;
        var titles = actions.Select((a, i) =>
            i == sel ? a.Name.Orange().Bold() : a.Name);
        var enumerable = titles as string[] ?? titles.ToArray();

        SelectionGrid(ref selected, enumerable.ToArray(), enumerable.Length, 6, ExpandWidth(true));
        GL.BeginVertical("box");
        header?.Invoke();
        actions[selected].Action();
        GL.EndVertical();
    }

    internal static void SubMenu(ref int selected, bool div = true, Action header = null, params NamedAction[] actions)
    {
        if (selected >= actions.Length)
        {
            selected = 0;
        }

        var sel = selected;
        var titles = actions.Select((a, i) =>
            i == sel ? a.Name.Orange().Bold() : a.Name);
        var enumerable = titles as string[] ?? titles.ToArray();
        //SelectionGrid(ref selected, enumerable.ToArray(), enumerable.Length, 6, ExpandWidth(false));
        using (HorizontalScope(GUI.skin.scrollView))
        {
            using (VerticalScope())
            {
                SelectionGrid(ref selected, enumerable.ToArray(), 0, SubmenuButtonStyle, AutoWidth());
                if (div)
                {
                    DivLast(20);
                    Space(-25);
                }

                header?.Invoke();
                actions[selected].Action();
            }
        }
    }
}
