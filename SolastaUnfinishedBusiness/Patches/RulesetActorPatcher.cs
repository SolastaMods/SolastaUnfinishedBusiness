using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
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
            RulesetAttribute attribute;

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
                    if (sourceCharacter.TryGetAttribute(source, out attribute))
                    {
                        sourceAmount = AttributeDefinitions.ComputeAbilityScoreModifier(attribute.CurrentValue);
                    }

                    break;
                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceCopyAttributeFromSummoner:
                    if (sourceCharacter.TryGetAttribute(source, out attribute))
                    {
                        __instance.Attributes.Add(source, attribute);
                    }

                    break;
                case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyAndAbilityBonus:
                    sourceAmount =
                        sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

                    if (sourceCharacter.TryGetAttribute(source, out attribute))
                    {
                        sourceAmount += AttributeDefinitions.ComputeAbilityScoreModifier(attribute.CurrentValue);
                    }

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
            var modulateSustainedDamage = typeof(IRulesetImplementationService).GetMethod("ModulateSustainedDamage");
            var myModulateSustainedDamage =
                typeof(ModulateSustainedDamage_Patch).GetMethod("MayModulateSustainedDamage");
            var myEnumerate = new Action<
                RulesetActor,
                List<FeatureDefinition>,
                Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
            >(MyEnumerate).Method;

            return instructions
                //PATCH: supports IIgnoreDamageAffinity    
                .ReplaceCall(modulateSustainedDamage,
                    2, "RulesetActor.ModulateSustainedDamage.1",
                    new CodeInstruction(OpCodes.Ldarg, 4), // source guid
                    new CodeInstruction(OpCodes.Call, myModulateSustainedDamage))
                //PATCH: add `IDamageAffinityProvider` from dynamic item properties
                //fixes game not applying damage reductions from dynamic item properties
                //used for Inventor's Resistant Armor infusions
                .ReplaceEnumerateFeaturesToBrowse("IDamageAffinityProvider",
                    -1, "RulesetActor.ModulateSustainedDamage.2",
                    new CodeInstruction(OpCodes.Call, myEnumerate));
        }

        [UsedImplicitly]
        public static float MayModulateSustainedDamage(
            IRulesetImplementationService service,
            IDamageAffinityProvider provider,
            RulesetActor actor,
            string damageType,
            float multiplier,
            List<string> sourceTags,
            bool wasFirstDamage,
            out int damageReduction,
            out string ancestryDamageType,
            ulong sourceGuid)
        {
            ServiceRepository.GetService<IRulesetEntityService>().TryGetEntityByGuid(sourceGuid, out var rulesetEntity);

            var caster = rulesetEntity as RulesetCharacter;
            var features = caster.GetSubFeaturesByType<IIgnoreDamageAffinity>();

            if (!features.Any(feature => feature.CanIgnoreDamageAffinity(provider, damageType)))
            {
                return service.ModulateSustainedDamage(provider, actor, damageType, multiplier, sourceTags,
                    wasFirstDamage,
                    out damageReduction, out ancestryDamageType);
            }

            damageReduction = 0;
            ancestryDamageType = string.Empty;

            return multiplier;
        }

        private static void MyEnumerate(
            RulesetActor actor,
            List<FeatureDefinition> featuresToBrowse,
            Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin)
        {
            actor.EnumerateFeaturesToBrowse<IDamageAffinityProvider>(actor.featuresToBrowse, featuresOrigin);

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
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var rollDieMethod = typeof(RuleDefinitions).GetMethod("RollDie", BindingFlags.Public | BindingFlags.Static);
            var myRollDieMethod = typeof(RollDie_Patch).GetMethod("RollDie");

            return instructions.ReplaceCalls(rollDieMethod, "RulesetActor.RollDie",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, myRollDieMethod));
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
                
                result = RuleDefinitions.RollDie(dieType, advantageType, out firstRoll, out secondRoll,
                    rollAlterationScore);
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

    //PATCH: supports DieRollModifierDamageTypeDependent
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RollDamage))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RollDamage))]
    internal class RulesetActor_RollDamage
    {
        internal static DamageForm CurrentDamageForm;

        [UsedImplicitly]
        public static void Prefix(DamageForm damageForm)
        {
            CurrentDamageForm = damageForm;
        }

        [UsedImplicitly]
        public static void Postfix()
        {
            CurrentDamageForm = null;
        }
    }

    //PATCH: supports DieRollModifierDamageTypeDependent
    [HarmonyPatch(typeof(RulesetActor), nameof(RulesetActor.RerollDieAsNeeded))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RulesetActor_RerollDieAsNeeded
    {
        [UsedImplicitly]
        public static bool Prefix(
            FeatureDefinitionDieRollModifier dieRollModifier,
            int rollScore,
            ref int __result)
        {
            if (dieRollModifier is not FeatureDefinitionDieRollModifierDamageTypeDependent
                    featureDefinitionDieRollModifierDamageTypeDependent
                || RulesetActor_RollDamage.CurrentDamageForm == null
                || featureDefinitionDieRollModifierDamageTypeDependent.damageTypes.Contains(RulesetActor_RollDamage
                    .CurrentDamageForm.damageType))
            {
                return true;
            }

            __result = rollScore;
            return false;
        }
    }
}
