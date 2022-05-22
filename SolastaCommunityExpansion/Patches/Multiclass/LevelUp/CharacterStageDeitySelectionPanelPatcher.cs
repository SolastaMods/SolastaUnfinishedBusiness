using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "UpdateRelevance")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterStageDeitySelectionPanel_UpdateRelevance
    {
        internal static void Postfix(RulesetCharacterHero ___currentHero, ref bool ___isRelevant)
        {
            if (LevelUpContext.IsLevelingUp(___currentHero))
            {
                ___isRelevant = LevelUpContext.RequiresDeity(___currentHero);
            }
        }
    }
}
