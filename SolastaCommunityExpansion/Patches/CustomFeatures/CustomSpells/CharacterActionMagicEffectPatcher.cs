using System.Collections;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells
{
    internal static class CharacterActionMagicEffectPatcher
    {
        //enable to perform automatic attacks after spell cast (like for sunlight blade cantrip) and chain effects
        [HarmonyPatch(typeof(CharacterActionMagicEffect), "ExecuteImpl")]
        internal static class CharacterActionMagicEffect_ExecuteImpl
        {
            internal static void Prefix(CharacterActionMagicEffect __instance)
            {
                BaseDefinition definition = __instance.InvokeMethod("GetBaseDefinition") as BaseDefinition;
                //skip spell animation if this is "attack after cast" spell
                if (definition.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>() != null)
                {
                    __instance.ActionParams.SetField("skipAnimationsAndVFX", true);
                }
            }


            internal static IEnumerator Postfix(IEnumerator __result,
                CharacterActionMagicEffect __instance)
            {
                while (__result.MoveNext())
                {
                    yield return __result.Current;
                }

                BaseDefinition definition = __instance.InvokeMethod("GetBaseDefinition") as BaseDefinition;

                //TODO: add possibility to get attack via feature
                //TODO: add possibility to process multiple attack features
                var customFeature = definition.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
                var getAttackAfterUse = customFeature?.PerformAttackAfterUse;

                CharacterActionAttack attackAction = null; //can be useful in chained spell effects

                if (getAttackAfterUse != null)
                {
                    var attackParams = getAttackAfterUse(__instance);
                    if (attackParams != null)
                    {
                        attackAction = new CharacterActionAttack(attackParams);
                        var enums = attackAction.Execute();
                        while (enums.MoveNext())
                        {
                            yield return enums.Current;
                        }
                    }
                }

                //chained effects would be useful for EOrb
                var chainAction = definition.GetFirstSubFeatureOfType<IChainMagicEffect>()
                    ?.GetNextMagicEffect(__instance, attackAction);
                
                if (chainAction != null)
                {
                    var enums = chainAction.Execute();
                    while (enums.MoveNext())
                    {
                        yield return enums.Current;
                    }
                }
            }
        }
    }
}
