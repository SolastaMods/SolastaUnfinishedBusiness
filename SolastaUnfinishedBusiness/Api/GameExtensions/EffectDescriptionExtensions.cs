using System.Collections.Generic;
using System.Linq;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class EffectDescriptionExtensions
{
    public static DamageForm FindFirstNonNegatedDamageFormOfType(this EffectDescription effect, bool canForceHalfDamage,
        List<string> types)
    {
        return effect?.effectForms
            .Where(x => x.FormType == EffectForm.EffectFormType.Damage &&
                        (x.SavingThrowAffinity != RuleDefinitions.EffectSavingThrowType.Negates) | canForceHalfDamage &&
                        (types == null || types.Count == 0 || types.Contains(x.damageForm.damageType)))
            .Select(effectForm => effectForm.damageForm)
            .FirstOrDefault();
    }

    public static DamageForm FindFirstDamageFormOfType(this EffectDescription effect, List<string> types)
    {
        return effect?.effectForms
            .Where(x => 
                x.FormType == EffectForm.EffectFormType.Damage &&
                        (types == null || types.Count == 0 || types.Contains(x.damageForm.damageType)))
            .Select(effectForm => effectForm.damageForm)
            .FirstOrDefault();
    }
}
