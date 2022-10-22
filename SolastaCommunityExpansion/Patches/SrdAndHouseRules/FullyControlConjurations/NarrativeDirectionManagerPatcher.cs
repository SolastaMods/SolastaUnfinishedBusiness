using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.FullyControlConjurations;

[HarmonyPatch(typeof(NarrativeDirectionManager), "PrepareDialogSequence")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class NarrativeDirectionManager_StartDialogSequence_Patch
{
    internal static void Prefix(List<GameLocationCharacter> involvedGameCharacters)
    {
        //PATCH: Only offer the first 4 players on dialogue sequences (PARTYSIZE)
        if (Main.Settings.OverridePartySize > DungeonMakerContext.GamePartySize)
        {
            var party = Gui.GameCampaign.Party.CharactersList
                .Select(x => x.RulesetCharacter)
                .ToList();

            involvedGameCharacters.RemoveAll(
                x => party.IndexOf(x.RulesetCharacter) >= DungeonMakerContext.GamePartySize);
        }
     
        if (!Main.Settings.FullyControlConjurations)
        {
            return;
        }

        involvedGameCharacters.RemoveAll(
            x => x.RulesetCharacter is RulesetCharacterMonster rulesetCharacterMonster
                 && ConjurationsContext.ConjuredMonsters.Contains(rulesetCharacterMonster
                     .MonsterDefinition));
    }
}
