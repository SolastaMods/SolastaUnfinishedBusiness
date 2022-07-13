using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Patches.BugFix;

// fix issue on 1.3.81b not allowing point buy
[HarmonyPatch(typeof(CharacterStageAbilityScoresPanel), "OnMethodToggleChangedCb")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageAbilityScoresPanel_OnMethodToggleChangedCb
{
    internal static void Prefix([NotNull] CharacterStageAbilityScoresPanel __instance, out bool on)
    {
        on = __instance.currentMethod == CharacterStageAbilityScoresPanel.AbilityScoreMethod.DiceRolls;
    }
}
