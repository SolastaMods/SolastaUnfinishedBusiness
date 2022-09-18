using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public delegate bool IsDefinitionValidHandler(BaseDefinition definition, RulesetCharacter character);

public sealed class ValidatorDefinitionApplication : IDefinitionApplicationValidator
{
    private readonly IsDefinitionValidHandler[] validators;

    public ValidatorDefinitionApplication(params IsCharacterValidHandler[] validators)
    {
        this.validators = validators.Select(v => new IsDefinitionValidHandler((_, c) => v(c))).ToArray();
    }

    public bool IsValid(BaseDefinition definition, [CanBeNull] RulesetCharacter character)
    {
        return character == null || validators.All(handler => handler(definition, character));
    }
}
