using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Encounters
{
    // this patch spawns the encounter in the informed location
    [HarmonyPatch(typeof(GameLocationScreenExploration), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationScreenExploration_HandleInput
    {
        internal static void Postfix(InputCommands.Id command)
        {
            Models.EncountersSpawnContext.ConfirmStageEncounter(command);
        }
    }
}
