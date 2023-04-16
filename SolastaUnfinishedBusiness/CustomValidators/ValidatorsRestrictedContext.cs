using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomValidators;

public static class ValidatorsRestrictedContext
{
    public static readonly IRestrictedContextValidator WeaponAttack =
        new RestrictedContextValidator((_, _, _, _, _, mode, _) => (OperationType.Set, mode != null));
}
