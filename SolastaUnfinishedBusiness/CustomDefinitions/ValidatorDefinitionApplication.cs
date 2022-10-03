using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal delegate bool IsDefinitionValidHandler(BaseDefinition definition, RulesetCharacter character);

internal sealed class ValidatorDefinitionApplication : IDefinitionApplicationValidator
{
    private readonly IsDefinitionValidHandler[] validators;

    internal ValidatorDefinitionApplication(params IsCharacterValidHandler[] validators)
    {
        this.validators = validators.Select(v => new IsDefinitionValidHandler((_, c) => v(c))).ToArray();
    }

    public bool IsValid(BaseDefinition definition, [CanBeNull] RulesetCharacter character)
    {
        return character == null || validators.All(handler => handler(definition, character));
    }
}
