using SolastaModApi;
using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;

namespace SolastaCommunityExpansion.Models
{
    internal class AwareConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        internal const string ConditionAwareName = "ZSConditionAware";
        private const string ConditionAwareGuid = "21163c80aa4b459892d8e066badc0b81";

        protected AwareConditionBuilder(string name, string guid) : base(ConditionSurprised, name, guid)
        {
            Definition.Features.Clear();
            Definition.GuiPresentation.Description = "Rules/&ConditionAwareDescription";
            Definition.GuiPresentation.Title = "Rules/&ConditionAwareTitle";
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            => new AwareConditionBuilder(name, guid).AddToDB();

        internal static readonly ConditionDefinition AwareCondition =
            CreateAndAddToDB(ConditionAwareName, ConditionAwareGuid);
    }

    internal static class SrdAndHouseRulesContext
    {
        internal static void Load()
        {
            _ = AwareConditionBuilder.AwareCondition;
        }

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

            // does all stealth rolls at the beginning to get a better Game Console output (single stealth roll scenario)
            var stealthRolls = new Dictionary<GameLocationCharacter, int>();

            if (!Main.Settings.EnableSRDCombatSurpriseRulesManyRolls && surprised)
            {
                foreach (GameLocationCharacter surprisingCharacter in surprisingParty)
                {
                    var stealthRoll = surprisingCharacter.RollAbilityCheck("Dexterity", "Stealth", 1, RuleDefinitions.AdvantageType.None, new ActionModifier(), false, -1, out _, true);

                    stealthRolls.Add(surprisingCharacter, stealthRoll);
                }
            }

            // checks for each surprised / surprising pairs if they can see each other. if so it restates the surprised flag based on a surprising stealth check vs. a surprised passive perception
            foreach (GameLocationCharacter surprisedCharacter in surprisedParty)
            {
                var reallySurprised = true;

                if (surprised)
                {
                    foreach (GameLocationCharacter surprisingCharacter in surprisingParty)
                    {
                        if (gameLocationBattleService.CanAttackerSeeCharacterFromPosition(surprisingCharacter.LocationPosition, surprisedCharacter.LocationPosition, surprisingCharacter, surprisedCharacter))
                        {
                            int perceptionOnTarget = 0;

                            if (surprisedCharacter.RulesetCharacter is RulesetCharacterMonster monster)
                            {
                                perceptionOnTarget = monster.MonsterDefinition.ComputePassivePerceptionScore();
                            }
                            else if (surprisedCharacter.RulesetCharacter is RulesetCharacterHero hero)
                            {
                                perceptionOnTarget = hero.ComputePassivePerception();
                            }

                            if (Main.Settings.EnableSRDCombatSurpriseRulesManyRolls)
                            {
                                surprisingCharacter.RollAbilityCheck("Dexterity", "Stealth", perceptionOnTarget, RuleDefinitions.AdvantageType.None, new ActionModifier(), false, -1, out RuleDefinitions.RollOutcome outcome, true);

                                if (outcome == RuleDefinitions.RollOutcome.CriticalFailure || outcome == RuleDefinitions.RollOutcome.Failure)
                                {
                                    reallySurprised = false;

                                    break;
                                }
                            }
                            else if (stealthRolls[surprisingCharacter] < perceptionOnTarget)
                            {
                                reallySurprised = false;

                                break;
                            }
                        }
                    }
                }

                // adds a dummy Aware condition for a better Game Console description on what is happening
                if (!reallySurprised)
                {
                    var conditionAwareDefinition = DatabaseRepository.GetDatabase<ConditionDefinition>().GetElement(AwareConditionBuilder.ConditionAwareName);
                    var conditionAware = RulesetCondition.CreateActiveCondition(surprisedCharacter.RulesetCharacter.Guid, conditionAwareDefinition, RuleDefinitions.DurationType.Round, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn, 0, string.Empty);
                    
                    surprisedCharacter.RulesetCharacter.AddConditionOfCategory("10Combat", conditionAware);
                }

                surprisedCharacter.StartBattle(surprised && reallySurprised);
            }
        }
    }
}
