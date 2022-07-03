using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomAbilityChecks;

//
// IChangeAbilityCheck
//
[HarmonyPatch(typeof(RulesetCharacter), "RollAbilityCheck")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_RollAbilityCheck
{
    internal static void Prefix(
        RulesetCharacter __instance,
        int baseBonus,
        string abilityScoreName,
        string proficiencyName,
        List<RuleDefinitions.TrendInfo> modifierTrends,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        int rollModifier,
        ref int minRoll)
    {
        var features = __instance.EnumerateFeaturesToBrowse<IChangeAbilityCheck>();

        if (features.Count <= 0)
        {
            return;
        }

        var newMinRoll = features
            .Max(x => x.MinRoll(__instance, baseBonus, rollModifier, abilityScoreName, proficiencyName,
                advantageTrends, modifierTrends));

        if (minRoll < newMinRoll)
        {
            minRoll = newMinRoll;
        }
    }
}

//
// IChangeAbilityCheck
//
[HarmonyPatch(typeof(RulesetCharacter), "ResolveContestCheck")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_ResolveContestCheck
{
    public static int ExtendedRollDie(
        RulesetCharacter rulesetCharacter,
        RuleDefinitions.DieType dieType,
        RuleDefinitions.RollContext rollContext,
        bool isProficient,
        RuleDefinitions.AdvantageType advantageType,
        out int firstRoll,
        out int secondRoll,
        bool enumerateFeatures,
        bool canRerollDice,
        int baseBonus,
        int rollModifier,
        string abilityScoreName,
        string proficiencyName,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        List<RuleDefinitions.TrendInfo> modifierTrends)
    {
        var result = rulesetCharacter.RollDie(dieType, rollContext, isProficient, advantageType, out firstRoll,
            out secondRoll, enumerateFeatures, canRerollDice);
        var features = rulesetCharacter.EnumerateFeaturesToBrowse<IChangeAbilityCheck>();

        if (features.Count <= 0)
        {
            return result;
        }

        var newMinRoll = features
            .Max(x => x.MinRoll(rulesetCharacter, baseBonus, rollModifier, abilityScoreName, proficiencyName,
                advantageTrends, modifierTrends));

        if (result < newMinRoll)
        {
            result = newMinRoll;
        }

        return result;
    }

    //
    // there are 2 calls to RollDie on this method
    // we replace them to allow us to compare the die result vs. the minRoll value from any IChangeAbilityCheck feature
    //
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = 0;
        var rollDieMethod = typeof(RulesetActor).GetMethod("RollDie");
        var extendedRollDieMethod = typeof(RulesetCharacter_ResolveContestCheck).GetMethod("ExtendedRollDie");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(rollDieMethod))
            {
                ++found;

                switch (found)
                {
                    // first call to roll die checks the initiator
                    case 1:
                        yield return new CodeInstruction(OpCodes.Ldarg, 1); // baseBonus
                        yield return new CodeInstruction(OpCodes.Ldarg, 2); // rollModifier
                        yield return new CodeInstruction(OpCodes.Ldarg, 3); // abilityScoreName
                        yield return new CodeInstruction(OpCodes.Ldarg, 4); // proficiencyName
                        yield return new CodeInstruction(OpCodes.Ldarg, 5); // advantageTrends
                        yield return new CodeInstruction(OpCodes.Ldarg, 6); // modifierTrends

                        break;
                    // second call to roll die checks the opponent
                    case 2:
                        yield return new CodeInstruction(OpCodes.Ldarg, 7); // opponentBaseBonus
                        yield return new CodeInstruction(OpCodes.Ldarg, 8); // opponentRollModifier
                        yield return new CodeInstruction(OpCodes.Ldarg, 9); // opponentAbilityScoreName
                        yield return new CodeInstruction(OpCodes.Ldarg, 10); // opponentProficiencyName
                        yield return new CodeInstruction(OpCodes.Ldarg, 11); // opponentAdvantageTrends
                        yield return new CodeInstruction(OpCodes.Ldarg, 12); // opponentModifierTrends

                        break;
                }

                yield return new CodeInstruction(OpCodes.Call, extendedRollDieMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }
}
