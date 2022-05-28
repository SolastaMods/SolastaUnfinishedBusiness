using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp
{
    [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "UpdateRelevance")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageSubclassSelectionPanel_UpdateRelevance
    {
        internal static void Postfix(CharacterStageSubclassSelectionPanel __instance)
        {
            if (LevelUpContext.IsLevelingUp(__instance.currentHero) && LevelUpContext.RequiresDeity(__instance.currentHero))
            {
                __instance.isRelevant = false;
            }
        }
    }
}
