using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IDefinitionWithPrerequisites
{
    [CanBeNull]
    public delegate string Validate(RulesetCharacter character, BaseDefinition definition);

    public List<Validate> Validators { get; }
}
