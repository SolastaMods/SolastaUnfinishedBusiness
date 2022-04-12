using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.SummonFormExtraOriginOfAmount
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "ApplySummonForm")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetImplementationManagerLocation_ApplySummonForm
    {
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
            int sourceAbilityBonus,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            ConditionDefinition addedCondition)
        {
            switch (addedCondition.AmountOrigin)
            {
                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus:
                    sourceAmount = formsParams.sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceCharacterLevel:
                    sourceAmount = formsParams.sourceCharacter.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel:
                    var sourceCharacter = (RulesetCharacterHero)formsParams.sourceCharacter;
                    // Find a better place to put this in?
                    string classType = addedCondition.AdditionalDamageType;
                    if (DatabaseRepository.GetDatabase<CharacterClassDefinition>().TryGetElement(classType, out CharacterClassDefinition characterClassDefinition)
                        && sourceCharacter.ClassesAndLevels != null
                        && sourceCharacter.ClassesAndLevels.TryGetValue(characterClassDefinition, out int classLevel))
                    {
                        sourceAmount = classLevel;
                    }
                    break;
            }

            return rulesetActor.InflictCondition(conditionDefinitionName, durationType, durationParameter, endOccurence, tag, sourceGuid, sourceFaction, effectLevel, effectDefinitionName, sourceAmount, sourceAbilityBonus);
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var addedConditionPos = Main.IsDebugBuild ? 36 : 26;
            var found = 0;
            var inflictConditionMethod = typeof(RulesetActor).GetMethod("InflictCondition");
            var extendInflictConditionMethod = typeof(RulesetImplementationManagerLocation_ApplySummonForm).GetMethod("ExtendInflictCondition");

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(inflictConditionMethod) && ++found == 3)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_2); // formsParam
                    yield return new CodeInstruction(OpCodes.Ldloc_S, addedConditionPos); // addedCondition
                    yield return new CodeInstruction(OpCodes.Call, extendInflictConditionMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
