using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.PlayerController
{
    // these patches init / shutdowns the Hero AI system
    [HarmonyPatch(typeof(BattleState_Intro), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BattleState_Intro_Begin
    {
        internal static void Prefix()
        {
            Models.PlayerControllerContext.PlayerInControlOfEnemy = false;
        }
    }

    [HarmonyPatch(typeof(BattleState_TurnInitialize), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BattleState_TurnInitialize_Begin
    {
        internal static void Prefix()
        {
            Models.PlayerControllerContext.Start();
        }
    }

    [HarmonyPatch(typeof(BattleState_TurnEnd), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BattleState_TurnEnd_Begin
    {
        internal static void Prefix()
        {
            Models.PlayerControllerContext.Stop();
        }
    }

    [HarmonyPatch(typeof(BattleState_Victory), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BattleState_Victory_Begin
    {
        internal static void Prefix()
        {
            Models.PlayerControllerContext.Stop();
        }
    }
}
