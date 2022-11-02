using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

// This adds the ability to do fully custom EffectForms but if possible you should use the standard ones.
// Damage and healing done through this CustomEffectForm will not trigger the proper events.
internal abstract class CustomEffectForm : EffectForm
{
    protected CustomEffectForm()
    {
        FormType = (EffectFormType)ExtraEffectFormType.Custom;
    }

    internal abstract void ApplyForm(
        RulesetImplementationDefinitions.ApplyFormsParams formsParams,
        List<string> effectiveDamageTypes,
        bool retargeting,
        bool proxyOnly,
        bool forceSelfConditionOnly,
        RuleDefinitions.EffectApplication effectApplication = RuleDefinitions.EffectApplication.All,
        List<EffectFormFilter> filters = null);

    // internal abstract void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap);
}
