using System.Collections.Generic;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    /**
     * This adds the ability to do fully custom EffectForms. If possible you should use the standard EffectForms.
     * Damage and healing done through this CustomEffectForm will not trigger the proper events.
     */
    public abstract class CustomEffectForm : EffectForm
    {
        protected CustomEffectForm()
        {
            this.FormType = (EffectFormType)ExtraEffectFormType.Custom;
        }

        public abstract void ApplyForm(
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            bool retargeting,
            bool proxyOnly,
            bool forceSelfConditionOnly,
            RuleDefinitions.EffectApplication effectApplication = RuleDefinitions.EffectApplication.All,
            List<EffectFormFilter> filters = null);

        public abstract void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap);
    }
}
