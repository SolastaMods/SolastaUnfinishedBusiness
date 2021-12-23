using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SolastaCommunityExpansion.Patches.Level20
{
    internal static class RulesetCharacterPatcher
    {
        [HarmonyPatch(typeof(RulesetCharacter), "RollAbilityCheck")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RulesetCharacter_RollAbilityCheck
        {
            internal static void Prefix(
                RulesetCharacter __instance,
                int baseBonus,
                int rollModifier,
                string abilityScoreName,
                string proficiencyName,
                ref int minRoll)
            {
                if (!Main.Settings.EnableLevel20)
                {
                    return;
                }

                if (abilityScoreName != AttributeDefinitions.Strength)
                {
                    return;
                }

                int? modifiedMinRoll = MinimumStrengthAbilityCheckDieRoll(__instance, baseBonus, rollModifier, proficiencyName);

                if (modifiedMinRoll.HasValue && modifiedMinRoll.Value > minRoll)
                {
                    minRoll = modifiedMinRoll.Value;
                }
            }
        }

        [HarmonyPatch(typeof(RulesetCharacter), "ResolveContestCheck")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RulesetCharacter_ResolveContestCheck
        {
            internal static void Prefix(
                RulesetCharacter __instance,
                int baseBonus,
                int rollModifier,
                string abilityScoreName,
                string proficiencyName,
                RulesetCharacter opponent,
                int opponentBaseBonus,
                int opponentRollModifier,
                string opponentAbilityScoreName,
                string opponentProficiencyName)
            {
                if (!Main.Settings.EnableLevel20)
                {
                    return;
                }

                // ResolveContestCheck calls RulesetActor.RollDie twice (once for you, once for your opponent).
                // SetNextAbilityCheckMinimum below tells the RulesetActor.RollDie patch that the next call to RollDie (for the specified RulesetCharacter) must have a certain minimum value.

                if (abilityScoreName == AttributeDefinitions.Strength)
                {
                    int? instanceMinRoll = MinimumStrengthAbilityCheckDieRoll(__instance, baseBonus, rollModifier, proficiencyName);

                    if (instanceMinRoll.HasValue)
                    {
                        RulesetActor_RollDie.SetNextAbilityCheckMinimum(__instance, instanceMinRoll.Value);
                    }
                }

                if (opponentAbilityScoreName == AttributeDefinitions.Strength)
                {
                    int? opponentMinRoll = MinimumStrengthAbilityCheckDieRoll(opponent, opponentBaseBonus, opponentRollModifier, opponentProficiencyName);

                    if (opponentMinRoll.HasValue)
                    {
                        RulesetActor_RollDie.SetNextAbilityCheckMinimum(opponent, opponentMinRoll.Value);
                    }
                }
            }
        }

        private static int? MinimumStrengthAbilityCheckDieRoll(RulesetCharacter character, int baseBonus, int rollModifier, string proficiencyName)
        {
            if (character == null)
            {
                return null;
            }

            int? minimumTotal = MinimumStrengthAbilityCheckTotal(character, proficiencyName);

            if (!minimumTotal.HasValue)
            {
                return null;
            }

            // Set the minimum die roll based on the bonuses, which indirectly sets the minimum total.
            // This can result in impossible die rolls, like rolling 23 on a d20, which the game doesn't seem to mind.
            return minimumTotal.Value - (baseBonus + rollModifier);
        }

        private static int? MinimumStrengthAbilityCheckTotal(RulesetCharacter character, string proficiencyName)
        {
            return character
                .EnumerateFeaturesToBrowse<IMinimumAbilityCheckTotal>()
                .Select(feature => feature.MinimumStrengthAbilityCheckTotal(character, proficiencyName))
                .Max();
        }
    }
}
