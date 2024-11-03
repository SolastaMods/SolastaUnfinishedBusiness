using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Races;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;

namespace SolastaUnfinishedBusiness.Models;

internal static class RacesContext
{
    private static readonly FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityDarknessPerceptive =
        FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityDarknessPerceptive")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage,
                abilityProficiencyPairs: (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddCustomSubFeatures(ValidatorsCharacter.IsUnlitOrDarkness)
            .AddToDB();

    internal static Dictionary<CharacterRaceDefinition, float> RaceScaleMap { get; } = [];
    internal static HashSet<CharacterRaceDefinition> Races { get; private set; } = [];
    internal static HashSet<CharacterRaceDefinition> Subraces { get; private set; } = [];

    internal static void Load()
    {
        Morphotypes.Load();

        LoadRace(RaceBattlebornBuilder.RaceBattleborn);
        LoadRace(RaceBolgrifBuilder.RaceBolgrif);
        LoadRace(RaceFairyBuilder.RaceFairy);
        LoadRace(RaceImpBuilder.RaceImp);
        LoadRace(RaceKoboldBuilder.RaceKobold);
        LoadRace(RaceMalakhBuilder.RaceMalakh);
        LoadRace(RaceOligathBuilder.RaceOligath);
        LoadRace(RaceWendigoBuilder.RaceWendigo);
        LoadRace(RaceWildlingBuilder.RaceWildling);
        LoadRace(RaceWyrmkinBuilder.RaceWyrmkin);
        LoadRace(RaceLizardfolkBuilder.RaceLizardfolk);
        LoadRace(RaceOniBuilder.RaceOni);

        _ = RaceTieflingBuilder.RaceTiefling;

        LoadSubrace(RaceTieflingBuilder.RaceTieflingDevilTongue);
        LoadSubrace(RaceTieflingBuilder.RaceTieflingFeral);
        LoadSubrace(RaceTieflingBuilder.RaceTieflingMephistopheles);
        LoadSubrace(RaceTieflingBuilder.RaceTieflingZariel);

        _ = RaceHalfElfBuilder.RaceHalfElfVariant;

        LoadSubrace(RaceHalfElfBuilder.RaceHalfElfHighVariant);
        LoadSubrace(RaceHalfElfBuilder.RaceHalfElfSylvanVariant);
        LoadSubrace(RaceHalfElfBuilder.RaceHalfElfDarkVariant);

        LoadSubrace(SubraceDarkelfBuilder.SubraceDarkelf);
        LoadSubrace(SubraceGrayDwarfBuilder.SubraceGrayDwarf);
        LoadSubrace(SubraceIronbornDwarfBuilder.SubraceIronbornDwarf);
        LoadSubrace(SubraceObsidianDwarfBuilder.SubraceObsidianDwarf);
        LoadSubrace(SubraceShadarKaiBuilder.SubraceShadarKai);

        // sorting
        Races = Races.OrderBy(x => x.FormatTitle()).ToHashSet();
        Subraces = Subraces.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.RaceEnabled
                     .Where(name => Races.All(x => x.Name != name))
                     .ToArray())
        {
            Main.Settings.RaceEnabled.Remove(name);
        }

        foreach (var name in Main.Settings.SubraceEnabled
                     .Where(name => Subraces.All(x => x.Name != name))
                     .ToArray())
        {
            Main.Settings.SubraceEnabled.Remove(name);
        }

        DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
            .Do(x => x.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock));

