using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Infrastructure;

namespace SolastaCommunityExpansion.Patches.LevelUp;

//PATCH: sorts the races panel by Title
[HarmonyPatch(typeof(CharacterStageRaceSelectionPanel), "Compare")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageRaceSelectionPanel_Compare
{
    internal static void Postfix(CharacterRaceDefinition left, CharacterRaceDefinition right, ref int __result)
    {
        if (Main.Settings.EnableSortingRaces)
        {
            __result = String.Compare(left.FormatTitle(), right.FormatTitle(),
                StringComparison.CurrentCultureIgnoreCase);
        }
    }
}

//TODO: consolidate this patch with one below this
[HarmonyPatch(typeof(CharacterStageRaceSelectionPanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageRaceSelectionPanel_OnBeginShow_1
{
    internal static void Prefix([NotNull] CharacterStageRaceSelectionPanel __instance)
    {
        //PATCH: avoids a restart when enabling / disabling races on the Mod UI panel
        var visibleRaces = DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
            .Where(x => !x.GuiPresentation.Hidden);
        var characterRaceDefinitions = visibleRaces as CharacterRaceDefinition[] ?? visibleRaces.ToArray();
        var visibleSubRaces = characterRaceDefinitions.SelectMany(x => x.SubRaces);
        var visibleMainRaces = characterRaceDefinitions.Where(x => !visibleSubRaces.Contains(x));

        var raceDefinitions = visibleMainRaces as CharacterRaceDefinition[] ?? visibleMainRaces.ToArray();
        __instance.eligibleRaces.SetRange(raceDefinitions.OrderBy(x => x.FormatTitle()));
        __instance.selectedSubRace.Clear();

        for (var key = 0; key < raceDefinitions.Length; ++key)
        {
            __instance.selectedSubRace[key] = 0;
        }
    }
}

[HarmonyPatch(typeof(CharacterStageRaceSelectionPanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageRaceSelectionPanel_OnBeginShow
{
    internal static void Prefix([NotNull] CharacterStageRaceSelectionPanel __instance)
    {
        var allRaces = new List<CharacterRaceDefinition>();
        var subRaces = new List<CharacterRaceDefinition>();

        allRaces.AddRange(DatabaseRepository.GetDatabase<CharacterRaceDefinition>().GetAllElements()
            .Where(x => !x.GuiPresentation.Hidden));

        __instance.eligibleRaces.Clear();
        __instance.selectedSubRace.Clear();
        __instance.sortedSubRaces.Clear();

        foreach (var characterRaceDefinition in allRaces
                     .Where(x => x.SubRaces is { Count: > 0 }))
        {
            if (characterRaceDefinition.SubRaces.Count > __instance.maxSubRacesPerRace)
            {
                __instance.maxSubRacesPerRace = characterRaceDefinition.SubRaces.Count;
            }

            foreach (var subRace in characterRaceDefinition.SubRaces)
            {
                subRaces.TryAdd(subRace);
            }
        }

        var num = 0;

        foreach (var key in allRaces
                     .Where(x => !subRaces.Contains(x)))
        {
            __instance.eligibleRaces.Add(key);
            __instance.selectedSubRace.Add(num++, 0);
            __instance.sortedSubRaces.Add(key, new List<CharacterRaceDefinition>());

            if (key.SubRaces.Count == 0)
            {
                continue;
            }

            foreach (var subRace in key.SubRaces
                         .Where(x => !x.GuiPresentation.Hidden))
            {
                __instance.sortedSubRaces[key].Add(subRace);
                __instance.sortedSubRaces[key].Sort(__instance);
            }
        }

        __instance.eligibleRaces.Sort(__instance);
    }
}
