using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Encounters
{
    // this patch spawns the encounter in the informed location
    [HarmonyPatch(typeof(GameLocationScreenBattle), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationScreenBattle_HandleInput
    {
        internal static void Postfix(InputCommands.Id command)
        {
            Models.EncountersSpawnContext.ConfirmStageEncounter(command);
        }
    }
}
