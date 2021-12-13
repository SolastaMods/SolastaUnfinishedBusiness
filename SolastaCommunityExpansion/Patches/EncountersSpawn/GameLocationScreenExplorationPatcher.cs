using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.EncountersSpawn
{
    // use this patch to stage the encounter on the desired location
    [HarmonyPatch(typeof(GameLocationScreenExploration), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationScreenExploration_HandleInput
    {
        internal static void Prefix(InputCommands.Id command)
        {
            if (command == Settings.CTRL_SHIFT_E && Models.EncountersSpawnContext.EncounterCharacters.Count > 0)
            {
                Models.EncountersSpawnContext.ConfirmStageEncounter();
            }
        }
    }
}
