using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.HeroController
{
    // these patches init / shutdowns the Hero AI system
    [HarmonyPatch(typeof(BattleState_TurnInitialize), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BattleState_TurnInitialize_Begin
    {
        internal static void Prefix()
        {
            Models.HeroControllerContext.Start();
        }
    }

    [HarmonyPatch(typeof(BattleState_TurnEnd), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BattleState_TurnEnd_Begin
    {
        internal static void Prefix()
        {
            Models.HeroControllerContext.Stop();
        }
    }

    [HarmonyPatch(typeof(BattleState_Victory), "Begin")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BattleState_Victory_Begin
    {
        internal static void Prefix()
        {
            Models.HeroControllerContext.Stop();
        }
    }
}
