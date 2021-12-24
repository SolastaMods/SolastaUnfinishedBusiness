namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    /**
     * This adds the ability to do fully custom EffectForms. If possible you should use the standard EffectForms.
     * Damage and healing done through this CustomEffectForm will not trigger the proper events.
     */
    class CustomEffectForm : EffectForm
    {
        public delegate void ApplyDelegate(CustomEffectForm effectForm,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            bool retargeting,
            bool proxyOnly,
            bool forceSelfConditionOnly);
        private ApplyDelegate Apply;

        internal void SetApplyDelegate(ApplyDelegate del)
        {
            Apply = del;
        }

        public void ApplyForm(
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            bool retargeting,
            bool proxyOnly,
            bool forceSelfConditionOnly)
        {
            if (Apply != null)
            {
                Apply(this, formsParams, retargeting, proxyOnly, forceSelfConditionOnly);
            }
        }
    }
}