        // final bootstrap
        LoadVision();
        SwitchDarknessPerceptive();
        SwitchDragonbornElementalBreathUsages();
        FlexibleBackgroundsContext.Load();
        FlexibleBackgroundsContext.SwitchFlexibleBackgrounds();
        FlexibleRacesContext.SwitchFlexibleRaces();
    }

    private static void LoadRace([NotNull] CharacterRaceDefinition characterRaceDefinition)
    {
        Races.Add(characterRaceDefinition);
        UpdateRaceVisibility(characterRaceDefinition);
    }

    private static void LoadSubrace([NotNull] CharacterRaceDefinition characterRaceDefinition)
    {
        Subraces.Add(characterRaceDefinition);
        UpdateSubraceVisibility(characterRaceDefinition);
    }

    private static void UpdateRaceVisibility([NotNull] CharacterRaceDefinition characterRaceDefinition)
    {
        characterRaceDefinition.GuiPresentation.hidden =
            !Main.Settings.RaceEnabled.Contains(characterRaceDefinition.Name);

        characterRaceDefinition.SubRaces.ForEach(x => x.GuiPresentation.hidden =
            !Main.Settings.RaceEnabled.Contains(characterRaceDefinition.Name));
    }

    private static void UpdateSubraceVisibility([NotNull] CharacterRaceDefinition characterRaceDefinition)
    {
        characterRaceDefinition.GuiPresentation.hidden =
            !Main.Settings.SubraceEnabled.Contains(characterRaceDefinition.Name);

        if (RaceHalfElfBuilder.RaceHalfElfVariant.SubRaces.Contains(characterRaceDefinition))
        {
            var hidden = RaceHalfElfBuilder.RaceHalfElfVariant.SubRaces.All(x => x.GuiPresentation.Hidden);

            RaceHalfElfBuilder.RaceHalfElfVariant.GuiPresentation.hidden = hidden;
        }
        else if (RaceTieflingBuilder.RaceTiefling.SubRaces.Contains(characterRaceDefinition))
        {
            var hidden = RaceTieflingBuilder.RaceTiefling.SubRaces.All(x => x.GuiPresentation.Hidden);

            RaceTieflingBuilder.RaceTiefling.GuiPresentation.hidden = hidden;
        }
    }

    internal static void Switch(CharacterRaceDefinition characterRaceDefinition, bool active)
    {
        var name = characterRaceDefinition.Name;

        if (active)
        {
            Main.Settings.RaceEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.RaceEnabled.Remove(name);
        }

        UpdateRaceVisibility(characterRaceDefinition);
    }

    internal static void SwitchSubrace(CharacterRaceDefinition characterRaceDefinition, bool active)
    {
        var name = characterRaceDefinition.Name;

        if (active)
        {
            Main.Settings.SubraceEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.SubraceEnabled.Remove(name);
        }

        UpdateSubraceVisibility(characterRaceDefinition);
    }

    private static void LoadVision()
    {
        if (Main.Settings.DisableSenseDarkVisionFromAllRaces)
        {
            foreach (var featureUnlocks in DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
                         .Select(crd => crd.FeatureUnlocks))
            {
                featureUnlocks.RemoveAll(x => x.FeatureDefinition == SenseDarkvision);
                // Half-orcs have a different darkvision.
                featureUnlocks.RemoveAll(x => x.FeatureDefinition == SenseDarkvision12);
            }
        }

        // ReSharper disable once InvertIf
        if (Main.Settings.DisableSenseSuperiorDarkVisionFromAllRaces)
        {
            foreach (var featureUnlocks in DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
                         .Select(crd => crd.FeatureUnlocks))
            {
                featureUnlocks.RemoveAll(x => x.FeatureDefinition == SenseSuperiorDarkvision);
            }
        }
    }

    internal static void SwitchDarknessPerceptive()
    {
        var races = new List<CharacterRaceDefinition>
        {
            RaceKoboldBuilder.SubraceDarkKobold,
            SubraceDarkelfBuilder.SubraceDarkelf,
            SubraceGrayDwarfBuilder.SubraceGrayDwarf
        };

        if (Main.Settings.AddDarknessPerceptiveToDarkRaces)
        {
            foreach (var characterRaceDefinition in races
                         .Where(a => !a.FeatureUnlocks.Exists(x =>
                             x.Level == 1 && x.FeatureDefinition == AbilityCheckAffinityDarknessPerceptive)))
            {
                characterRaceDefinition.FeatureUnlocks.Add(
                    new FeatureUnlockByLevel(AbilityCheckAffinityDarknessPerceptive, 1));
            }
        }
        else
        {
            foreach (var characterRaceDefinition in races
                         .Where(a => a.FeatureUnlocks.Exists(x =>
                             x.Level == 1 && x.FeatureDefinition == AbilityCheckAffinityDarknessPerceptive)))
            {
                characterRaceDefinition.FeatureUnlocks.RemoveAll(x =>
                    x.Level == 1 && x.FeatureDefinition == AbilityCheckAffinityDarknessPerceptive);
            }
        }
    }

    internal static void SwitchDragonbornElementalBreathUsages()
    {
        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
            .Where(x =>
                x.Name.StartsWith("PowerDragonbornBreathWeapon") ||
                x.Name == "PowerFeatDragonFear");

        foreach (var power in powers)
        {
            if (Main.Settings.ChangeDragonbornElementalBreathUsages)
            {
                power.usesAbilityScoreName = AttributeDefinitions.Constitution;
                power.usesDetermination = UsesDetermination.AbilityBonusPlusFixed;
                power.fixedUsesPerRecharge = 0;
            }
            else
            {
                power.usesAbilityScoreName = AttributeDefinitions.Charisma;
                power.usesDetermination = UsesDetermination.Fixed;
                power.fixedUsesPerRecharge = 1;
            }
        }
    }
}
