using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Subclasses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.CustomValidators;

public static class ValidatorsRestrictedContext
{
    public static readonly IRestrictedContextValidator WeaponAttack =
        new RestrictedContextValidator((_, _, _, _, _, mode, _) => (OperationType.Set, mode != null));

    public static readonly IRestrictedContextValidator MeleeWeaponAttackOrOathOfThunder =
        new RestrictedContextValidator((_, _, character, _, _, mode, _) => (OperationType.Set,
            mode != null && (ValidatorsWeapon.IsMelee(mode) ||
                             (character.GetSubclassLevel(Paladin, OathOfThunder.Name) > 0 &&
                              OathOfThunder.IsValidWeapon(mode, null, null)))));
}
