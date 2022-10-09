using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class ValidatorPowerUse : IPowerUseValidity
{
    private readonly IsCharacterValidHandler[] validators;

    public static IPowerUseValidity NotInCombat = new ValidatorPowerUse(_ =>
        !ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress);

    public static IPowerUseValidity InCombat = new ValidatorPowerUse(_ =>
        ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress);

    internal ValidatorPowerUse(params IsCharacterValidHandler[] validators)
    {
        this.validators = validators;
    }

    public bool CanUsePower(RulesetCharacter character)
    {
        return character.IsValid(validators);
    }
}
