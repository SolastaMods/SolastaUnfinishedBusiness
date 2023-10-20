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
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using TA;
using UnityEngine;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static ConsoleStyleDuplet;

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
            ref RulesetCondition newCondition)
        {
            //PATCH: allow conditions to force specific category
            if (newCondition.conditionDefinition == null)
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
            SrdAndHouseRulesContext.AddLightSourceIfNeeded(__instance, newCondition);

            var definition = newCondition.ConditionDefinition;

            //PATCH: notifies custom condition features that condition is applied
            if (__instance is not RulesetCharacter rulesetCharacter)
            {
                return;
            }

            definition.GetAllSubFeaturesOfType<IOnConditionAddedOrRemoved>()
                .Do(c => c.OnConditionAdded(rulesetCharacter, newCondition));

            definition.Features
                .SelectMany(f => f.GetAllSubFeaturesOfType<IOnConditionAddedOrRemoved>())
                .Do(c => c.OnConditionAdded(rulesetCharacter, newCondition));
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
            //PATCH: support for FeatureDefinitionReduceDamage
            var reduction = FeatureDefinitionReduceDamage.DamageReduction(formsParams, rolledDamage, damageType);
            rolledDamage -= reduction;
            rollInfo.modifier -= reduction;
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
            //PATCH: support for `IRemoveConditionOnSourceTurnStart` - removes appropriately marked conditions
            ConditionRemovedOnSourceTurnStartPatch.RemoveConditionIfNeeded(__instance, occurenceType);
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

            if (trackingCondition != null)
            {
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
            }

            if (rulesetCondition.ConditionDefinition.AmountOrigin == ConditionDefinition.OriginOfAmount.AddDice)
            {
                formsParams.addDice = rulesetCondition.Amount;
            }

            formsParams.formAbilityBonus = rulesetCondition.SourceAbilityBonus;
            service.ApplyEffectForms(
                rulesetCondition.ConditionDefinition.RecurrentEffectForms, formsParams, null, out _, out _);

            var effectFormsApplied = service.ConditionRecurrentEffectFormsApplied;

            effectFormsApplied?.Invoke(__instance, rulesetCondition);

            return false;
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

            if (!character.conditionsByCategory.ContainsKey(category))
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

            if (!character.conditionsByCategory.ContainsKey(category))
            {
                return;
            }

            foreach (var rulesetCondition in character.conditionsByCategory[category])
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

            if (!character.conditionsByCategory.ContainsKey(category))
            {
                return;
            }

            foreach (var rulesetCondition in character.conditionsByCategory[category]
                         .Where(rulesetCondition => !sources.Contains(rulesetCondition.SourceGuid)))
            {
                ActionSwitching.AccountRemovedCondition(character, rulesetCondition);
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

            if (!character.conditionsByCategory.ContainsKey(category))
            {
                return;
            }

            foreach (var rulesetCondition in character.conditionsByCategory[category]
                         .Where(rulesetCondition => rulesetCondition.ConditionDefinition.Name == type ||
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
            Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin)
        {
            __instance.EnumerateFeaturesToBrowse<IDieRollModificationProvider>(featuresToBrowse, featuresOrigin);

            var effectForms =
                RulesetCharacterPatcher.RollMagicAttack_Patch.CurrentMagicEffect?.EffectDescription.EffectForms;

            if (effectForms == null)
            {
                return;
            }

            var damageTypes = effectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Select(x => x.DamageForm.DamageType)
                .ToList();

            var proxies = effectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Summon &&
                            x.SummonForm.SummonType == SummonForm.Type.EffectProxy)
                .Select(x =>
                    DatabaseHelper.GetDefinition<EffectProxyDefinition>(x.SummonForm.EffectProxyDefinitionName))
                .ToList();

            var damageTypesFromProxyAttacks = proxies
                .Where(x => x.canAttack && x.attackMethod == ProxyAttackMethod.CasterSpellAbility)
                .Select(x => x.DamageType).ToList();

            var damageTypesFromProxyAttackPowers = proxies
                .Where(x => x.attackPower != null)
                .Select(x => x.attackPower)
                .SelectMany(x => x.EffectDescription.EffectForms)
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Select(x => x.DamageForm.DamageType).ToList();

            damageTypes.AddRange(damageTypesFromProxyAttacks);
            damageTypes.AddRange(damageTypesFromProxyAttackPowers);

            featuresToBrowse.RemoveAll(x =>
                x is FeatureDefinitionDieRollModifierDamageTypeDependent y &&
                !y.damageTypes.Intersect(damageTypes).Any());
        }

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
                advantageType == AdvantageType.Advantage && ElvenPrecisionLogic.Active)
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
                        rollContext,
                        actor as RulesetCharacter,
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

            int DoRoll()
            {
                return karmic
                    ? RollKarmicDie(diceType, rollAlterationScore)
                    : 1 + DeterministicRandom.Range(0, DiceMaxValue[(int)diceType]);
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
        public static void Prefix(RulesetActor __instance,
            DieType dieType,
            RollContext rollContext,
            ref bool enumerateFeatures,
            ref bool canRerollDice)
        {
            if (dieType == DieType.D1)
            {
                canRerollDice = false;
                return;
            }

            //PATCH: support for `RoguishRaven` Rogue subclass
            if (!__instance.HasSubFeatureOfType<RoguishRaven.RavenRerollAnyDamageDieMarker>() ||
                rollContext != RollContext.AttackDamageValueRoll)
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

                    //TODO: make this more generic. it supports Ancient Forest and Light Bearer subclasses
                    //this will also not work if both subclasses are present...
                    if (modifier.Operation != AttributeModifierOperation.Additive ||
                        attribute.Key != AttributeDefinitions.HealingPool)
                    {
                        continue;
                    }

                    var levels =
                        hero.GetSubclassLevel(DatabaseHelper.CharacterClassDefinitions.Druid,
                            CircleOfTheAncientForest.Name) +
                        hero.GetSubclassLevel(DatabaseHelper.CharacterClassDefinitions.Ranger,
                            RangerLightBearer.Name);

                    modifier.Value = levels * 5;
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
            __instance.EnumerateFeaturesToBrowse<FeatureDefinitionSavingThrowAffinity>(featuresToBrowse,
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

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var rulesetCondition in __instance.AllConditions)
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

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var rulesetCondition in __instance.AllConditions)
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
