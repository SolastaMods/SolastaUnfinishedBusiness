using System.Collections;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class CharacterActionMagicEffectPatcher
{
    [HarmonyPatch(typeof(CharacterActionMagicEffect), "ExecuteImpl")]
    internal static class ExecuteImpl_Patch
    {
        internal static void Prefix([NotNull] CharacterActionMagicEffect __instance)
        {
            var definition = __instance.GetBaseDefinition();

            //PATCH: skip spell animation if this is "attack after cast" spell
            if (definition.HasSubFeatureOfType<IPerformAttackAfterMagicEffectUse>())
            {
                __instance.ActionParams.SkipAnimationsAndVFX = true;
            }
        }


        internal static IEnumerator Postfix(
            [NotNull] IEnumerator __result,
            CharacterActionMagicEffect __instance)
        {
            //PATCH: support for `IPerformAttackAfterMagicEffectUse` and `IChainMagicEffect` feature
            // enables to perform automatic attacks after spell cast (like for sunlight blade cantrip) and chain effects

            while (!Global.IsSpellStrike && __result.MoveNext())
            {
                yield return __result.Current;
            }

            var definition = __instance.GetBaseDefinition();

            //TODO: add possibility to get attack via feature
            //TODO: add possibility to process multiple attack features
            var customFeature = definition.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();

            CharacterActionAttack attackAction = null;
            var getAttackAfterUse = customFeature?.PerformAttackAfterUse;
            var attackOutcome = RuleDefinitions.RollOutcome.Neutral;
            var attackParams = getAttackAfterUse?.Invoke(__instance);

            if (attackParams != null)
            {
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

            var saveRangeType = __instance.actionParams.activeEffect.EffectDescription.rangeType;

            if (Global.IsSpellStrike)
            {
                if (attackOutcome is not (RuleDefinitions.RollOutcome.Success
                    or RuleDefinitions.RollOutcome.CriticalSuccess))
                {
                    __instance.actionParams.activeEffect.EffectDescription.rangeType =
                        RuleDefinitions.RangeType.MeleeHit;
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
}
