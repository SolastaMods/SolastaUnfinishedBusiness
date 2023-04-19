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
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Subclasses;
using TA;
using UnityEngine;
using static ConsoleStyleDuplet;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetActorPatcher
{
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.AddConditionOfCategory))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AddConditionOfCategory_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            RulesetActor __instance,
            ref string category,
            RulesetCondition newCondition)
        {
            //PATCH: allow conditions to force specific category
            if (newCondition.conditionDefinition == null)
            {
                return;
            }

            var feature = newCondition.conditionDefinition.GetFirstSubFeatureOfType<IForceConditionCategory>();

            if (feature == null)
            {
                return;
            }

            category = feature.GetForcedCategory(__instance, newCondition, category);
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

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonusNegative:
                    sourceAmount =
                        -sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceCharacterLevel:
                    sourceAmount =
                        sourceCharacter.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel:
                    sourceAmount = sourceCharacter.GetClassLevel(source);
                    break;

                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceAbilityBonus:
                    sourceAmount =
                        AttributeDefinitions.ComputeAbilityScoreModifier(sourceCharacter.TryGetAttributeValue(source));
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

                case ConditionDefinition.OriginOfAmount.None:
                    break;
                case ConditionDefinition.OriginOfAmount.SourceDamage:
                    break;
                case ConditionDefinition.OriginOfAmount.SourceGain:
                    break;
                case ConditionDefinition.OriginOfAmount.AddDice:
                    break;
                case ConditionDefinition.OriginOfAmount.Fixed:
                    break;
                case ConditionDefinition.OriginOfAmount.SourceHalfHitPoints:
                    break;
                case ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility:
                    break;
                case ConditionDefinition.OriginOfAmount.SourceSpellAttack:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(conditionDefinitionName));
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RemoveCondition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RemoveCondition_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetActor __instance, RulesetCondition rulesetCondition)
        {
            //PATCH: INotifyConditionRemoval
            if (rulesetCondition == null || rulesetCondition.ConditionDefinition == null)
            {
                return;
            }

            foreach (var notifyConditionRemoval in rulesetCondition.ConditionDefinition
                         .GetAllSubFeaturesOfType<INotifyConditionRemoval>())
            {
                notifyConditionRemoval.AfterConditionRemoved(__instance, rulesetCondition);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.ProcessConditionsMatchingOccurenceType))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ProcessConditionsMatchingOccurenceType_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetActor __instance, RuleDefinitions.TurnOccurenceType occurenceType)
        {
            //PATCH: support for `IConditionRemovedOnSourceTurnStart` - removes appropriately marked conditions
            ConditionRemovedOnSourceTurnStartPatch.RemoveConditionIfNeeded(__instance, occurenceType);
        }
    }

#if false
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.ProcessConditionsMatchingInterruption))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class ProcessConditionsMatchingInterruption_Patch
    {
        [UsedImplicitly] public static void Prefix(RulesetActor __instance,
            RuleDefinitions.ConditionInterruption interruption,
            int amount)
        {
            //PATCH: support for 'ProcessConditionInterruptionHandler'
            foreach (var handler in __instance.GetSubFeaturesByType<ProcessConditionInterruptionHandler>())
            {
                handler(__instance, interruption, amount);
            }
        }
    }
