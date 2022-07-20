using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomDefinitions;

public interface IFeatureApplicationValidator
{
    bool IsValid(RulesetCharacter character);
}

public sealed class FeatureApplicationValidator : IFeatureApplicationValidator
{
    private readonly CharacterValidator[] validators;

    public FeatureApplicationValidator(params CharacterValidator[] validators)
    {
        this.validators = validators;
    }

    public bool IsValid([CanBeNull] RulesetCharacter character)
    {
        return character == null || character.IsValid(validators);
    }
}
