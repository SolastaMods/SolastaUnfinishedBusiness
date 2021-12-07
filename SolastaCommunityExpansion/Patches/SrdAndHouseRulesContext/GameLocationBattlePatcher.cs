using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRulesContext
{
    [HarmonyPatch(typeof(GameLocationBattle), "StartContenders")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattle_StartContenders
    {
        internal static bool Prefix(GameLocationBattle __instance, bool partySurprised, bool enemySurprised)
        {
            if (Main.Settings.EnableSRDCombatSurpriseRules && (partySurprised || enemySurprised))
            {
                StartContenders(partySurprised, __instance.PlayerContenders, __instance.EnemyContenders);
                StartContenders(enemySurprised, __instance.EnemyContenders, __instance.PlayerContenders);

                return false;
            }

            return true;
        }

        internal static void StartContenders(bool surprised, List<GameLocationCharacter> surprisedParty, List<GameLocationCharacter> surprisingParty)
        {
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (surprised)
            {
                foreach (GameLocationCharacter surprisedCharacter in surprisedParty)
                {
                    var isReallySurprised = true;

                    foreach (GameLocationCharacter surprisingCharacter in surprisingParty)
                    {
                        if (gameLocationBattleService.CanAttackerSeeCharacterFromPosition(surprisingCharacter.LocationPosition, surprisedCharacter.LocationPosition, surprisingCharacter, surprisedCharacter))
                        {
                            int perceptionOnTarget = surprisedCharacter.ComputePassivePerceptionOnTarget(surprisingCharacter, out bool _);

                            surprisingCharacter.RollAbilityCheck("Dexterity", "Stealth", perceptionOnTarget, RuleDefinitions.AdvantageType.None, new ActionModifier(), false, -1, out RuleDefinitions.RollOutcome outcome, true);

                            if (outcome == RuleDefinitions.RollOutcome.CriticalFailure || outcome == RuleDefinitions.RollOutcome.Failure)
                            {
                                isReallySurprised = false;

                                break;
                            }
                        }
                    }

                    surprisedCharacter.StartBattle(isReallySurprised);
                }
            }
            else
            {
                foreach (GameLocationCharacter surprisedCharacter in surprisedParty)
                {
                    surprisedCharacter.StartBattle(surprised);
                }
            }
        }
    }
}
