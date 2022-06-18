using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Races;

namespace SolastaCommunityExpansion.Models;

internal static class RacesContext
{
    internal static Dictionary<CharacterRaceDefinition, float> RaceScaleMap { get; } = new();

    internal static HashSet<CharacterRaceDefinition> Races { get; private set; } = new();

    private static void SortRacesFeatures()
    {
        var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();

        foreach (var characterRaceDefinition in dbCharacterRaceDefinition)
        {
            characterRaceDefinition.FeatureUnlocks.Sort((a, b) =>
            {
                var result = a.Level - b.Level;

                if (result == 0)
                {
                    result = a.FeatureDefinition.FormatTitle().CompareTo(b.FeatureDefinition.FormatTitle());
                }

                return result;
            });
        }
    }

    internal static void Load()
    {
        Morphotypes.Load();

        //
        // TODO: Add this to a setting on UI
        //
        _ = DarkelfSubraceBuilder.DarkelfSubrace;

        LoadRace(BolgrifRaceBuilder.BolgrifRace);
        LoadRace(GnomeRaceBuilder.GnomeRace);
        LoadRace(HalfElfVariantRaceBuilder.HalfElfVariantRace); // depends on DarkElf subrace

        Races = Races.OrderBy(x => x.FormatTitle()).ToHashSet();

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            SortRacesFeatures();
        }

        RaceScaleMap[BolgrifRaceBuilder.BolgrifRace] = 8.8f / 6.4f;
        RaceScaleMap[GnomeRaceBuilder.GnomeRace] = -0.04f / -0.06f;
    }

    private static void LoadRace(CharacterRaceDefinition characterRaceDefinition)
    {
        if (!Races.Contains(characterRaceDefinition))
        {
            Races.Add(characterRaceDefinition);
        }

        UpdateRaceVisibility(characterRaceDefinition);
    }

    private static void UpdateRaceVisibility(CharacterRaceDefinition characterRaceDefinition)
    {
        characterRaceDefinition.SubRaces.ForEach(x =>
            x.GuiPresentation.hidden = !Main.Settings.RaceEnabled.Contains(characterRaceDefinition.Name));
        characterRaceDefinition.GuiPresentation.hidden =
            !Main.Settings.RaceEnabled.Contains(characterRaceDefinition.Name);
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
