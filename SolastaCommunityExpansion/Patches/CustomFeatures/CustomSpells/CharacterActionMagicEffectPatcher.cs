using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Classes.Magus;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells;

//enable to perform automatic attacks after spell cast (like for sunlight blade cantrip) and chain effects
[HarmonyPatch(typeof(CharacterActionMagicEffect), "ExecuteImpl")]
internal static class CharacterActionMagicEffect_ExecuteImpl
{
    internal static void Prefix(CharacterActionMagicEffect __instance)
    {
        var definition = __instance.GetBaseDefinition();
        var spellStrike = Magus.CanSpellStrike(__instance);
        
        //skip spell animation if this is "attack after cast" spell
        if (definition.HasSubFeatureOfType<IPerformAttackAfterMagicEffectUse>() || spellStrike)
        {
            __instance.ActionParams.SkipAnimationsAndVFX = true;
        }

        if (spellStrike)
        {
            __instance.needToWaitCastAnimation = false;
        }

        Global.IsSpellStrike = spellStrike;
        Global.SpellStrikeRollOutcome = RuleDefinitions.RollOutcome.Neutral;
    }


    internal static IEnumerator Postfix(IEnumerator __result,
        CharacterActionMagicEffect __instance)
    {
        while (__result.MoveNext() && !Global.IsSpellStrike)
        {
            yield return __result.Current;
        }    
        
        var definition = __instance.GetBaseDefinition();

        //TODO: add possibility to get attack via feature
        //TODO: add possibility to process multiple attack features
        var customFeature = definition.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
        var effect = __instance.actionParams.RulesetEffect.EffectDescription;
        
        if (customFeature == null && Global.IsSpellStrike)
        { 
            customFeature = Magus.SpellStrike.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
        } 

        var getAttackAfterUse = customFeature?.PerformAttackAfterUse;

        CharacterActionAttack attackAction = null;
        var attackOutcome = RuleDefinitions.RollOutcome.Neutral;

        var attackParams = getAttackAfterUse?.Invoke(__instance);
        if (attackParams != null)
        {
            if (Global.IsSpellStrike)
            {
                 Magus.PrepareSpellStrike(__instance, attackParams);
            }
            
            void AttackImpactStartHandler(
                GameLocationCharacter attacker,
                GameLocationCharacter defender,
                RuleDefinitions.RollOutcome outcome,
                CharacterActionParams actionParams,
                RulesetAttackMode attackMode,
                ActionModifier attackModifier)
            {
                attackOutcome = outcome;
            }
                
            attackParams.ActingCharacter.AttackImpactStart += AttackImpactStartHandler;
            
            attackAction = new CharacterActionAttack(attackParams);
            var enums = attackAction.Execute();
            while (enums.MoveNext())
            {
                yield return enums.Current;
            }
                
            attackParams.ActingCharacter.AttackImpactStart -= AttackImpactStartHandler;
        }

        Magus.SpellStrikePower.effectDescription.effectParticleParameters = null;
        Magus.SpellStrikeAdditionalDamage.impactParticleReference = null;
        
        if (Global.IsSpellStrike)
        {
            if (attackOutcome is not (RuleDefinitions.RollOutcome.Success
                or RuleDefinitions.RollOutcome.CriticalSuccess))
            {
                Global.SpellStrikeRollOutcome = RuleDefinitions.RollOutcome.Neutral;
                yield break;
            }

            Global.SpellStrikeRollOutcome = attackOutcome;
        }
        
        while (__result.MoveNext())
        {
            yield return __result.Current;
        }    
        
        //chained effects would be useful for EOrb
        var chainAction = definition.GetFirstSubFeatureOfType<IChainMagicEffect>()
            ?.GetNextMagicEffect(__instance, attackAction, attackOutcome);

        if (chainAction == null)
        {
            yield break;
        }

        {
            var enums = chainAction.Execute();
            while (enums.MoveNext())
            {
                yield return enums.Current;
            }
        }
    }
}

[HarmonyPatch(typeof(RulesetCharacter), "RollAttackMode")]
internal static class RulesetCharacter_RollAttackMode
{
    internal static void Postfix(RulesetCharacter __instance,  ref int __result, RulesetAttackMode attackMode, RulesetActor target)
    {
        if (Global.IsSpellStrike)
        {
            Global.SpellStrikeDieRoll = __result;
        }
    }
}

