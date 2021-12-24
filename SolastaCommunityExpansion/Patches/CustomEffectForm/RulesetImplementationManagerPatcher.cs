

using System.Collections.Generic;
using System.Linq;
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
           foreach(CustomFeatureDefinitions.CustomEffectForm customEffect in effectForms.OfType<CustomFeatureDefinitions.CustomEffectForm>())
            {
                customEffect.ApplyForm(formsParams, retargeting, proxyOnly, forceSelfConditionOnly);
            }
        }
    }
}
