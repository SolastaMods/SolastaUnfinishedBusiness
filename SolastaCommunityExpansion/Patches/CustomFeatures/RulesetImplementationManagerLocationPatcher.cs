using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
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
                    if (DatabaseRepository.GetDatabase<CharacterClassDefinition>().TryGetElement(classType, out CharacterClassDefinition characterClassDefinition))
                    {
                        if (sourceCharacter.ClassesAndLevels != null
                            && sourceCharacter.ClassesAndLevels.TryGetValue(characterClassDefinition, out int classLevel))
                        {
                            sourceAmount = classLevel;
                        }
                    }
                    break;
            }

            return rulesetActor.InflictCondition(conditionDefinitionName, durationType, durationParameter, endOccurence, tag, sourceGuid, sourceFaction, effectLevel, effectDefinitionName, sourceAmount, sourceAbilityBonus);
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var inflictConditionMethod = typeof(RulesetActor).GetMethod("InflictCondition");
            var extendInflictConditionMethod = typeof(RulesetImplementationManagerLocation_ApplySummonForm).GetMethod("ExtendInflictCondition");

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(inflictConditionMethod))
                {
                    //
                    // WARNING: review parameter value 35 before release
                    //
                    yield return new CodeInstruction(OpCodes.Ldarg_2); // formsParam
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 35); // addedCondition local from for loop
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
