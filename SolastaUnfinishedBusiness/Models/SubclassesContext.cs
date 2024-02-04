using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Models;

internal static class SubclassesContext
{
    internal static readonly SortedList<string, CharacterClassDefinition> Klasses = [];

    internal static readonly Dictionary<CharacterClassDefinition, KlassListContext> KlassListContextTab = [];

    private static Dictionary<CharacterSubclassDefinition, DeityDefinition> DeityChoiceList
    {
        get;
    } = [];

    private static Dictionary<CharacterSubclassDefinition, FeatureDefinitionSubclassChoice> SubclassesChoiceList
    {
        get;
    } = [];

    private static IEnumerable<string> DeprecatedSubsList { get; } =
    [
        "CollegeOfHarlequin",
        "MartialMarshal",
        "MartialMartialDefender",
        "PatronMoonlit",
        "RoguishRaven",
        "WayOfTheDistantHand"
    ];

    internal static void Load()
    {
        RegisterClassesContext();

        foreach (var abstractSubClassInstance in typeof(AbstractSubclass)
                     .Assembly.GetTypes()
                     .Where(t => t.IsSubclassOf(typeof(AbstractSubclass)) && !t.IsAbstract)
                     .Select(t => (AbstractSubclass)Activator.CreateInstance(t))
                     .Where(t => !DeprecatedSubsList.Contains(t.Subclass.Name)))
        {
            LoadSubclass(abstractSubClassInstance);
        }

        // settings paring
        var subclasses = Main.Settings.KlassListSubclassEnabled
            .SelectMany(x => x.Value)
            .Where(name => KlassListContextTab
                .SelectMany(x => x.Value.AllSubClasses)
                .All(y => y.Name != name))
            .ToList();

        foreach (var kvp in Main.Settings.KlassListSubclassEnabled)
        {
            kvp.Value.RemoveAll(x => subclasses.Contains(x));
        }

        // sorting
        if (Main.Settings.EnableSortingFutureFeatures)
        {
            DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
                .Do(x => x.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock));
        }
    }

    internal static void LateLoad()
    {
        CircleOfTheLife.LateLoad();
        CollegeOfLife.LateLoad();
        RangerSurvivalist.LateLoad();
        SorcerousFieldManipulator.LateLoad();
    }

    private static void RegisterClassesContext()
    {
        foreach (var klass in DatabaseRepository.GetDatabase<CharacterClassDefinition>())
        {
            var klassName = klass.Name;

            Klasses.Add(klassName, klass);
            KlassListContextTab.Add(klass, new KlassListContext(klass));
            Main.Settings.DisplayKlassToggle.TryAdd(klassName, true);
            Main.Settings.KlassListSliderPosition.TryAdd(klassName, 4);
            Main.Settings.KlassListSubclassEnabled.TryAdd(klassName, []);
        }
    }

    private static void LoadSubclass([NotNull] AbstractSubclass subclassBuilder)
    {
        var klass = subclassBuilder.Klass;
        var subclass = subclassBuilder.Subclass;

        if (subclassBuilder.SubclassChoice != null && subclassBuilder.DeityDefinition == null)
        {
            SubclassesChoiceList.Add(subclass, subclassBuilder.SubclassChoice);
        }
        else if (subclassBuilder.SubclassChoice == null && subclassBuilder.DeityDefinition != null)
        {
            DeityChoiceList.Add(subclass, subclassBuilder.DeityDefinition);
        }

        KlassListContextTab[klass].RegisterSubclass(subclass);
    }

    internal static bool IsAllSetSelected()
    {
        return KlassListContextTab.Values.All(subclassListContext => subclassListContext.IsAllSetSelected);
    }

    internal static void SelectAllSet(bool toggle)
    {
        foreach (var subclassListContext in KlassListContextTab.Values)
        {
            subclassListContext.SelectAllSetInternal(toggle);
        }
    }

    internal sealed class KlassListContext
    {
        internal KlassListContext(CharacterClassDefinition characterClassDefinition)
        {
            Klass = characterClassDefinition;
            AllSubClasses = [];
        }

        private List<string> SelectedSubclasses => Main.Settings.KlassListSubclassEnabled[Klass.Name];
        private CharacterClassDefinition Klass { get; }
        internal HashSet<CharacterSubclassDefinition> AllSubClasses { get; }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        internal bool IsAllSetSelected => AllSubClasses.Count == SelectedSubclasses.Count;

        internal void SelectAllSetInternal(bool toggle)
        {
            foreach (var subclass in AllSubClasses)
            {
                Switch(subclass, toggle);
            }
        }

        internal void RegisterSubclass(CharacterSubclassDefinition characterSubclassDefinition)
        {
            AllSubClasses.Add(characterSubclassDefinition);
            UpdateSubclassVisibility(characterSubclassDefinition);
        }

        internal void Switch(CharacterSubclassDefinition characterSubclassDefinition, bool active)
        {
            var klass = Klass.Name;
            var subclass = characterSubclassDefinition.Name;

            if (active)
            {
                Main.Settings.KlassListSubclassEnabled[klass].TryAdd(subclass);
            }
            else
            {
                Main.Settings.KlassListSubclassEnabled[klass].Remove(subclass);
            }

            UpdateSubclassVisibility(characterSubclassDefinition);
        }

        private void UpdateSubclassVisibility([NotNull] CharacterSubclassDefinition characterSubclassDefinition)
        {
            var klass = Klass.Name;
            var subclass = characterSubclassDefinition.Name;

            if (SubclassesChoiceList.TryGetValue(characterSubclassDefinition, out var choiceList))
            {
                if (Main.Settings.KlassListSubclassEnabled[klass].Contains(subclass))
                {
                    choiceList.Subclasses.TryAdd(subclass);
                }
                else
                {
                    choiceList.Subclasses.Remove(subclass);
                }
            }
            else if (DeityChoiceList.TryGetValue(characterSubclassDefinition, out var deityDefinition))
            {
                if (Main.Settings.KlassListSubclassEnabled[klass].Contains(subclass))
                {
                    deityDefinition.Subclasses.TryAdd(subclass);
                }
                else
                {
                    deityDefinition.Subclasses.Remove(subclass);
                }
            }

            // enable / disable Inventor class based on subclasses selection
            if (Klass == InventorClass.Class)
            {
                InventorClass.Class.GuiPresentation.hidden = Main.Settings.KlassListSubclassEnabled[klass].Count == 0;
            }
        }
    }
}
