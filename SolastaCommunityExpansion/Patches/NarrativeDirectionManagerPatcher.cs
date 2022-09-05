using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class NarrativeDirectionManagerPatcher
{
    //PATCH: FullyControlConjurations
    [HarmonyPatch(typeof(NarrativeDirectionManager), "PrepareDialogSequence")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class PrepareDialogSequence_Patch
    {
        internal static void Prefix(List<GameLocationCharacter> involvedGameCharacters)
        {
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

    //PATCH: EnableLogDialoguesToConsole
    [HarmonyPatch(typeof(NarrativeDirectionManager), "StartDialogSequence")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class StartDialogSequence_Patch
    {
        internal static void Postfix()
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
