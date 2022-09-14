using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public sealed class ValidatorFeatureApplication : IFeatureApplicationValidator
{
    private readonly CharacterValidator[] validators;

    public ValidatorFeatureApplication(params CharacterValidator[] validators)
    {
        this.validators = validators;
    }

    public bool IsValid([CanBeNull] RulesetCharacter character)
    {
        return character == null || character.IsValid(validators);
    }
}
