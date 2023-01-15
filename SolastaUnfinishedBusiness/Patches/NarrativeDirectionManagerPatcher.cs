using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NarrativeDirectionManagerPatcher
{
    [HarmonyPatch(typeof(NarrativeDirectionManager), "PrepareDialogSequence")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class PrepareDialogSequence_Patch
    {
        [UsedImplicitly]
        public static void Prefix(List<GameLocationCharacter> involvedGameCharacters)
        {
            //PATCH: Don't offer controlled conjurations on dialogue sequences (FullyControlConjurations)
            if (Main.Settings.FullyControlConjurations)
            {
                involvedGameCharacters.RemoveAll(
                    x => x.RulesetCharacter is RulesetCharacterMonster rulesetCharacterMonster
                         && SrdAndHouseRulesContext.ConjuredMonsters.Contains(rulesetCharacterMonster
                             .MonsterDefinition));
            }

            //PATCH: Only offer the first 4 players on dialogue sequences (PARTYSIZE)
            if (Main.Settings.OverridePartySize <= ToolsContext.GamePartySize
                || Main.Settings.AllowAllPlayersOnNarrativeSequences)
            {
                return;
            }

            {
                var party = Gui.GameCampaign.Party.CharactersList
                    .Select(x => x.RulesetCharacter)
                    .ToList();

                involvedGameCharacters.RemoveAll(
                    x => party.IndexOf(x.RulesetCharacter) >= ToolsContext.GamePartySize);
            }
        }
    }

    //PATCH: EnableLogDialoguesToConsole
    [HarmonyPatch(typeof(NarrativeDirectionManager), "StartDialogSequence")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class StartDialogSequence_Patch
    {
        [UsedImplicitly]
        public static void Postfix()
        {
            if (!Main.Settings.EnableLogDialoguesToConsole)
            {
                return;
            }

            var screen = Gui.GuiService.GetScreen<GuiConsoleScreen>();

            screen.Show();
        }
    }
}
