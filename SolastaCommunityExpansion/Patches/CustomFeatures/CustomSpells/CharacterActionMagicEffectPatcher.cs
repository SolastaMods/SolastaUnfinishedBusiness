using System.Collections;
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
        var spellStrike = Magus.CanSpellStrike(__instance.actionParams);
        
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
            if (attackOutcome is (RuleDefinitions.RollOutcome.Success
                or RuleDefinitions.RollOutcome.CriticalSuccess))
            {
                __instance.SpendMagicEffectUses();
            }
            
            yield break;
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
