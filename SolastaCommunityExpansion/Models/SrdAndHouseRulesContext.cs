using SolastaModApi.Extensions;
using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;

namespace SolastaCommunityExpansion.Models
{
    internal static class SrdAndHouseRulesContext
    {
        internal static void Load()
        {
            var dbConditionDefinition = DatabaseRepository.GetDatabase<ConditionDefinition>();
            var conditionUnsurprised = UnityEngine.Object.Instantiate(dbConditionDefinition.GetElement("ConditionSurprised"));

            conditionUnsurprised.name = "ConditionUnsurprised";
            conditionUnsurprised.SetGuid(SolastaModApi.GuidHelper.Create(new System.Guid(Settings.GUID), conditionUnsurprised.name).ToString());
            conditionUnsurprised.GuiPresentation.SetTitle("Rules/&ConditionUnsurprisedTitle");
            conditionUnsurprised.GuiPresentation.SetDescription("Rules/&ConditionUnsurprisedDescription");
            conditionUnsurprised.Features.Clear();

            dbConditionDefinition.Add(conditionUnsurprised);
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

            foreach (GameLocationCharacter surprisedCharacter in surprisedParty)
            {
                var isReallySurprised = true;

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

                            surprisingCharacter.RollAbilityCheck("Dexterity", "Stealth", perceptionOnTarget, RuleDefinitions.AdvantageType.None, new ActionModifier(), false, -1, out RuleDefinitions.RollOutcome outcome, true);

                            if (outcome == RuleDefinitions.RollOutcome.CriticalFailure || outcome == RuleDefinitions.RollOutcome.Failure)
                            {
                                var conditionUnsurprised = DatabaseRepository.GetDatabase<ConditionDefinition>().GetElement("ConditionUnsurprised");

                                surprisedCharacter.RulesetCharacter.AddConditionOfCategory("10Combat", RulesetCondition.CreateActiveCondition(surprisedCharacter.RulesetCharacter.Guid, conditionUnsurprised, RuleDefinitions.DurationType.Round, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn, 0UL, string.Empty));
                                isReallySurprised = false;

                                break;
                            }
                        }
                    }
                }

                surprisedCharacter.StartBattle(surprised && isReallySurprised);
            }
        }
    }
}
