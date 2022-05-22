using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Tools.DefaultParty
{
    [HarmonyPatch(typeof(CharacterSelectionModal), "EnumeratePlates")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterSelectionModal_EnumeratePlates
    {
        internal static void Prefix()
        {
            var heroes = ServiceRepository.GetService<ICharacterPoolService>().Pool.Keys.ToList();

            Main.Settings.TestPartyHeroes.RemoveAll(x => !heroes.Contains(x));
        }
    }
}
