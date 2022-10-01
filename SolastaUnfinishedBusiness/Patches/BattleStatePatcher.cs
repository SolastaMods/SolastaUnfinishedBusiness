using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[HarmonyPatch(typeof(BattleState_TurnInitialize), "Begin")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class BattleState_TurnInitialize_Begin
{
    internal static void Prefix(BattleState_TurnInitialize __instance)
    {
        //PATCH: EnableHeroesControlledByComputer and EnableEnemiesControlledByPlayer
        PlayerControllerContext.Start(__instance.Battle);
    }
}

[HarmonyPatch(typeof(BattleState_TurnEnd), "Begin")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class BattleState_TurnEnd_Begin
{
    internal static void Prefix(BattleState_TurnEnd __instance)
    {
        //PATCH: EnableHeroesControlledByComputer and EnableEnemiesControlledByPlayer
        PlayerControllerContext.Stop(__instance.Battle);
    }
}

[HarmonyPatch(typeof(BattleState_Victory), "Begin")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class BattleState_Victory_Begin
{
    internal static void Prefix(BattleState_Victory __instance)
    {
        //PATCH: EnableHeroesControlledByComputer and EnableEnemiesControlledByPlayer
        PlayerControllerContext.Stop(__instance.Battle);
    }
}

[HarmonyPatch(typeof(BattleState_Victory), "Update")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class BattleState_Victory_Update
{
    public static void Postfix()
    {
        //PATCH: AutoPauseOnVictory
        if (!Main.Settings.AutoPauseOnVictory)
        {
            return;
        }

        if (Gui.Battle != null)
        {
            return;
        }

        if (ServiceRepository.GetService<INarrativeDirectionService>()?.CurrentSequence != null)
        {
            // Don't pause in the middle of a narrative sequence as it hangs the game
            // For example during the tutorial shoving the rock to destroy the bridge transitions
            // directly into a narrative sequence. I believe there are several other battle ->
            // narrative transitions in the game like the crown vision paladin fight
            return;
        }

        Gui.PauseGameAsNeeded();
    }
}
