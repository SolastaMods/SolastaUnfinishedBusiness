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

        LoadRace(BolgrifRaceBuilder.BolgrifRace);
        LoadRace(DarkelfSubraceBuilder.DarkelfSubrace);
        LoadRace(GnomeRaceBuilder.GnomeRace);
        LoadRace(HalfElfVariantRaceBuilder.HalfElfVariantRace); // depends on DarkElf sub race

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

    private static void UpdateRaceVisibility(CharacterRaceDefinition characterRaceDefinition)
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
