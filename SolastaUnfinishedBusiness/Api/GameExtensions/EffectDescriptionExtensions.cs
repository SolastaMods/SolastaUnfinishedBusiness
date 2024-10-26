using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class EffectDescriptionExtensions
{
    public static DamageForm FindFirstNonNegatedDamageFormOfType(this EffectDescription effect, bool canForceHalfDamage,
        List<string> types)
    {
        return effect?.effectForms
            .Where(x => x.FormType == EffectForm.EffectFormType.Damage &&
                        (x.SavingThrowAffinity != EffectSavingThrowType.Negates) | canForceHalfDamage &&
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

    public static bool HasNotNegatedDamageForm(this EffectDescription effect, SavingThrowData savingThrowData,
        bool canForceHalfDamage, bool hasSpecialHalfDamage)
    {
        return effect.effectForms
            .Any(x => (x.FormType == EffectForm.EffectFormType.Damage
                       && savingThrowData.SaveOutcome is not RollOutcome.CriticalSuccess and not RollOutcome.Success) ||
                      x.SavingThrowAffinity switch
                      {
                          EffectSavingThrowType.Negates => canForceHalfDamage,
                          EffectSavingThrowType.HalfDamage => canForceHalfDamage || !hasSpecialHalfDamage,
                          _ => true
                      }
            );
    }
}
