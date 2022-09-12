using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public sealed class PowerUseValidity : IPowerUseValidity
{
    private readonly CharacterValidator[] validators;

    public PowerUseValidity(params CharacterValidator[] validators)
    {
        this.validators = validators;
    }

    public bool CanUsePower(RulesetCharacter character)
    {
        return character.IsValid(validators);
    }
}
