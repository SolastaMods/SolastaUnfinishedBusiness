using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomEffectForm
{
    [HarmonyPatch(typeof(RulesetImplementationManager), "ApplyEffectForms")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetImplementationManager_ApplyEffectForms
    {
        public static void Postfix(List<EffectForm> effectForms,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            bool retargeting = false,
            bool proxyOnly = false,
            bool forceSelfConditionOnly = false)
        {
            foreach (CustomDefinitions.CustomEffectForm customEffect in effectForms.OfType<CustomDefinitions.CustomEffectForm>())
            {
                customEffect.ApplyForm(formsParams, retargeting, proxyOnly, forceSelfConditionOnly);
            }
        }
    }
}
