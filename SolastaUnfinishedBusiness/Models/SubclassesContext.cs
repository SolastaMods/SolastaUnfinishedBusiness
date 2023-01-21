using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Models;

internal static class SubclassesContext
{
    private static Dictionary<CharacterSubclassDefinition, FeatureDefinitionSubclassChoice> SubclassesChoiceList
    {
        get;
    } = new();

    internal static HashSet<CharacterSubclassDefinition> Subclasses { get; } = new();

    internal static void Load()
    {
        // Barbarian
        LoadSubclass(new PathOfTheLight());
        LoadSubclass(new PathOfTheSpirits());

        // Bard
        LoadSubclass(new CollegeOfGuts());
        LoadSubclass(new CollegeOfHarlequin());
        LoadSubclass(new CollegeOfLife());
        LoadSubclass(new CollegeOfWarDancer());

        // Druid
        LoadSubclass(new CircleOfTheAncientForest());
        LoadSubclass(new CircleOfTheForestGuardian());
        LoadSubclass(new CircleOfTheNight());

        // Fighter
        LoadSubclass(new MartialMarshal());
        LoadSubclass(new MartialRoyalKnight());
        LoadSubclass(new MartialSpellShield());
        LoadSubclass(new MartialTactician());

        // Paladin
        LoadSubclass(new OathOfAncients());
        LoadSubclass(new OathOfHatred());

        // Ranger
        LoadSubclass(new RangerArcanist());
        LoadSubclass(new RangerWildMaster());

        // Rogue
        LoadSubclass(new RoguishOpportunist());
        LoadSubclass(new RoguishRaven());

        // Sorcerer
        LoadSubclass(new SorcerousDivineHeart());
        LoadSubclass(new SorcerousFieldManipulator());

        // Monk
        LoadSubclass(new WayOfTheDistantHand());
        LoadSubclass(new WayOfTheDragon(), true);
        LoadSubclass(new WayOfTheSilhouette());

        // Warlock
        LoadSubclass(new PatronElementalist());
        LoadSubclass(new PatronMoonlit());
        LoadSubclass(new PatronRiftWalker());
        LoadSubclass(new PatronSoulBlade());

        // Wizard
        LoadSubclass(new WizardArcaneFighter());
        LoadSubclass(new WizardBladeDancer());
        LoadSubclass(new WizardDeadMaster());
        LoadSubclass(new WizardSpellMaster());

        // settings paring
        foreach (var name in Main.Settings.SubclassEnabled
                     .Where(name => Subclasses.All(x => x.Name != name))
                     .ToList())
        {
            Main.Settings.SubclassEnabled.Remove(name);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
                .Do(x => x.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock));
        }
    }

    private static void LoadSubclass([NotNull] AbstractSubclass subclassBuilder, bool isBetaContent = false)
    {
        var subclass = subclassBuilder.Subclass;

        SubclassesChoiceList.Add(subclass, subclassBuilder.SubclassChoice);

        if (isBetaContent && !Main.Settings.EnableBetaContent)
        {
            return;
        }

        if (!Subclasses.Contains(subclass))
        {
            Subclasses.Add(subclass);
        }

        UpdateSubclassVisibility(subclass);
    }

    private static void UpdateSubclassVisibility([NotNull] CharacterSubclassDefinition characterSubclassDefinition)
    {
        var name = characterSubclassDefinition.Name;
        var choiceList = SubclassesChoiceList[characterSubclassDefinition];

        if (Main.Settings.SubclassEnabled.Contains(name))
        {
            choiceList.Subclasses.TryAdd(name);
        }
        else
        {
            choiceList.Subclasses.Remove(name);
        }
    }

    internal static void Switch(CharacterSubclassDefinition characterSubclassDefinition, bool active)
    {
        if (!Subclasses.Contains(characterSubclassDefinition))
        {
            return;
        }

        var name = characterSubclassDefinition.Name;

        if (active)
        {
            Main.Settings.SubclassEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.SubclassEnabled.Remove(name);
        }

        UpdateSubclassVisibility(characterSubclassDefinition);
    }
}
