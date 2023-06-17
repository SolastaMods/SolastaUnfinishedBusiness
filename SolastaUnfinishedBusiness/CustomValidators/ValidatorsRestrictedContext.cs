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

    public static readonly IRestrictedContextValidator MeleeWeaponAttack =
        new RestrictedContextValidator((_, _, character, _, _, mode, _) => (OperationType.Set,
            mode != null && (ValidatorsWeapon.IsMelee(mode))));

    public static readonly IRestrictedContextValidator IsOathOfThunder =
        new RestrictedContextValidator((_, _, character, _, _, mode, _) => (OperationType.Set,
            (character.GetSubclassLevel(Paladin, OathOfThunder.Name) > 0 &&
             OathOfThunder.IsValidWeapon(mode, null, null))));

    public static readonly IRestrictedContextValidator UseDemonHunterWeapon =
        new RestrictedContextValidator((_, _, character, _, _, mode, _) => (OperationType.Set,
            OathOfDemonHunter.IsValidWeapon(mode, null, character)));
}
