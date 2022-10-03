using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class NarrativeDirectionManagerPatcher
{
    [HarmonyPatch(typeof(NarrativeDirectionManager), "PrepareDialogSequence")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class PrepareDialogSequence_Patch
    {
        public static void Prefix(List<GameLocationCharacter> involvedGameCharacters)
        {
            //PATCH: FullyControlConjurations
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
    public static class StartDialogSequence_Patch
    {
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
