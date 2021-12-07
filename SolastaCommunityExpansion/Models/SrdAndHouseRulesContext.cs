using System.Collections.Generic;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;

namespace SolastaCommunityExpansion.Models
{
    internal static class SrdAndHouseRulesContext
    {
        private const int SURPRISE_CHECK_DC_BASE = 10;

        internal static void Load()
        {
            var conditionUnsurprised = UnityEngine.Object.Instantiate(DatabaseRepository.GetDatabase<ConditionDefinition>().GetElement("ConditionSurprised"));

            conditionUnsurprised.name = "ConditionUnsurprised";
            conditionUnsurprised.SetGuid(SolastaModApi.GuidHelper.Create(new System.Guid(Settings.GUID), conditionUnsurprised.name).ToString());
            conditionUnsurprised.GuiPresentation.SetTitle("Rules/&ConditionUnsurprisedTitle");
            conditionUnsurprised.GuiPresentation.SetDescription("Rules/&ConditionUnsurprisedDescription");
            conditionUnsurprised.Features.Clear();

            DatabaseRepository.GetDatabase<ConditionDefinition>().Add(conditionUnsurprised);
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
                            int perceptionOnTarget = surprisedCharacter.ComputePassivePerceptionOnTarget(surprisingCharacter, out bool _);

                            surprisingCharacter.RollAbilityCheck("Dexterity", "Stealth", SURPRISE_CHECK_DC_BASE + perceptionOnTarget, RuleDefinitions.AdvantageType.None, new ActionModifier(), false, -1, out RuleDefinitions.RollOutcome outcome, true);

                            if (outcome == RuleDefinitions.RollOutcome.CriticalFailure || outcome == RuleDefinitions.RollOutcome.Failure)
                            {
                                var conditionUnsurprised = UnityEngine.Object.Instantiate(DatabaseRepository.GetDatabase<ConditionDefinition>().GetElement("ConditionUnsurprised"));

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
