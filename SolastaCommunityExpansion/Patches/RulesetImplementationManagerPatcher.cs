using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetImplementationManagerPatcher
{
    [HarmonyPatch(typeof(RulesetImplementationManager), "ApplyEffectForms")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ApplyEffectForms_Patch
    {
        public static void Postfix( 
            List<EffectForm> effectForms,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            List<string> effectiveDamageTypes,
            bool retargeting,
            bool proxyOnly,
            bool forceSelfConditionOnly,
            RuleDefinitions.EffectApplication effectApplication,
            List<EffectFormFilter> filters)
        {
            foreach (var customEffect in effectForms.OfType<CustomDefinitions.CustomEffectForm>())
            {
                customEffect.ApplyForm(formsParams, retargeting, proxyOnly, forceSelfConditionOnly, effectApplication,
                    filters);
            }
        }
    }
}
