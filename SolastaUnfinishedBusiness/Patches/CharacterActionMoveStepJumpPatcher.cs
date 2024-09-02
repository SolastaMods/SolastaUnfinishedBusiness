using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionMoveStepJumpPatcher
{
    //PATCH: allow check reactions on jump checks regardless of success / failure
    [HarmonyPatch(typeof(CharacterActionMoveStepJump), nameof(CharacterActionMoveStepJump.RollChecksIfNecessary))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollChecksIfNecessary_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref IEnumerator __result, CharacterActionMoveStepJump __instance)
        {
            __result = Process(__instance);

            return false;
        }

        private static IEnumerator Process(CharacterActionMoveStepJump action)
        {
            var actingCharacter = action.ActingCharacter;
            var actionModifier = action.ActionParams.ActionModifiers[0];

            if (CharacterActionMoveStepJump.NeedsAcrobaticsCheck(action.landingPosition))
            {
                const int CHECK_DC = 10;
                const RuleDefinitions.AdvantageType BASE_AFFINITY = RuleDefinitions.AdvantageType.None;

                var abilityCheckRoll = actingCharacter.RollAbilityCheck(
                    AttributeDefinitions.Dexterity,
                    SkillDefinitions.Acrobatics,
                    CHECK_DC,
                    BASE_AFFINITY,
                    actionModifier,
                    false,
                    -1,
                    out var outcome,
                    out var successDelta,
                    true);

                var abilityCheckData = new AbilityCheckData
                {
                    AbilityCheckRoll = abilityCheckRoll,
                    AbilityCheckRollOutcome = outcome,
                    AbilityCheckSuccessDelta = successDelta,
                    AbilityCheckActionModifier = actionModifier,
                    Action = action
                };

                yield return TryAlterOutcomeAttributeCheck
                    .HandleITryAlterOutcomeAttributeCheck(actingCharacter, abilityCheckData);

                action.AbilityCheckRoll = abilityCheckData.AbilityCheckRoll;
                action.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckRollOutcome;
                action.AbilityCheckSuccessDelta = abilityCheckData.AbilityCheckSuccessDelta;
            }

            if (action.AbilityCheckRollOutcome != RuleDefinitions.RollOutcome.Failure &&
                CharacterActionMoveStepJump.NeedsAthleticsCheck(action.ActingCharacter, action.jumpPosition,
                    action.landingPosition))
            {
                const int CHECK_DC = 15;
                const RuleDefinitions.AdvantageType BASE_AFFINITY = RuleDefinitions.AdvantageType.None;

                var abilityCheckRoll = action.ActingCharacter.RollAbilityCheck(
                    AttributeDefinitions.Strength,
                    SkillDefinitions.Athletics,
                    CHECK_DC,
                    BASE_AFFINITY,
                    action.ActionParams.ActionModifiers[0],
                    false,
                    -1,
                    out var outcome,
                    out var successDelta,
                    true);

                var abilityCheckData = new AbilityCheckData
                {
                    AbilityCheckRoll = abilityCheckRoll,
                    AbilityCheckRollOutcome = outcome,
                    AbilityCheckSuccessDelta = successDelta,
                    AbilityCheckActionModifier = actionModifier,
                    Action = action
                };

                yield return TryAlterOutcomeAttributeCheck
                    .HandleITryAlterOutcomeAttributeCheck(actingCharacter, abilityCheckData);

                action.AbilityCheckRoll = abilityCheckData.AbilityCheckRoll;
                action.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckRollOutcome;
                action.AbilityCheckSuccessDelta = abilityCheckData.AbilityCheckSuccessDelta;
            }
            else if (CharacterActionMoveStepJump.AutomaticPenalty(
                         action.ActingCharacter, action.jumpPosition, action.landingPosition))
            {
                action.AbilityCheckRollOutcome = RuleDefinitions.RollOutcome.Failure;
            }
        }
    }
}
