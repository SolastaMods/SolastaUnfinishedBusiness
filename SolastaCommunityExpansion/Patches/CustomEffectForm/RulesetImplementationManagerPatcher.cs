

using System.Collections.Generic;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomEffectForm
{
    [HarmonyPatch(typeof(RulesetImplementationManager), "ApplyEffectForms")]
    internal static class RulesetImplementationManagerApplyCustomEffectForm
    {
        public static void Postfix(List<EffectForm> effectForms,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            bool retargeting = false,
            bool proxyOnly = false,
            bool forceSelfConditionOnly = false)
        {
           foreach(EffectForm effectForm in effectForms)
            {
                if (effectForm is CustomFeatureDefinitions.CustomEffectForm customEffect)
                {
                    customEffect.ApplyForm(effectForm, formsParams, retargeting, proxyOnly, forceSelfConditionOnly);
                }
            }
        }
    }
}
