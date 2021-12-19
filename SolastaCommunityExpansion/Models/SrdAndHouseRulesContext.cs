using HarmonyLib;
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
            AdjustChainLightningSpell();
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
            var stealthRolls = new Dictionary<GameLocationCharacter, int>();

            // revalidates a surprised character against surprising contenders
            bool IsReallySurprised(GameLocationCharacter surprisedCharacter)
            {
                foreach (GameLocationCharacter surprisingCharacter in surprisingParty)
                {
                    if (gameLocationBattleService.CanAttackerSeeCharacterFromPosition(surprisingCharacter.LocationPosition, surprisedCharacter.LocationPosition, surprisingCharacter, surprisedCharacter))
                    {
                        int perceptionOnTarget;
                        int stealthCheck;

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
                            stealthCheck = surprisingCharacter.RollAbilityCheck("Dexterity", "Stealth", perceptionOnTarget, RuleDefinitions.AdvantageType.None, new ActionModifier(), false, -1, out _, true);
                        }
                        else
                        {
                            stealthCheck = stealthRolls[surprisingCharacter];
                        }

                        if (stealthCheck < perceptionOnTarget)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            if (surprised)
            {
                // only calculates one single set of stealth checks when multiple rolls are disabled
                if (!Main.Settings.EnableSRDCombatSurpriseRulesManyRolls)
                {
                    foreach (GameLocationCharacter surprisingCharacter in surprisingParty)
                    {
                        var stealthRoll = surprisingCharacter.RollAbilityCheck("Dexterity", "Stealth", 1, RuleDefinitions.AdvantageType.None, new ActionModifier(), false, -1, out _, true);

                        stealthRolls.Add(surprisingCharacter, stealthRoll);
                    }
                }
           
                foreach (GameLocationCharacter surprisedCharacter in surprisedParty)
                {
                    // revalidates the surprise
                    if (IsReallySurprised(surprisedCharacter))
                    {
                        surprisedCharacter.StartBattle(true);
                    }
                    else
                    {
                        var gameConsole = Gui.Game.GameConsole;
                        var methodConditionRemoved = AccessTools.Method("GameConsole:ConditionRemoved");

                        methodConditionRemoved.Invoke(gameConsole, new object[] { surprisedCharacter.RulesetCharacter, new RulesetCondition() { ConditionDefinition = ConditionSurprised } });
                        surprisedCharacter.StartBattle(false);
                    }
                }
            }
            else
            {
                foreach (GameLocationCharacter surprisedCharacter in surprisedParty)
                {
                    surprisedCharacter.StartBattle(false);
                }
            }
        }

        /// <summary>
        /// Allow the user to select targets when using 'Chain Lightning'.
        /// </summary>
        internal static void AdjustChainLightningSpell()
        {
            var spell = SolastaModApi.DatabaseHelper.SpellDefinitions.ChainLightning.EffectDescription;

            if (Main.Settings.AdjustChainLightningSpell)
            {
                // This is half bug-fix, half houses rules since it's not completely SRD but better than implemented.
                // Spell should arc from target (range 150ft) onto upto 3 extra selectable targets (range 30ft from first).
                // Fix by allowing 4 selectable targets.
                spell.TargetType = RuleDefinitions.TargetType.IndividualsUnique;
                spell.SetTargetParameter(4);

                // TODO: may need to tweak range parameters but it works as is.
            }
            else
            {
                spell.TargetType = RuleDefinitions.TargetType.ArcFromIndividual;
                spell.SetTargetParameter(3);
            }
        }
    }
}
