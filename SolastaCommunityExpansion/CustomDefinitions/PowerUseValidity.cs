using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomDefinitions;

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
