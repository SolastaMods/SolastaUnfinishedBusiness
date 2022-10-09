using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class ValidatorPowerUse : IPowerUseValidity
{
    public static readonly IPowerUseValidity NotInCombat = new ValidatorPowerUse(_ =>
        !ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress);

    public static readonly IPowerUseValidity InCombat = new ValidatorPowerUse(_ =>
        ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress);

    private readonly IsCharacterValidHandler[] validators;

    internal ValidatorPowerUse(params IsCharacterValidHandler[] validators)
    {
        this.validators = validators;
    }

    public bool CanUsePower(RulesetCharacter character)
    {
        return character.IsValid(validators);
    }
}
