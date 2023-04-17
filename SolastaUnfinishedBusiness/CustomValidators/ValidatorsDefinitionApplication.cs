using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomValidators;

internal delegate bool IsDefinitionValidHandler(BaseDefinition definition, RulesetCharacter character);

internal sealed class ValidatorsDefinitionApplication : IDefinitionApplicationValidator
{
    private readonly IsDefinitionValidHandler[] validators;

    internal ValidatorsDefinitionApplication(params IsCharacterValidHandler[] validators)
    {
        this.validators = validators.Select(v => new IsDefinitionValidHandler((_, c) => v(c))).ToArray();
    }

    public bool IsValid(BaseDefinition definition, [CanBeNull] RulesetCharacter character)
    {
        return character == null || validators.All(handler => handler(definition, character));
    }
}
