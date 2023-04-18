using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Models;

internal static class SubclassesContext
{
    private static Dictionary<CharacterSubclassDefinition, DeityDefinition> DeityChoiceList
    {
        get;
    } = new();

    private static Dictionary<CharacterSubclassDefinition, FeatureDefinitionSubclassChoice> SubclassesChoiceList
    {
        get;
    } = new();

    internal static HashSet<CharacterSubclassDefinition> Subclasses { get; } = new();

    internal static void Load()
    {
        // Barbarian
        LoadSubclass(new PathOfTheElements());
        LoadSubclass(new PathOfTheLight());
        LoadSubclass(new PathOfTheReaver());
        LoadSubclass(new PathOfTheSavagery());
        LoadSubclass(new PathOfTheSpirits());

        // Bard
        LoadSubclass(new CollegeOfGuts());
        LoadSubclass(new CollegeOfHarlequin());
        LoadSubclass(new CollegeOfLife());
        LoadSubclass(new CollegeOfWarDancer());

        // Cleric
        LoadSubclass(new DomainDefiler());
        LoadSubclass(new DomainSmith());

        // Druid
        LoadSubclass(new CircleOfTheAncientForest());
        LoadSubclass(new CircleOfTheForestGuardian());
        LoadSubclass(new CircleOfTheNight());

        // Fighter
        LoadSubclass(new MartialMarshal());
        LoadSubclass(new MartialRoyalKnight());
        LoadSubclass(new MartialSpellShield());
        LoadSubclass(new MartialTactician());
        LoadSubclass(new MartialWeaponMaster());

        // Paladin
        LoadSubclass(new OathOfAltruism());
        LoadSubclass(new OathOfAncients());
        LoadSubclass(new OathOfDread());
        LoadSubclass(new OathOfHatred());

        // Ranger
        LoadSubclass(new RangerArcanist());
        LoadSubclass(new RangerLightBearer());
        LoadSubclass(new RangerHellWalker());
        LoadSubclass(new RangerWildMaster());

        // Rogue
        LoadSubclass(new RoguishAcrobat());
        LoadSubclass(new RoguishArcaneScoundrel());
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

    internal static void LateLoad()
    {
        CollegeOfLife.LateLoad();
    }

    private static void LoadSubclass([NotNull] AbstractSubclass subclassBuilder, bool isBetaContent = false)
    {
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

        if (SubclassesChoiceList.TryGetValue(characterSubclassDefinition, out var choiceList))
        {
            if (Main.Settings.SubclassEnabled.Contains(name))
            {
                choiceList.Subclasses.TryAdd(name);
            }
            else
            {
                choiceList.Subclasses.Remove(name);
            }
        }
        else if (DeityChoiceList.TryGetValue(characterSubclassDefinition, out var deityDefinition))
        {
            if (Main.Settings.SubclassEnabled.Contains(name))
            {
                deityDefinition.Subclasses.TryAdd(name);
            }
            else
            {
                deityDefinition.Subclasses.Remove(name);
            }
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
