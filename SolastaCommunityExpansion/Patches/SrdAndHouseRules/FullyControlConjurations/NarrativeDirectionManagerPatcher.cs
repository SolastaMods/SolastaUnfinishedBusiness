using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.FullyControlConjurations
{
    [HarmonyPatch(typeof(NarrativeDirectionManager), "PrepareDialogSequence")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class NarrativeDirectionManager_StartDialogSequence_Patch
    {
        internal static void Prefix(List<GameLocationCharacter> involvedGameCharacters)
        {
            if (!Main.Settings.FullyControlConjurations)
            {
                return;
            }

            involvedGameCharacters.RemoveAll(
                x => x.RulesetCharacter is RulesetCharacterMonster rulesetCharacterMonster
                    && Models.ConjurationsContext.ConjuredMonsters.Contains(rulesetCharacterMonster.MonsterDefinition));
        }
    }
}