[HarmonyPatch(typeof(RulesetCharacter), "RollMagicAttack")]
internal static class RulesetCharacter_RollMagicAttack
{
    internal static bool Prefix(ref int __result, out RuleDefinitions.RollOutcome outcome)
    {
        if (Global.IsSpellStrike && Global.SpellStrikeRollOutcome is RuleDefinitions.RollOutcome.Success or RuleDefinitions.RollOutcome.CriticalSuccess)
        {
            __result = Global.SpellStrikeDieRoll;
            outcome = Global.SpellStrikeRollOutcome;
            return false;
        }
        
        outcome = RuleDefinitions.RollOutcome.Failure;
        return true;
    }
    
    internal static void Postfix(ref int __result, ref RuleDefinitions.RollOutcome outcome)
    {
        if (Global.IsSpellStrike && Global.SpellStrikeRollOutcome is RuleDefinitions.RollOutcome.Success or RuleDefinitions.RollOutcome.CriticalSuccess)
        {
            __result = Global.SpellStrikeDieRoll;
            outcome = Global.SpellStrikeRollOutcome;
        }
    }
}

/*[HarmonyPatch(typeof(RulesetCharacter), "RollAttack")]
internal static class RulesetCharacter_RollAttack
{
    internal static bool Prefix(ref int __result, BaseDefinition attackMethod)
    {
        Main.Log($"Prefix RollAttack {attackMethod} {Global.IsSpellStrike} {Global.SpellStrikeRollOutcome}", true);
        if (Global.IsSpellStrike && Global.SpellStrikeRollOutcome is RuleDefinitions.RollOutcome.CriticalSuccess or RuleDefinitions.RollOutcome.CriticalSuccess
            && attackMethod is SpellDefinition)
        {
            __result = Global.SpellStrikeDieRoll;
            return false;
        }
        
        return true;
    }
}*/

