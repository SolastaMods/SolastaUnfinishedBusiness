using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomValidators;

internal delegate bool IsDefinitionValidHandler(BaseDefinition definition, RulesetCharacter character);

internal sealed class ValidatorsDefinitionApplication : IDefinitionApplicationValidator
{
    private readonly IsDefinitionValidHandler[] _validators;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ValidatorsDefinitionApplication(params IsCharacterValidHandler[] validators)
    {
        _validators = validators.Select(v => new IsDefinitionValidHandler((_, c) => v(c))).ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid(BaseDefinition definition, [CanBeNull] RulesetCharacter character)
    {
        return character == null || _validators.All(handler => handler(definition, character));
    }
}
