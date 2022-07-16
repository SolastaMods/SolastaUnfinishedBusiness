using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Properties;

namespace SolastaCommunityExpansion.Models;

internal static class AdditionalNamesContext
{
    internal static void Load()
    {
        if (!Main.Settings.OfferAdditionalLoreFriendlyNames)
        {
            return;
        }

        var payload = Resources.Names;
        var lines = new List<string>(payload.Split(new[] {Environment.NewLine}, StringSplitOptions.None));

        foreach (var line in lines)
        {
            var columns = line.Split(new[] {'\t'}, 3);
            var raceName = columns[0];
            var gender = columns[1];
            var name = columns[2];
            var race = (CharacterRaceDefinition)AccessTools
                .Property(typeof(DatabaseHelper.CharacterRaceDefinitions), raceName).GetValue(null);
            var racePresentation = race.RacePresentation;
            var options = (List<string>)
                AccessTools.Property(racePresentation.GetType(), $"{gender}NameOptions").GetValue(racePresentation);

            options.Add(name);
        }
    }
}
