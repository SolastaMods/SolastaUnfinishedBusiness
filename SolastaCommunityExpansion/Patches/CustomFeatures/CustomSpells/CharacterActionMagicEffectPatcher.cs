using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Classes.Magus;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells;

//enable to perform automatic attacks after spell cast (like for sunlight blade cantrip) and chain effects
[HarmonyPatch(typeof(CharacterActionMagicEffect), "ExecuteImpl")]
internal static class CharacterActionMagicEffect_ExecuteImpl
{
    internal static void Prefix([NotNull] CharacterActionMagicEffect __instance)
    {
        var definition = __instance.GetBaseDefinition();
        var spellStrike = Magus.CanSpellStrike(__instance);

        // skip spell animation if this is "attack after cast" spell
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


    internal static IEnumerator Postfix(
        [NotNull] IEnumerator __result,
        CharacterActionMagicEffect __instance)
    {
        while (!Global.IsSpellStrike && __result.MoveNext())
        {
            yield return __result.Current;
        }

        var definition = __instance.GetBaseDefinition();

        //TODO: add possibility to get attack via feature
        //TODO: add possibility to process multiple attack features
        var customFeature = definition.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();

        if (customFeature == null && Global.IsSpellStrike)
        {
            customFeature = Magus.SpellStrike.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
        }

        CharacterActionAttack attackAction = null;
        var getAttackAfterUse = customFeature?.PerformAttackAfterUse;
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

        var saveRangeType = __instance.actionParams.activeEffect.EffectDescription.rangeType;

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

        if (chainAction != null)
        {
            var enums = chainAction.Execute();

            while (enums.MoveNext())
            {
                yield return enums.Current;
            }
        }

        __instance.actionParams.activeEffect.EffectDescription.rangeType = saveRangeType;
    }
}

[HarmonyPatch(typeof(RulesetCharacter), "RollAttackMode")]
internal static class RulesetCharacter_RollAttackMode
{
    internal static void Prefix(
        RulesetCharacter __instance,
        RulesetAttackMode attackMode,
        bool ignoreAdvantage,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        bool testMode)
    {
        Global.ElvenAccuracyHero = null;

        if (ignoreAdvantage
            || !testMode
            || attackMode.abilityScore is AttributeDefinitions.Strength or AttributeDefinitions.Constitution)
        {
            return;
        }

        var advantageType = RuleDefinitions.ComputeAdvantage(advantageTrends);

        if (advantageType != RuleDefinitions.AdvantageType.Advantage)
        {
            return;
        }

        var hero = __instance as RulesetCharacterHero ?? __instance.OriginalFormCharacter as RulesetCharacterHero;

        if (hero != null && hero.TrainedFeats.Any(x => x.Name.Contains(ZappaFeats.ElvenAccuracyTag)))
        {
            Global.ElvenAccuracyHero = hero;
        }
    }

    internal static void Postfix(ref int __result)
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
    internal static bool Prefix(
        RulesetCharacter __instance,
        ref int __result,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        ref RuleDefinitions.RollOutcome outcome,
        bool testMode)
    {
        //
        // SUPPORTS ELVEN ACCURACY FEAT
        //
        Global.ElvenAccuracyHero = null;

        if (testMode)
        {
            var advantageType = RuleDefinitions.ComputeAdvantage(advantageTrends);

            if (advantageType == RuleDefinitions.AdvantageType.Advantage)
            {
                var hero = __instance as RulesetCharacterHero ??
                           __instance.OriginalFormCharacter as RulesetCharacterHero;

                if (hero != null && hero.TrainedFeats.Any(x => x.Name.Contains(ZappaFeats.ElvenAccuracyTag)))
                {
                    Global.ElvenAccuracyHero = hero;
                }
            }
        }

        if (!Global.IsSpellStrike)
        {
            return true;
        }

        __result = Global.SpellStrikeDieRoll;
        outcome = Global.SpellStrikeRollOutcome;

        return false;
    }
}
