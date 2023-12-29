using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Races;

namespace SolastaUnfinishedBusiness.Models;

internal static class RacesContext
{
    internal static Dictionary<CharacterRaceDefinition, float> RaceScaleMap { get; } = [];

    internal static HashSet<CharacterRaceDefinition> Races { get; private set; } = [];

    internal static void Load()
    {
        Morphotypes.Load();

        LoadRace(RaceBattlebornBuilder.RaceBattleborn);
        LoadRace(RaceBolgrifBuilder.RaceBolgrif);
        LoadRace(RaceFairyBuilder.RaceFairy);
        LoadRace(RaceHalfElfBuilder.RaceHalfElfVariant);
        LoadRace(RaceImpBuilder.RaceImp);
        LoadRace(RaceKoboldBuilder.RaceKobold);
        LoadRace(RaceMalakhBuilder.RaceMalakh);
        LoadRace(RaceOligathBuilder.RaceOligath);
        LoadRace(RaceTieflingBuilder.RaceTiefling);
        LoadRace(RaceWendigoBuilder.RaceWendigo);
        LoadRace(RaceWildlingBuilder.RaceWildling);
        LoadRace(RaceWyrmkinBuilder.RaceWyrmkin);
        LoadRace(RaceOniBuilder.RaceOni);
        LoadRace(SubraceDarkelfBuilder.SubraceDarkelf);
        LoadRace(SubraceGrayDwarfBuilder.SubraceGrayDwarf);
        LoadRace(SubraceIronbornDwarfBuilder.SubraceIronbornDwarf);
        LoadRace(SubraceObsidianDwarfBuilder.SubraceObsidianDwarf);

        // sorting
        Races = Races.OrderBy(x => x.FormatTitle()).ToHashSet();

        // settings paring
        foreach (var name in Main.Settings.RaceEnabled
                     .Where(name => Races.All(x => x.Name != name))
                     .ToList())
        {
            Main.Settings.RaceEnabled.Remove(name);
        }

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
                .Do(x => x.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock));
        }
    }

    private static void LoadRace([NotNull] CharacterRaceDefinition characterRaceDefinition)
    {
        Races.Add(characterRaceDefinition);
        UpdateRaceVisibility(characterRaceDefinition);
        // if (characterRaceDefinition.SubRaces.Count > 0)
        // {
        //     foreach (var subRace in characterRaceDefinition.SubRaces)
        //     {
        //         LoadRace(subRace);
        //     }
        // }
        // else
        // {
        //     Races.Add(characterRaceDefinition);
        //     UpdateRaceVisibility(characterRaceDefinition);
        // }
    }

    private static void UpdateRaceVisibility([NotNull] CharacterRaceDefinition characterRaceDefinition)
    {
        characterRaceDefinition.GuiPresentation.hidden =
            !Main.Settings.RaceEnabled.Contains(characterRaceDefinition.Name);

        characterRaceDefinition.SubRaces.ForEach(x => x.GuiPresentation.hidden =
            !Main.Settings.RaceEnabled.Contains(characterRaceDefinition.Name));

        // var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();
        // var masterRace = dbCharacterRaceDefinition
        //     .FirstOrDefault(x => x.SubRaces.Contains(characterRaceDefinition));
        //
        // if (masterRace == null)
        // {
        //     return;
        // }
        //
        // masterRace.GuiPresentation.hidden = masterRace.SubRaces.All(x => x.GuiPresentation.Hidden);
    }

    internal static void Switch(CharacterRaceDefinition characterRaceDefinition, bool active)
    {
        // if (!Races.Contains(characterRaceDefinition))
        // {
        //     return;
        // }

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
