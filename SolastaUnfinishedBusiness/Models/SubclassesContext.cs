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

        // Druid
        LoadSubclass(new CircleOfTheForestGuardian());

        // Fighter
        LoadSubclass(new MartialMarshal());
        LoadSubclass(new MartialSpellShield());
        LoadSubclass(new MartialTactician());

        // Ranger
        LoadSubclass(new RangerArcanist());

        // Rogue
        LoadSubclass(new RoguishOpportunist());
        LoadSubclass(new RoguishRaven());

        // Sorcerer
        LoadSubclass(new SorcerousDivineHeart());

        // Monk
        LoadSubclass(new WayOfTheDistantHand());
        LoadSubclass(new WayOfSilhouette());

        // Warlock
        LoadSubclass(new PatronMoonlit());
        LoadSubclass(new PatronSoulBlade());

        // Wizard
        LoadSubclass(new WizardBladeDancer());
        LoadSubclass(new WizardDeadMaster());

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

    private static void LoadSubclass([NotNull] AbstractSubclass subclassBuilder)
    {
        var subclass = subclassBuilder.Subclass;

        if (!Subclasses.Contains(subclass))
        {
            SubclassesChoiceList.Add(subclass, subclassBuilder.SubclassChoice);
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
