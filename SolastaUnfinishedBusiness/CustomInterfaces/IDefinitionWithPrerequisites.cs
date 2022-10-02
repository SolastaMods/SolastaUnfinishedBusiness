using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IDefinitionWithPrerequisites
{
    [CanBeNull]
    internal delegate bool Validate(RulesetCharacter character, BaseDefinition definition, out string requirement);

    internal List<Validate> Validators { get; }
}
