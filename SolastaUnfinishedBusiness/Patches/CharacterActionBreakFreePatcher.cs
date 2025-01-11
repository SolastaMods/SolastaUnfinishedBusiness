using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionBreakFreePatcher
{
    //PATCH: this is almost vanilla code except for the checks on Web and Bound by Ice conditions
    [HarmonyPatch(typeof(CharacterActionBreakFree), nameof(CharacterActionBreakFree.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref IEnumerator __result, CharacterActionBreakFree __instance)
        {
            __result = Process(__instance);

            return false;
        }

        private static IEnumerator Process(CharacterActionBreakFree __instance)
        {
            var character = __instance.ActingCharacter;
            var rulesetCharacter = character.RulesetCharacter;
            var restrainingCondition = AiHelpers.GetRestrainingCondition(rulesetCharacter);

            if (restrainingCondition == null)
            {
                yield break;
            }

            var sourceGuid = restrainingCondition.SourceGuid;
            var action = (AiHelpers.BreakFreeType)restrainingCondition.Amount;
            var actionModifier = new ActionModifier();
            var checkDC = 10;
            var success = false;
            string abilityScoreName;
            string proficiencyName;

            switch (action)
            {
                case AiHelpers.BreakFreeType.DoNoCheckAndRemoveCondition:
                    rulesetCharacter.RemoveCondition(restrainingCondition);
                    yield break;

                case AiHelpers.BreakFreeType.DoStrengthCheckAgainstCasterDC:
                {
                    CalculateDC(AttributeDefinitions.Strength);
                    yield return RollAbilityCheck();
                    break;
                }
                case AiHelpers.BreakFreeType.DoWisdomCheckAgainstCasterDC:
                {
                    CalculateDC(AttributeDefinitions.Wisdom);
                    yield return RollAbilityCheck();
                    break;
                }
                case AiHelpers.BreakFreeType.DoStrengthOrDexterityContestCheckAgainstStrengthAthletics:
                {
                    var rulesetSource = EffectHelpers.GetCharacterByGuid(sourceGuid);
                    var source = GameLocationCharacter.GetFromActor(rulesetSource);
                    var abilityCheckData = new AbilityCheckData
                    {
                        AbilityCheckActionModifier = new ActionModifier(), Action = __instance
                    };
                    var opponentAbilityCheckData = new AbilityCheckData
                    {
                        AbilityCheckActionModifier = new ActionModifier(), Action = __instance
                    };

                    abilityScoreName =
                        __instance.ActionParams.BreakFreeMode == ActionDefinitions.BreakFreeMode.Athletics
                            ? AttributeDefinitions.Strength
                            : AttributeDefinitions.Dexterity;

                    yield return TryAlterOutcomeAttributeCheck.ResolveRolls(
                        source, character, ActionDefinitions.Id.BreakFree, abilityCheckData,
                        opponentAbilityCheckData, abilityScoreName);

                    __instance.AbilityCheckRoll = opponentAbilityCheckData.AbilityCheckRoll;
                    __instance.AbilityCheckRollOutcome = opponentAbilityCheckData.AbilityCheckRollOutcome;
                    __instance.AbilityCheckSuccessDelta = opponentAbilityCheckData.AbilityCheckSuccessDelta;

                    // this is the success of the opponent
                    success = __instance.AbilityCheckRollOutcome
                        is RollOutcome.Success or RollOutcome.CriticalSuccess;

                    break;
                }
                default:
                {
                    abilityScoreName =
                        __instance.ActionParams.BreakFreeMode == ActionDefinitions.BreakFreeMode.Athletics
                            ? AttributeDefinitions.Strength
                            : AttributeDefinitions.Dexterity;

                    proficiencyName = __instance.ActionParams.BreakFreeMode == ActionDefinitions.BreakFreeMode.Athletics
                        ? SkillDefinitions.Athletics
                        : SkillDefinitions.Acrobatics;

                    if (restrainingCondition!.HasSaveOverride)
                    {
                        checkDC = restrainingCondition.SaveOverrideDC;
                    }
                    else
                    {
                        if (RulesetEntity.TryGetEntity(sourceGuid, out RulesetEffect entity1))
                        {
                            checkDC = entity1.SaveDC;
                        }
                        else if (RulesetEntity.TryGetEntity(sourceGuid, out RulesetCharacterMonster entity2))
                        {
                            checkDC = 10 + AttributeDefinitions
                                .ComputeAbilityScoreModifier(entity2.GetAttribute(AttributeDefinitions.Strength)
                                    .CurrentValue);
                        }
                    }

                    yield return RollAbilityCheck();
                    break;
                }
            }

            if (success)
            {
                rulesetCharacter.RemoveCondition(restrainingCondition);
            }

            rulesetCharacter.BreakFreeExecuted?.Invoke(__instance.ActingCharacter.RulesetCharacter, success);

            yield break;

            void CalculateDC(string newAbilityScoreName)
            {
                if (RulesetEntity.TryGetEntity(sourceGuid, out RulesetCharacterHero rulesetCharacterHero))
                {
                    checkDC = rulesetCharacterHero.SpellRepertoires
                        .Select(x => x.SaveDC)
                        .Max();
                }

                rulesetCharacter.LogCharacterActivatesAbility(
                    string.Empty,
                    "Feedback/&BreakFreeAttempt",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Negative,
                            restrainingCondition.ConditionDefinition.FormatTitle())
                    ]);

                abilityScoreName = newAbilityScoreName;
                proficiencyName = string.Empty;
            }

            IEnumerator RollAbilityCheck()
            {
                var abilityCheckRoll = __instance.ActingCharacter.RollAbilityCheck(
                    abilityScoreName,
                    proficiencyName,
                    checkDC,
                    AdvantageType.None,
                    actionModifier,
                    false,
                    -1,
                    out var rollOutcome,
                    out var successDelta,
                    true);

                //PATCH: support for Bardic Inspiration roll off battle and ITryAlterOutcomeAttributeCheck
                var abilityCheckData = new AbilityCheckData
                {
                    AbilityCheckRoll = abilityCheckRoll,
                    AbilityCheckRollOutcome = rollOutcome,
                    AbilityCheckSuccessDelta = successDelta,
                    AbilityCheckActionModifier = actionModifier,
                    Action = __instance
                };

                yield return TryAlterOutcomeAttributeCheck
                    .HandleITryAlterOutcomeAttributeCheck(__instance.ActingCharacter, abilityCheckData);

                __instance.AbilityCheckRoll = abilityCheckData.AbilityCheckRoll;
                __instance.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckRollOutcome;
                __instance.AbilityCheckSuccessDelta = abilityCheckData.AbilityCheckSuccessDelta;

                success = __instance.AbilityCheckRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess;
            }
        }
    }
}
