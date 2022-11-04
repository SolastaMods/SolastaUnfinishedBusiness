using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

public static class RulesetImplementationManagerLocationPatcher
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsMetamagicOptionAvailable")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class IsMetamagicOptionAvailable_Patch
    {
        public static void Postfix(
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
                || caster is not RulesetCharacterHero)
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
    public static class IsSituationalContextValid_Patch
    {
        public static void Postfix(
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
    public static class InstantiateActiveDeviceFunction_Patch
    {
        public static bool Prefix(
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
    public static class ApplySummonForm_Patch
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
            var sourceCharacter = (RulesetCharacterHero)formsParams.sourceCharacter;

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

                    // Find a better place to put this in?
                    var classType = addedCondition.AdditionalDamageType;
                    if (DatabaseHelper.TryGetDefinition<CharacterClassDefinition>(classType,
                            out var characterClassDefinition)
                        && sourceCharacter.ClassesAndLevels != null
                        && sourceCharacter.ClassesAndLevels.TryGetValue(characterClassDefinition, out var classLevel))
                    {
                        sourceAmount = classLevel;
                    }

                    break;
                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceAbilityBonus:
                    // Find a better place to put this in?
                    var attributeName = addedCondition.AdditionalDamageType;
                    if (sourceCharacter.TryGetAttribute(attributeName, out var attribute))
                    {
                        sourceAmount = AttributeDefinitions.ComputeAbilityScoreModifier(attribute.CurrentValue);
                    }

                    break;
            }

            return rulesetActor.InflictCondition(conditionDefinitionName, durationType, durationParameter, endOccurence,
                tag, sourceGuid, sourceFaction, effectLevel, effectDefinitionName, sourceAmount, sourceAbilityBonus,
                sourceProficiencyBonus, bardicInspirationDie);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var addedConditionPos = Main.IsDebugBuild ? 37 : 27;
            var inflictConditionMethod = typeof(RulesetActor).GetMethod("InflictCondition");
            var extendInflictConditionMethod =
                typeof(ApplySummonForm_Patch).GetMethod("ExtendInflictCondition");

            return instructions.ReplaceCode(instruction => instruction.Calls(inflictConditionMethod),
                3,
                0,
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldloc_S, addedConditionPos),
                new CodeInstruction(OpCodes.Call, extendInflictConditionMethod));
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "ApplyMotionForm")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ApplyMotionForm_Patch
    {
        public static bool Prefix(EffectForm effectForm, RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            //PATCH: support for `PushesFromEffectPoint`
            // allows push/grab motion effects to work relative to casting point, instead of caster's position
            // used for Grenadier's force grenades
            // if effect source definition has marker, and forms params have position, will try to push target from that point

            return PushesFromEffectPoint.TryPushFromEffectTargetPoint(effectForm, formsParams);
        }
    }
}
