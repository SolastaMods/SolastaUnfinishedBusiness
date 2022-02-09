using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "ApplySummonForm")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]

    internal static class RulesetImplementationManagerLocation_ApplySummonForm
    {
        internal static Dictionary<string, int> ConditionToAmount { get; } = new();

        public static RulesetCondition ExtendInflictCondition(
            RulesetActor rulesetActor,
            string conditionDefinitionName,
            RuleDefinitions.DurationType durationType,
            int durationParameter,
            RuleDefinitions.TurnOccurenceType endOccurence,
            string tag,
            ulong sourceGuid,
            string sourceFaction,
            int effectLevel,
            string effectDefinitionName,
            int sourceAmount,
            int sourceAbilityBonus)
        {
            if (ConditionToAmount.ContainsKey(conditionDefinitionName))
            {
                sourceAmount = ConditionToAmount[conditionDefinitionName];
            }

            return rulesetActor.InflictCondition(conditionDefinitionName, durationType, durationParameter, endOccurence, tag, sourceGuid, sourceFaction, effectLevel, effectDefinitionName, sourceAmount, sourceAbilityBonus);
        }

        public static void ExtendRemoveCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition, bool refresh = true, bool showGraphics = true)
        {
            rulesetActor.RemoveCondition(rulesetCondition, refresh, showGraphics);

            if (rulesetCondition?.ConditionDefinition is INotifyConditionRemoval notifiedDefinition)
            {
                notifiedDefinition.AfterConditionRemoved(rulesetActor, rulesetCondition);
            }
        }

        public static void RegisterConditionToAmount(EffectForm effectForm, RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            var summonForm = effectForm.SummonForm;

            if (summonForm.SummonType == SummonForm.Type.Creature && !string.IsNullOrEmpty(summonForm.MonsterDefinitionName) 
                && DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(summonForm.MonsterDefinitionName, out MonsterDefinition monsterDefinition))
            {
                for (int index = 0; index < summonForm.Number; ++index)
                {
                    formsParams.sourceCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionSummoningAffinity>(formsParams.sourceCharacter.FeaturesToBrowse);

                    foreach (FeatureDefinitionSummoningAffinity summoningAffinity in formsParams.sourceCharacter.FeaturesToBrowse.OfType<FeatureDefinitionSummoningAffinity>())
                    {
                        if (string.IsNullOrEmpty(summoningAffinity.RequiredMonsterTag) || monsterDefinition.CreatureTags.Contains(summoningAffinity.RequiredMonsterTag))
                        {
                            foreach (ConditionDefinition addedCondition in summoningAffinity.AddedConditions)
                            {
                                int sourceAmount = 0;

                                switch (addedCondition.AmountOrigin)
                                {
                                    case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus:
                                        sourceAmount = formsParams.sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                                        ConditionToAmount.AddOrReplace(addedCondition.Name, sourceAmount);
                                        break;

                                    case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceCharacterLevel:
                                        sourceAmount = formsParams.sourceCharacter.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
                                        ConditionToAmount.AddOrReplace(addedCondition.Name, sourceAmount);
                                        break;

                                    case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel:
                                        var sourceCharacter = (RulesetCharacterHero)formsParams.sourceCharacter;
                                        // Find a better place to put this in?
                                        string classType = addedCondition.AdditionalDamageType;
                                        if (DatabaseRepository.GetDatabase<CharacterClassDefinition>().TryGetElement(classType, out CharacterClassDefinition characterClassDefinition))
                                        {
                                            if (sourceCharacter.ClassesAndLevels != null
                                                && sourceCharacter.ClassesAndLevels.TryGetValue(characterClassDefinition, out int classLevel))
                                            {
                                                sourceAmount = classLevel;
                                            }

                                            ConditionToAmount.AddOrReplace(addedCondition.Name, sourceAmount);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void UnregisterConditionToAmount()
        {
            ConditionToAmount.Clear();
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var inflictConditionMethod = typeof(RulesetActor).GetMethod("InflictCondition");
            var removeConditionMethod = typeof(RulesetActor).GetMethod("RemoveCondition");
            var extendInflictConditionMethod = typeof(RulesetImplementationManagerLocation_ApplySummonForm).GetMethod("ExtendInflictCondition");
            var extendRemoveConditionMethod = typeof(RulesetImplementationManagerLocation_ApplySummonForm).GetMethod("ExtendRemoveCondition");
            var registerConditionToAmountMethod = typeof(RulesetImplementationManagerLocation_ApplySummonForm).GetMethod("RegisterConditionToAmount");
            var unregisterConditionToAmountMethod = typeof(RulesetImplementationManagerLocation_ApplySummonForm).GetMethod("UnregisterConditionToAmount");

            yield return new CodeInstruction(OpCodes.Ldarg_1); // effectForm
            yield return new CodeInstruction(OpCodes.Ldarg_2); // formsParam
            yield return new CodeInstruction(OpCodes.Call, registerConditionToAmountMethod);

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(inflictConditionMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, extendInflictConditionMethod);
                }
                else if (instruction.Calls(removeConditionMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, extendRemoveConditionMethod);
                }
                else if (instruction.opcode == OpCodes.Ret)
                {
                    yield return new CodeInstruction(OpCodes.Call, unregisterConditionToAmountMethod);
                    yield return instruction;
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
