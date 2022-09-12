using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

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
