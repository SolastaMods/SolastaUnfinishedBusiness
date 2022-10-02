using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IDefinitionWithPrerequisites
{
    [CanBeNull]
    public delegate bool Validate(RulesetCharacter character, BaseDefinition definition, out string requirement);

    public List<Validate> Validators { get; }
}
