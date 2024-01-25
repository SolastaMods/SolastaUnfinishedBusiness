using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Validators;

internal delegate bool IsDefinitionValidHandler(BaseDefinition definition, RulesetCharacter character);

internal sealed class ValidateDefinitionApplication : IValidateDefinitionApplication
{
    private readonly IsDefinitionValidHandler[] _validators;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ValidateDefinitionApplication(params IsCharacterValidHandler[] validators)
    {
        _validators = validators.Select(v => new IsDefinitionValidHandler((_, c) => v(c))).ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid(BaseDefinition definition, [CanBeNull] RulesetCharacter character)
    {
        return character == null || _validators.All(handler => handler(definition, character));
    }
}