/*//enable to perform automatic attacks after spell cast (like for sunlight blade cantrip) and chain effects
[HarmonyPatch(typeof(CharacterActionMagicEffect), "ExecuteMagicAttack")]
internal static class CharacterActionMagicEffect_ExecuteMagicAttack
{
    internal static bool Prefix()
    {
        // skip the original code
        return false;
    }
    
    internal static IEnumerator Postfix(IEnumerator __result, CharacterActionMagicEffect __instance, RulesetEffect activeEffect,
        GameLocationCharacter target,
        ActionModifier attackModifier,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool checkMagicalAttackDamage)
    {
        CharacterActionMagicEffect action = __instance;
        IGameLocationBattleService battleService = ServiceRepository.GetService<IGameLocationBattleService>();
        EffectDescription effectDescription = activeEffect.EffectDescription;
        action.Outcome = RuleDefinitions.RollOutcome.Success;
        bool needToRollDie = effectDescription.RangeType == RuleDefinitions.RangeType.MeleeHit || effectDescription.RangeType == RuleDefinitions.RangeType.RangeHit;
        if (needToRollDie)
        {
          RuleDefinitions.RollOutcome outcome = RuleDefinitions.RollOutcome.Failure;
          int successDelta = 0;
          int dieRoll = -1;
          if (Global.SpellStrikeRollOutcome != RuleDefinitions.RollOutcome.Success &&
              Global.SpellStrikeRollOutcome != RuleDefinitions.RollOutcome.CriticalSuccess)
          {
              dieRoll = action.ActingCharacter.RulesetCharacter.RollMagicAttack(activeEffect, target.RulesetActor, activeEffect.GetEffectSource(), attackModifier.AttacktoHitTrends, attackModifier.AttackAdvantageTrends, false, attackModifier.AttackRollModifier, out outcome, out successDelta, -1, true);
          }
          else
          {
              outcome = Global.SpellStrikeRollOutcome;
          }
          
          if (outcome == RuleDefinitions.RollOutcome.Success || outcome == RuleDefinitions.RollOutcome.CriticalSuccess)
          {
              // if dieRoll is not -1 means we didn't roll so go with the magic attack hit
              if (dieRoll != RuleDefinitions.DiceMaxValue[8] && dieRoll != -1) 
                  yield return battleService.HandleCharacterAttackHit(action.ActingCharacter, target, attackModifier, dieRoll, successDelta, effectDescription.RangeType == RuleDefinitions.RangeType.RangeHit);
              if (dieRoll != -1)
                action.ActingCharacter.RulesetCharacter.RollMagicAttack(activeEffect, target.RulesetActor, activeEffect.GetEffectSource(), attackModifier.AttacktoHitTrends, attackModifier.AttackAdvantageTrends, false, attackModifier.AttackRollModifier, out outcome, out successDelta, dieRoll, false);
              if ((outcome == RuleDefinitions.RollOutcome.Success || outcome == RuleDefinitions.RollOutcome.CriticalSuccess) && checkMagicalAttackDamage && action.HasOneDamageForm(actualEffectForms))
                  yield return battleService.HandleCharacterMagicalAttackDamage(action.ActingCharacter, target, attackModifier, activeEffect, actualEffectForms, firstTarget, outcome == RuleDefinitions.RollOutcome.CriticalSuccess);
          }
          else 
              action.ActingCharacter.RulesetCharacter.RollMagicAttack(activeEffect, target.RulesetActor, activeEffect.GetEffectSource(), attackModifier.AttacktoHitTrends, attackModifier.AttackAdvantageTrends, false, attackModifier.AttackRollModifier, out outcome, out successDelta, dieRoll, false);
          action.ActingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(RuleDefinitions.ConditionInterruption.Attacks);
          action.Outcome = outcome;
        }
        else if (checkMagicalAttackDamage && action.HasOneDamageForm(actualEffectForms)) 
            yield return battleService.HandleCharacterMagicalAttackDamage(action.ActingCharacter, target, attackModifier, activeEffect, actualEffectForms, firstTarget, false);
        if ((!needToRollDie || action.Outcome == RuleDefinitions.RollOutcome.Success || action.Outcome == RuleDefinitions.RollOutcome.CriticalSuccess || activeEffect.EffectDescription.HalfDamageOnAMiss) && (activeEffect.EffectDescription.RecurrentEffect == RuleDefinitions.RecurrentEffect.No || (activeEffect.EffectDescription.RecurrentEffect & RuleDefinitions.RecurrentEffect.OnActivation) != RuleDefinitions.RecurrentEffect.No) && (target.RulesetCharacter == null || !target.RulesetCharacter.IsDeadOrDyingOrUnconscious))
        { 
            RuleDefinitions.RollOutcome saveOutcome = RuleDefinitions.RollOutcome.Neutral;
            int saveOutcomeDelta = 0; 
            bool hasBorrowedLuck = target.RulesetActor.HasConditionOfTypeOrSubType("ConditionDomainMischiefBorrowedLuck");
            action.RolledSaveThrow = activeEffect.TryRollSavingThrow(action.ActingCharacter.RulesetCharacter, action.ActingCharacter.Side, target.RulesetActor, attackModifier, !needToRollDie, out saveOutcome, out saveOutcomeDelta);
            action.SaveOutcome = saveOutcome;
            action.SaveOutcomeDelta = saveOutcomeDelta;
            if ((action.SaveOutcome == RuleDefinitions.RollOutcome.Success || action.SaveOutcome == RuleDefinitions.RollOutcome.CriticalSuccess) && activeEffect.EffectDescription.GrantedConditionOnSave != null)
            {
                ConditionDefinition grantedConditionOnSave = activeEffect.EffectDescription.GrantedConditionOnSave;
                int durationParameter = RuleDefinitions.RollDie(grantedConditionOnSave.DurationParameterDie, RuleDefinitions.AdvantageType.None, out int _, out int _) * grantedConditionOnSave.DurationParameter;
                target.RulesetActor.AddConditionOfCategory("11Effect", RulesetCondition.CreateActiveCondition(target.RulesetActor.Guid, grantedConditionOnSave, grantedConditionOnSave.DurationType, durationParameter, RuleDefinitions.TurnOccurenceType.StartOfTurn, target.RulesetCharacter.Guid, string.Empty, 0, effectDefinitionName: string.Empty));
            }
            if (action.RolledSaveThrow && action.SaveOutcome == RuleDefinitions.RollOutcome.Failure)
                yield return battleService.HandleFailedSavingThrow(action, action.ActingCharacter, target, attackModifier, !needToRollDie, hasBorrowedLuck);
        }
        if (!action.RolledSaveThrow && activeEffect.EffectDescription.HasShoveRoll)
          action.successfulShove = CharacterActionShove.ResolveRolls(action.ActingCharacter, target, ActionDefinitions.Id.Shove);
    }
}*/
