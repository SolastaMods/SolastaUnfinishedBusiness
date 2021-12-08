using HarmonyLib;
using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;

namespace SolastaCommunityExpansion.Models
{
    internal static class SrdAndHouseRulesContext
    {
        internal static void ApplyConditionBlindedShouldNotAllowOpportunityAttack()
        {
            // Use the shocked condition affinity which has the desired effect
            if (Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack)
            {
                if (!ConditionBlinded.Features.Contains(ActionAffinityConditionShocked))
                {
                    ConditionBlinded.Features.Add(ActionAffinityConditionShocked);
                }
            }
            else
            {
                if (ConditionBlinded.Features.Contains(ActionAffinityConditionShocked))
                {
                    ConditionBlinded.Features.Remove(ActionAffinityConditionShocked);
                }
            }
        }

        internal static void StartContenders(bool surprised, List<GameLocationCharacter> surprisedParty, List<GameLocationCharacter> surprisingParty)
        {
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var stealthRolls = new Dictionary<GameLocationCharacter, int>();

            // does all stealth rolls at the beginning to get a better Game Console output (single stealth roll scenario)
            if (!Main.Settings.EnableSRDCombatSurpriseRulesManyRolls && surprised)
            {
                foreach (GameLocationCharacter surprisingCharacter in surprisingParty)
                {
                    var stealthRoll = surprisingCharacter.RollAbilityCheck("Dexterity", "Stealth", 1, RuleDefinitions.AdvantageType.None, new ActionModifier(), false, -1, out _, true);

                    stealthRolls.Add(surprisingCharacter, stealthRoll);
                }
            }

            // revalidates a surprised character against surprising contenders
            bool IsReallySurprised(GameLocationCharacter surprisedCharacter)
            {
                foreach (GameLocationCharacter surprisingCharacter in surprisingParty)
                {
                    if (gameLocationBattleService.CanAttackerSeeCharacterFromPosition(surprisingCharacter.LocationPosition, surprisedCharacter.LocationPosition, surprisingCharacter, surprisedCharacter))
                    {
                        int perceptionOnTarget;

                        if (surprisedCharacter.RulesetCharacter is RulesetCharacterMonster monster)
                        {
                            perceptionOnTarget = monster.MonsterDefinition.ComputePassivePerceptionScore();
                        }
                        else
                        {
                            perceptionOnTarget = surprisedCharacter.RulesetCharacter.ComputePassivePerception();
                        }

                        if (Main.Settings.EnableSRDCombatSurpriseRulesManyRolls)
                        {
                            surprisingCharacter.RollAbilityCheck("Dexterity", "Stealth", perceptionOnTarget, RuleDefinitions.AdvantageType.None, new ActionModifier(), false, -1, out RuleDefinitions.RollOutcome outcome, true);

                            if (outcome == RuleDefinitions.RollOutcome.CriticalFailure || outcome == RuleDefinitions.RollOutcome.Failure)
                            {
                                return false;
                            }
                        }
                        else if (stealthRolls[surprisingCharacter] < perceptionOnTarget)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            // checks for each surprised / surprising pairs if they can see each other. if so it restates the surprised flag based on a surprising stealth check vs. a surprised passive perception
            foreach (GameLocationCharacter surprisedCharacter in surprisedParty)
            {
                var reallySurprised = true;

                if (surprised)
                {
                    reallySurprised = IsReallySurprised(surprisedCharacter);

                    // adds feedback to Game Console
                    if (!reallySurprised)
                    {
                        var gameConsole = Gui.Game.GameConsole;
                        var consoleTableDefinition = AccessTools.Field(gameConsole.GetType(), "consoleTableDefinition").GetValue(gameConsole) as ConsoleTableDefinition;
                        var addCharacterEntryMethod = AccessTools.Method("GameConsole:AddCharacterEntry");
                        var gameConsoleEntry = new GameConsoleEntry("Feedback/&ConditionRemovedLine", consoleTableDefinition)
                        {
                            Indent = true
                        };

                        addCharacterEntryMethod.Invoke(gameConsole, new object[] { surprisedCharacter.RulesetCharacter, gameConsoleEntry });
                        gameConsoleEntry.AddParameter(ConsoleStyleDuplet.ParameterType.AbilityInfo, ConditionSurprised.FormatTitle(), tooltipContent: ConditionSurprised.FormatDescription());
                        gameConsole.AddEntry(gameConsoleEntry);
                    }
                }

                // adds the contender to the battle with the revised surprise calculation
                surprisedCharacter.StartBattle(surprised && reallySurprised);
            }
        }
    }
}
