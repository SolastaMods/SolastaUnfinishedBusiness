using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.LevelUp;

[HarmonyPatch(typeof(CharacterStageRaceSelectionPanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageRaceSelectionPanel_OnBeginShow
{
    internal static void Prefix(CharacterStageRaceSelectionPanel __instance)
    {
        var allRaces = new List<CharacterRaceDefinition>();
        var subRaces = new List<CharacterRaceDefinition>();

        allRaces.AddRange(DatabaseRepository.GetDatabase<CharacterRaceDefinition>().GetAllElements()
            .Where(x => !x.GuiPresentation.Hidden));

        __instance.eligibleRaces.Clear();
        __instance.selectedSubRace.Clear();
        __instance.sortedSubRaces.Clear();

        foreach (var characterRaceDefinition in allRaces
                     .Where(x => x.SubRaces != null && x.SubRaces.Count > 0))
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
