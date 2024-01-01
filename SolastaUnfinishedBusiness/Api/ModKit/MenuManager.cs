using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal interface IMenuPage
{
    string Name { get; }

    int Priority { get; }

    // ReSharper disable once UnusedParameter.Global
    void OnGUI(UnityModManager.ModEntry modEntry);
}

internal interface IMenuTopPage : IMenuPage;

internal interface IMenuSelectablePage : IMenuPage;

internal interface IMenuBottomPage : IMenuPage;

internal sealed class MenuManager : INotifyPropertyChanged
{
    private static Exception _caughtException;
    private readonly List<IMenuBottomPage> _bottomPages = [];
    private readonly List<IMenuSelectablePage> _selectablePages = [];
    private readonly List<IMenuTopPage> _topPages = [];
    private static int SelectedTab => Main.Settings.SelectedTab;

    private int TabIndex
    {
        set
        {
            if (Main.Settings.SelectedTab == value)
            {
                return;
            }

            Main.Settings.SelectedTab = value;
            NotifyPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    // This method is called by the Set accessor of each property.
    // The CallerMemberName attribute that is applied to the optional propertyName
    // parameter causes the property name of the caller to be substituted as an argument.
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    internal void Enable(UnityModManager.ModEntry modEntry, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes()
                     .Where(type => !type.IsInterface && !type.IsAbstract && typeof(IMenuPage).IsAssignableFrom(type)))
        {
            if (typeof(IMenuTopPage).IsAssignableFrom(type))
            {
                _topPages.Add(Activator.CreateInstance(type, true) as IMenuTopPage);
            }

            if (typeof(IMenuSelectablePage).IsAssignableFrom(type))
            {
                _selectablePages.Add(Activator.CreateInstance(type, true) as IMenuSelectablePage);
            }

            if (typeof(IMenuBottomPage).IsAssignableFrom(type))
            {
                _bottomPages.Add(Activator.CreateInstance(type, true) as IMenuBottomPage);
            }
        }

        _topPages.Sort(Comparison);
        _selectablePages.Sort(Comparison);
        _bottomPages.Sort(Comparison);

        modEntry.OnGUI += OnGUI;

        return;

        static int Comparison(IMenuPage x, IMenuPage y)
        {
            return x.Priority - y.Priority;
        }
    }

    private void OnGUI(UnityModManager.ModEntry modEntry)
    {
        var hasPriorPage = false;

        try
        {
            if (_caughtException != null)
            {
                GUILayout.Label("ERROR".Red().Bold() +
                                $": caught exception {_caughtException}");

                if (GUILayout.Button("Reset".Orange().Bold(),
                        GUILayout.ExpandWidth(false)))
                {
                    _caughtException = null;
                }

                return;
            }

            var e = Event.current;

            UI.UserHasHitReturn = e.keyCode == KeyCode.Return;
            UI.FocusedControlName = GUI.GetNameOfFocusedControl();

            if (_topPages.Count > 0)
            {
                foreach (var page in _topPages)
                {
                    if (hasPriorPage)
                    {
                        GUILayout.Space(10f);
                    }

                    page.OnGUI(modEntry);
                    hasPriorPage = true;
                }
            }

            if (_selectablePages.Count is > 0 and > 1)
            {
                if (hasPriorPage)
                {
                    //GUILayout.Space(10f);
                }

                var tabIndex = SelectedTab;
                UI.TabBar(ref tabIndex, null,
                    _selectablePages.Select(page => new NamedAction(page.Name, () => page.OnGUI(modEntry)))
                        .ToArray());
                TabIndex = tabIndex;
            }

            if (_bottomPages.Count <= 0)
            {
                return;
            }

            foreach (var page in _bottomPages)
            {
                if (hasPriorPage)
                {
                    GUILayout.Space(10f);
                }

                page.OnGUI(modEntry);
                hasPriorPage = true;
            }
        }
        catch (Exception e)
        {
            _caughtException = e;
        }
    }
}
