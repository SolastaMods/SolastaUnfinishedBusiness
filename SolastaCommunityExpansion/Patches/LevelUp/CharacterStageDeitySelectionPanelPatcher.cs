using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp;

[HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "UpdateRelevance")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterStageDeitySelectionPanel_UpdateRelevance
{
    internal static void Postfix(CharacterStageDeitySelectionPanel __instance)
    {
        if (LevelUpContext.IsLevelingUp(__instance.currentHero))
        {
            __instance.isRelevant = LevelUpContext.RequiresDeity(__instance.currentHero);
        }
    }
}
