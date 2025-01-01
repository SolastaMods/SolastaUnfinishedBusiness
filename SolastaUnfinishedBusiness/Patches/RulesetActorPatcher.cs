using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Subclasses.Builders;
using SolastaUnfinishedBusiness.Validators;
using TA;
using UnityEngine;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static ConsoleStyleDuplet;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetActorPatcher
{
    //PATCH: supports DieRollModifierDamageTypeDependent
    private static void EnumerateIDieRollModificationProvider(
        RulesetCharacter __instance,
        List<FeatureDefinition> featuresToBrowse,
        Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin)
    {
        __instance.EnumerateFeaturesToBrowse<IDieRollModificationProvider>(__instance.featuresToBrowse, featuresOrigin);

        if (__instance.featuresToBrowse.Count == 0)
        {
            return;
        }

        var effectForms =
            RulesetCharacterPatcher.RollMagicAttack_Patch.CurrentMagicEffect?.EffectDescription.EffectForms;
        var damageForm = RollDamage_Patch.CurrentDamageForm;
        List<string> damageTypes = [];

        if (damageForm != null)
        {
            damageTypes.Add(damageForm.DamageType);
        }

        if (effectForms != null)
        {
            damageTypes.AddRange(effectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Select(x => x.DamageForm.DamageType)
                .ToArray());

            var proxies = effectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Summon &&
                            x.SummonForm.SummonType == SummonForm.Type.EffectProxy)
                .Select(x =>
                    DatabaseHelper.GetDefinition<EffectProxyDefinition>(x.SummonForm.EffectProxyDefinitionName))
                .ToArray();

            var damageTypesFromProxyAttacks = proxies
                .Where(x => x.canAttack && x.attackMethod == ProxyAttackMethod.CasterSpellAbility)
                .Select(x => x.DamageType)
                .ToArray();

            var damageTypesFromProxyAttackPowers = proxies
                .Where(x => x.attackPower)
                .Select(x => x.attackPower)
                .SelectMany(x => x.EffectDescription.EffectForms)
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Select(x => x.DamageForm.DamageType)
                .ToArray();

            damageTypes.AddRange(damageTypesFromProxyAttacks);
            damageTypes.AddRange(damageTypesFromProxyAttackPowers);
        }

        if (damageTypes.Count == 0)
        {
            return;
        }

        __instance.featuresToBrowse.RemoveAll(x =>
            x.GetAllSubFeaturesOfType<IValidateDieRollModifier>().Any(y =>
                !y.CanModifyRoll(__instance, __instance.featuresToBrowse, damageTypes)));
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.AddConditionOfCategory))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AddConditionOfCategory_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            RulesetActor __instance,
            ref string category,
            ref RulesetCondition newCondition)
        {
            //PATCH: allow conditions to force specific category
            if (!newCondition.conditionDefinition)
            {
                return;
            }

            // Enable RulesetConditionCustom
            var replaceWithRulesetConditionCustom = newCondition.conditionDefinition
                .GetFirstSubFeatureOfType<IBindToRulesetConditionCustom>();
            if (replaceWithRulesetConditionCustom != null)
            {
                var originalCondition = newCondition;
                // The original condition is yet to register, however it is got from its pool, so we should return it
                replaceWithRulesetConditionCustom.ReplaceRulesetCondition(originalCondition, out newCondition);
                if (originalCondition != newCondition)
                {
                    RulesetCondition.objectPool.Return(originalCondition);
                }
            }

            var feature = newCondition.conditionDefinition.GetFirstSubFeatureOfType<IForceConditionCategory>();

            if (feature == null)
            {
                return;
            }

            category = feature.GetForcedCategory(__instance, newCondition, category);
        }

        [UsedImplicitly]
        public static void Postfix(RulesetActor __instance, RulesetCondition newCondition)
        {
            RulesContext.AddLightSourceIfNeeded(__instance, newCondition);

            var definition = newCondition.ConditionDefinition;

            if (__instance is not RulesetCharacter rulesetCharacter)
            {
                return;
            }

            //PATCH: notifies custom condition features that condition is applied
            definition.GetAllSubFeaturesOfType<IOnConditionAddedOrRemoved>()
                .Do(c => c.OnConditionAdded(rulesetCharacter, newCondition));

            definition.Features
                .SelectMany(f => f.GetAllSubFeaturesOfType<IOnConditionAddedOrRemoved>())
                .Do(c => c.OnConditionAdded(rulesetCharacter, newCondition));
        }
    }

    //PATCH: supports ExtraSituationalContext that touches ArmorClass
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RefreshFlagArmorClassDependencyToPositioning))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshFlagArmorClassDependencyToPositioning_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetActor __instance)
        {
            __instance.isArmorClassDependentToPositioning = __instance
                .GetFeaturesByType<FeatureDefinitionAttributeModifier>()
                .Any(attributeModifier =>
                    attributeModifier.ModifiedAttribute == AttributeDefinitions.ArmorClass &&
                    (attributeModifier.SituationalContext
                         is SituationalContext.AttackerAwayFromTarget
                         or SituationalContext.AttackerNextToTarget
                         or SituationalContext.AttackerOnHigherGroundThanTarget
                         or SituationalContext.NextToWallWithShieldAndMaxMediumArmor
                         or SituationalContext.RagingSurroundedByEnemies
                         or SituationalContext.ConsciousAllyNextToTarget
                         or SituationalContext.WearingShieldEnemyNextToAbleEnemy ||
                     // BEGIN PATCH
                     (ExtraSituationalContext)attributeModifier.SituationalContext
                     is ExtraSituationalContext.HasBladeMasteryWeaponTypesInHands
                     or ExtraSituationalContext.HasGreatswordInHands
                     or ExtraSituationalContext.HasLongswordInHands
                     or ExtraSituationalContext.HasMeleeWeaponInMainHandAndFreeOffhand
                     or ExtraSituationalContext.HasMonkMeleeWeaponInMainHand
                     or ExtraSituationalContext.IsNotInBrightLight
                     or ExtraSituationalContext.IsRagingAndDualWielding
                     or ExtraSituationalContext.AttackerWithMeleeOrUnarmedAndTargetWithinReachOrYeomanWithLongbow
                     or ExtraSituationalContext.NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget
                     or ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield
                     or ExtraSituationalContext.WearingNoArmorOrLightArmorWithTwoHandedQuarterstaff));
            // END PATCH

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.InflictDamage))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InflictDamage_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            ref int rolledDamage,
            string damageType,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            RollInfo rollInfo)
        {
            // supports Shield Techniques feat
            if (formsParams.targetCharacter is { Side: Side.Ally } &&
                formsParams.targetCharacter.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, "ConditionFeatShieldTechniquesMark"))
            {
                rolledDamage /= 2;
            }

            //PATCH: support for FeatureDefinitionReduceDamage
            var reduction = FeatureDefinitionReduceDamage.DamageReduction(formsParams, rolledDamage, damageType);

            rolledDamage -= reduction;
            //rollInfo.modifier -= reduction;
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.InflictCondition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InflictCondition_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            RulesetActor __instance,
            string conditionDefinitionName,
            ulong sourceGuid,
            ref int sourceAmount)
        {
            //PATCH: Implements `ExtraOriginOfAmount`
            var sourceCharacter = EffectHelpers.GetCharacterByGuid(sourceGuid);

            if (sourceCharacter == null)
            {
                return;
            }

            if (!DatabaseHelper.TryGetDefinition<ConditionDefinition>(conditionDefinitionName, out var addedCondition))
            {
                return;
            }

            // Find a better place to put this in?
            var source = addedCondition.AdditionalDamageType;

            switch (addedCondition.AmountOrigin)
            {
                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus:
                    sourceAmount =
                        sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel:
                    sourceAmount = sourceCharacter.GetClassLevel(source);
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceAbilityBonus:
                    sourceAmount = Math.Max(1,
                        AttributeDefinitions.ComputeAbilityScoreModifier(sourceCharacter.TryGetAttributeValue(source)));
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceCopyAttributeFromSummoner:
                    if (sourceCharacter.TryGetAttribute(source, out var attribute))
                    {
                        __instance.Attributes.Add(source, attribute);
                    }

                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyAndAbilityBonus:
                    sourceAmount =
                        sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus) +
                        AttributeDefinitions.ComputeAbilityScoreModifier(sourceCharacter.TryGetAttributeValue(source));
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceGambitDieRoll:
                    var dieType = GambitsBuilders.GetGambitDieSize(sourceCharacter);
                    var dieRoll = RollDie(dieType, AdvantageType.None, out _, out _);

                    sourceCharacter.ShowDieRoll(dieType, dieRoll, title: "Feedback/&AdditionalDamageGambitDieFormat");
                    sourceAmount = dieRoll;
                    break;

                //Do nothing for default origins
                case ConditionDefinition.OriginOfAmount.None:
                case ConditionDefinition.OriginOfAmount.SourceDamage:
                case ConditionDefinition.OriginOfAmount.SourceGain:
                case ConditionDefinition.OriginOfAmount.AddDice:
                case ConditionDefinition.OriginOfAmount.Fixed:
                case ConditionDefinition.OriginOfAmount.SourceHalfHitPoints:
                case ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility:
                case ConditionDefinition.OriginOfAmount.SourceSpellAttack:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(conditionDefinitionName));
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.ProcessConditionsMatchingOccurenceType))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ProcessConditionsMatchingOccurenceType_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetActor __instance, TurnOccurenceType occurenceType)
        {
            //PATCH: support for `ExtraTurnOccurenceType.StartOfSourceTurn`
            RemoveStartOfSourceTurnOccuranceIfNeeded(__instance, occurenceType);
        }

        private static void RemoveStartOfSourceTurnOccuranceIfNeeded(
            // ReSharper disable once SuggestBaseTypeForParameter
            RulesetActor __instance,
            TurnOccurenceType occurenceType)
        {
            if (Gui.Battle == null)
            {
                return;
            }

            if (occurenceType != TurnOccurenceType.StartOfTurn)
            {
                return;
            }

            foreach (var contender in Gui.Battle.AllContenders
                         .Where(x => x is { destroying: false, destroyedBody: false, RulesetActor: not null })
                         .ToArray())
            {
                var conditionsToRemove = new List<RulesetCondition>();

                conditionsToRemove.AddRange(
                    contender.RulesetActor.ConditionsByCategory
                        .SelectMany(x => x.Value)
                        .Where(x =>
                            x.SourceGuid == __instance.Guid &&
                            //TODO: check this later with proper QA
                            // x.RemainingRounds == 0 &&
                            x.EndOccurence == (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn));

                foreach (var conditionToRemove in conditionsToRemove)
                {
                    contender.RulesetActor.RemoveCondition(conditionToRemove);
                }
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RollDamage))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollDamage_Patch
    {
        internal static DamageForm CurrentDamageForm;

        [UsedImplicitly]
        public static void Prefix(RulesetActor __instance, DamageForm damageForm, ref bool maximumDamage)
        {
            if (__instance is RulesetCharacter rulesetCharacter)
            {
                maximumDamage = rulesetCharacter
                    .GetEffectControllerOrSelf()
                    .GetSubFeaturesByType<IForceMaxDamageTypeDependent>()
                    .Any(x => x.IsValid(__instance, damageForm));
            }

            CurrentDamageForm = damageForm;
        }

        [UsedImplicitly]
        public static void Postfix()
        {
            CurrentDamageForm = null;
        }
    }

    //PATCH: allow additional dice on recurrent damage form to be correctly calculated from effect advancement
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.ExecuteRecurrentForms))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteRecurrentForms_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetActor __instance, RulesetCondition rulesetCondition)
        {
            if (rulesetCondition.ConditionDefinition.RecurrentEffectForms.Count <= 0)
            {
                return false;
            }

            var service = ServiceRepository.GetService<IRulesetImplementationService>();
            var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();
            var entity = RulesetEntity.GetEntity<RulesetCharacter>(rulesetCondition.SourceGuid);

            formsParams.FillSourceAndTarget(entity, __instance);

            var trackingCondition = entity?.FindEffectTrackingCondition(rulesetCondition);

            if (trackingCondition == null)
            {
                return false;
            }

            formsParams.FillFromActiveEffect(trackingCondition);

            //BEGIN PATCH
            var effectAdvancement = trackingCondition.EffectDescription.EffectAdvancement;

            formsParams.addDice = effectAdvancement.EffectIncrementMethod switch
            {
                EffectIncrementMethod.PerAdditionalSlotLevel => trackingCondition.EffectDescription
                    .EffectAdvancement.additionalDicePerIncrement * (trackingCondition.EffectLevel -
                                                                     (trackingCondition.GetEffectSource() is
                                                                         SpellDefinition spellDefinition
                                                                         ? spellDefinition.SpellLevel
                                                                         : 0)),
                EffectIncrementMethod.CasterLevelTable => trackingCondition.EffectDescription.EffectAdvancement
                    .ComputeAdditionalDiceByCasterLevel(
                        __instance.TryGetAttributeValue(AttributeDefinitions.CharacterLevel)),
                _ => formsParams.addDice
            };
            //END PATCH

            if (rulesetCondition.ConditionDefinition.AmountOrigin == ConditionDefinition.OriginOfAmount.AddDice)
            {
                formsParams.addDice = rulesetCondition.Amount;
            }

            formsParams.formAbilityBonus = rulesetCondition.SourceAbilityBonus;
            service.ApplyEffectForms(
                rulesetCondition.ConditionDefinition.RecurrentEffectForms, formsParams, [], out _, out _);

            var effectFormsApplied = service.ConditionRecurrentEffectFormsApplied;

            effectFormsApplied?.Invoke(__instance, rulesetCondition);

            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RemoveCondition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RemoveCondition_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollSavingThrowMethod = typeof(RulesetActor).GetMethod("RollSavingThrow");
            var myRollSavingThrowMethod =
                typeof(RemoveCondition_Patch).GetMethod("RollSavingThrow");

            return instructions
                .ReplaceCalls(rollSavingThrowMethod,
                    "RulesetActor.RemoveCondition",
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, myRollSavingThrowMethod));
        }

        [UsedImplicitly]
        public static void RollSavingThrow(
            RulesetCharacter __instance,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            RulesetCondition rulesetCondition)
        {
            var caster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);
            var effectForms = new List<EffectForm>();

            rulesetCondition.BuildDummyEffectForms(effectForms);
            __instance.MyRollSavingThrow(
                caster,
                saveBonus,
                abilityScoreName,
                sourceDefinition,
                modifierTrends,
                advantageTrends,
                rollModifier,
                saveDC,
                hasHitVisual,
                ref outcome,
                ref outcomeDelta,
                effectForms);
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.ProcessConditionsMatchingInterruption))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ProcessConditionsMatchingInterruption_Patch
    {
#if false
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollSavingThrowMethod = typeof(RulesetActor).GetMethod("RollSavingThrow");
            var myRollSavingThrowMethod =
                typeof(RemoveCondition_Patch).GetMethod("RollSavingThrow");

            return instructions
                .ReplaceCalls(rollSavingThrowMethod,
                    "RulesetActor.ProcessConditionsMatchingInterruption",
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Call, myRollSavingThrowMethod));
        }

        [UsedImplicitly]
        public static void RollSavingThrow(
            RulesetCharacter __instance,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            RulesetCondition rulesetCondition)
        {
            var caster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);
            var effectForms = new List<EffectForm>();

            rulesetCondition.BuildDummyEffectForms(effectForms);
            __instance.MyRollSavingThrow(
                caster,
                saveBonus,
                abilityScoreName,
                sourceDefinition,
                modifierTrends,
                advantageTrends,
                rollModifier,
                saveDC,
                hasHitVisual,
                ref outcome,
                ref outcomeDelta,
                effectForms);
        }
