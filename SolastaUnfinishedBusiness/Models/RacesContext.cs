using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Races;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaUnfinishedBusiness.Models;

internal static class RacesContext
{
    internal static Dictionary<CharacterRaceDefinition, float> RaceScaleMap { get; } = new();

    internal static HashSet<CharacterRaceDefinition> Races { get; private set; } = new();

    private static void SortRacesFeatures()
    {
        foreach (var characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>())
        {
            characterRaceDefinition.FeatureUnlocks.Sort((a, b) =>
            {
                var result = a.Level - b.Level;

                if (result != 0)
                {
                    return result;
                }

                // hack as TA code requires this FEATURE to be last on list
                if (a.FeatureDefinition == FeatureSetDragonbornBreathWeapon)
                {
                    return 1;
                }

                if (b.FeatureDefinition == FeatureSetDragonbornBreathWeapon)
                {
                    return -1;
                }

                return String.Compare(a.FeatureDefinition.FormatTitle(), b.FeatureDefinition.FormatTitle(),
                    StringComparison.CurrentCultureIgnoreCase);
            });
        }
    }

    internal static void Load()
    {
        Morphotypes.Load();

        LoadRace(DarkelfSubraceBuilder.SubraceDarkelf);
        LoadRace(GrayDwarfSubraceBuilder.SubraceGrayDwarf);
        LoadRace(RaceBolgrifBuilder.RaceBolgrif);
        LoadRace(RaceHalfElfVariantRaceBuilder.RaceHalfElfVariant); // depends on DarkElf sub race

        // sorting
        Races = Races.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.RaceEnabled
                     .Where(name => Races.All(x => x.Name != name)))
        {
            Main.Settings.RaceEnabled.Remove(name);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            SortRacesFeatures();
        }
    }

    private static void LoadRace([NotNull] CharacterRaceDefinition characterRaceDefinition)
    {
        if (characterRaceDefinition.SubRaces.Count > 0)
        {
            foreach (var subRace in characterRaceDefinition.SubRaces)
            {
                LoadRace(subRace);
            }
        }
        else
        {
            Races.Add(characterRaceDefinition);
            UpdateRaceVisibility(characterRaceDefinition);
        }
    }

    private static void UpdateRaceVisibility([NotNull] CharacterRaceDefinition characterRaceDefinition)
    {
        characterRaceDefinition.GuiPresentation.hidden =
            !Main.Settings.RaceEnabled.Contains(characterRaceDefinition.Name);

        var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();
        var masterRace = dbCharacterRaceDefinition
            .FirstOrDefault(x => x.SubRaces.Contains(characterRaceDefinition));

        if (masterRace == null)
        {
            return;
        }

        masterRace.GuiPresentation.hidden = masterRace.SubRaces.All(x => x.GuiPresentation.Hidden);
    }

    internal static void Switch(CharacterRaceDefinition characterRaceDefinition, bool active)
    {
        if (!Races.Contains(characterRaceDefinition))
        {
            return;
        }

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
}
