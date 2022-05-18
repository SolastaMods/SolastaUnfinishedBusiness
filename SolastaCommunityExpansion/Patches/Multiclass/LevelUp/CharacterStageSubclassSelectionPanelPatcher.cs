using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "UpdateRelevance")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageSubclassSelectionPanel_UpdateRelevance
    {
        internal static void Postfix(RulesetCharacterHero ___currentHero, ref bool ___isRelevant)
        {
            if (LevelUpContext.IsLevelingUp(___currentHero) && LevelUpContext.RequiresDeity(___currentHero))
            {
                ___isRelevant = false;
            }
        }
    }
}
