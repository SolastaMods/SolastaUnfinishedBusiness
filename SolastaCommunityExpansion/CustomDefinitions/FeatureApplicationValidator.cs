using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomDefinitions;

public interface IFeatureApplicationValidator
{
    bool IsValid(RulesetCharacter character);
}

public class FeatureApplicationValidator : IFeatureApplicationValidator
{
    private readonly CharacterValidator[] validators;

    public FeatureApplicationValidator(params CharacterValidator[] validators)
    {
        this.validators = validators;
    }

    public bool IsValid(RulesetCharacter character)
    {
        return character == null || character.IsValid(validators);
    }
}