#endif

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
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>,
                ulong
            >(MyEnumerate).Method;

            return instructions
                .ReplaceEnumerateFeaturesToBrowse("IDamageAffinityProvider",
                    -1, "RulesetActor.ModulateSustainedDamage",
                    new CodeInstruction(OpCodes.Ldarg, 4), // source guid
                    new CodeInstruction(OpCodes.Call, myEnumerate));
        }

        private static void MyEnumerate(
            RulesetActor actor,
            List<FeatureDefinition> featuresToBrowse,
            Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin,
            ulong guid)
        {
            //PATCH: supports IIgnoreDamageAffinity   
            actor.EnumerateFeaturesToBrowse<IDamageAffinityProvider>(featuresToBrowse, featuresOrigin);

            ServiceRepository.GetService<IRulesetEntityService>().TryGetEntityByGuid(guid, out var rulesetEntity);

            var caster = rulesetEntity as RulesetCharacter;
            var features = caster.GetSubFeaturesByType<IIgnoreDamageAffinity>();

            foreach (var feature in features.ToList())
            {
                featuresToBrowse.RemoveAll(x =>
                    x is IDamageAffinityProvider y && feature.CanIgnoreDamageAffinity(y, caster));
            }

            //PATCH: add `IDamageAffinityProvider` from dynamic item properties
            //fixes game not applying damage reductions from dynamic item properties
            //used for Inventor's Resistant Armor infusions
            if (actor is not RulesetCharacterHero hero)
            {
                return;
            }

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

    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RollDie))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RollDie_Patch
    {
        //PATCH: supports DieRollModifierDamageTypeDependent
        private static void EnumerateIDieRollModificationProvider(
            RulesetCharacter __instance,
            List<FeatureDefinition> featuresToBrowse,
            Dictionary<FeatureDefinition,
                RuleDefinitions.FeatureOrigin> featuresOrigin)
        {
            __instance.EnumerateFeaturesToBrowse<IDieRollModificationProvider>(featuresToBrowse);

            var damageType = RulesetCharacterPatcher.RollMagicAttack_Patch
                .CurrentMagicEffect?.EffectDescription.FindFirstDamageForm()?.damageType;

            if (damageType != null)
            {
                featuresToBrowse.RemoveAll(x =>
                    x is FeatureDefinitionDieRollModifierDamageTypeDependent y && !y.damageTypes.Contains(damageType));
            }
        }


        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollDieMethod = typeof(RuleDefinitions).GetMethod("RollDie", BindingFlags.Public | BindingFlags.Static);
            var myRollDieMethod = typeof(RollDie_Patch).GetMethod("RollDie");
            var enumerate = new Action<
                RulesetCharacter,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(EnumerateIDieRollModificationProvider).Method;

            return instructions
                .ReplaceCalls(rollDieMethod, "RulesetActor.RollDie.1",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call, myRollDieMethod))
                .ReplaceEnumerateFeaturesToBrowse("IDieRollModificationProvider",
                    -1, "RulesetCharacter.RefreshSpellRepertoires",
                    new CodeInstruction(OpCodes.Call, enumerate));
        }

        [UsedImplicitly]
        public static int RollDie(
            RuleDefinitions.DieType dieType,
            RuleDefinitions.AdvantageType advantageType,
            out int firstRoll,
            out int secondRoll,
            float rollAlterationScore,
            RulesetActor actor,
            RuleDefinitions.RollContext rollContext)
        {
            int result;

            if (rollContext == RuleDefinitions.RollContext.AttackRoll &&
                advantageType == RuleDefinitions.AdvantageType.Advantage && ElvenPrecisionLogic.Active)
            {
                result = Roll3DicesAndKeepBest(actor.Name, dieType, out firstRoll, out secondRoll, rollAlterationScore);
            }
            else
            {
                var changeDiceRollList = actor.GetSubFeaturesByType<IChangeDiceRoll>()
                    .Where(x => x.IsValid(rollContext, actor as RulesetCharacter))
                    .ToList();

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
                    changeDiceRoll.AfterRoll(rollContext, actor as RulesetCharacter,
                        ref firstRoll,
                        ref secondRoll);
                }
            }

            if (rollContext != RuleDefinitions.RollContext.AttackRoll)
            {
                return result;
            }

            Global.FirstAttackRoll = firstRoll;
            Global.SecondAttackRoll = secondRoll;

            return result;
        }

        private static int Roll3DicesAndKeepBest(
            string roller,
            RuleDefinitions.DieType diceType,
            out int firstRoll,
            out int secondRoll,
            float rollAlterationScore
        )
        {
            var karmic = rollAlterationScore != 0.0;

            int DoRoll()
            {
                return karmic
                    ? RuleDefinitions.RollKarmicDie(diceType, rollAlterationScore)
                    : 1 + DeterministicRandom.Range(0, RuleDefinitions.DiceMaxValue[(int)diceType]);
            }

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
        }

        // TODO: make this more generic
        [UsedImplicitly]
        public static void Prefix(RulesetActor __instance, RuleDefinitions.RollContext rollContext,
            ref bool enumerateFeatures, ref bool canRerollDice)
        {
            //PATCH: support for `RoguishRaven` Rogue subclass
            if (!__instance.HasSubFeatureOfType<RoguishRaven.RavenRerollAnyDamageDieMarker>() ||
                rollContext != RuleDefinitions.RollContext.AttackDamageValueRoll)
            {
                return;
            }

            enumerateFeatures = true;
            canRerollDice = true;
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
            var hero = actor as RulesetCharacterHero;

            if (hero == null && actor is RulesetCharacterMonster monster)
            {
                hero = monster.OriginalFormCharacter as RulesetCharacterHero;
            }

            if (hero == null)
            {
                return;
            }

            foreach (var attribute in actor.Attributes)
            {
                foreach (var modifier in attribute.Value.ActiveModifiers
                             .Where(x => x.Operation
                                 is AttributeModifierOperation.MultiplyByClassLevel
                                 or AttributeModifierOperation.Additive
                                 or AttributeModifierOperation.MultiplyByClassLevelBeforeAdditions))
                {
                    var level = attribute.Key switch
                    {
                        AttributeDefinitions.HealingPool =>
                            hero.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Paladin),
                        AttributeDefinitions.KiPoints =>
                            hero.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Monk),
                        AttributeDefinitions.SorceryPoints =>
                            hero.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Sorcerer),
                        _ => 0
                    };

                    if (level > 0)
                    {
                        modifier.Value = level;
                    }

                    //TODO: make this more generic. it supports Ranger Light Bearer subclass
                    if (modifier.Operation == AttributeModifierOperation.Additive &&
                        attribute.Key == AttributeDefinitions.HealingPool)
                    {
                        modifier.Value = hero.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Ranger) * 5;
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
            var enumerate = new Action<
                RulesetCharacter,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(EnumerateFeatureDefinitionSavingThrowAffinity).Method;

            //PATCH: make ISpellCastingAffinityProvider from dynamic item properties apply to repertoires
            return instructions.ReplaceEnumerateFeaturesToBrowse("ISavingThrowAffinityProvider",
                -1, "RulesetActor.ComputeSavingThrowModifier",
                new CodeInstruction(OpCodes.Call, enumerate));
        }

        private static void EnumerateFeatureDefinitionSavingThrowAffinity(
            RulesetCharacter __instance,
            List<FeatureDefinition> featuresToBrowse,
            Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin)
        {
            __instance.EnumerateFeaturesToBrowse<FeatureDefinitionSavingThrowAffinity>(featuresToBrowse,
                featuresOrigin);
            featuresToBrowse.RemoveAll(x =>
                !__instance.IsValid(x.GetAllSubFeaturesOfType<IsCharacterValidHandler>()));
        }
    }

    //PATCH: allow recurrent effect forms effect level to be modified
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.ExecuteRecurrentForms))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteRecurrentForms_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var myApplyEffectForm = typeof(ExecuteRecurrentForms_Patch).GetMethod("ApplyEffectForms");
            var applyEffectForm = typeof(IRulesetImplementationService).GetMethod("ApplyEffectForms");

            //PATCH: supports IModifyRecurrentMagicEffect interface
            return instructions.ReplaceCalls(
                applyEffectForm,
                "RulesetActor.ExecuteRecurrentForms",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, myApplyEffectForm));
        }

        [UsedImplicitly]
        public static int ApplyEffectForms(
            RulesetImplementationManager __instance,
            List<EffectForm> effectForms,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            List<string> effectiveDamageTypes,
            out bool damageAbsorbedByTemporaryHitPoints,
            out bool terminateEffectOnTarget,
            bool retargeting,
            bool proxyOnly,
            bool forceSelfConditionOnly,
            RuleDefinitions.EffectApplication effectApplication,
            List<EffectFormFilter> filters,
            RulesetActor rulesetActor,
            RulesetCondition rulesetCondition)
        {
            var newEffectForms = new List<EffectForm>();

            newEffectForms.AddRange(effectForms);

            foreach (var modifyRecurrentMagicEffect in rulesetCondition.ConditionDefinition
                         .GetAllSubFeaturesOfType<IModifyRecurrentMagicEffect>())
            {
                foreach (var effectForm in newEffectForms)
                {
                    modifyRecurrentMagicEffect.ModifyEffect(rulesetCondition, effectForm, rulesetActor);
                }
            }

            return __instance.ApplyEffectForms(
                newEffectForms, formsParams, effectiveDamageTypes,
                out damageAbsorbedByTemporaryHitPoints, out terminateEffectOnTarget,
                retargeting, proxyOnly, forceSelfConditionOnly, effectApplication, filters);
        }
    }
}
