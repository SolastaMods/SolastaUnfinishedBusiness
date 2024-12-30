using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.Displays;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Models;

internal static class SubclassesContext
{
    internal static readonly SortedList<string, (string, CharacterClassDefinition)> Klasses = [];

    internal static readonly Dictionary<CharacterClassDefinition, KlassListContext> KlassListContextTab = [];

    private static Dictionary<CharacterSubclassDefinition, DeityDefinition> DeityChoiceList
    {
        get;
    } = [];

    private static Dictionary<CharacterSubclassDefinition, FeatureDefinitionSubclassChoice> SubclassesChoiceList
    {
        get;
    } = [];

    internal static void Load()
    {
        // kept for backward compatibility
        var wayOfTheWealAndWoe = new WayOfTheWealAndWoe();

        wayOfTheWealAndWoe.Subclass.GuiPresentation.hidden = true;

        RegisterClassesContext();

        foreach (var abstractSubClassInstance in typeof(AbstractSubclass)
                     .Assembly.GetTypes()
                     .Where(t => t.IsSubclassOf(typeof(AbstractSubclass)) && !t.IsAbstract)
                     .Select(t => (AbstractSubclass)Activator.CreateInstance(t)))
        {
            LoadSubclass(abstractSubClassInstance);
        }

        // settings paring
        var subclasses = Main.Settings.KlassListSubclassEnabled
            .SelectMany(x => x.Value)
            .Where(name => KlassListContextTab
                .SelectMany(x => x.Value.AllSubClasses)
                .All(y => y.Name != name))
            .ToArray();

        foreach (var kvp in Main.Settings.KlassListSubclassEnabled)
        {
            kvp.Value.RemoveAll(x => subclasses.Contains(x));
        }

        DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
            .Do(x => x.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock));

        // bootstrap
        SwitchSchoolRestrictionsFromShadowCaster();
        SwitchSchoolRestrictionsFromSpellBlade();
    }

    internal static void LateLoad()
    {
        CircleOfTheLife.LateLoad();
        CollegeOfLife.LateLoad();
        RangerSurvivalist.LateLoad();
        SorcerousFieldManipulator.LateLoad();
        WizardAbjuration.LateLoad();
        WizardDeadMaster.LateLoad();
        WizardEvocation.LateLoad();
    }

    private static void RegisterClassesContext()
    {
        foreach (var klass in DatabaseRepository.GetDatabase<CharacterClassDefinition>())
        {
            var klassName = klass.Name;
            var postfix = klassName == InventorClass.ClassName ? " \u00a9".Grey() : string.Empty;

            Klasses.Add(klass.FormatTitle() + postfix, (klassName, klass));
            KlassListContextTab.Add(klass, new KlassListContext(klass));
            Main.Settings.DisplayKlassToggle.TryAdd(klassName, false);
            Main.Settings.KlassListSliderPosition.TryAdd(klassName, 4);
            Main.Settings.KlassListSubclassEnabled.TryAdd(klassName, []);
        }
    }

    private static void LoadSubclass([NotNull] AbstractSubclass subclassBuilder)
    {
        var klass = subclassBuilder.Klass;
        var subclass = subclassBuilder.Subclass;

        if (subclassBuilder.SubclassChoice && !subclassBuilder.DeityDefinition)
        {
            SubclassesChoiceList.Add(subclass, subclassBuilder.SubclassChoice);
        }
        else if (!subclassBuilder.SubclassChoice && subclassBuilder.DeityDefinition)
        {
            DeityChoiceList.Add(subclass, subclassBuilder.DeityDefinition);
        }

        KlassListContextTab[klass].RegisterSubclass(subclass);
    }

    internal static bool IsAllSetSelected()
    {
        return KlassListContextTab.Values.All(subclassListContext => subclassListContext.IsAllSetSelected);
    }

    internal static bool IsTabletopSetSelected()
    {
        return KlassListContextTab.Values.All(subclassListContext => subclassListContext.IsTabletopSetSelected);
    }

    internal static void SelectAllSet(bool toggle)
    {
        foreach (var subclassListContext in KlassListContextTab.Values)
        {
            subclassListContext.SelectAllSetInternal(toggle);
        }
    }

    internal static void SelectTabletopSet(bool toggle)
    {
        foreach (var subclassListContext in KlassListContextTab.Values)
        {
            subclassListContext.SelectTabletopSetInternal(toggle);
        }
    }


    internal static void SwitchSchoolRestrictionsFromShadowCaster()
    {
        if (Main.Settings.RemoveSchoolRestrictionsFromShadowCaster)
        {
            FeatureDefinitionCastSpells.CastSpellShadowcaster.RestrictedSchools.Clear();
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellShadowcaster.RestrictedSchools.SetRange(
                SchoolAbjuration,
                SchoolDivination,
                SchoolIllusion,
                SchoolNecromancy);
        }
    }

    internal static void SwitchSchoolRestrictionsFromSpellBlade()
    {
        if (Main.Settings.RemoveSchoolRestrictionsFromSpellBlade)
        {
            FeatureDefinitionCastSpells.CastSpellMartialSpellBlade.RestrictedSchools.Clear();
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellMartialSpellBlade.RestrictedSchools.SetRange(
                SchoolConjuration,
                SchoolEnchantement,
                SchoolEvocation,
                SchoolTransmutation);
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

        // ReSharper disable once MemberHidesStaticFromOuterClass
        internal bool IsTabletopSetSelected =>
            ModUi.TabletopDefinitions.Intersect(AllSubClasses).Count() == SelectedSubclasses.Count &&
            SelectedSubclasses.All(ModUi.TabletopDefinitionNames.Contains);

        internal void SelectAllSetInternal(bool toggle)
        {
            foreach (var subclass in AllSubClasses)
            {
                Switch(subclass, toggle);
            }
        }

        internal void SelectTabletopSetInternal(bool toggle)
        {
            foreach (var subclass in AllSubClasses)
            {
                Switch(subclass, toggle && ModUi.TabletopDefinitions.Contains(subclass));
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
