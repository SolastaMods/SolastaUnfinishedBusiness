using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUiCharactersPanel
{
    [HarmonyPatch(typeof(CharacterSelectionModal), "CharactersFiltered")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterSelectionModal_CharactersFiltered
    {
        public static bool Prefix(List<RulesetCharacterHero.Snapshot> filteredHeroesList, List<CharacterPlateToggle> ___characterPlates)
        {
            if (!Main.Settings.FutureFeatureSorting)
            {
                return true;
            }

            // Duplicate the logic of CharactersPanel.CharactersFiltered to fix mapping of filteredHeroesList to character plates

            var cpDict = ___characterPlates.ToDictionary(cp => Path.GetFileNameWithoutExtension(cp.Filename), cp => cp);
            var numHeroes = filteredHeroesList.Count;

            foreach (var h in filteredHeroesList.Select((h, i) => new { Hero = h, Index = i }))
            {
                if (cpDict.TryGetValue(h.Hero.Name, out var characterPlateToggle))
                {
                    cpDict[h.Hero.Name].transform.SetSiblingIndex(numHeroes - h.Index - 1);
                }
            }

            foreach(var kvp in cpDict)
            {
                kvp.Value.gameObject.SetActive(filteredHeroesList.Any(h => h.Name == kvp.Key));
            }

            return false;
        }
    }
}
