using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Models;
using TA;

namespace SolastaCommunityExpansion.Patches;

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
                    case (RuleDefinitions.TurnOccurenceType) ExtraTurnOccurenceType.OnMoveEnd:
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

        internal static IEnumerator Postfix(
            IEnumerator __result,
            GameLocationBattleManager __instance,
            GameLocationCharacter mover
        )
        {
            //PATCH: support for Polearm Expert AoO
            //processes saved movenent to trigger AoO when appropriate

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
            //PATCH: support for Sentinel feat - allows reaction attack on enemy atatcking ally 
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

            //PATCH: Support elven precision feat
            CheckElvenPrecisionContext(__result, attackParams.attacker.RulesetCharacter, attackParams.attackMode);

            //PATCH: add modifier or advantage/disadvantage for physical and spell attack
            ApplyCustomMoidifiers(attackParams, __result);
        }

        //TODO: move this somewhere else and maybe split?
        private static void ApplyCustomMoidifiers(BattleDefinitions.AttackEvaluationParams attackParams, bool __result)
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

        //TODO: move this somewhere else
        private static void CheckElvenPrecisionContext(bool result, RulesetCharacter character,
            RulesetAttackMode attackMode)
        {
            if (!result || character is not RulesetCharacterHero hero || attackMode == null)
            {
                return;
            }

            foreach (var sub in from feat in hero.TrainedFeats
                     where feat.Name.Contains(ZappaFeats.ElvenAccuracyTag)
                     select feat.GetFirstSubFeatureOfType<ElvenPrecisionContext>()
                     into context
                     where context != null
                     select context)
            {
                sub.Qualified =
                    attackMode.abilityScore is not AttributeDefinitions.Strength or AttributeDefinitions.Constitution;
            }
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