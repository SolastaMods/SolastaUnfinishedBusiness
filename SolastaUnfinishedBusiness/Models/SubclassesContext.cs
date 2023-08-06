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
    // ReSharper disable once InconsistentNaming
    private static readonly SortedList<string, CharacterClassDefinition> klasses = new();

    internal static readonly Dictionary<CharacterClassDefinition, SubclassListContext> SubclassListContextTab = new();

    internal static SortedList<string, CharacterClassDefinition> Klasses
    {
        get
        {
            if (klasses.Count != 0)
            {
                return klasses;
            }

            foreach (var klass in SubclassListContextTab.Keys)
            {
                klasses.Add(klass.FormatTitle(), klass);
            }

            return klasses;
        }
    }

    private static Dictionary<CharacterSubclassDefinition, DeityDefinition> DeityChoiceList
    {
        get;
    } = new();

    private static Dictionary<CharacterSubclassDefinition, FeatureDefinitionSubclassChoice> SubclassesChoiceList
    {
        get;
    } = new();

    internal static void Load()
    {
        // Barbarian
        LoadSubclass(new PathOfTheElements());
        LoadSubclass(new PathOfTheLight());
        LoadSubclass(new PathOfTheReaver());
        LoadSubclass(new PathOfTheSavagery());
        LoadSubclass(new PathOfTheSpirits());
        LoadSubclass(new PathOfTheYeoman());

        // Bard
        LoadSubclass(new CollegeOfAudacity());
        LoadSubclass(new CollegeOfGuts());
        LoadSubclass(new CollegeOfHarlequin());
        LoadSubclass(new CollegeOfLife());
        LoadSubclass(new CollegeOfValiance());
        LoadSubclass(new CollegeOfWarDancer());

        // Cleric
        LoadSubclass(new DomainDefiler());
        LoadSubclass(new DomainSmith());

        // Druid
        LoadSubclass(new CircleOfTheAncientForest());
        LoadSubclass(new CircleOfTheForestGuardian());
        LoadSubclass(new CircleOfTheLife());
        LoadSubclass(new CircleOfTheNight());

        // Fighter
        LoadSubclass(new MartialArcaneArcher());
        LoadSubclass(new MartialDefender());
        LoadSubclass(new MartialMarshal());
        LoadSubclass(new MartialRoyalKnight());
        LoadSubclass(new MartialSpellShield());
        LoadSubclass(new MartialTactician());
        LoadSubclass(new MartialWeaponMaster());

        // Inventor
        LoadSubclass(new InnovationAlchemy());
        LoadSubclass(new InnovationArmor());
        LoadSubclass(new InnovationArtillerist());
        LoadSubclass(new InnovationVitriolist());
        LoadSubclass(new InnovationVivisectionist());
        LoadSubclass(new InnovationWeapon());

        // Paladin
        LoadSubclass(new OathOfAltruism());
        LoadSubclass(new OathOfAncients());
        LoadSubclass(new OathOfDemonHunter());
        LoadSubclass(new OathOfDread());
        LoadSubclass(new OathOfHatred());
        LoadSubclass(new OathOfThunder());

        // Ranger
        LoadSubclass(new RangerArcanist());
        LoadSubclass(new RangerLightBearer());
        LoadSubclass(new RangerHellWalker());
        LoadSubclass(new RangerSkyWarrior());
        LoadSubclass(new RangerSurvivalist());
        LoadSubclass(new RangerWildMaster());

        // Rogue
        LoadSubclass(new RoguishAcrobat());
        LoadSubclass(new RoguishArcaneScoundrel());
        LoadSubclass(new RoguishBladeCaller());
        LoadSubclass(new RoguishDuelist());
        LoadSubclass(new RoguishOpportunist());
        LoadSubclass(new RoguishRaven());
        LoadSubclass(new RoguishSlayer());

        // Sorcerer
        LoadSubclass(new SorcerousDivineHeart());
        LoadSubclass(new SorcerousFieldManipulator());
        LoadSubclass(new SorcerousSorrAkkath());
        LoadSubclass(new SorcerousSpellBlade());

        // Monk
        LoadSubclass(new WayOfTheDiscordance());
        LoadSubclass(new WayOfTheDistantHand());
        LoadSubclass(new WayOfTheDragon());
        LoadSubclass(new WayOfTheSilhouette());
        LoadSubclass(new WayOfTheTempest());
        LoadSubclass(new WayOfTheWealAndWoe());

        // Warlock
        LoadSubclass(new PatronEldritchSurge());
        LoadSubclass(new PatronElementalist());
        LoadSubclass(new PatronMoonlit());
        LoadSubclass(new PatronMountain());
        LoadSubclass(new PatronRiftWalker());
        LoadSubclass(new PatronSoulBlade());

        // Wizard
        LoadSubclass(new WizardArcaneFighter());
        LoadSubclass(new WizardBladeDancer());
        LoadSubclass(new WizardDeadMaster());
        LoadSubclass(new WizardGraviturgist());
        LoadSubclass(new WizardSpellMaster());

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
        else
        {
            throw new Exception("Subclass builder requires a Deity Definition or a SubClassChoice definition");
        }

        SubclassListContextTab.TryAdd(klass, new SubclassListContext(klass));
        SubclassListContextTab[klass].AllSubClasses.Add(subclass);
        Main.Settings.DisplayKlassToggle.TryAdd(klass.Name, true);
        Main.Settings.KlassListSliderPosition.TryAdd(klass.Name, 4);
        Main.Settings.KlassListSubclassEnabled.TryAdd(klass.Name, new List<string>());
    }

    internal static bool IsAllSetSelected()
    {
        return SubclassListContextTab.Values.All(subclassListContext => subclassListContext.IsAllSetSelected);
    }

    internal static void SelectAllSet(bool toggle)
    {
        foreach (var subclassListContext in SubclassListContextTab.Values)
        {
            subclassListContext.SelectAllSetInternal(toggle);
        }
    }

    internal sealed class SubclassListContext
    {
        internal SubclassListContext(CharacterClassDefinition characterClassDefinition)
        {
            Klass = characterClassDefinition;
            AllSubClasses = new HashSet<CharacterSubclassDefinition>();
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
