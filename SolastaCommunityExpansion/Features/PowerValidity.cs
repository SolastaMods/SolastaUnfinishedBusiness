using SolastaCommunityExpansion.Api.AdditionalExtensions;

namespace SolastaCommunityExpansion.Features;

public interface IPowerUseValidity
{
    bool CanUsePower(RulesetCharacter character);
}

public class PowerUseValidity : IPowerUseValidity
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