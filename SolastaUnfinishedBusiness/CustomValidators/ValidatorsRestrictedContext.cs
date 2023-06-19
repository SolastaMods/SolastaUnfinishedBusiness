using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Subclasses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.CustomValidators;

public static class ValidatorsRestrictedContext
{
    public static readonly IRestrictedContextValidator IsWeaponAttack =
        new RestrictedContextValidator((_, _, _, _, _, mode, _) => (OperationType.Set, mode != null));

    public static readonly IRestrictedContextValidator IsMeleeWeaponAttack =
        new RestrictedContextValidator((_, _, _, _, _, mode, _) => (OperationType.Set, ValidatorsWeapon.IsMelee(mode)));

    public static readonly IRestrictedContextValidator IsOathOfThunder =
        new RestrictedContextValidator((_, _, character, _, _, mode, _) => (OperationType.Set,
            character.GetSubclassLevel(Paladin, OathOfThunder.Name) >= 3 &&
            OathOfThunder.IsOathOfThunderWeapon(mode, null, character)));

    public static readonly IRestrictedContextValidator IsOathOfDemonHunter =
        new RestrictedContextValidator((_, _, character, _, _, mode, _) => (OperationType.Set,
            character.GetSubclassLevel(Paladin, OathOfDemonHunter.Name) >= 3 &&
            OathOfDemonHunter.IsOathOfDemonHunterWeapon(mode, null, character)));
}
