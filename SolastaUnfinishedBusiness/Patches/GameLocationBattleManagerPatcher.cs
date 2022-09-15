using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Models;
using TA;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameLocationBattleManagerPatcher
{
    [HarmonyPatch(typeof(GameLocationBattleManager), "CanPerformReadiedActionOnCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CanPerformReadiedActionOnCharacter_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            //PATCH: Makes only preferred cantrip valid if it is selected and forced
            CustomReactionsContext.ForcePreferredCantripUsage(codes);

            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "IsValidAttackForReadiedAction")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsValidAttackForReadiedAction_Patch
    {
        internal static void Postfix(
            GameLocationBattleManager __instance,
            ref bool __result,
            BattleDefinitions.AttackEvaluationParams attackParams,
            bool forbidDisadvantage)
        {
            //PATCH: Checks if attack cantrip is valid to be cast as readied action on a target
            // Used to properly check if melee cantrip can hit target when used for readied action

            if (!DatabaseHelper.TryGetDefinition<SpellDefinition>(attackParams.effectName, null, out var cantrip))
            {
                return;
            }

            var canAttack = cantrip.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>()?.CanAttack;

            if (canAttack != null)
            {
                __result = canAttack(attackParams.attacker, attackParams.defender);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMoveStart")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleCharacterMoveStart_Patch
    {
        internal static void Prefix(GameLocationBattleManager __instance,
            GameLocationCharacter mover,
            int3 destination
        )
        {
            //PATCH: support for Polearm Expert AoO
            //Stores character movements to be processed later
            AttacksOfOpportunity.ProcessOnCharacterMoveStart(mover, destination);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMoveEnd")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleCharacterMoveEnd_Patch
    {
        //DEMOTE: Magus code
#if false
        internal static void Prefix(GameLocationCharacter mover)
        {
            //PATCH: support for conditions that trigger on movement end
            //Mostly for Magus's `Rupture Strike`
            //TODO: move this code to separate file
            
            if (mover.RulesetCharacter.isDeadOrDyingOrUnconscious)
            {
                return;
            }

            var matchingOccurenceConditions = new List<RulesetCondition>();
            foreach (var item2 in mover.RulesetCharacter.ConditionsByCategory
                         .SelectMany(item => item.Value))
            {
                switch (item2.endOccurence)
                {
                    case (RuleDefinitions.TurnOccurenceType)ExtraTurnOccurenceType.OnMoveEnd:
                        matchingOccurenceConditions.Add(item2);
                        break;
                }
            }

            var effectManager =
                ServiceRepository.GetService<IWorldLocationSpecialEffectsService>() as
                    WorldLocationSpecialEffectsManager;

            foreach (var condition in matchingOccurenceConditions)
            {
                Main.Log($"source character GUID {condition.sourceGuid}");

                if (effectManager != null)
                {
                    effectManager.ConditionAdded(mover.RulesetCharacter, condition, true);
                    mover.RulesetActor.ExecuteRecurrentForms(condition);
                    effectManager.ConditionRemoved(mover.RulesetCharacter, condition);
                }

                if (condition.HasFinished && !condition.IsDurationDefinedByEffect())
                {
                    mover.RulesetActor.RemoveCondition(condition);
                    mover.RulesetActor.ProcessConditionDurationEnded(condition);
                }
                else if (condition.CanSaveToCancel && condition.HasSaveOverride)
                {
                    mover.RulesetActor.SaveToCancelCondition(condition);
                }
                else
                {
                    mover.RulesetActor.ConditionOccurenceReached?.Invoke(mover.RulesetActor, condition);
                }
            }
        }
#endif

        internal static IEnumerator Postfix(
            IEnumerator __result,
            GameLocationBattleManager __instance,
            GameLocationCharacter mover
        )
        {
            //PATCH: support for Polearm Expert AoO
            //processes saved movement to trigger AoO when appropriate

            while (__result.MoveNext())
            {
                yield return __result.Current;
            }

            var extraEvents = AttacksOfOpportunity.ProcessOnCharacterMoveEnd(__instance, mover);

            while (extraEvents.MoveNext())
            {
                yield return extraEvents.Current;
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "PrepareBattleEnd")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class PrepareBattleEnd_Patch
    {
        internal static void Prefix(GameLocationBattleManager __instance)
        {
            //PATCH: support for Polearm Expert AoO
            //clears movement cache on battle end

            AttacksOfOpportunity.CleanMovingCache();
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackFinished")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleCharacterAttackFinished
    {
        internal static IEnumerator Postfix(
            IEnumerator __result,
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode
        )
        {
            //PATCH: support for Sentinel feat - allows reaction attack on enemy attacking ally 
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }

            var extraEvents =
                AttacksOfOpportunity.ProcessOnCharacterAttackFinished(__instance, attacker, defender,
                    attackerAttackMode);

            while (extraEvents.MoveNext())
            {
                yield return extraEvents.Current;
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "CanAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CanAttack_Patch
    {
        internal static void Postfix(
            GameLocationBattleManager __instance,
            BattleDefinitions.AttackEvaluationParams attackParams,
            bool __result
        )
        {
            //PATCH: support for features removing ranged attack disadvantage
            RangedAttackInMeleeDisadvantageRemover.CheckToRemoveRangedDisadvantage(attackParams);

            //PATCH: add modifier or advantage/disadvantage for physical and spell attack
            ApplyCustomModifiers(attackParams, __result);

            //PATCH: Support elven precision feat
            // should come last as adv / dis make diff here
            ZappaFeats.CheckElvenPrecisionContext(__result, attackParams.attacker.RulesetCharacter,
                attackParams.attackMode);
        }

        //TODO: move this somewhere else and maybe split?
        private static void ApplyCustomModifiers(BattleDefinitions.AttackEvaluationParams attackParams, bool __result)
        {
            if (!__result)
            {
                return;
            }

            var attacker = attackParams.attacker.RulesetCharacter;
            var defender = attackParams.defender.RulesetCharacter;
            if (attacker == null)
            {
                return;
            }

            switch (attackParams.attackProximity)
            {
                case BattleDefinitions.AttackProximity.PhysicalRange or BattleDefinitions.AttackProximity.PhysicalReach:
                    // handle physical attack roll
                    var attackModifiers = attacker.GetSubFeaturesByType<IOnComputeAttackModifier>();
                    foreach (var feature in attackModifiers)
                    {
                        feature.ComputeAttackModifier(attacker, defender, attackParams.attackMode,
                            ref attackParams.attackModifier);
                    }

                    break;

                case BattleDefinitions.AttackProximity.MagicRange or BattleDefinitions.AttackProximity.MagicReach:
                    // handle magic attack roll
                    var magicAttackModifiers = attacker.GetSubFeaturesByType<IIncreaseSpellAttackRoll>();
                    foreach (var feature in magicAttackModifiers)
                    {
                        var modifier = feature.GetSpellAttackRollModifier(attacker);
                        attackParams.attackModifier.attackRollModifier += modifier;
                        attackParams.attackModifier.attackToHitTrends.Add(new RuleDefinitions.TrendInfo(modifier,
                            feature.sourceType,
                            feature.sourceName, null));
                    }

                    break;
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleAdditionalDamageOnCharacterAttackHitConfirmed")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleAdditionalDamageOnCharacterAttackHitConfirmed_Patch
    {
        internal static bool Prefix(
            GameLocationBattleManager __instance,
            out IEnumerator __result,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            RuleDefinitions.AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            //PATCH: Completely replace this method to suppoort several features. Modified method based on TA provided sources.
            __result = GameLocationBattleManagerTweaks.HandleAdditionalDamageOnCharacterAttackHitConfirmed(__instance,
                attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms,
                rulesetEffect, criticalHit, firstTarget);

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "ComputeAndNotifyAdditionalDamage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ComputeAndNotifyAdditionalDamage_Patch
    {
        internal static bool Prefix(
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            IAdditionalDamageProvider provider,
            List<EffectForm> actualEffectForms,
            CharacterActionParams reactionParams,
            RulesetAttackMode attackMode,
            bool criticalHit)
        {
            //PATCH: Completely replace this method to support several features. Modified method based on TA provided sources.
            GameLocationBattleManagerTweaks.ComputeAndNotifyAdditionalDamage(__instance, attacker, defender, provider,
                actualEffectForms, reactionParams, attackMode, criticalHit);

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleTargetReducedToZeroHP")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleTargetReducedToZeroHP_Patch
    {
        internal static IEnumerator Postfix(
            IEnumerator __result,
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode rulesetAttackMode,
            RulesetEffect activeEffect
        )
        {
            //PATCH: Support for `ITargetReducedToZeroHP` feature
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }

            var features = attacker.RulesetActor.GetSubFeaturesByType<ITargetReducedToZeroHP>();

            foreach (var feature in features)
            {
                var extraEvents =
                    feature.HandleCharacterReducedToZeroHP(attacker, downedCreature, rulesetAttackMode, activeEffect);

                while (extraEvents.MoveNext())
                {
                    yield return extraEvents.Current;
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMagicalAttackHitConfirmed")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleCharacterMagicalAttackHitConfirmed_Patch
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            Main.Logger.Log("HandleCharacterMagicalAttackDamage");

            //PATCH: set critical strike global variable
            Global.CriticalHit = criticalHit;

            //PATCH: support for `IOnMagicalAttackDamageEffect`
            var features = attacker.RulesetActor.GetSubFeaturesByType<IOnMagicalAttackDamageEffect>();

            //call all before handlers
            foreach (var feature in features)
            {
                feature.BeforeOnMagicalAttackDamage(attacker, defender, magicModifier, rulesetEffect,
                    actualEffectForms, firstTarget, criticalHit);
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            //call all after handlers
            foreach (var feature in features)
            {
                feature.AfterOnMagicalAttackDamage(attacker, defender, magicModifier, rulesetEffect,
                    actualEffectForms, firstTarget, criticalHit);
            }

            Global.CriticalHit = false;
        }
    }
}
