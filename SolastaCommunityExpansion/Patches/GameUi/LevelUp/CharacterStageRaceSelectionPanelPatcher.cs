using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(CharacterStageRaceSelectionPanel), "Compare")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageRaceSelectionPanel_Compare
    {
        internal static void Postfix(CharacterRaceDefinition left, CharacterRaceDefinition right, ref int __result)
        {
            if (!Main.Settings.EnableSortingRaces)
            {
                return;
            }

            __result = left.FormatTitle().CompareTo(right.FormatTitle());
        }
    }

    // avoids a restart when enabling / disabling races on the Mod UI panel
    [HarmonyPatch(typeof(CharacterStageRaceSelectionPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageRaceSelectionPanel_OnBeginShow
    {
        internal static void Prefix(CharacterStageRaceSelectionPanel __instance)
        {
            var visibleRaces = DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
                .Where(x => !x.GuiPresentation.Hidden);
            var visibleSubRaces = visibleRaces.SelectMany(x => x.SubRaces);
            var visibleMainRaces = visibleRaces.Where(x => !visibleSubRaces.Contains(x));

            __instance.eligibleRaces.SetRange(visibleMainRaces.OrderBy(x => x.FormatTitle()));
            __instance.selectedSubRace.Clear();

            for (var key = 0; key < visibleMainRaces.Count(); ++key)
            {
                __instance.selectedSubRace[key] = 0;
            }
        }
    }
}