#endif

        [UsedImplicitly]
        public static bool Prefix(RulesetActor __instance, ConditionInterruption interruption, int amount)
        {
            ProcessConditionsMatchingInterruption(__instance, interruption, amount);

            return false;
        }

        private static void ProcessConditionsMatchingInterruption(
            RulesetActor __instance, ConditionInterruption interruption, int amount = 0)
        {
            if (__instance.matchingInterruption)
            {
                return;
            }

            __instance.matchingInterruption = true;
            __instance.matchingInterruptionConditions.Clear();

            //PATCH: avoid enumeration issues with ToArray
            //reported interaction between Disheartening Performance and Hideous Laughter
            foreach (var rulesetCondition in __instance.conditionsByCategory.SelectMany(x => x.Value).ToArray())
            {
                if (!rulesetCondition.ConditionDefinition.HasSpecialInterruptionOfType(interruption) ||
                    __instance.matchingInterruptionConditions.Contains(rulesetCondition) ||
                    (interruption == ConditionInterruption.NoAttackOrDamagedInTurn &&
                     rulesetCondition.FirstRound) ||
                    (amount < rulesetCondition.ConditionDefinition.InterruptionDamageThreshold &&
                     interruption
                         is ConditionInterruption.Damaged
                         or ConditionInterruption.AttacksAndDamages
                         or ConditionInterruption.DamagedByFriendly))
                {
                    continue;
                }

                var matched = false;

                if (rulesetCondition.ConditionDefinition.InterruptionRequiresSavingThrow &&
                    (!string.IsNullOrEmpty(rulesetCondition.SaveOverrideAbilityScoreName) ||
                     rulesetCondition.ConditionDefinition.InterruptionSavingThrowComputationMethod !=
                     InterruptionSavingThrowComputationMethod.SaveOverride))
                {
                    var effectModifier = new ActionModifier();
                    var abilityScoreName = string.Empty;
                    var saveDC = 0;
                    var savingThrowAffinity =
                        rulesetCondition.ConditionDefinition.InterruptionSavingThrowAffinity;
                    var schoolOfMagic = string.Empty;
                    var empty1 = string.Empty;
                    var empty2 = string.Empty;

                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (rulesetCondition.ConditionDefinition.InterruptionSavingThrowComputationMethod)
                    {
                        case InterruptionSavingThrowComputationMethod.SaveOverride:
                            abilityScoreName = rulesetCondition.SaveOverrideAbilityScoreName;
                            saveDC = rulesetCondition.SaveOverrideDC;
                            break;
                        case InterruptionSavingThrowComputationMethod.LikeConcentration:
                        {
                            abilityScoreName = rulesetCondition.ConditionDefinition.InterruptionSavingThrowAbility;
                            saveDC = Mathf.FloorToInt(amount / 2f);
                            if (saveDC < 10)
                            {
                                saveDC = 10;
                            }

                            break;
                        }
                    }

                    BaseDefinition sourceDefinition = null;

                    if (DatabaseRepository.GetDatabase<SpellDefinition>()
                        .TryGetElement(rulesetCondition.SaveOverrideSourceName, out var result1))
                    {
                        sourceDefinition = result1;
                        schoolOfMagic = result1.SchoolOfMagic;
                    }
                    else if (DatabaseRepository.GetDatabase<FeatureDefinition>()
                             .TryGetElement(rulesetCondition.SaveOverrideSourceName, out var result2))
                    {
                        sourceDefinition = result2;
                    }
                    else if (rulesetCondition.EffectDefinitionName != null)
                    {
                        if (DatabaseRepository.GetDatabase<SpellDefinition>()
                            .TryGetElement(rulesetCondition.EffectDefinitionName, out result1))
                        {
                            sourceDefinition = result1;
                            schoolOfMagic = result1.SchoolOfMagic;
                        }
                        else if (DatabaseRepository.GetDatabase<FeatureDefinition>()
                                 .TryGetElement(rulesetCondition.EffectDefinitionName, out result2))
                        {
                            sourceDefinition = result2;
                        }
                    }

                    var savingThrowBonus =
                        __instance.ComputeBaseSavingThrowBonus(abilityScoreName,
                            effectModifier.SavingThrowModifierTrends);
                    __instance.ComputeSavingThrowModifier(abilityScoreName, EffectForm.EffectFormType.Condition, empty1,
                        schoolOfMagic, string.Empty, rulesetCondition.ConditionDefinition.Name, empty2,
                        effectModifier, __instance.accountedProviders);
                    __instance.accountedProviders.Clear();

                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (savingThrowAffinity)
                    {
                        case AdvantageType.Advantage:
                            effectModifier.SavingThrowAdvantageTrends.Add(new TrendInfo(1,
                                FeatureSourceType.Spell, rulesetCondition.EffectDefinitionName, __instance));
                            break;
                        case AdvantageType.Disadvantage:
                            effectModifier.SavingThrowAdvantageTrends.Add(new TrendInfo(-1,
                                FeatureSourceType.Spell, rulesetCondition.EffectDefinitionName, __instance));
                            break;
                    }

                    //BEGIN PATCH
                    // __instance.RollSavingThrow(savingThrowBonus, str, sourceDefinition,
                    //     effectModifier.SavingThrowModifierTrends, effectModifier.SavingThrowAdvantageTrends,
                    //     effectModifier.GetSavingThrowModifier(str), saveDC, false, out var outcome,
                    //     out _);
                    var caster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);
                    var effectForms = new List<EffectForm>();
                    var outcomeDelta = 0;
                    var outcome = RollOutcome.Failure;

                    rulesetCondition.BuildDummyEffectForms(effectForms);
                    __instance.MyRollSavingThrow(
                        caster,
                        savingThrowBonus,
                        abilityScoreName,
                        sourceDefinition,
                        effectModifier.SavingThrowModifierTrends,
                        effectModifier.SavingThrowAdvantageTrends,
                        effectModifier.GetSavingThrowModifier(abilityScoreName),
                        saveDC,
                        false,
                        ref outcome,
                        ref outcomeDelta,
                        effectForms);
                    //END PATCH

                    if (outcome == RollOutcome.Success ==
                        !rulesetCondition.ConditionDefinition.KeepConditionIfSavingThrowSucceeds)
                    {
                        matched = true;
                    }
                }
                else
                {
                    matched = true;
                }

                if (!matched)
                {
                    continue;
                }

                __instance.matchingInterruptionConditions.Add(rulesetCondition);

                if (rulesetCondition.ConditionDefinition.RecurrentEffectForms.All(x =>
                        x.FormType != EffectForm.EffectFormType.TemporaryHitPoints))
                {
                    continue;
                }

                //PATCH: avoid enumeration issues with ToArray
                foreach (var condition in __instance.ConditionsByCategory.SelectMany(x => x.Value).ToArray())
                {
                    if (condition.ConditionDefinition.IsSubtypeOf("ConditionTemporaryHitPoints"))
                    {
                        __instance.matchingInterruptionConditions.TryAdd(condition);
                    }
                }
            }

            for (var index = __instance.matchingInterruptionConditions.Count - 1; index >= 0; --index)
            {
                __instance.RemoveCondition(__instance.matchingInterruptionConditions[index]);
            }

            __instance.matchingInterruptionConditions.Clear();
            __instance.matchingInterruption = false;
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RemoveConditionOfCategory))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RemoveConditionOfCategory_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetActor __instance, string category, RulesetCondition rulesetCondition)
        {
            //PATCH: support for action switching
            if (!Main.Settings.EnableActionSwitching)
            {
                return;
            }

            if (__instance is not RulesetCharacter character)
            {
                return;
            }

            if (!character.ConditionsByCategory.ContainsKey(category))
            {
                return;
            }

            ActionSwitching.AccountRemovedCondition(character, rulesetCondition);
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RemoveAllConditionsOfCategory))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RemoveAllConditionsOfCategory_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetActor __instance, string category)
        {
            //PATCH: support for action switching
            if (!Main.Settings.EnableActionSwitching)
            {
                return;
            }

            if (__instance is not RulesetCharacter character)
            {
                return;
            }

            if (!character.ConditionsByCategory.TryGetValue(category, out var value))
            {
                return;
            }

            foreach (var rulesetCondition in value)
            {
                ActionSwitching.AccountRemovedCondition(character, rulesetCondition);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RemoveAllConditionsOfCategoryExcludingSources))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RemoveAllConditionsOfCategoryExcludingSources_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetActor __instance, string category, List<ulong> sources)
        {
            //PATCH: support for action switching
            if (!Main.Settings.EnableActionSwitching)
            {
                return;
            }

            if (__instance is not RulesetCharacter character)
            {
                return;
            }

            if (!character.ConditionsByCategory.TryGetValue(category, out var value))
            {
                return;
            }

            foreach (var rulesetCondition in value.Where(rulesetCondition =>
                         !sources.Contains(rulesetCondition.SourceGuid)))
            {
                ActionSwitching.AccountRemovedCondition(character, rulesetCondition);
            }
        }
    }

    //PATCH: handle exception case of Aura of Vitality spell where it should have future immunity but not remove cond
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.HandleConditionImmunity))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleConditionImmunity_Patch
    {
        private static FeatureDefinitionConditionAffinity _conditionAffinity;

        [UsedImplicitly]
        public static void Prefix(List<FeatureDefinitionConditionAffinity> conditionAffinities)
        {
            _conditionAffinity =
                conditionAffinities.FirstOrDefault(x =>
                    x.Name == "ConditionAffinityAuraOfVitalityLifeDrained");

            if (_conditionAffinity)
            {
                conditionAffinities.Remove(_conditionAffinity);
            }
        }

        [UsedImplicitly]
        public static void Postfix(List<FeatureDefinitionConditionAffinity> conditionAffinities)
        {
            if (_conditionAffinity)
            {
                conditionAffinities.Add(_conditionAffinity);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RemoveAllConditionsOfCategoryAndType))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RemoveAllConditionsOfCategoryAndType_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetActor __instance, string category, string type)
        {
            //PATCH: support for action switching
            if (!Main.Settings.EnableActionSwitching)
            {
                return;
            }

            if (__instance is not RulesetCharacter character)
            {
                return;
            }

            if (!character.ConditionsByCategory.TryGetValue(category, out var value))
            {
                return;
            }

            foreach (var rulesetCondition in value.Where(rulesetCondition =>
                         rulesetCondition.ConditionDefinition.Name == type ||
                         rulesetCondition.ConditionDefinition.IsSubtypeOf(type)))
            {
                ActionSwitching.AccountRemovedCondition(character, rulesetCondition);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.ModulateSustainedDamage))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ModulateSustainedDamage_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var myEnumerate = new Action<
                RulesetActor,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, FeatureOrigin>,
                ulong
            >(EnumerateIDamageAffinityProvider).Method;

            return instructions
                .ReplaceEnumerateFeaturesToBrowse<IDamageAffinityProvider>(-1,
                    "RulesetActor.ModulateSustainedDamage",
                    new CodeInstruction(OpCodes.Ldarg, 4), // source guid
                    new CodeInstruction(OpCodes.Call, myEnumerate));
        }

        private static void EnumerateIDamageAffinityProvider(
            RulesetActor actor,
            List<FeatureDefinition> featuresToBrowse,
            Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin,
            ulong guid)
        {
            //PATCH: supports IIgnoreDamageAffinity   
            actor.EnumerateFeaturesToBrowse<IDamageAffinityProvider>(featuresToBrowse, featuresOrigin);

            ServiceRepository.GetService<IRulesetEntityService>().TryGetEntityByGuid(guid, out var rulesetEntity);

            var caster = rulesetEntity switch
            {
                RulesetCharacterEffectProxy rulesetCharacterEffectProxy =>
                    EffectHelpers.GetCharacterByGuid(rulesetCharacterEffectProxy.ControllerGuid),
                RulesetCharacter rulesetCharacter => rulesetCharacter,
                _ => null
            };

            if (caster != null)
            {
                var features = caster.GetSubFeaturesByType<IModifyDamageAffinity>();

                foreach (var feature in features)
                {
                    feature.ModifyDamageAffinity(actor, caster, featuresToBrowse);
                }
            }

            //PATCH: allow to remove damage affinities if necessary
            var featuresActor = actor.GetSubFeaturesByType<IModifyDamageAffinity>();

            foreach (var feature in featuresActor)
            {
                feature.ModifyDamageAffinity(actor, caster, featuresToBrowse);
            }

            if (actor is not RulesetCharacterHero hero)
            {
                return;
            }

            //PATCH: add `IDamageAffinityProvider` from dynamic item properties
            //fixes game not applying damage reductions from dynamic item properties
            //used for Inventor's Resistant Armor infusions
            foreach (var equipedItem in hero.CharacterInventory.InventorySlotsByName
                         .Select(keyValuePair => keyValuePair.Value)
                         .Where(slot => slot.EquipedItem != null && !slot.Disabled && !slot.ConfigSlot)
                         .Select(slot => slot.EquipedItem))
            {
                featuresToBrowse.AddRange(equipedItem.DynamicItemProperties
                    .Select(dynamicItemProperty => dynamicItemProperty.FeatureDefinition)
                    .Where(definition => definition is IDamageAffinityProvider));
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RollDiceAndSum))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollDiceAndSum_Patch
    {
        //PATCH: supports DieRollModifierDamageTypeDependent
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .ReplaceEnumerateFeaturesToBrowse<IDieRollModificationProvider>("RulesetCharacter.RollDiceAndSum",
                    EnumerateIDieRollModificationProvider);
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RollDie))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollDie_Patch
    {
        //PATCH: supports DieRollModifierDamageTypeDependent
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollDieMethod = typeof(RuleDefinitions).GetMethod("RollDie", BindingFlags.Public | BindingFlags.Static);
            var myRollDieMethod = typeof(RollDie_Patch).GetMethod("RollDie");

            return instructions
                .ReplaceCalls(rollDieMethod, "RulesetActor.RollDie.1",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call, myRollDieMethod))
                .ReplaceEnumerateFeaturesToBrowse<IDieRollModificationProvider>("RulesetCharacter.RollDie.2",
                    EnumerateIDieRollModificationProvider);
        }

        [UsedImplicitly]
        public static int RollDie(
            DieType dieType,
            AdvantageType advantageType,
            out int firstRoll,
            out int secondRoll,
            float rollAlterationScore,
            RulesetActor actor,
            RollContext rollContext)
        {
            int result;

            if (rollContext == RollContext.AttackRoll &&
                advantageType == AdvantageType.Advantage && ElvenPrecision.Active)
            {
                result = Roll3DicesAndKeepBest(actor.Name, dieType, out firstRoll, out secondRoll, rollAlterationScore);
            }
            else
            {
                var changeDiceRollList = actor.GetSubFeaturesByType<IModifyDiceRoll>();

                foreach (var changeDiceRoll in changeDiceRollList)
                {
                    changeDiceRoll.BeforeRoll(rollContext, actor as RulesetCharacter,
                        ref dieType,
                        ref advantageType);
                }

                result = RuleDefinitions.RollDie(
                    dieType, advantageType, out firstRoll, out secondRoll, rollAlterationScore);

                foreach (var changeDiceRoll in changeDiceRollList)
                {
                    changeDiceRoll.AfterRoll(
                        dieType,
                        advantageType,
                        rollContext,
                        actor as RulesetCharacter,
                        ref firstRoll,
                        ref secondRoll,
                        ref result);
                }
            }

            if (rollContext != RollContext.AttackRoll)
            {
                return result;
            }

            var glc = GameLocationCharacter.GetFromActor(actor);

            if (glc == null)
            {
                return result;
            }

            var lowestAttackRoll = Math.Min(firstRoll, secondRoll);
            var highestAttackRoll = Math.Max(firstRoll, secondRoll);

            if (!glc.UsedSpecialFeatures.TryAdd("LowestAttackRoll", lowestAttackRoll))
            {
                glc.UsedSpecialFeatures["LowestAttackRoll"] = lowestAttackRoll;
            }

            if (!glc.UsedSpecialFeatures.TryAdd("HighestAttackRoll", highestAttackRoll))
            {
                glc.UsedSpecialFeatures["HighestAttackRoll"] = highestAttackRoll;
            }

            return result;
        }

        private static int Roll3DicesAndKeepBest(
            string roller,
            DieType diceType,
            out int firstRoll,
            out int secondRoll,
            float rollAlterationScore)
        {
            var karmic = rollAlterationScore != 0.0;

            var roll1 = DoRoll();
            var roll2 = DoRoll();
            var roll3 = DoRoll();

            var kept = Math.Max(roll1, roll2);
            var replaced = Math.Min(roll1, roll2);

            var entry = new GameConsoleEntry("Feedback/&ElvenAccuracyTriggered",
                Gui.Game.GameConsole.consoleTableDefinition);

            entry.AddParameter(ParameterType.Player, roller);
            entry.AddParameter(ParameterType.AttackSpellPower, "Tooltip/&FeatElvenAccuracyBaseTitle",
                tooltipContent: "Tooltip/&FeatElvenAccuracyBaseDescription");
            entry.AddParameter(ParameterType.AbilityInfo, kept.ToString());
            entry.AddParameter(ParameterType.AbilityInfo, replaced.ToString());
            entry.AddParameter(ParameterType.AbilityInfo, roll3.ToString());

            Gui.Game.GameConsole.AddEntry(entry);

            firstRoll = kept;
            secondRoll = roll3;

            return Mathf.Max(firstRoll, secondRoll);

            int DoRoll()
            {
                return karmic
                    ? RollKarmicDie(diceType, rollAlterationScore)
                    : 1 + DeterministicRandom.Range(0, DiceMaxValue[(int)diceType]);
            }
        }

        //PATCH: avoid an infinite loop trying to re-roll D1s
        [UsedImplicitly]
        public static void Prefix(
            DieType dieType,
            ref bool canRerollDice)
        {
            canRerollDice = dieType != DieType.D1 && canRerollDice;
        }
    }

    //PATCH: uses class level instead of character level on attributes calculation (Multiclass)
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RefreshAttributes))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshAttributes_Patch
    {
        // private static readonly Regex ClassPattern = new($"{AttributeDefinitions.TagClass}(.*)\\d+");

        private static void RefreshClassModifiers(RulesetActor actor)
        {
            if (actor is not RulesetCharacter rulesetCharacter)
            {
                return;
            }

            var hero = rulesetCharacter.GetOriginalHero();

            if (hero == null)
            {
                return;
            }

            foreach (var attribute in actor.Attributes)
            {
                var rulesetAttribute = attribute.Value;

                if (!rulesetAttribute.upToDate)
                {
                    rulesetAttribute.Refresh();
                }

                foreach (var attributeModifier in rulesetAttribute.ActiveModifiers)
                {
                    switch (attributeModifier.Operation)
                    {
                        case AttributeModifierOperation.MultiplyByClassLevel
                            or AttributeModifierOperation.MultiplyByClassLevelBeforeAdditions:
                            attributeModifier.Value = attribute.Key switch
                            {
                                AttributeDefinitions.HealingPool =>
                                    hero.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Paladin),
                                AttributeDefinitions.KiPoints =>
                                    hero.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Monk),
                                AttributeDefinitions.SorceryPoints =>
                                    hero.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Sorcerer),
                                _ => 0
                            };
                            break;
                        case AttributeModifierOperation.AddHalfProficiencyBonus:
                        {
                            var halfPb = hero.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus) / 2;

                            attributeModifier.Value = halfPb;
                            break;
                        }
                        case AttributeModifierOperation.AddProficiencyBonus:
                        {
                            var pb = hero.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

                            attributeModifier.Value = pb;
                            break;
                        }
                        case AttributeModifierOperation.Additive when
                            attribute.Key == AttributeDefinitions.HealingPool:
                        {
                            var levels =
                                hero.GetSubclassLevel(DatabaseHelper.CharacterClassDefinitions.Druid,
                                    CircleOfTheAncientForest.Name) +
                                hero.GetSubclassLevel(DatabaseHelper.CharacterClassDefinitions.Ranger,
                                    RangerLightBearer.Name);

                            attributeModifier.Value = levels * 5;
                            break;
                        }
                    }
                }
            }
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            // needed for sorcery points, healing pools, ki points to be of proper sizes when multiclass
            // adds custom method right before the end that recalculates modifier values specifically for class-level modifiers
            var refreshAttributes = typeof(RulesetEntity).GetMethod("RefreshAttributes");
            var custom = new Action<RulesetActor>(RefreshClassModifiers).Method;

            return instructions.ReplaceCalls(refreshAttributes, "RulesetActor.RefreshAttributes",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, custom),
                new CodeInstruction(OpCodes.Call, refreshAttributes)); // checked for Call vs CallVirtual
        }
    }

    //PATCH: allow ISavingThrowAffinityProvider to be validated with IsCharacterValidHandler
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.ComputeSavingThrowModifier))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeSavingThrowModifier_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse<ISavingThrowAffinityProvider>(
                "RulesetActor.ComputeSavingThrowModifier", EnumerateFeatureDefinitionSavingThrowAffinity);
        }

        private static void EnumerateFeatureDefinitionSavingThrowAffinity(
            RulesetCharacter __instance,
            List<FeatureDefinition> featuresToBrowse,
            Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin)
        {
            __instance.EnumerateFeaturesToBrowse<ISavingThrowAffinityProvider>(featuresToBrowse,
                featuresOrigin);
            featuresToBrowse.RemoveAll(x =>
                !__instance.IsValid(x.GetAllSubFeaturesOfType<IsCharacterValidHandler>()));
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.SerializeElements))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SerializeElements_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: supports class inheriting RulesetCondition for saving serializable data
            //change
            //this.conditionsByCategory = serializer.SerializeElement<string, List<RulesetCondition>>("ConditionsByCategory", this.conditionsByCategory);
            //to
            //this.conditionsByCategory = serializer.SerializeElement<string, List<RulesetCondition>>("ConditionsByCategory", this.conditionsByCategory, Serializer.SerializationOption.SerializeTypeName);
            var originalMethod = typeof(IElementsSerializer)
                .GetMethodExt("SerializeElement", typeof(string), typeof(Dictionary<,>))
                .MakeGenericMethod(typeof(string), typeof(List<RulesetCondition>));
            var replacingMethod = typeof(IElementsSerializer)
                .GetMethodExt("SerializeElement", typeof(string), typeof(Dictionary<,>),
                    typeof(Serializer.SerializationOption))
                .MakeGenericMethod(typeof(string), typeof(List<RulesetCondition>));

            return instructions.ReplaceCalls(
                originalMethod,
                "RulesetActor.SerializeElements",
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Callvirt, replacingMethod));
        }
    }

    //PATCH: allow ISpellAffinityProvider to be validated with IRemoveSpellOrSpellLevelImmunity
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.IsImmuneToSpell))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsImmuneToSpell_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse<ISpellAffinityProvider>(
                "RulesetActor.IsImmuneToSpell", EnumerateFeatureDefinitionSpellImmunity);
        }

        private static void EnumerateFeatureDefinitionSpellImmunity(
            RulesetCharacter __instance,
            List<FeatureDefinition> featuresToBrowse,
            Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin)
        {
            __instance.EnumerateFeaturesToBrowse<ISpellAffinityProvider>(featuresToBrowse, featuresOrigin);
            __instance.GetAllConditions(__instance.AllConditionsForEnumeration);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var rulesetCondition in __instance.AllConditionsForEnumeration)
            {
                var immunityRemovingFeatures = rulesetCondition.conditionDefinition
                    .GetAllSubFeaturesOfType<IRemoveSpellOrSpellLevelImmunity>();

                if (!immunityRemovingFeatures.Any(x => x.IsValid(__instance, rulesetCondition)))
                {
                    continue;
                }

                foreach (var immunityRemovingFeature in immunityRemovingFeatures)
                {
                    featuresToBrowse.RemoveAll(x =>
                        immunityRemovingFeature.ShouldRemoveImmunity(((ISpellAffinityProvider)x).IsImmuneToSpell));
                }
            }
        }
    }

    //PATCH: allow ISpellAffinityProvider to be validated with IRemoveSpellOrSpellLevelImmunity
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.IsImmuneToSpellLevel))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsImmuneToSpellLevel_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse<ISpellAffinityProvider>(
                "RulesetActor.IsImmuneToSpell", EnumerateFeatureDefinitionSpellImmunityLevel);
        }

        private static void EnumerateFeatureDefinitionSpellImmunityLevel(
            RulesetCharacter __instance,
            List<FeatureDefinition> featuresToBrowse,
            Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin)
        {
            __instance.EnumerateFeaturesToBrowse<ISpellAffinityProvider>(featuresToBrowse, featuresOrigin);
            __instance.GetAllConditions(__instance.AllConditionsForEnumeration);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var rulesetCondition in __instance.AllConditionsForEnumeration)
            {
                var immunityRemovingFeatures = rulesetCondition.conditionDefinition
                    .GetAllSubFeaturesOfType<IRemoveSpellOrSpellLevelImmunity>();
                if (!immunityRemovingFeatures.Any(x => x.IsValid(__instance, rulesetCondition)))
                {
                    continue;
                }

                foreach (var immunityRemovingFeature in immunityRemovingFeatures)
                {
                    featuresToBrowse.RemoveAll(x =>
                        immunityRemovingFeature.ShouldRemoveImmunityLevel(((ISpellAffinityProvider)x)
                            .IsImmuneToSpellLevel));
                }
            }
        }
    }
}
