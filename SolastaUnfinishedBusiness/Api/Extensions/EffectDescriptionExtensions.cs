using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Api.Extensions;

internal static class EffectDescriptionExtensions
{
    public static DamageForm FindFirstNonNegatedDamageFormOfType(this EffectDescription effect, bool canForceHalfDamage,
        List<string> types)
    {
        if (effect == null)
        {
            return null;
        }

        foreach (EffectForm effectForm in effect.effectForms)
        {
            if (effectForm.FormType == EffectForm.EffectFormType.Damage &&
                effectForm.SavingThrowAffinity != RuleDefinitions.EffectSavingThrowType.Negates | canForceHalfDamage
                && (types == null || types.Empty() || types.Contains(effectForm.damageForm.damageType)))
                return effectForm.damageForm;
        }

        return null;
    }

    public static DamageForm FindFirstDamageFormOfType(this EffectDescription effect, List<string> types)
    {
        if (effect == null)
        {
            return null;
        }

        foreach (EffectForm effectForm in effect.effectForms)
        {
            if (effectForm.FormType == EffectForm.EffectFormType.Damage
                && (types == null || types.Empty() || types.Contains(effectForm.damageForm.damageType)))
                return effectForm.damageForm;
        }

        return null;
    }
}
