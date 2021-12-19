using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Encounters
{
    [HarmonyPatch(typeof(BattleState_TurnInitialize), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BattleState_TurnInitialize_Begin
    {
        internal static void Prefix(BattleState_TurnInitialize __instance)
        {
            Models.PlayerControllerContext.Start(__instance.Battle);
        }
    }

    [HarmonyPatch(typeof(BattleState_TurnEnd), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BattleState_TurnEnd_Begin
    {
        internal static void Prefix(BattleState_TurnEnd __instance)
        {
            Models.PlayerControllerContext.Stop(__instance.Battle);
        }
    }

    [HarmonyPatch(typeof(BattleState_Victory), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BattleState_Victory_Begin
    {
        internal static void Prefix(BattleState_Victory __instance)
        {
            Models.PlayerControllerContext.Stop(__instance.Battle);
        }
    }
}
