using System.Collections;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells;

//enable to perform automatic attacks after spell cast (like for sunlight blade cantrip) and chain effects
[HarmonyPatch(typeof(CharacterActionMagicEffect), "ExecuteImpl")]
internal static class CharacterActionMagicEffect_ExecuteImpl
{
    internal static void Prefix(CharacterActionMagicEffect __instance)
    {
        var definition = __instance.GetBaseDefinition();
        //skip spell animation if this is "attack after cast" spell
        if (definition.HasSubFeatureOfType<IPerformAttackAfterMagicEffectUse>())
        {
            __instance.ActionParams.SkipAnimationsAndVFX = true;
        }
    }


    internal static IEnumerator Postfix(IEnumerator __result,
        CharacterActionMagicEffect __instance)
    {
        while (__result.MoveNext())
        {
            yield return __result.Current;
        }

        var definition = __instance.GetBaseDefinition();

        //TODO: add possibility to get attack via feature
        //TODO: add possibility to process multiple attack features
        var customFeature = definition.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
        var getAttackAfterUse = customFeature?.PerformAttackAfterUse;

        CharacterActionAttack attackAction = null;
        var attackOutcome = RuleDefinitions.RollOutcome.Neutral;

        if (getAttackAfterUse != null)
        {
            var attackParams = getAttackAfterUse(__instance);
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
