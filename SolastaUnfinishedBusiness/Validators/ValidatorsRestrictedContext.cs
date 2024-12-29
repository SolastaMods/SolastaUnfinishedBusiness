using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Validators;

public static class ValidatorsRestrictedContext
{
    public static readonly IValidateContextInsteadOfRestrictedProperty IsMeleeOrUnarmedAttack =
        new ValidateContextInsteadOfRestrictedProperty((_, _, _, _, _, mode, effect) =>
            (OperationType.Set, mode is { Ranged: false, Thrown: false } ||
                                effect is { EffectDescription.RangeType: RuleDefinitions.RangeType.MeleeHit }));

    public static readonly IValidateContextInsteadOfRestrictedProperty IsWeaponOrUnarmedAttack =
        new ValidateContextInsteadOfRestrictedProperty((_, _, _, _, _, mode, _) =>
            (OperationType.Set, mode != null));

    public static readonly IValidateContextInsteadOfRestrictedProperty IsMeleeWeaponAttack =
        new ValidateContextInsteadOfRestrictedProperty((_, _, _, _, _, mode, _) =>
            (OperationType.Set, ValidatorsWeapon.IsMelee(mode)));

    public static readonly IValidateContextInsteadOfRestrictedProperty IsOathOfThunder =
        new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) =>
            (OperationType.Set, OathOfThunder.IsOathOfThunderWeapon(mode, null, character)));

    public static readonly IValidateContextInsteadOfRestrictedProperty IsOathOfDemonHunter =
        new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) =>
            (OperationType.Set, OathOfDemonHunter.IsOathOfDemonHunterWeapon(mode, null, character)));
}
