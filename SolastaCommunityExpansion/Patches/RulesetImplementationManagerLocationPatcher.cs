using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetImplementationManagerLocationPatcher
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsMetamagicOptionAvailable")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsMetamagicOptionAvailable_Patch
    {
        //TODO: improve sunlight blade so it can properly work when twinned
        private static readonly string[] NotAllowedSpells = { "SunlightBlade" };

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

            if (Array.IndexOf(NotAllowedSpells, spellDefinition.Name) >= 0)
            {
                failure = "Cannot be twinned";
                __result = false;

                return;
            }

            if (!Main.Settings.FixSorcererTwinnedLogic)
            {
                return;
            }

            var effectDescription = spellDefinition.effectDescription;
            if (effectDescription.TargetType is TargetType.Individuals or TargetType.IndividualsUnique)
            {
                if (rulesetEffectSpell.ComputeTargetParameter() != 1)
                {
                    failure = FailureFlagInvalidSingleTarget;
                    __result = false;
                }
            }
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

    //PATCH: Implements ExtraOriginOfAmount
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "ApplySummonForm")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetImplementationManagerLocation_ApplySummonForm
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
                typeof(RulesetImplementationManagerLocation_ApplySummonForm).GetMethod("ExtendInflictCondition");

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
