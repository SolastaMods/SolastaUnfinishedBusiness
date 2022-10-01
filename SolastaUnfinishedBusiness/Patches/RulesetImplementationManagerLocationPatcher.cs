using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetImplementationManagerLocationPatcher
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsMetamagicOptionAvailable")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsMetamagicOptionAvailable_Patch
    {
        internal static void Postfix(
            ref bool __result,
            RulesetEffectSpell rulesetEffectSpell,
            RulesetCharacter caster,
            MetamagicOptionDefinition metamagicOption,
            ref string failure)
        {
            //PATCH: fix twinned spells offering
            //disables sunlight blade twinning, since it is not supported for now
            //plus fixes vanilla code not accounting for things possible in MC
            if (metamagicOption != DatabaseHelper.MetamagicOptionDefinitions.MetamagicTwinnedSpell
                || caster is not RulesetCharacterHero hero)
            {
                return;
            }

            var spellDefinition = rulesetEffectSpell.SpellDefinition;

            if (!Main.Settings.FixSorcererTwinnedLogic)
            {
                return;
            }

            var effectDescription = spellDefinition.effectDescription;

            if (effectDescription.TargetType is not (TargetType.Individuals or TargetType.IndividualsUnique) ||
                rulesetEffectSpell.ComputeTargetParameter() == 1)
            {
                return;
            }

            failure = FailureFlagInvalidSingleTarget;
            __result = false;
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsSituationalContextValid")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsSituationalContextValid_Patch
    {
        internal static void Postfix(
            ref bool __result,
            RulesetImplementationDefinitions.SituationalContextParams contextParams)
        {
            //PATCH: supports custom situational context
            //used to tweak `Reckless` feat to properly work with reach weapons
            //and for Blade Dancer subclass features
            __result = CustomSituationalContext.IsContextValid(contextParams, __result);
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "InstantiateActiveDeviceFunction")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InstantiateActiveDeviceFunction_Patch
    {
        internal static bool Prefix(
            RulesetImplementationManagerLocation __instance,
            ref RulesetEffect __result,
            RulesetCharacter user,
            RulesetItemDevice usableDevice,
            RulesetDeviceFunction usableDeviceFunction,
            int addedCharges,
            bool delayRegistration)
        {
            //PATCH: support `RulesetEffectPowerWithAdvancement` by creating custom instance when needed
            return RulesetEffectPowerWithAdvancement.InstantiateActiveDeviceFunction(__instance, ref __result, user,
                usableDevice, usableDeviceFunction, addedCharges, delayRegistration);
        }
    }

    //PATCH: Implements ExtraOriginOfAmount
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "ApplySummonForm")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ApplySummonForm_Patch
    {
        public static RulesetCondition ExtendInflictCondition(
            RulesetActor rulesetActor,
            string conditionDefinitionName,
            DurationType durationType,
            int durationParameter,
            TurnOccurenceType endOccurence,
            string tag,
            ulong sourceGuid,
            string sourceFaction,
            int effectLevel,
            string effectDefinitionName,
            int sourceAmount,
            int sourceAbilityBonus,
            int sourceProficiencyBonus,
            DieType bardicInspirationDie,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            ConditionDefinition addedCondition)
        {
            switch (addedCondition.AmountOrigin)
            {
                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus:
                    sourceAmount =
                        formsParams.sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceCharacterLevel:
                    sourceAmount =
                        formsParams.sourceCharacter.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel:
                    var sourceCharacter = (RulesetCharacterHero)formsParams.sourceCharacter;
                    // Find a better place to put this in?
                    var classType = addedCondition.AdditionalDamageType;
                    if (DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                            .TryGetElement(classType, out var characterClassDefinition)
                        && sourceCharacter.ClassesAndLevels != null
                        && sourceCharacter.ClassesAndLevels.TryGetValue(characterClassDefinition, out var classLevel))
                    {
                        sourceAmount = classLevel;
                    }

                    break;
            }

            return rulesetActor.InflictCondition(conditionDefinitionName, durationType, durationParameter, endOccurence,
                tag, sourceGuid, sourceFaction, effectLevel, effectDefinitionName, sourceAmount, sourceAbilityBonus,
                sourceProficiencyBonus, bardicInspirationDie);
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var addedConditionPos = Main.IsDebugBuild ? 37 : 27;
            var found = 0;
            var inflictConditionMethod = typeof(RulesetActor).GetMethod("InflictCondition");
            var extendInflictConditionMethod =
                typeof(ApplySummonForm_Patch).GetMethod("ExtendInflictCondition");

            foreach (var instruction in instructions)
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
