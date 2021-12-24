

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    class CustomEffectForm : EffectForm
    {
        public delegate bool ApplyDelegate(EffectForm effectForm,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            bool retargeting,
            bool proxyOnly,
            bool forceSelfConditionOnly);
        private ApplyDelegate Apply;

        internal void SetApplyDelegate(ApplyDelegate del)
        {
            Apply = del;
        }

        public void ApplyForm(EffectForm effectForm,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            bool retargeting,
            bool proxyOnly,
            bool forceSelfConditionOnly)
        {
            if (Apply != null)
            {
                Apply(effectForm, formsParams, retargeting, proxyOnly, forceSelfConditionOnly);
            }
        }
    }
}
