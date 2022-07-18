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
                __instance.actionParams.activeEffect.EffectDescription.rangeType = RuleDefinitions.RangeType.MeleeHit;
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
    internal static void Postfix(RulesetCharacter __instance, ref int __result, RulesetAttackMode attackMode,
        RulesetActor target)
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
        if (Global.IsSpellStrike)
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
        if (!Global.IsSpellStrike)
        {
            return;
        }

        __result = Global.SpellStrikeDieRoll;
        outcome = Global.SpellStrikeRollOutcome;
    }
}
